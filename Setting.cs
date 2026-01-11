using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace LightHeavyIndustry
{
    [FileLocation(nameof(LightHeavyIndustry))]
    [SettingsUIGroupOrder(kBlacklistSection, kWhitelistSection, kUninstallSection)]
    public class Setting : ModSetting
    {
        public const string kBlacklistSection = "Blacklist";
        public const string kWhitelistSection = "Whitelist";
        public const string kUninstallSection = "Uninstall";

        public Setting(IMod mod) : base(mod)
        {
            // Initialize with empty lists
            if (LightIndustryBlacklist == null) LightIndustryBlacklist = new List<string>();
            if (HeavyIndustryBlacklist == null) HeavyIndustryBlacklist = new List<string>();
            if (LightIndustryWhitelist == null) LightIndustryWhitelist = new List<string>();
            if (HeavyIndustryWhitelist == null) HeavyIndustryWhitelist = new List<string>();
        }

        // Store available building names - populated by the zoning system
        [SettingsUIHidden]
        public static List<string> AvailableLightBuildingNames { get; set; } = new();

        [SettingsUIHidden]
        public static List<string> AvailableHeavyBuildingNames { get; set; } = new();

        // ===== LIGHT INDUSTRY BLACKLIST =====

        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        public List<string> LightIndustryBlacklist { get; set; } = new();

        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        [SettingsUIMultilineText]
        public string LightIndustryBlacklistDisplay
        {
            get => LightIndustryBlacklist.Count > 0
                ? string.Join("\n", LightIndustryBlacklist)
                : "# No buildings blacklisted";
            set { }
        }

        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetLightBuildingDropdownItems))]
        public string SelectedLightBlacklistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        public bool AddToLightBlacklist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedLightBlacklistBuilding) &&
                    SelectedLightBlacklistBuilding != "None")
                {
                    if (!LightIndustryBlacklist.Contains(SelectedLightBlacklistBuilding))
                    {
                        LightIndustryBlacklist.Add(SelectedLightBlacklistBuilding);
                        Mod.log.Info($"Added to Light Industry blacklist: {SelectedLightBlacklistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        public bool RemoveLastLightBlacklist
        {
            set
            {
                if (value && LightIndustryBlacklist.Count > 0)
                {
                    var removed = LightIndustryBlacklist[LightIndustryBlacklist.Count - 1];
                    LightIndustryBlacklist.RemoveAt(LightIndustryBlacklist.Count - 1);
                    Mod.log.Info($"Removed from Light Industry blacklist: {removed}");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        public bool ClearLightBlacklist
        {
            set
            {
                if (value)
                {
                    LightIndustryBlacklist.Clear();
                    Mod.log.Info("Cleared Light Industry blacklist");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        public static DropdownItem<string>[] GetLightBuildingDropdownItems()
        {
            var items = new List<DropdownItem<string>> { new() { value = "None", displayName = "-- Select Building --" } };

            foreach (var buildingName in AvailableLightBuildingNames.OrderBy(n => n))
            {
                items.Add(new DropdownItem<string> { value = buildingName, displayName = buildingName });
            }

            return items.ToArray();
        }

        // ===== HEAVY INDUSTRY BLACKLIST =====

        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        public List<string> HeavyIndustryBlacklist { get; set; } = new();

        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        [SettingsUIMultilineText]
        public string HeavyIndustryBlacklistDisplay
        {
            get => HeavyIndustryBlacklist.Count > 0
                ? string.Join("\n", HeavyIndustryBlacklist)
                : "# No buildings blacklisted";
            set { }
        }

        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHeavyBuildingDropdownItems))]
        public string SelectedHeavyBlacklistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        public bool AddToHeavyBlacklist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedHeavyBlacklistBuilding) &&
                    SelectedHeavyBlacklistBuilding != "None")
                {
                    if (!HeavyIndustryBlacklist.Contains(SelectedHeavyBlacklistBuilding))
                    {
                        HeavyIndustryBlacklist.Add(SelectedHeavyBlacklistBuilding);
                        Mod.log.Info($"Added to Heavy Industry blacklist: {SelectedHeavyBlacklistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        public bool RemoveLastHeavyBlacklist
        {
            set
            {
                if (value && HeavyIndustryBlacklist.Count > 0)
                {
                    var removed = HeavyIndustryBlacklist[HeavyIndustryBlacklist.Count - 1];
                    HeavyIndustryBlacklist.RemoveAt(HeavyIndustryBlacklist.Count - 1);
                    Mod.log.Info($"Removed from Heavy Industry blacklist: {removed}");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        public bool ClearHeavyBlacklist
        {
            set
            {
                if (value)
                {
                    HeavyIndustryBlacklist.Clear();
                    Mod.log.Info("Cleared Heavy Industry blacklist");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        public static DropdownItem<string>[] GetHeavyBuildingDropdownItems()
        {
            var items = new List<DropdownItem<string>> { new() { value = "None", displayName = "-- Select Building --" } };

            foreach (var buildingName in AvailableHeavyBuildingNames.OrderBy(n => n))
            {
                items.Add(new DropdownItem<string> { value = buildingName, displayName = buildingName });
            }

            return items.ToArray();
        }

        // ===== LIGHT INDUSTRY WHITELIST =====

        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        public List<string> LightIndustryWhitelist { get; set; } = new();

        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        [SettingsUIMultilineText]
        public string LightIndustryWhitelistDisplay
        {
            get => LightIndustryWhitelist.Count > 0
                ? string.Join("\n", LightIndustryWhitelist)
                : "# No buildings whitelisted";
            set { }
        }

        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetLightBuildingDropdownItems))]
        public string SelectedLightWhitelistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        public bool AddToLightWhitelist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedLightWhitelistBuilding) &&
                    SelectedLightWhitelistBuilding != "None")
                {
                    if (!LightIndustryWhitelist.Contains(SelectedLightWhitelistBuilding))
                    {
                        LightIndustryWhitelist.Add(SelectedLightWhitelistBuilding);
                        Mod.log.Info($"Added to Light Industry whitelist: {SelectedLightWhitelistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        public bool RemoveLastLightWhitelist
        {
            set
            {
                if (value && LightIndustryWhitelist.Count > 0)
                {
                    var removed = LightIndustryWhitelist[LightIndustryWhitelist.Count - 1];
                    LightIndustryWhitelist.RemoveAt(LightIndustryWhitelist.Count - 1);
                    Mod.log.Info($"Removed from Light Industry whitelist: {removed}");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        public bool ClearLightWhitelist
        {
            set
            {
                if (value)
                {
                    LightIndustryWhitelist.Clear();
                    Mod.log.Info("Cleared Light Industry whitelist");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        // ===== HEAVY INDUSTRY WHITELIST =====

        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        public List<string> HeavyIndustryWhitelist { get; set; } = new();

        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        [SettingsUIMultilineText]
        public string HeavyIndustryWhitelistDisplay
        {
            get => HeavyIndustryWhitelist.Count > 0
                ? string.Join("\n", HeavyIndustryWhitelist)
                : "# No buildings whitelisted";
            set { }
        }

        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHeavyBuildingDropdownItems))]
        public string SelectedHeavyWhitelistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        public bool AddToHeavyWhitelist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedHeavyWhitelistBuilding) &&
                    SelectedHeavyWhitelistBuilding != "None")
                {
                    if (!HeavyIndustryWhitelist.Contains(SelectedHeavyWhitelistBuilding))
                    {
                        HeavyIndustryWhitelist.Add(SelectedHeavyWhitelistBuilding);
                        Mod.log.Info($"Added to Heavy Industry whitelist: {SelectedHeavyWhitelistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        public bool RemoveLastHeavyWhitelist
        {
            set
            {
                if (value && HeavyIndustryWhitelist.Count > 0)
                {
                    var removed = HeavyIndustryWhitelist[HeavyIndustryWhitelist.Count - 1];
                    HeavyIndustryWhitelist.RemoveAt(HeavyIndustryWhitelist.Count - 1);
                    Mod.log.Info($"Removed from Heavy Industry whitelist: {removed}");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        public bool ClearHeavyWhitelist
        {
            set
            {
                if (value)
                {
                    HeavyIndustryWhitelist.Clear();
                    Mod.log.Info($"Cleared Heavy Industry whitelist");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        // ===== UNINSTALL SETTINGS =====

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kUninstallSection, "SafeUninstall")]
        public bool ConvertToVanillaZones
        {
            set
            {
                if (value)
                {
                    Mod.log.Info("Convert to Vanilla button clicked - will run on next update");
                }
            }
            get => false;
        }

        public override void SetDefaults()
        {
            LightIndustryBlacklist = new List<string>();
            HeavyIndustryBlacklist = new List<string>();
            LightIndustryWhitelist = new List<string>();
            HeavyIndustryWhitelist = new List<string>();
        }
    }
}