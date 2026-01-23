/*

        // Probability that reduced decorations will be KEPT (not removed)
        private const float LIGHT_DECORATION_PROBABILITY = 0.3f;

        // List of decorations that should appear with REDUCED PROBABILITY
        private static readonly HashSet<string> ReducedProbabilityDecorations = new(StringComparer.OrdinalIgnoreCase)
        {
            "IndustrialManufacturingDecoration01_1x1 Agriculture",
            "IndustrialManufacturingDecoration01_1x3 Agriculture",
            "IndustrialManufacturingDecoration01_2x2 Agriculture",
            "IndustrialManufacturingDecoration01_2x4 Agriculture",
            "IndustrialManufacturingDecoration02_1x1 Forestry",
            "IndustrialManufacturingDecoration02_1x3 Forestry",
            "IndustrialManufacturingDecoration02_2x2 Forestry",
            "IndustrialManufacturingDecoration02_2x4 Forestru",
            "IndustrialManufacturingDecoration03_1x1 Oil",
            "IndustrialManufacturingDecoration03_1x3 Oil",
            "IndustrialManufacturingDecoration03_2x4 Oil",
            "IndustrialManufacturingDecoration04_1x1 Ore",
            "IndustrialManufacturingDecoration04_1x3 Ore",
            "IndustrialManufacturingDecoration05_1x1 Aquaculture",
            "IndustrialManufacturingDecoration05_1x3 Aquaculture",
            "IndustrialManufacturingDecoration05_2x2 Aquaculture",
            "IndustrialManufacturingDecoration05_2x4 Aquaculture",
            "IndustrialManufacturingDecorationRandom01_1x1",
            "IndustrialManufacturingDecorationRandom01_1x3",
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
*/

using Colossal.Logging;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Economy;
using Game.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Creates Light Industry, Heavy Industry, and Warehouse zone types
    /// Light = no chimneys/smoke, minimal pollution, lower profit
    /// Heavy = exact same as vanilla Industrial Manufacturing
    /// Warehouse = storage-only buildings for both Light and Heavy products
    /// </summary>
    public partial class LightHeavyIndustryZoningSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private List<PrefabBase> _allPrefabs;

        private ZonePrefab _vanillaIndustrialZone;
        private ZonePrefab _lightIndustryZone;
        private ZonePrefab _heavyIndustryZone;
        private ZonePrefab _warehouseZone;

        // List of sub-object names to ALWAYS remove from Light Industry prefabs
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
            "IndustrialManufacturingDecorationRandom01_2x2",
            "IndustrialManufacturingDecorationRandom01_2x4",
            
            // Warning Lights
            "WarningLight01", "WarningLight02", "WarningLightRandom01"
        };

        // Probability that reduced decorations will be KEPT (not removed)
        private const float LIGHT_DECORATION_PROBABILITY = 0.1f;

        // List of decorations that should appear with REDUCED PROBABILITY
        private static readonly HashSet<string> ReducedProbabilityDecorations = new(StringComparer.OrdinalIgnoreCase)
        {
            "IndustrialManufacturingDecoration01_1x1 Agriculture",
            "IndustrialManufacturingDecoration01_1x3 Agriculture",
            "IndustrialManufacturingDecoration01_2x2 Agriculture",
            "IndustrialManufacturingDecoration01_2x4 Agriculture",
            "IndustrialManufacturingDecoration02_1x1 Forestry",
            "IndustrialManufacturingDecoration02_1x3 Forestry",
            "IndustrialManufacturingDecoration02_2x2 Forestry",
            "IndustrialManufacturingDecoration02_2x4 Forestru",
            "IndustrialManufacturingDecoration03_1x1 Oil",
            "IndustrialManufacturingDecoration03_1x3 Oil",
            "IndustrialManufacturingDecoration03_2x4 Oil",
            "IndustrialManufacturingDecoration04_1x1 Ore",
            "IndustrialManufacturingDecoration04_1x3 Ore",
            "IndustrialManufacturingDecoration05_1x1 Aquaculture",
            "IndustrialManufacturingDecoration05_1x3 Aquaculture",
            "IndustrialManufacturingDecoration05_2x2 Aquaculture",
            "IndustrialManufacturingDecoration05_2x4 Aquaculture",
            "IndustrialManufacturingDecorationRandom01_1x1",
            "IndustrialManufacturingDecorationRandom01_1x3",
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
                Mod.log.Info("=== Creating Light/Heavy/Warehouse Industry Zones ===");

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

                // Separate warehouses from manufacturing buildings based on building TYPE
                var warehouseBuildings = industrialBuildings
                    .Where(b => IsWarehouseTypeBuilding(b))
                    .ToArray();

                var manufacturingBuildings = industrialBuildings
                    .Where(b => !IsWarehouseTypeBuilding(b))
                    .ToArray();

                Mod.log.Info($"  - {manufacturingBuildings.Length} manufacturing-type buildings");
                Mod.log.Info($"  - {warehouseBuildings.Length} warehouse-type buildings");

                // Populate dropdown lists for settings UI
                PopulateSettingsDropdowns(industrialBuildings);

                // Create Light Industry zone (vanilla yellow color)
                _lightIndustryZone = CreateLightIndustryZone(_vanillaIndustrialZone);
                if (_lightIndustryZone != null)
                {
                    _prefabSystem.AddPrefab(_lightIndustryZone);
                    Mod.log.Info($"Created Light Industry zone: {_lightIndustryZone.name}");

                    // Clone buildings for Light Industry (manufacturing only if warehouse zone enabled)
                    var buildingsToClone = Mod.Settings.EnableWarehouseZone ? manufacturingBuildings : industrialBuildings;
                    var lightBuildings = CloneBuildingsForZone(
                        buildingsToClone,
                        _lightIndustryZone,
                        "LightIndustrial",
                        isLightIndustry: true,
                        removeChimneys: true);

                    foreach (var building in lightBuildings)
                    {
                        _prefabSystem.AddPrefab(building);
                    }

                    Mod.log.Info($"Cloned {lightBuildings.Count} buildings for Light Industrial Manufacturing");

                    // NOW apply economic modifiers after buildings are added to the system
                    ApplyEconomicModifiersToZone(lightBuildings, isLightIndustry: true);
                }

                // Create Heavy Industry zone (bright orange)
                _heavyIndustryZone = CreateHeavyIndustryZone(_vanillaIndustrialZone);
                if (_heavyIndustryZone != null)
                {
                    _prefabSystem.AddPrefab(_heavyIndustryZone);
                    Mod.log.Info($"Created Heavy Industry zone: {_heavyIndustryZone.name}");

                    // Clone buildings for Heavy Industry (manufacturing only if warehouse zone enabled)
                    var buildingsToClone = Mod.Settings.EnableWarehouseZone ? manufacturingBuildings : industrialBuildings;
                    var heavyBuildings = CloneBuildingsForZone(
                        buildingsToClone,
                        _heavyIndustryZone,
                        "HeavyIndustrial",
                        isLightIndustry: false,
                        removeChimneys: false);


                    foreach (var building in heavyBuildings)
                    {
                        _prefabSystem.AddPrefab(building);
                    }

                    Mod.log.Info($"Cloned {heavyBuildings.Count} buildings for Heavy Industrial Manufacturing");

                    // NOW apply economic modifiers after buildings are added to the system
                    ApplyEconomicModifiersToZone(heavyBuildings, isLightIndustry: false);
                }

                // Create Warehouse zone (greenish-yellow) if enabled
                if (Mod.Settings.EnableWarehouseZone)
                {
                    _warehouseZone = CreateWarehouseZone(_vanillaIndustrialZone);
                    if (_warehouseZone != null)
                    {
                        _prefabSystem.AddPrefab(_warehouseZone);
                        Mod.log.Info($"Created Warehouse zone: {_warehouseZone.name}");

                        // Clone warehouse-type buildings only
                        var warehousePrefabs = CloneBuildingsForZone(
                            warehouseBuildings,
                            _warehouseZone,
                            "WarehouseIndustrial",
                            isLightIndustry: true, // Use light industry settings for warehouses
                            removeChimneys: true);

                        foreach (var building in warehousePrefabs)
                        {
                            _prefabSystem.AddPrefab(building);
                        }

                        Mod.log.Info($"Cloned {warehousePrefabs.Count} buildings for Warehouse zone");

                        // Apply economic modifiers
                        ApplyEconomicModifiersToZone(warehousePrefabs, isLightIndustry: true);
                    }
                }
                else
                {
                    Mod.log.Info("Warehouse zone disabled - warehouses will spawn in Light/Heavy zones");
                }

                _zonesCreated = true;

                // Register UI labels for the zones
                RegisterZoneLabels();

                // Log economic summary
                LogEconomicSummary();

                Mod.log.Info("=== Zone Creation Complete ===");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Failed to create zones");
            }
        }



        /// <summary>
        /// Check if a building is a WAREHOUSE TYPE (storage-only) vs MANUFACTURING TYPE (produces goods)
        /// Warehouse buildings have storage capacity but NO manufacturing capability
        /// Based on BuildingProperties: if it has AllowedStored but NOT AllowedManufactured, it's a warehouse
        /// </summary>
        private bool IsWarehouseTypeBuilding(BuildingPrefab building)
        {
            var buildingProps = building.GetComponent<BuildingProperties>();
            if (buildingProps == null)
            {
                return false; // No properties = assume manufacturing
            }

            // Check what the building can do based on its properties
            bool hasManufactured = buildingProps.m_AllowedManufactured != null && buildingProps.m_AllowedManufactured.Length > 0;
            bool hasStored = buildingProps.m_AllowedStored != null && buildingProps.m_AllowedStored.Length > 0;

            // Warehouse = has storage but NO manufacturing
            // Manufacturing = has manufacturing capability (may also have storage)
            if (hasStored && !hasManufactured)
            {
                return true; // WAREHOUSE: only stores, doesn't produce
            }

            return false; // MANUFACTURING: produces goods (even if it also stores them)
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

            // Set allowed manufactured goods for Light Industry (consumer goods)
            var zoneProperties = zone.GetComponent<ZoneProperties>();
            if (zoneProperties != null)
            {
                zoneProperties.m_AllowedManufactured = new ResourceInEditor[]
                {
                    ResourceInEditor.ConvenienceFood,
                    ResourceInEditor.Food,
                    ResourceInEditor.Furniture,
                    ResourceInEditor.Paper,
                    ResourceInEditor.Vehicles,
                    ResourceInEditor.Electronics,
                    ResourceInEditor.Machinery,
                    ResourceInEditor.Pharmaceuticals,
                    ResourceInEditor.Beverages,
                    ResourceInEditor.Textiles
                };

                Mod.log.Info("Light Industry allowed products: Consumer goods only");
            }

            // Set custom icon
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

            // Set allowed manufactured goods for Heavy Industry (industrial materials)
            var zoneProperties = zone.GetComponent<ZoneProperties>();
            if (zoneProperties != null)
            {
                zoneProperties.m_AllowedManufactured = new ResourceInEditor[]
                {
                    ResourceInEditor.Metals,
                    ResourceInEditor.Petrochemicals,
                    ResourceInEditor.Steel,
                    ResourceInEditor.Minerals,
                    ResourceInEditor.Chemicals,
                    ResourceInEditor.Plastics,
                    ResourceInEditor.Concrete
                };

                Mod.log.Info("Heavy Industry allowed products: Industrial materials only");
            }

            // Set custom icon
            var uiObj = zone.GetComponent<UIObject>();
            if (uiObj != null)
            {
                uiObj.m_Icon = $"coui://{Mod.HostName}/HeavyIndustry.svg";
                Mod.log.Info($"Set Heavy Industry icon to: {uiObj.m_Icon}");
            }

            return zone;
        }

        private ZonePrefab CreateWarehouseZone(ZonePrefab sourceZone)
        {
            var zone = sourceZone.Clone("WarehouseIndustrial") as ZonePrefab;

            if (zone == null)
            {
                Mod.log.Error("Failed to clone zone for Warehouses");
                return null;
            }

            // Greenish-yellow color (more green than vanilla yellow)
            zone.m_Edge = new Color(0.65f, 0.75f, 0.0f);
            Mod.log.Info($"Warehouse zone color: {zone.m_Edge} (greenish-yellow)");

            // Set pollution on the ZONE itself (same as light industry)
            var zonePollution = zone.GetComponent<ZonePollution>();
            if (zonePollution != null)
            {
                zonePollution.m_AirPollution = PollutionConstants.LIGHT_INDUSTRY_AIR;
                zonePollution.m_GroundPollution = PollutionConstants.LIGHT_INDUSTRY_GROUND;
                zonePollution.m_NoisePollution = PollutionConstants.LIGHT_INDUSTRY_NOISE;
                Mod.log.Info($"Set warehouse zone pollution: Air={zonePollution.m_AirPollution}, Ground={zonePollution.m_GroundPollution}, Noise={zonePollution.m_NoisePollution}");
            }

            // Warehouses can store ALL products (both light and heavy)
            // But CANNOT manufacture anything (empty manufactured array)
            var zoneProperties = zone.GetComponent<ZoneProperties>();
            if (zoneProperties != null)
            {
                // CRITICAL: Empty manufactured resources = warehouse only!
                zoneProperties.m_AllowedManufactured = new ResourceInEditor[] { };

                // Can store everything
                zoneProperties.m_AllowedStored = new ResourceInEditor[]
                {
                    // Light Industry products
                    ResourceInEditor.ConvenienceFood,
                    ResourceInEditor.Food,
                    ResourceInEditor.Furniture,
                    ResourceInEditor.Paper,
                    ResourceInEditor.Vehicles,
                    ResourceInEditor.Electronics,
                    ResourceInEditor.Machinery,
                    ResourceInEditor.Pharmaceuticals,
                    ResourceInEditor.Beverages,
                    ResourceInEditor.Textiles,
                    // Heavy Industry products
                    ResourceInEditor.Metals,
                    ResourceInEditor.Petrochemicals,
                    ResourceInEditor.Steel,
                    ResourceInEditor.Minerals,
                    ResourceInEditor.Chemicals,
                    ResourceInEditor.Plastics,
                    ResourceInEditor.Concrete,
                    // Raw materials
                    ResourceInEditor.Wood,
                    ResourceInEditor.Grain,
                    ResourceInEditor.Livestock,
                    ResourceInEditor.Fish,
                    ResourceInEditor.Vegetables,
                    ResourceInEditor.Cotton,
                    ResourceInEditor.Oil,
                    ResourceInEditor.Ore,
                    ResourceInEditor.Coal,
                    ResourceInEditor.Stone
                };

                Mod.log.Info("Warehouse zone: NO manufacturing allowed, ALL storage allowed");
            }

            // Set custom icon
            var uiObj = zone.GetComponent<UIObject>();
            if (uiObj != null)
            {
                uiObj.m_Icon = $"coui://{Mod.HostName}/Warehouses.svg";
                Mod.log.Info($"Set Warehouse icon to: {uiObj.m_Icon}");
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
            int filteredByType = 0;

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

            // Get zone properties to check if this is a warehouse-only zone
            var zoneProps = targetZone.GetComponent<ZoneProperties>();
            bool isWarehouseOnlyZone = zoneProps != null &&
                                        (zoneProps.m_AllowedManufactured == null || zoneProps.m_AllowedManufactured.Length == 0);

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

                    // CRITICAL: Filter by building type for warehouse zones
                    var buildingProps = sourceBuilding.GetComponent<BuildingProperties>();
                    if (buildingProps != null)
                    {
                        bool hasManufacturing = buildingProps.m_AllowedManufactured != null && buildingProps.m_AllowedManufactured.Length > 0;
                        bool hasStorage = buildingProps.m_AllowedStored != null && buildingProps.m_AllowedStored.Length > 0;

                        if (isWarehouseOnlyZone)
                        {
                            // Warehouse zone: ONLY accept storage buildings with NO manufacturing
                            if (hasManufacturing || !hasStorage)
                            {
                                filteredByType++;
                                continue; // Skip manufacturing buildings
                            }
                        }
                        else
                        {
                            // Manufacturing zone: ONLY accept manufacturing buildings
                            if (!hasManufacturing)
                            {
                                filteredByType++;
                                continue; // Skip warehouse-only buildings
                            }
                        }
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

                    // Apply BuildingProperties resource filtering (this doesn't need entity)
                    ApplyBuildingPropertiesFiltering(building, isLightIndustry, isWarehouseOnlyZone);

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

            if (filteredByType > 0)
            {
                Mod.log.Info($"Filtered out {filteredByType} buildings by type (warehouse vs manufacturing)");
            }

            if (totalChimneysRemoved > 0)
            {
                Mod.log.Info($"Removed {totalChimneysRemoved} chimney/smoke/decoration components from building prefabs");
            }

            return clonedBuildings;
        }

        /// <summary>
        /// Apply BuildingProperties resource filtering (doesn't require entity)
        /// </summary>
        private void ApplyBuildingPropertiesFiltering(BuildingPrefab building, bool isLightIndustry, bool isWarehouseOnly = false)
        {
            var buildingProperties = building.GetComponent<BuildingProperties>();
            if (buildingProperties != null)
            {
                if (isWarehouseOnly)
                {
                    // Warehouse buildings: NO manufacturing, only storage
                    buildingProperties.m_AllowedManufactured = new ResourceInEditor[] { };

                    // Keep existing storage capabilities (already filtered by zone)
                    // No changes needed to m_AllowedStored
                }
                else if (isLightIndustry)
                {
                    // Light Industry: Consumer goods only
                    buildingProperties.m_AllowedManufactured = new ResourceInEditor[]
                    {
                        ResourceInEditor.ConvenienceFood,
                        ResourceInEditor.Food,
                        ResourceInEditor.Furniture,
                        ResourceInEditor.Paper,
                        ResourceInEditor.Electronics,
                        ResourceInEditor.Machinery,
                        ResourceInEditor.Pharmaceuticals,
                        ResourceInEditor.Beverages,
                        ResourceInEditor.Textiles
                    };

                    // Warehouses can store consumer goods + light raw materials
                    buildingProperties.m_AllowedStored = new ResourceInEditor[]
                    {
                        // Manufactured consumer goods
                        ResourceInEditor.ConvenienceFood,
                        ResourceInEditor.Food,
                        ResourceInEditor.Furniture,
                        ResourceInEditor.Paper,
                        ResourceInEditor.Electronics,
                        ResourceInEditor.Machinery,
                        ResourceInEditor.Pharmaceuticals,
                        ResourceInEditor.Beverages,
                        ResourceInEditor.Textiles,
                        ResourceInEditor.Vehicles,
                        // Raw materials (agriculture/forestry)
                        ResourceInEditor.Wood,
                        ResourceInEditor.Grain,
                        ResourceInEditor.Livestock,
                        ResourceInEditor.Fish,
                        ResourceInEditor.Vegetables,
                        ResourceInEditor.Cotton
                    };
                }
                else
                {
                    // Heavy Industry: Industrial materials only
                    buildingProperties.m_AllowedManufactured = new ResourceInEditor[]
                    {
                        ResourceInEditor.Metals,
                        ResourceInEditor.Petrochemicals,
                        ResourceInEditor.Steel,
                        ResourceInEditor.Minerals,
                        ResourceInEditor.Chemicals,
                        ResourceInEditor.Plastics,
                        ResourceInEditor.Concrete,
                        ResourceInEditor.Vehicles
                    };

                    // Warehouses can store industrial materials + heavy raw materials
                    buildingProperties.m_AllowedStored = new ResourceInEditor[]
                    {
                        // Manufactured industrial materials
                        ResourceInEditor.Metals,
                        ResourceInEditor.Petrochemicals,
                        ResourceInEditor.Steel,
                        ResourceInEditor.Minerals,
                        ResourceInEditor.Chemicals,
                        ResourceInEditor.Plastics,
                        ResourceInEditor.Concrete,
                        // Raw materials (mining/oil)
                        ResourceInEditor.Oil,
                        ResourceInEditor.Ore,
                        ResourceInEditor.Coal,
                        ResourceInEditor.Stone
                    };
                }
            }
        }

        /// <summary>
        /// Apply economic modifiers to building PREFABS using ECS components
        /// MUST be called AFTER buildings are added to PrefabSystem with AddPrefab()
        /// </summary>
        private void ApplyEconomicModifiersToZone(List<BuildingPrefab> buildings, bool isLightIndustry)
        {
            int successCount = 0;
            int failCount = 0;

            foreach (var building in buildings)
            {
                try
                {
                    // Get the prefab entity for this building
                    if (!_prefabSystem.TryGetEntity(building, out var prefabEntity))
                    {
                        Mod.log.Warn($"Could not get entity for building: {building.name}");
                        failCount++;
                        continue;
                    }

                    float productionMult = isLightIndustry ?
                        EconomicConstants.LIGHT_PRODUCTION_MULTIPLIER :
                        EconomicConstants.HEAVY_PRODUCTION_MULTIPLIER;
                    float electricityMult = isLightIndustry ?
                        EconomicConstants.LIGHT_ELECTRICITY_MULTIPLIER :
                        EconomicConstants.HEAVY_ELECTRICITY_MULTIPLIER;
                    float waterMult = isLightIndustry ?
                        EconomicConstants.LIGHT_WATER_MULTIPLIER :
                        EconomicConstants.HEAVY_WATER_MULTIPLIER;
                    float garbageMult = isLightIndustry ?
                        EconomicConstants.LIGHT_GARBAGE_MULTIPLIER :
                        EconomicConstants.HEAVY_GARBAGE_MULTIPLIER;
                    float upkeepMult = isLightIndustry ?
                        EconomicConstants.LIGHT_UPKEEP_MULTIPLIER :
                        EconomicConstants.HEAVY_UPKEEP_MULTIPLIER;

                    // 1. Modify ServiceConsumption (ComponentBase - this works)
                    var serviceConsumption = building.GetComponent<ServiceConsumption>();
                    if (serviceConsumption != null)
                    {
                        serviceConsumption.m_ElectricityConsumption = (int)(serviceConsumption.m_ElectricityConsumption * electricityMult);
                        serviceConsumption.m_WaterConsumption = (int)(serviceConsumption.m_WaterConsumption * waterMult);
                        serviceConsumption.m_GarbageAccumulation = (int)(serviceConsumption.m_GarbageAccumulation * garbageMult);
                        serviceConsumption.m_Upkeep = (int)(serviceConsumption.m_Upkeep * upkeepMult);
                    }

                    // 2. Modify WorkplaceData (ECS component on prefab entity)
                    if (EntityManager.HasComponent<WorkplaceData>(prefabEntity))
                    {
                        var workplaceData = EntityManager.GetComponentData<WorkplaceData>(prefabEntity);

                        if (isLightIndustry)
                        {
                            // Light Industry: Complex work (high education)
                            workplaceData.m_Complexity = WorkplaceComplexity.Complex;
                        }
                        else
                        {
                            // Heavy Industry: Simple work (low education)
                            workplaceData.m_Complexity = WorkplaceComplexity.Simple;
                        }

                        EntityManager.SetComponentData(prefabEntity, workplaceData);
                    }

                    // 3. Modify IndustrialProcessData (ECS component on prefab entity)
                    if (EntityManager.HasComponent<IndustrialProcessData>(prefabEntity))
                    {
                        var industrialProcess = EntityManager.GetComponentData<IndustrialProcessData>(prefabEntity);

                        // Scale output
                        industrialProcess.m_Output.m_Amount = (int)(industrialProcess.m_Output.m_Amount * productionMult);

                        // Scale inputs proportionally
                        industrialProcess.m_Input1.m_Amount = (int)(industrialProcess.m_Input1.m_Amount * productionMult);
                        industrialProcess.m_Input2.m_Amount = (int)(industrialProcess.m_Input2.m_Amount * productionMult);

                        EntityManager.SetComponentData(prefabEntity, industrialProcess);
                    }

                    successCount++;

                    // Log first 3 buildings to verify modifiers are working
                    if (successCount <= 3)
                    {
                        LogBuildingStats(building, prefabEntity, isLightIndustry);
                    }
                }
                catch (Exception ex)
                {
                    Mod.log.Error(ex, $"Failed to apply economic modifiers to {building.name}");
                    failCount++;
                }
            }

            Mod.log.Info($"Applied economic modifiers: {successCount} succeeded, {failCount} failed");
        }

        /// <summary>
        /// Log building stats to verify economic modifiers are applied correctly
        /// </summary>
        private void LogBuildingStats(BuildingPrefab building, Entity prefabEntity, bool isLight)
        {
            Mod.log.Info($"=== VERIFY: {building.name} ({(isLight ? "Light" : "Heavy")}) ===");

            // Check ServiceConsumption (ComponentBase)
            var service = building.GetComponent<ServiceConsumption>();
            if (service != null)
            {
                Mod.log.Info($"  Electricity: {service.m_ElectricityConsumption}");
                Mod.log.Info($"  Water: {service.m_WaterConsumption}");
                Mod.log.Info($"  Garbage: {service.m_GarbageAccumulation}");
                Mod.log.Info($"  Upkeep: ${service.m_Upkeep}");
            }

            // Check ECS components on prefab entity
            if (EntityManager.HasComponent<WorkplaceData>(prefabEntity))
            {
                var workplace = EntityManager.GetComponentData<WorkplaceData>(prefabEntity);
                Mod.log.Info($"  Complexity: {workplace.m_Complexity}");
            }

            if (EntityManager.HasComponent<IndustrialProcessData>(prefabEntity))
            {
                var process = EntityManager.GetComponentData<IndustrialProcessData>(prefabEntity);
                Mod.log.Info($"  Output Amount: {process.m_Output.m_Amount}");
                Mod.log.Info($"  Input1 Amount: {process.m_Input1.m_Amount}");
            }

            // Check BuildingProperties (ComponentBase)
            var buildingProps = building.GetComponent<BuildingProperties>();
            if (buildingProps != null)
            {
                Mod.log.Info($"  Allowed Manufactured: {buildingProps.m_AllowedManufactured?.Length ?? 0} types");
                Mod.log.Info($"  Allowed Stored: {buildingProps.m_AllowedStored?.Length ?? 0} types");
            }
        }

        private int RemoveChimneysFromPrefabComponents(BuildingPrefab building)
        {
            int removedCount = 0;
            var random = new System.Random(building.name.GetHashCode()); // Deterministic per building

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

                                    // EXACT MATCH CHECK for effects (always remove)
                                    if (EffectNamesToRemove.Contains(prefabName))
                                    {
                                        shouldRemove = true;
                                    }

                                    // SUBSTRING CHECK for chimneys, smoke, fire, lights (always remove)
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

                                    // PROBABILITY CHECK for decorations (sometimes remove)
                                    if (!shouldRemove && ReducedProbabilityDecorations.Contains(prefabName))
                                    {
                                        // Roll dice - remove if random value exceeds probability threshold
                                        if (random.NextDouble() > LIGHT_DECORATION_PROBABILITY)
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

        public bool IsWarehouseZone(ZonePrefab zone)
        {
            return zone != null && zone.name == "WarehouseIndustrial";
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

                Entity warehouseEntity = Entity.Null;
                if (_warehouseZone != null)
                {
                    if (!_prefabSystem.TryGetEntity(_warehouseZone, out warehouseEntity))
                    {
                        Mod.log.Warn("Could not get entity for Warehouse zone");
                    }
                }

                var localeSource = new ZoneLocalizationSource(lightEntity, heavyEntity, warehouseEntity);

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
                Setting.AvailableWarehouseBuildingNames.Clear();

                // Add all industrial building names to both lists (they're the same source buildings)
                foreach (var building in industrialBuildings.OrderBy(b => b.name))
                {
                    Setting.AvailableLightBuildingNames.Add(building.name);
                    Setting.AvailableHeavyBuildingNames.Add(building.name);
                }

                // Add warehouse building names separately
                var warehouses = industrialBuildings.Where(b => IsWarehouseTypeBuilding(b)).ToArray();
                foreach (var warehouse in warehouses.OrderBy(b => b.name))
                {
                    Setting.AvailableWarehouseBuildingNames.Add(warehouse.name);
                }

                Mod.log.Info($"SUCCESS: Populated dropdown lists with {industrialBuildings.Length} building names");
                Mod.log.Info($"AvailableLightBuildingNames.Count = {Setting.AvailableLightBuildingNames.Count}");
                Mod.log.Info($"AvailableHeavyBuildingNames.Count = {Setting.AvailableHeavyBuildingNames.Count}");
                Mod.log.Info($"AvailableWarehouseBuildingNames.Count = {Setting.AvailableWarehouseBuildingNames.Count}");

                // NOW register the settings UI since the dropdowns are ready
                Mod.Settings.RegisterInOptionsUI();
                Mod.log.Info("Settings UI registered - dropdowns are now populated");
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "Failed to populate settings dropdowns");
            }
        }


        /// <summary>
        /// Log ALL components on a building prefab
        /// </summary>
        private void LogAllComponents(BuildingPrefab building, string label)
        {
            Mod.log.Info($"{label} - ALL COMPONENTS:");

            var components = new List<ComponentBase>();
            building.GetComponents(components);

            foreach (var component in components)
            {
                Mod.log.Info($"  - {component.GetType().Name}");
            }

            // Also check if there's a prefab entity and what ECS components it has
            if (_prefabSystem.TryGetEntity(building, out var prefabEntity))
            {
                Mod.log.Info($"{label} - ECS COMPONENTS:");

                // Log common industrial components
                if (EntityManager.HasComponent<IndustrialProcessData>(prefabEntity))
                {
                    Mod.log.Info($"  - Has IndustrialProcessData");
                    var processData = EntityManager.GetComponentData<IndustrialProcessData>(prefabEntity);
                    Mod.log.Info($"    Output: {processData.m_Output.m_Resource} x{processData.m_Output.m_Amount}");
                    Mod.log.Info($"    Input1: {processData.m_Input1.m_Resource} x{processData.m_Input1.m_Amount}");
                    Mod.log.Info($"    Input2: {processData.m_Input2.m_Resource} x{processData.m_Input2.m_Amount}");
                }
                else
                {
                    Mod.log.Info($"  - NO IndustrialProcessData");
                }

                if (EntityManager.HasComponent<WorkplaceData>(prefabEntity))
                {
                    Mod.log.Info($"  - Has WorkplaceData");
                    var workData = EntityManager.GetComponentData<WorkplaceData>(prefabEntity);
                    Mod.log.Info($"    Complexity: {workData.m_Complexity}");
                }
                else
                {
                    Mod.log.Info($"  - NO WorkplaceData");
                }

                if (EntityManager.HasComponent<Game.Prefabs.SpawnableBuildingData>(prefabEntity))
                {
                    Mod.log.Info($"  - Has SpawnableBuildingData");
                }

                // Dump ALL ECS components
                Mod.log.Info($"  - ALL ECS components on this prefab:");
                var allTypes = EntityManager.GetComponentTypes(prefabEntity);
                foreach (var type in allTypes)
                {
                    var typeName = type.GetManagedType()?.Name ?? type.ToString();
                    Mod.log.Info($"    * {typeName}");
                }
                allTypes.Dispose();
            }
        }

        /// <summary>
        /// Log detailed BuildingProperties information
        /// </summary>
        private void LogBuildingPropertiesDetails(BuildingPrefab building, string label)
        {
            var props = building.GetComponent<BuildingProperties>();
            if (props == null)
            {
                Mod.log.Info($"{label} - NO BuildingProperties component!");
                return;
            }

            Mod.log.Info($"{label} - BuildingProperties Details:");
            Mod.log.Info($"  m_AllowedManufactured: {props.m_AllowedManufactured?.Length ?? 0} types");
            if (props.m_AllowedManufactured != null && props.m_AllowedManufactured.Length > 0)
            {
                foreach (var resource in props.m_AllowedManufactured)
                    Mod.log.Info($"    - {resource}");
            }

            Mod.log.Info($"  m_AllowedStored: {props.m_AllowedStored?.Length ?? 0} types");
            if (props.m_AllowedStored != null && props.m_AllowedStored.Length > 0)
            {
                foreach (var resource in props.m_AllowedStored)
                    Mod.log.Info($"    - {resource}");
            }

            Mod.log.Info($"  m_AllowedSold: {props.m_AllowedSold?.Length ?? 0} types");
            if (props.m_AllowedSold != null && props.m_AllowedSold.Length > 0)
            {
                foreach (var resource in props.m_AllowedSold)
                    Mod.log.Info($"    - {resource}");
            }

            Mod.log.Info($"  m_AllowedInput: {props.m_AllowedInput?.Length ?? 0} types");
            if (props.m_AllowedInput != null && props.m_AllowedInput.Length > 0)
            {
                foreach (var resource in props.m_AllowedInput)
                    Mod.log.Info($"    - {resource}");
            }

            Mod.log.Info($"  m_ResidentialProperties: {props.m_ResidentialProperties}");

            // Log any other properties that might exist
            var propsType = props.GetType();
            Mod.log.Info($"  BuildingProperties type: {propsType.Name}");
            Mod.log.Info($"  All properties:");
            foreach (var prop in propsType.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(props);
                    if (value != null && !prop.Name.StartsWith("m_Allowed"))
                    {
                        Mod.log.Info($"    {prop.Name}: {value}");
                    }
                }
                catch { }
            }
        }


        /// <summary>
        /// Log a summary of economic differences between Light and Heavy Industry
        /// </summary>
        private void LogEconomicSummary()
        {
            Mod.log.Info("=== Economic Configuration Summary ===");
            Mod.log.Info("LIGHT INDUSTRY:");
            Mod.log.Info($"  Production: {EconomicConstants.LIGHT_PRODUCTION_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Electricity: {EconomicConstants.LIGHT_ELECTRICITY_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Water: {EconomicConstants.LIGHT_WATER_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Garbage: {EconomicConstants.LIGHT_GARBAGE_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Upkeep: {EconomicConstants.LIGHT_UPKEEP_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Education: Complex (needs educated workers)");
            Mod.log.Info($"  Pollution: {PollutionConstants.LIGHT_INDUSTRY_AIR}/{PollutionConstants.LIGHT_INDUSTRY_GROUND}/{PollutionConstants.LIGHT_INDUSTRY_NOISE}");
            Mod.log.Info($"  Products: Consumer goods only");

            Mod.log.Info("HEAVY INDUSTRY:");
            Mod.log.Info($"  Production: {EconomicConstants.HEAVY_PRODUCTION_MULTIPLIER:P0} of vanilla (15% BOOST!)");
            Mod.log.Info($"  Electricity: {EconomicConstants.HEAVY_ELECTRICITY_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Water: {EconomicConstants.HEAVY_WATER_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Garbage: {EconomicConstants.HEAVY_GARBAGE_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Upkeep: {EconomicConstants.HEAVY_UPKEEP_MULTIPLIER:P0} of vanilla");
            Mod.log.Info($"  Education: Simple (accepts uneducated workers)");
            Mod.log.Info($"  Pollution: 100/100/100 (vanilla heavy pollution)");
            Mod.log.Info($"  Products: Industrial materials only");

            if (Mod.Settings.EnableWarehouseZone)
            {
                Mod.log.Info("WAREHOUSE ZONE:");
                Mod.log.Info($"  Enabled: YES");
                Mod.log.Info($"  Products: ALL (light + heavy)");
                Mod.log.Info($"  Pollution: Same as Light Industry");
            }
            else
            {
                Mod.log.Info("WAREHOUSE ZONE: Disabled (warehouses spawn in Light/Heavy zones)");
            }
        }
    }
}