using System;
using System.Collections.Generic;
using System.Reflection;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Monitors spawned industrial buildings and:
    /// - Hides chimneys/smoke from Light Industry buildings (backup for prefab-level removal)
    /// - Sets pollution values
    /// </summary>
    public partial class LightIndustryBuildingProcessorSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _completedBuildingsQuery;
        private HashSet<int> _processedBuildingIndices = new();

        // List of effect names to hide
        private static readonly HashSet<string> EffectNamesToRemove = new(StringComparer.OrdinalIgnoreCase)
        {
            // VFX Effects
            "FireBigVFX", "FireEmbersVFX", "FireMediumVFX", "FireMovingMediumVFX",
            "FireSmallVFX", "FireTinyVFX", "GasFlareFIreVFX", "SmokeFromFireVFX",
            "WaterVaporFactoryBig", "WaterVaporFactorySmallVFX", "WaterVaporHugeVFX",
            
            // Chimney Props
            "IndustrialChimneyLarge01 Agriculture", "IndustrialChimneyLarge02 Forestry",
            "IndustrialChimneyLarge03 Oil", "IndustrialChimneyLarge04 Ore",
            "IndustrialChimneyLargeRandom01",
            "IndustrialChimneyMedium01 Agriculture", "IndustrialChimneyMedium02 Forestry",
            "IndustrialChimneyMedium03 Oil", "IndustrialChimneyMedium04 Ore",
            "IndustrialChimneyMediumRandom01",
            "IndustrialChimneySmall01 Agriculture", "IndustrialChimneySmall02 Forestry",
            "IndustrialChimneySmall03 Oil", "IndustrialChimneySmall04 Ore",
            "IndustrialChimneySmallRandom01",
            
            // Decoration Props
            "IndustrialManufacturingDecoration03_2x2 Oil",
            "IndustrialManufacturingDecoration04_2x2 Ore",
            "IndustrialManufacturingDecoration04_2x4 Ore",
            
            // Warning Lights
            "WarningLight01", "WarningLight02", "WarningLightRandom01"
        };

        protected override void OnCreate()
        {
            base.OnCreate();

            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _completedBuildingsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Building>(),
                    ComponentType.ReadOnly<PrefabRef>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Game.Objects.UnderConstruction>(),
                    ComponentType.ReadOnly<Deleted>()
                }
            });

            Mod.log.Info("LightIndustryBuildingProcessorSystem created");
        }

        protected override void OnUpdate()
        {
            if (!Mod.Settings.Enabled) return;

            var buildings = _completedBuildingsQuery.ToEntityArray(Allocator.Temp);
            int processedThisFrame = 0;

            foreach (var buildingEntity in buildings)
            {
                if (_processedBuildingIndices.Contains(buildingEntity.Index))
                    continue;

                if (!IsLightIndustryBuilding(buildingEntity))
                {
                    _processedBuildingIndices.Add(buildingEntity.Index);
                    continue;
                }

                _processedBuildingIndices.Add(buildingEntity.Index);

                try
                {
                    ProcessLightIndustryBuilding(buildingEntity);
                    processedThisFrame++;
                }
                catch (Exception ex)
                {
                    Mod.log.Error(ex, $"Failed to process Light Industry building {buildingEntity.Index}");
                }
            }

            if (processedThisFrame > 0)
            {
                Mod.log.Info($"Processed {processedThisFrame} Light Industrial buildings this frame");
            }

            buildings.Dispose();
        }

        private bool IsLightIndustryBuilding(Entity buildingEntity)
        {
            if (!EntityManager.HasComponent<PrefabRef>(buildingEntity))
                return false;

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

                if (getPrefabMethod == null) return false;

                var genericMethod = getPrefabMethod.MakeGenericMethod(typeof(BuildingPrefab));
                var buildingPrefab = genericMethod.Invoke(_prefabSystem, new object[] { prefabRef.m_Prefab }) as BuildingPrefab;

                if (buildingPrefab == null) return false;

                return buildingPrefab.name.StartsWith("LightIndustrial_");
            }
            catch
            {
                return false;
            }
        }

        private void ProcessLightIndustryBuilding(Entity buildingEntity)
        {
            // Hide chimneys and smoke effects (backup layer)
            int hiddenCount = HideChimneysAndSmoke(buildingEntity);

            if (hiddenCount > 0)
            {
                Mod.log.Info($"Processing Light Industrial building: {buildingEntity.Index} - Hidden {hiddenCount} chimney/smoke sub-objects");
            }

            // Set pollution values
            SetLightIndustryPollution(buildingEntity);
        }

        private int HideChimneysAndSmoke(Entity buildingEntity)
        {
            if (!EntityManager.HasBuffer<Game.Objects.SubObject>(buildingEntity))
                return 0;

            var subObjects = EntityManager.GetBuffer<Game.Objects.SubObject>(buildingEntity);
            int hiddenCount = 0;

            Mod.log.Debug($"  Building has {subObjects.Length} sub-objects");

            // Iterate through all sub-objects
            for (int i = 0; i < subObjects.Length; i++)
            {
                var subObj = subObjects[i];
                Entity subEntity = subObj.m_SubObject;

                if (!EntityManager.Exists(subEntity))
                    continue;

                if (!EntityManager.HasComponent<PrefabRef>(subEntity))
                    continue;

                var subPrefabRef = EntityManager.GetComponentData<PrefabRef>(subEntity);

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

                    var genericMethod = getPrefabMethod.MakeGenericMethod(typeof(PrefabBase));
                    var prefab = genericMethod.Invoke(_prefabSystem, new object[] { subPrefabRef.m_Prefab }) as PrefabBase;

                    if (prefab == null) continue;

                    string prefabName = prefab.name;

                    // Check if this is an effect to hide
                    bool shouldHide = EffectNamesToRemove.Contains(prefabName);

                    if (!shouldHide)
                    {
                        string lowerName = prefabName.ToLower();
                        if (lowerName.Contains("chimney") ||
                            lowerName.Contains("smoke") ||
                            lowerName.Contains("steam") ||
                            lowerName.Contains("vapor") ||
                            lowerName.Contains("fire") ||
                            lowerName.Contains("warninglight"))
                        {
                            shouldHide = true;
                        }
                    }

                    if (shouldHide)
                    {
                        Mod.log.Info($"    Hiding: {prefabName}");

                        // Add Deleted component to hide the sub-object
                        if (!EntityManager.HasComponent<Deleted>(subEntity))
                        {
                            EntityManager.AddComponent<Deleted>(subEntity);
                            hiddenCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Mod.log.Warn($"    Error processing sub-object {i}: {ex.Message}");
                }
            }

            return hiddenCount;
        }

        private void SetLightIndustryPollution(Entity buildingEntity)
        {
            if (!EntityManager.HasComponent<PrefabRef>(buildingEntity))
                return;

            var prefabRef = EntityManager.GetComponentData<PrefabRef>(buildingEntity);

            if (!EntityManager.HasComponent<PollutionData>(prefabRef.m_Prefab))
                return;

            var pollutionData = EntityManager.GetComponentData<PollutionData>(prefabRef.m_Prefab);

            Mod.log.Debug($"  Original pollution - Ground: {pollutionData.m_GroundPollution}, Air: {pollutionData.m_AirPollution}, Noise: {pollutionData.m_NoisePollution}");

            if (!Mod.Settings.DryRun)
            {
                pollutionData.m_AirPollution = 0;
                pollutionData.m_GroundPollution = Mod.Settings.LightIndustryGroundPollution;
                pollutionData.m_NoisePollution = 30;

                EntityManager.SetComponentData(prefabRef.m_Prefab, pollutionData);

                Mod.log.Debug($"  Set Light Industry pollution - Ground: {pollutionData.m_GroundPollution}, Air: {pollutionData.m_AirPollution}, Noise: {pollutionData.m_NoisePollution}");
            }
        }
    }
}