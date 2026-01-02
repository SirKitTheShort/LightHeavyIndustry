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
        private readonly Game.UI.InGame.PrefabUISystem _prefabUISystem;

        public ZoneLocalizationSource(Entity lightZone, Entity heavyZone)
        {
            _lightZoneEntity = lightZone;
            _heavyZoneEntity = heavyZone;
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
                entries[lightDescID] = "Light industry with manufacturing and warehouses. Lower pollution and noise, but less profitable. No chimneys or smoke.";
            }

            // Get the title and description IDs for Heavy Industry
            if (_heavyZoneEntity != Entity.Null)
            {
                _prefabUISystem.GetTitleAndDescription(_heavyZoneEntity, out var heavyTitleID, out var heavyDescID);

                entries[heavyTitleID] = "Heavy Industrial Manufacturing";
                entries[heavyDescID] = "Heavy industry with manufacturing and warehouses. Higher pollution and noise, but more profitable. Traditional industrial appearance.";
            }

            return entries;
        }

        public void Unload() { }
    }
}