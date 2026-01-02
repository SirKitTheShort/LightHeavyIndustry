using System;
using System.Reflection;
using Game;
using Game.Buildings;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Converts all Light/Heavy Industry buildings back to vanilla Industrial
    /// Use this before uninstalling the mod to prevent buildings from disappearing
    /// </summary>
    public partial class ZoneConversionSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _allBuildingsQuery;
        private ZonePrefab _vanillaIndustrialZone;

        protected override void OnCreate()
        {
            base.OnCreate();

            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _allBuildingsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Building>(),
                    ComponentType.ReadOnly<PrefabRef>()
                }
            });

            Mod.log.Info("ZoneConversionSystem created");
        }

        protected override void OnUpdate()
        {
            // Only run when manually triggered from settings
            if (!Mod.Settings.ConvertToVanillaZones)
                return;

            // Reset the flag immediately
            Mod.Settings.ConvertToVanillaZones = false;

            try
            {
                Mod.log.Info("=== Converting All Industry Back to Vanilla ===");

                // Find vanilla Industrial Manufacturing zone
                FindVanillaZone();

                if (_vanillaIndustrialZone == null)
                {
                    Mod.log.Error("Cannot find vanilla Industrial Manufacturing zone!");
                    return;
                }

                // Convert all Light/Heavy buildings
                int converted = ConvertAllBuildings();

                Mod.log.Info($"=== Conversion Complete: {converted} buildings converted ===");
                Mod.log.Info("You can now safely uninstall the mod.");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Zone conversion failed");
            }
        }

        private void FindVanillaZone()
        {
            // Access m_Prefabs list
            var prefabsField = typeof(PrefabSystem).GetField("m_Prefabs",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (prefabsField == null)
            {
                Mod.log.Error("Could not access m_Prefabs field");
                return;
            }

            var allPrefabs = prefabsField.GetValue(_prefabSystem) as System.Collections.Generic.List<PrefabBase>;

            if (allPrefabs == null)
            {
                Mod.log.Error("m_Prefabs is null");
                return;
            }

            // Find vanilla Industrial Manufacturing zone
            foreach (var prefab in allPrefabs)
            {
                if (prefab is ZonePrefab zone && zone.name == "Industrial Manufacturing")
                {
                    _vanillaIndustrialZone = zone;
                    Mod.log.Info($"Found vanilla zone: {zone.name}");
                    break;
                }
            }
        }

        private int ConvertAllBuildings()
        {
            var buildings = _allBuildingsQuery.ToEntityArray(Allocator.Temp);
            int convertedCount = 0;

            Mod.log.Info($"Scanning {buildings.Length} buildings...");

            foreach (var buildingEntity in buildings)
            {
                if (!EntityManager.HasComponent<PrefabRef>(buildingEntity))
                    continue;

                var prefabRef = EntityManager.GetComponentData<PrefabRef>(buildingEntity);

                try
                {
                    var getPrefabMethod = _prefabSystem.GetType().GetMethod(
                        "GetPrefab",
                        BindingFlags.Instance | BindingFlags.Public,
                        null,
                        new Type[] { typeof(Entity) },
                        null
                    );

                    if (getPrefabMethod == null) continue;

                    var genericMethod = getPrefabMethod.MakeGenericMethod(typeof(BuildingPrefab));
                    var buildingPrefab = genericMethod.Invoke(_prefabSystem, new object[] { prefabRef.m_Prefab }) as BuildingPrefab;

                    if (buildingPrefab == null) continue;

                    // Check if this is one of our custom zone buildings
                    if (buildingPrefab.name.StartsWith("LightIndustrial_") ||
                        buildingPrefab.name.StartsWith("HeavyIndustrial_"))
                    {
                        // Find the vanilla equivalent
                        string vanillaName = buildingPrefab.name
                            .Replace("LightIndustrial_", "")
                            .Replace("HeavyIndustrial_", "");

                        var vanillaBuilding = FindVanillaBuildingPrefab(vanillaName);

                        if (vanillaBuilding != null)
                        {
                            // Get the vanilla building's prefab entity
                            if (_prefabSystem.TryGetEntity(vanillaBuilding, out var vanillaPrefabEntity))
                            {
                                // Update the building's prefab reference
                                prefabRef.m_Prefab = vanillaPrefabEntity;
                                EntityManager.SetComponentData(buildingEntity, prefabRef);

                                convertedCount++;

                                if (convertedCount <= 10) // Log first 10
                                {
                                    Mod.log.Info($"  Converted: {buildingPrefab.name} → {vanillaName}");
                                }
                            }
                        }
                        else
                        {
                            Mod.log.Warn($"Could not find vanilla building: {vanillaName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Mod.log.Warn($"Error converting building {buildingEntity.Index}: {ex.Message}");
                }
            }

            buildings.Dispose();
            return convertedCount;
        }

        private BuildingPrefab FindVanillaBuildingPrefab(string name)
        {
            var prefabsField = typeof(PrefabSystem).GetField("m_Prefabs",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (prefabsField == null) return null;

            var allPrefabs = prefabsField.GetValue(_prefabSystem) as System.Collections.Generic.List<PrefabBase>;

            if (allPrefabs == null) return null;

            foreach (var prefab in allPrefabs)
            {
                if (prefab is BuildingPrefab building && building.name == name)
                {
                    return building;
                }
            }

            return null;
        }
    }
}