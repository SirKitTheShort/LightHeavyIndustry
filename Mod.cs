using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace LightHeavyIndustry
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(LightHeavyIndustry)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        internal static Setting Settings;
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            // Initialize settings
            m_Setting = new Setting(this);
            Settings = m_Setting;
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            AssetDatabase.global.LoadSettings(nameof(LightHeavyIndustry), m_Setting, new Setting(this));

            // Register the zoning system FIRST (it creates the zones)
            log.Info("Registering LightHeavyIndustryZoningSystem...");
            updateSystem.UpdateBefore<LightHeavyIndustry.Systems.LightHeavyIndustryZoningSystem, Game.Prefabs.PrefabSystem>(SystemUpdatePhase.MainLoop);

            // Register the building processor system (removes chimneys from spawned buildings)
            log.Info("Registering LightIndustryBuildingProcessorSystem...");
            updateSystem.UpdateAt<LightHeavyIndustry.Systems.LightIndustryBuildingProcessorSystem>(SystemUpdatePhase.GameSimulation);

            // Register the zone conversion system (for safe uninstall)
            log.Info("Registering ZoneConversionSystem...");
            updateSystem.UpdateAt<LightHeavyIndustry.Systems.ZoneConversionSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
                Settings = null;
            }
        }
    }
}