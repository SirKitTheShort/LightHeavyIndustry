using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.SceneFlow;
using System.IO;

namespace LightHeavyIndustry
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(LightHeavyIndustry)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        internal static Setting Settings;
        private Setting m_Setting;

        // CRITICAL: This is the hostname for the coui:// protocol
        public const string HostName = "lightheavyindustry";

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                log.Info($"Current mod asset at {asset.path}");

                // Register UI resources for coui:// protocol
                var modPath = Path.GetDirectoryName(asset.GetMeta().path);
                var uiPath = Path.Combine(modPath, "UI");

                if (Directory.Exists(uiPath))
                {
                    log.Info($"Registering UI resources from: {uiPath}");
                    try
                    {
                        UIManager.defaultUISystem.AddHostLocation(HostName, uiPath);
                        log.Info($"UI resources registered at coui://{HostName}/");
                    }
                    catch (System.Exception ex)
                    {
                        log.Error($"Failed to register UI resources: {ex.Message}");
                    }
                }
                else
                {
                    log.Warn($"UI folder not found at: {uiPath}");
                }
            }

            // Initialize settings (but DON'T register in UI yet - wait for buildings to load)
            m_Setting = new Setting(this);
            Settings = m_Setting;

            // Load saved settings from disk
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            AssetDatabase.global.LoadSettings(nameof(LightHeavyIndustry), m_Setting, new Setting(this));

            // Register the zoning system FIRST (it creates the zones and populates dropdowns)
            log.Info("Registering LightHeavyIndustryZoningSystem...");
            updateSystem.UpdateBefore<LightHeavyIndustry.Systems.LightHeavyIndustryZoningSystem, Game.Prefabs.PrefabSystem>(SystemUpdatePhase.MainLoop);

            // Register the building processor system (removes chimneys from spawned buildings)
            log.Info("Registering LightIndustryBuildingProcessorSystem...");
            updateSystem.UpdateAt<LightHeavyIndustry.Systems.LightIndustryBuildingProcessorSystem>(SystemUpdatePhase.GameSimulation);

            // Register the zone conversion system (for safe uninstall)
            log.Info("Registering ZoneConversionSystem...");
            updateSystem.UpdateAt<LightHeavyIndustry.Systems.ZoneConversionSystem>(SystemUpdatePhase.GameSimulation);

            // IMPORTANT: Register settings in UI AFTER systems are set up
            // The zoning system will signal when dropdowns are ready
            log.Info("Settings loaded - will register UI after building list is populated");
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