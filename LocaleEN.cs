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
                { m_Setting.GetOptionTabLocaleID(Setting.kBlacklistSection), "Building Blacklists" },
                { m_Setting.GetOptionTabLocaleID(Setting.kWhitelistSection), "Building Whitelists" },
                { m_Setting.GetOptionTabLocaleID(Setting.kUninstallSection), "Safe Uninstall" },
                
                // Groups
                { m_Setting.GetOptionGroupLocaleID("LightIndustryBlacklist"), "Light Industry Blacklist" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryBlacklist"), "Heavy Industry Blacklist" },
                { m_Setting.GetOptionGroupLocaleID("LightIndustryWhitelist"), "Light Industry Whitelist" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryWhitelist"), "Heavy Industry Whitelist" },
                { m_Setting.GetOptionGroupLocaleID("SafeUninstall"), "Before Uninstalling Mod" },
                
                // Light Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Current Blacklisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Buildings currently excluded from Light Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Select Building" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Choose a building to blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddToLightBlacklist)), "Add to Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddToLightBlacklist)), "Add selected building to blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RemoveLastLightBlacklist)), "Remove Last" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RemoveLastLightBlacklist)), "Remove most recent entry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightBlacklist)), "Clear All" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightBlacklist)), "Remove all blacklisted buildings." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightBlacklist)), "This will clear ALL blacklisted buildings!" },
                
                // Heavy Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Current Blacklisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Buildings currently excluded from Heavy Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Select Building" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Choose a building to blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddToHeavyBlacklist)), "Add to Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddToHeavyBlacklist)), "Add selected building to blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RemoveLastHeavyBlacklist)), "Remove Last" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RemoveLastHeavyBlacklist)), "Remove most recent entry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Clear All" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Remove all blacklisted buildings." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyBlacklist)), "This will clear ALL blacklisted buildings!" },
                
                // Light Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Current Whitelisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Buildings forced to appear in Light Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Select Building" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Choose a building to whitelist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddToLightWhitelist)), "Add to Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddToLightWhitelist)), "Add selected building to whitelist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RemoveLastLightWhitelist)), "Remove Last" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RemoveLastLightWhitelist)), "Remove most recent entry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightWhitelist)), "Clear All" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightWhitelist)), "Remove all whitelisted buildings." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightWhitelist)), "This will clear ALL whitelisted buildings!" },
                
                // Heavy Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Current Whitelisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Buildings forced to appear in Heavy Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Select Building" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Choose a building to whitelist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddToHeavyWhitelist)), "Add to Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddToHeavyWhitelist)), "Add selected building to whitelist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RemoveLastHeavyWhitelist)), "Remove Last" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RemoveLastHeavyWhitelist)), "Remove most recent entry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Clear All" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Remove all whitelisted buildings." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyWhitelist)), "This will clear ALL whitelisted buildings!" },
                
                // Uninstall settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConvertToVanillaZones)), "Convert All Buildings to Vanilla" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConvertToVanillaZones)), "Click BEFORE uninstalling to convert all Light/Heavy Industrial buildings back to vanilla." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ConvertToVanillaZones)), "This will convert ALL Light/Heavy Industrial buildings to vanilla!" },
            };
        }

        public void Unload() { }
    }
}