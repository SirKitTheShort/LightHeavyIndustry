using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace LightHeavyIndustry
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting) => m_Setting = setting;

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Mod title
                { m_Setting.GetSettingsLocaleID(), "Light/Heavy Industry" },
                
                // Sections
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionTabLocaleID(Setting.kPollutionSection), "Pollution Settings" },
                { m_Setting.GetOptionTabLocaleID(Setting.kBlacklistSection), "Building Blacklist" },
                { m_Setting.GetOptionTabLocaleID(Setting.kUninstallSection), "Safe Uninstall" },
                
                // Groups
                { m_Setting.GetOptionGroupLocaleID("Options"), "General Options" },
                { m_Setting.GetOptionGroupLocaleID("Values"), "Pollution Values" },
                { m_Setting.GetOptionGroupLocaleID("Buildings"), "Excluded Buildings" },
                { m_Setting.GetOptionGroupLocaleID("SafeUninstall"), "Before Uninstalling Mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroup), "Actions" },
                
                // Main settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Enabled)), "Enable Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Enabled)), "Turns the mod on/off. Disable if you want to use vanilla industrial zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DryRun)), "Dry Run Mode" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DryRun)), "If enabled, the mod will only log what it would do without making any changes. Useful for testing." },
                
                // Pollution settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryGroundPollution)), "Light Industry Ground Pollution" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryGroundPollution)), "Ground pollution level for Light Industrial Manufacturing buildings. 0 = none (0/3), 30 = low (1/3), 60 = medium (2/3), 100 = high (3/3). Air pollution is always 0, noise is always 30 (1/3)." },
                
                // Blacklist settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryBlacklistText)), "Additional Blacklisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryBlacklistText)), "Additional building prefab names (one per line) that should NOT appear in Light Industrial Manufacturing zones. This is for modded/custom assets only - vanilla buildings are already filtered in code. Use the CS2 Asset Editor to find building names. Lines starting with # are comments." },
                
                // Uninstall settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConvertToVanillaZones)), "Convert All Buildings to Vanilla" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConvertToVanillaZones)), "Click this button BEFORE uninstalling the mod. This will convert all Light/Heavy Industrial buildings back to vanilla Industrial Manufacturing buildings, preventing them from disappearing when you remove the mod." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ConvertToVanillaZones)), "This will convert ALL Light/Heavy Industrial buildings to vanilla. Make sure this is what you want before proceeding!" },
            };
        }

        public void Unload() { }
    }
}