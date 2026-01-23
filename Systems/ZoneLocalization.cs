using System.Collections.Generic;
using Colossal;
using Game.SceneFlow;
using Unity.Entities;

namespace LightHeavyIndustry.Systems
{
    /// <summary>
    /// Provides UI labels for the custom zone types
    /// </summary>
    public class ZoneLocalizationSource : IDictionarySource
    {
        private readonly Entity _lightZoneEntity;
        private readonly Entity _heavyZoneEntity;
        private readonly Entity _warehouseZoneEntity;
        private readonly Game.UI.InGame.PrefabUISystem _prefabUISystem;

        public ZoneLocalizationSource(Entity lightZone, Entity heavyZone, Entity warehouseZone)
        {
            _lightZoneEntity = lightZone;
            _heavyZoneEntity = heavyZone;
            _warehouseZoneEntity = warehouseZone;
            _prefabUISystem = World.DefaultGameObjectInjectionWorld?
                .GetExistingSystemManaged<Game.UI.InGame.PrefabUISystem>();
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>();

            if (_prefabUISystem == null)
            {
                Mod.log.Warn("PrefabUISystem not available for zone localization");
                return entries;
            }

            // Get the title and description IDs for Light Industry
            if (_lightZoneEntity != Entity.Null)
            {
                _prefabUISystem.GetTitleAndDescription(_lightZoneEntity, out var lightTitleID, out var lightDescID);
                entries[lightTitleID] = "Light Industrial Manufacturing";
                entries[lightDescID] = "Workshops and factories that produce consumer goods with a smaller environmental impact.";
            }

            // Get the title and description IDs for Heavy Industry
            if (_heavyZoneEntity != Entity.Null)
            {
                _prefabUISystem.GetTitleAndDescription(_heavyZoneEntity, out var heavyTitleID, out var heavyDescID);
                entries[heavyTitleID] = "Heavy Industrial Manufacturing";
                entries[heavyDescID] = "Workshops and factories that produce intermediate materials with significant environmental impact.";
            }

            // Get the title and description IDs for Warehouse Zone (if enabled)
            if (_warehouseZoneEntity != Entity.Null)
            {
                _prefabUISystem.GetTitleAndDescription(_warehouseZoneEntity, out var warehouseTitleID, out var warehouseDescID);
                entries[warehouseTitleID] = "Warehousing";
                entries[warehouseDescID] = "Warehouses, storage yards, and distribution facilities used for storing and moving industrial goods.";
            }

            return entries;
        }

        public void Unload() { }
    }
}