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
                { m_Setting.GetOptionTabLocaleID(Setting.kManagementSection), "Clear Lists" },
                { m_Setting.GetOptionTabLocaleID(Setting.kUninstallSection), "Safe Uninstall" },
                
                // Groups
                { m_Setting.GetOptionGroupLocaleID("LightIndustryBlacklist"), "Light Industry Blacklist" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryBlacklist"), "Heavy Industry Blacklist" },
                { m_Setting.GetOptionGroupLocaleID("LightIndustryWhitelist"), "Light Industry Whitelist" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryWhitelist"), "Heavy Industry Whitelist" },
                { m_Setting.GetOptionGroupLocaleID("ClearLists"), "Clear Lists" },
                { m_Setting.GetOptionGroupLocaleID("SafeUninstall"), "Before Uninstalling Mod" },
                
                // Light Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Current Blacklisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Buildings currently excluded from Light Industry zones. These vanilla buildings will NOT be cloned into Light Industry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Select Building to Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Choose a vanilla building to exclude from Light Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddLightBlacklist)), "Add to Light Industry Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddLightBlacklist)), "Add the selected building to Light Industry blacklist." },
                
                // Heavy Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Current Blacklisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Buildings currently excluded from Heavy Industry zones. These vanilla buildings will NOT be cloned into Heavy Industry." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Select Building to Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Choose a vanilla building to exclude from Heavy Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddHeavyBlacklist)), "Add to Heavy Industry Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddHeavyBlacklist)), "Add the selected building to Heavy Industry blacklist." },
                
                // Light Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Current Whitelisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Buildings forced to appear in Light Industry zones. Whitelist overrides blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Select Building to Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Choose a vanilla building to force-include in Light Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddLightWhitelist)), "Add to Light Industry Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddLightWhitelist)), "Add the selected building to Light Industry whitelist (overrides blacklist)." },
                
                // Heavy Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Current Whitelisted Buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Buildings forced to appear in Heavy Industry zones. Whitelist overrides blacklist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Select Building to Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Choose a vanilla building to force-include in Heavy Industry zones." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddHeavyWhitelist)), "Add to Heavy Industry Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddHeavyWhitelist)), "Add the selected building to Heavy Industry whitelist (overrides blacklist)." },
                
                // Clear buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightBlacklist)), "Clear Light Industry Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightBlacklist)), "Remove all buildings from Light Industry blacklist." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightBlacklist)), "This will clear ALL Light Industry blacklisted buildings!" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Clear Heavy Industry Blacklist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Remove all buildings from Heavy Industry blacklist." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyBlacklist)), "This will clear ALL Heavy Industry blacklisted buildings!" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightWhitelist)), "Clear Light Industry Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightWhitelist)), "Remove all buildings from Light Industry whitelist." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightWhitelist)), "This will clear ALL Light Industry whitelisted buildings!" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Clear Heavy Industry Whitelist" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Remove all buildings from Heavy Industry whitelist." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyWhitelist)), "This will clear ALL Heavy Industry whitelisted buildings!" },
                
                // Uninstall settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConvertToVanillaZones)), "Convert All Buildings to Vanilla" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConvertToVanillaZones)), "Click BEFORE uninstalling to convert all Light/Heavy Industrial buildings back to vanilla Industrial Manufacturing." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ConvertToVanillaZones)), "This will convert ALL Light/Heavy Industrial buildings to vanilla Industrial Manufacturing!" },
            };
        }

        public void Unload() { }
    }
}