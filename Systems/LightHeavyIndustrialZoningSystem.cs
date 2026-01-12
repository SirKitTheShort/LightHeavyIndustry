using Colossal.Logging;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Pollution constants for Light Industry - easily adjustable
    /// </summary>
    public static class PollutionConstants
    {
        // Light Industry Pollution Values
        public const int LIGHT_INDUSTRY_AIR = 0;
        public const int LIGHT_INDUSTRY_GROUND = 5;
        public const int LIGHT_INDUSTRY_NOISE = 30;
    }

    /// <summary>
    /// Creates Light Industry and Heavy Industry zone types
    /// Light = no chimneys/smoke, minimal pollution, lower profit
    /// Heavy = exact same as vanilla Industrial Manufacturing
    /// </summary>
    public partial class LightHeavyIndustryZoningSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private List<PrefabBase> _allPrefabs;

        private ZonePrefab _vanillaIndustrialZone;
        private ZonePrefab _lightIndustryZone;
        private ZonePrefab _heavyIndustryZone;

        // List of sub-object names to remove from Light Industry prefabs
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
            
            // Decoration Props - EXACT names only
            "IndustrialManufacturingDecoration03_2x2 Oil",
            "IndustrialManufacturingDecoration04_2x2 Ore",
            "IndustrialManufacturingDecoration04_2x4 Ore",
            "IndustrialManufacturingDecorationRandom01_2x2",
            "IndustrialManufacturingDecorationRandom01_2x4",
            
            // Warning Lights
            "WarningLight01", "WarningLight02", "WarningLightRandom01"
        };

        // Hardcoded blacklist of building prefabs that shouldn't appear in Light Industry
        private static readonly HashSet<string> HardcodedLightBlacklist = new()
        {
            "LightIndustrial_IndustrialStorageOre01_L1_6x6",
            "LightIndustrial_IndustrialStorageOre01_L2_6x6",
            "LightIndustrial_IndustrialStorageOre01_L3_6x6",
            "LightIndustrial_IndustrialStorageOre01_L4_6x6",
            "LightIndustrial_IndustrialStorageOre01_L5_6x6"
        };

        // Hardcoded blacklist for Heavy Industry
        private static readonly HashSet<string> HardcodedHeavyBlacklist = new()
        {
            // Add building names here from CS2 Asset Editor if needed
        };

        private bool _zonesCreated = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Access the internal m_Prefabs list using reflection
            var prefabsField = typeof(PrefabSystem).GetField("m_Prefabs",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (prefabsField == null)
            {
                Mod.log.Error("Could not access m_Prefabs field in PrefabSystem!");
                return;
            }

            _allPrefabs = prefabsField.GetValue(_prefabSystem) as List<PrefabBase>;

            if (_allPrefabs == null)
            {
                Mod.log.Error("m_Prefabs is null!");
                return;
            }

            Mod.log.Info("LightHeavyIndustryZoningSystem created successfully");

            // Populate dropdown lists immediately so they're ready when settings UI loads
            // We'll update them again in OnUpdate() when we actually find the buildings
            PopulateDropdownsWithPlaceholder();
        }

        /// <summary>
        /// Populate dropdowns with a placeholder message until buildings are loaded
        /// </summary>
        private void PopulateDropdownsWithPlaceholder()
        {
            Setting.AvailableLightBuildingNames.Clear();
            Setting.AvailableHeavyBuildingNames.Clear();

            // This will make the dropdowns show "Loading..." until OnUpdate() runs
            Setting.AvailableLightBuildingNames.Add("LoadingPlaceholder");
            Setting.AvailableHeavyBuildingNames.Add("LoadingPlaceholder");

            Mod.log.Info("Dropdown placeholders set - will populate with real buildings in OnUpdate()");
        }

        protected override void OnUpdate()
        {
            // Only create zones once
            if (_zonesCreated) return;

            try
            {
                Mod.log.Info("=== Creating Light/Heavy Industry Zones ===");

                // Find the vanilla Industrial Manufacturing zone
                _vanillaIndustrialZone = _allPrefabs
                    .OfType<ZonePrefab>()
                    .FirstOrDefault(z => z.name == "Industrial Manufacturing");

                if (_vanillaIndustrialZone == null)
                {
                    Mod.log.Error("Could not find vanilla 'Industrial Manufacturing' zone!");
                    return;
                }

                Mod.log.Info($"Found vanilla zone: {_vanillaIndustrialZone.name}");

                // Get all buildings that can spawn in industrial zones
                var industrialBuildings = _allPrefabs
                    .OfType<BuildingPrefab>()
                    .Where(b => b.components.OfType<SpawnableBuilding>()
                        .Any(c => c.m_ZoneType == _vanillaIndustrialZone))
                    .ToArray();

                Mod.log.Info($"Found {industrialBuildings.Length} industrial buildings");

                // Populate dropdown lists for settings UI
                PopulateSettingsDropdowns(industrialBuildings);

                // Create Light Industry zone (vanilla yellow color)
                _lightIndustryZone = CreateLightIndustryZone(_vanillaIndustrialZone);
                if (_lightIndustryZone != null)
                {
                    _prefabSystem.AddPrefab(_lightIndustryZone);
                    Mod.log.Info($"Created Light Industry zone: {_lightIndustryZone.name}");

                    // Clone buildings for Light Industry (filtered by blacklist/whitelist, chimneys removed from prefabs)
                    var lightBuildings = CloneBuildingsForZone(
                        industrialBuildings,
                        _lightIndustryZone,
                        "LightIndustrial",
                        isLightIndustry: true,
                        removeChimneys: true);

                    foreach (var building in lightBuildings)
                    {
                        _prefabSystem.AddPrefab(building);
                    }

                    Mod.log.Info($"Cloned {lightBuildings.Count} buildings for Light Industrial Manufacturing");
                }

                // Create Heavy Industry zone (bright orange)
                _heavyIndustryZone = CreateHeavyIndustryZone(_vanillaIndustrialZone);
                if (_heavyIndustryZone != null)
                {
                    _prefabSystem.AddPrefab(_heavyIndustryZone);
                    Mod.log.Info($"Created Heavy Industry zone: {_heavyIndustryZone.name}");

                    // Clone all buildings for Heavy Industry (filtered by blacklist/whitelist, keep chimneys)
                    var heavyBuildings = CloneBuildingsForZone(
                        industrialBuildings,
                        _heavyIndustryZone,
                        "HeavyIndustrial",
                        isLightIndustry: false,
                        removeChimneys: false);

                    foreach (var building in heavyBuildings)
                    {
                        _prefabSystem.AddPrefab(building);
                    }

                    Mod.log.Info($"Cloned {heavyBuildings.Count} buildings for Heavy Industrial Manufacturing");
                }

                _zonesCreated = true;

                // Register UI labels for the zones
                RegisterZoneLabels();

                Mod.log.Info("=== Zone Creation Complete ===");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Failed to create zones");
            }
        }

        private ZonePrefab CreateLightIndustryZone(ZonePrefab sourceZone)
        {
            var zone = sourceZone.Clone("LightIndustrialManufacturing") as ZonePrefab;

            if (zone == null)
            {
                Mod.log.Error("Failed to clone zone for Light Industry");
                return null;
            }

            Mod.log.Info($"Light Industrial Manufacturing zone color: {zone.m_Edge} (vanilla yellow)");

            // Set pollution on the ZONE itself
            var zonePollution = zone.GetComponent<ZonePollution>();
            if (zonePollution != null)
            {
                zonePollution.m_AirPollution = PollutionConstants.LIGHT_INDUSTRY_AIR;
                zonePollution.m_GroundPollution = PollutionConstants.LIGHT_INDUSTRY_GROUND;
                zonePollution.m_NoisePollution = PollutionConstants.LIGHT_INDUSTRY_NOISE;
                Mod.log.Info($"Set zone pollution: Air={zonePollution.m_AirPollution}, Ground={zonePollution.m_GroundPollution}, Noise={zonePollution.m_NoisePollution}");
            }
            else
            {
                Mod.log.Warn("Light Industry zone has no ZonePollution component!");
            }

            // Set custom icon using coui:// protocol
            var uiObj = zone.GetComponent<UIObject>();
            if (uiObj != null)
            {
                uiObj.m_Icon = $"coui://{Mod.HostName}/LightIndustry.svg";
                Mod.log.Info($"Set Light Industry icon to: {uiObj.m_Icon}");
            }

            return zone;
        }

        private ZonePrefab CreateHeavyIndustryZone(ZonePrefab sourceZone)
        {
            var zone = sourceZone.Clone("HeavyIndustrialManufacturing") as ZonePrefab;

            if (zone == null)
            {
                Mod.log.Error("Failed to clone zone for Heavy Industry");
                return null;
            }

            zone.m_Edge = new Color(1.0f, 0.5f, 0.0f);
            Mod.log.Info($"Heavy Industrial Manufacturing zone color: {zone.m_Edge} (bright orange)");

            // Set custom icon using coui:// protocol
            var uiObj = zone.GetComponent<UIObject>();
            if (uiObj != null)
            {
                uiObj.m_Icon = $"coui://{Mod.HostName}/HeavyIndustry.svg";
                Mod.log.Info($"Set Heavy Industry icon to: {uiObj.m_Icon}");
            }

            return zone;
        }

        private List<BuildingPrefab> CloneBuildingsForZone(
            BuildingPrefab[] sourceBuildings,
            ZonePrefab targetZone,
            string prefix,
            bool isLightIndustry,
            bool removeChimneys)
        {
            var clonedBuildings = new List<BuildingPrefab>();
            int blacklistedCount = 0;
            int whitelistedCount = 0;
            int totalChimneysRemoved = 0;

            // Get appropriate blacklist and whitelist
            var hardcodedBlacklist = isLightIndustry ? HardcodedLightBlacklist : HardcodedHeavyBlacklist;
            var userBlacklist = isLightIndustry ? Mod.Settings.LightIndustryBlacklist : Mod.Settings.HeavyIndustryBlacklist;
            var userWhitelist = isLightIndustry ? Mod.Settings.LightIndustryWhitelist : Mod.Settings.HeavyIndustryWhitelist;

            // Combine hardcoded and user blacklists
            var combinedBlacklist = new HashSet<string>(hardcodedBlacklist);
            foreach (var item in userBlacklist)
            {
                combinedBlacklist.Add(item);
            }

            foreach (var sourceBuilding in sourceBuildings)
            {
                try
                {
                    // Check whitelist first - whitelist overrides blacklist
                    bool isWhitelisted = userWhitelist.Contains(sourceBuilding.name);

                    if (!isWhitelisted && combinedBlacklist.Contains(sourceBuilding.name))
                    {
                        blacklistedCount++;
                        continue;
                    }

                    if (isWhitelisted)
                    {
                        whitelistedCount++;
                    }

                    var newName = $"{prefix}_{sourceBuilding.name}";
                    var building = sourceBuilding.Clone(newName) as BuildingPrefab;

                    if (building == null)
                    {
                        Mod.log.Warn($"Failed to clone building: {sourceBuilding.name}");
                        continue;
                    }

                    // Assign to the new zone
                    var spawnableBuilding = building.GetComponent<SpawnableBuilding>();
                    if (spawnableBuilding != null)
                    {
                        spawnableBuilding.m_ZoneType = targetZone;
                    }

                    // Remove chimneys/smoke from prefab components
                    if (removeChimneys)
                    {
                        int removed = RemoveChimneysFromPrefabComponents(building);
                        totalChimneysRemoved += removed;
                    }

                    clonedBuildings.Add(building);
                }
                catch (Exception ex)
                {
                    Mod.log.Error(ex, $"Failed to clone building {sourceBuilding.name}");
                }
            }

            if (blacklistedCount > 0)
            {
                Mod.log.Info($"Excluded {blacklistedCount} blacklisted buildings");
            }

            if (whitelistedCount > 0)
            {
                Mod.log.Info($"Force-included {whitelistedCount} whitelisted buildings");
            }

            if (totalChimneysRemoved > 0)
            {
                Mod.log.Info($"Removed {totalChimneysRemoved} chimney/smoke/decoration components from building prefabs");
            }

            return clonedBuildings;
        }

        private int RemoveChimneysFromPrefabComponents(BuildingPrefab building)
        {
            int removedCount = 0;

            try
            {
                // Get all components from the building prefab
                var componentsList = new List<ComponentBase>();
                building.GetComponents(componentsList);

                // Find and modify ObjectSubObjects components
                foreach (var component in componentsList)
                {
                    if (component is ObjectSubObjects subObjectsComponent)
                    {
                        var subObjects = subObjectsComponent.m_SubObjects;
                        if (subObjects != null && subObjects.Length > 0)
                        {
                            var filteredSubObjects = new List<ObjectSubObjectInfo>();

                            foreach (var subObjInfo in subObjects)
                            {
                                if (subObjInfo.m_Object != null)
                                {
                                    string prefabName = subObjInfo.m_Object.name;
                                    bool shouldRemove = false;

                                    // EXACT MATCH CHECK for effect names
                                    if (EffectNamesToRemove.Contains(prefabName))
                                    {
                                        shouldRemove = true;
                                    }

                                    // SUBSTRING CHECK for chimneys, smoke, fire, lights (NOT decorations)
                                    if (!shouldRemove)
                                    {
                                        string lowerName = prefabName.ToLower();

                                        if (lowerName.Contains("chimney") ||
                                            lowerName.Contains("smoke") ||
                                            lowerName.Contains("steam") ||
                                            lowerName.Contains("vapor") ||
                                            lowerName.Contains("fire") ||
                                            lowerName.Contains("warninglight"))
                                        {
                                            shouldRemove = true;
                                        }
                                    }

                                    if (shouldRemove)
                                    {
                                        removedCount++;
                                    }
                                    else
                                    {
                                        filteredSubObjects.Add(subObjInfo);
                                    }
                                }
                                else
                                {
                                    filteredSubObjects.Add(subObjInfo);
                                }
                            }

                            // Update the component with filtered sub-objects
                            if (filteredSubObjects.Count < subObjects.Length)
                            {
                                subObjectsComponent.m_SubObjects = filteredSubObjects.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, $"Error removing chimneys from prefab {building.name}");
            }

            return removedCount;
        }

        public bool IsLightIndustryZone(ZonePrefab zone)
        {
            return zone != null && zone.name == "LightIndustrialManufacturing";
        }

        public bool IsHeavyIndustryZone(ZonePrefab zone)
        {
            return zone != null && zone.name == "HeavyIndustrialManufacturing";
        }

        private void RegisterZoneLabels()
        {
            try
            {
                if (!_prefabSystem.TryGetEntity(_lightIndustryZone, out var lightEntity))
                {
                    Mod.log.Warn("Could not get entity for Light Industry zone");
                    return;
                }

                if (!_prefabSystem.TryGetEntity(_heavyIndustryZone, out var heavyEntity))
                {
                    Mod.log.Warn("Could not get entity for Heavy Industry zone");
                    return;
                }

                var localeSource = new ZoneLocalizationSource(lightEntity, heavyEntity);

                foreach (var localeId in GameManager.instance.localizationManager.GetSupportedLocales())
                {
                    GameManager.instance.localizationManager.AddSource(localeId, localeSource);
                }

                Mod.log.Info("Zone UI labels registered successfully");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Failed to register zone labels");
            }
        }

        /// <summary>
        /// Populate the static lists in Settings that power the dropdown menus
        /// </summary>
        private void PopulateSettingsDropdowns(BuildingPrefab[] industrialBuildings)
        {
            try
            {
                Mod.log.Info($"PopulateSettingsDropdowns called with {industrialBuildings.Length} buildings");

                // Clear existing lists (including placeholder)
                Setting.AvailableLightBuildingNames.Clear();
                Setting.AvailableHeavyBuildingNames.Clear();

                // Add all industrial building names to both lists (they're the same source buildings)
                foreach (var building in industrialBuildings.OrderBy(b => b.name))
                {
                    Setting.AvailableLightBuildingNames.Add(building.name);
                    Setting.AvailableHeavyBuildingNames.Add(building.name);
                }

                Mod.log.Info($"SUCCESS: Populated dropdown lists with {industrialBuildings.Length} building names");
                Mod.log.Info($"AvailableLightBuildingNames.Count = {Setting.AvailableLightBuildingNames.Count}");
                Mod.log.Info($"AvailableHeavyBuildingNames.Count = {Setting.AvailableHeavyBuildingNames.Count}");

                // NOW register the settings UI since the dropdowns are ready
                Mod.Settings.RegisterInOptionsUI();
                Mod.log.Info("Settings UI registered - dropdowns are now populated");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Failed to populate settings dropdowns");
            }
        }
    }
}