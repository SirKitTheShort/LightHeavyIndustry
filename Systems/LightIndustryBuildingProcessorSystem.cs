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
    /// Minimal monitoring system for Light Industry buildings
    /// Only processes buildings ONCE when they transition from UnderConstruction to completed
    /// </summary>
    public partial class LightIndustryBuildingProcessorSystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _justCompletedBuildingsQuery;

        // Track which buildings have been processed (prevent duplicate processing)
        private HashSet<int> _processedBuildings = new();

        // Frame counter for cleanup
        private uint _frameCounter = 0;

        protected override void OnCreate()
        {
            base.OnCreate();

            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Query for buildings that JUST completed construction
            // This query will only match buildings for ONE frame (when Updated component is added after construction completes)
            _justCompletedBuildingsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Building>(),
                    ComponentType.ReadOnly<PrefabRef>(),
                    ComponentType.ReadOnly<Game.Buildings.IndustrialProperty>(),
                    ComponentType.ReadOnly<Updated>()  // CRITICAL: Only buildings that just got Updated component
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Game.Objects.UnderConstruction>(),
                    ComponentType.ReadOnly<Deleted>()
                }
            });

            Mod.log.Info("LightIndustryBuildingProcessorSystem created (minimal mode - only processes newly completed buildings once)");
        }

        protected override void OnUpdate()
        {
            _frameCounter++;

            // Only process if there are buildings that just completed construction
            if (!_justCompletedBuildingsQuery.IsEmptyIgnoreFilter)
            {
                ProcessJustCompletedBuildings();
            }

            // Cleanup old entries every 10 seconds to prevent memory growth
            if (_frameCounter % 600 == 0)
            {
                CleanupOldEntries();
            }
        }

        /// <summary>
        /// Process buildings that JUST completed construction (one-time processing)
        /// </summary>
        private void ProcessJustCompletedBuildings()
        {
            var buildings = _justCompletedBuildingsQuery.ToEntityArray(Allocator.Temp);
            int processedCount = 0;

            foreach (var buildingEntity in buildings)
            {
                // Skip if already processed
                if (_processedBuildings.Contains(buildingEntity.Index))
                    continue;

                if (!IsLightIndustryBuilding(buildingEntity))
                    continue;

                try
                {
                    // Mark as processed - no pollution setting needed, it's on the zone!
                    _processedBuildings.Add(buildingEntity.Index);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    Mod.log.Error(ex, $"Failed to process Light Industry building {buildingEntity.Index}");
                }
            }

            if (processedCount > 0)
            {
                Mod.log.Info($"Processed {processedCount} newly completed Light Industry buildings");
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

        private void SetLightIndustryPollution(Entity buildingEntity)
        {
            if (!EntityManager.HasComponent<PrefabRef>(buildingEntity))
                return;

            var prefabRef = EntityManager.GetComponentData<PrefabRef>(buildingEntity);

            if (!EntityManager.HasComponent<PollutionData>(prefabRef.m_Prefab))
                return;

            var pollutionData = EntityManager.GetComponentData<PollutionData>(prefabRef.m_Prefab);

            // Set the pollution values using the constants
            pollutionData.m_AirPollution = PollutionConstants.LIGHT_INDUSTRY_AIR;
            pollutionData.m_GroundPollution = PollutionConstants.LIGHT_INDUSTRY_GROUND;
            pollutionData.m_NoisePollution = PollutionConstants.LIGHT_INDUSTRY_NOISE;

            EntityManager.SetComponentData(prefabRef.m_Prefab, pollutionData);

            Mod.log.Debug($"Set pollution on building {buildingEntity.Index}: Air={pollutionData.m_AirPollution}, Ground={pollutionData.m_GroundPollution}, Noise={pollutionData.m_NoisePollution}");
        }

        /// <summary>
        /// Clean up processed building IDs that no longer exist to prevent memory leak
        /// </summary>
        private void CleanupOldEntries()
        {
            if (_processedBuildings.Count == 0)
                return;

            var toRemove = new List<int>();

            foreach (var buildingIndex in _processedBuildings)
            {
                // Check if entity still exists by trying to find it
                // If we can't find it in reasonable time, assume it's been demolished
                bool exists = false;

                var allBuildings = GetEntityQuery(ComponentType.ReadOnly<Building>()).ToEntityArray(Allocator.Temp);
                foreach (var entity in allBuildings)
                {
                    if (entity.Index == buildingIndex && EntityManager.Exists(entity))
                    {
                        exists = true;
                        break;
                    }
                }
                allBuildings.Dispose();

                if (!exists)
                {
                    toRemove.Add(buildingIndex);
                }
            }

            foreach (var index in toRemove)
            {
                _processedBuildings.Remove(index);
            }

            if (toRemove.Count > 0)
            {
                Mod.log.Info($"Cleaned up {toRemove.Count} demolished building entries from tracking");
            }
        }
    }
}