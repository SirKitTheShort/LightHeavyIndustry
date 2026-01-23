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
    [SettingsUIGroupOrder(kZoneSettings, kBlacklistSection, kWhitelistSection, kManagementSection, kUninstallSection)]
    public class Setting : ModSetting
    {
        public const string kZoneSettings = "ZoneSettings";
        public const string kBlacklistSection = "Blacklist";
        public const string kWhitelistSection = "Whitelist";
        public const string kManagementSection = "Management";
        public const string kUninstallSection = "Uninstall";

        public Setting(IMod mod) : base(mod)
        {
            // Initialize with empty lists
            if (LightIndustryBlacklist == null) LightIndustryBlacklist = new List<string>();
            if (HeavyIndustryBlacklist == null) HeavyIndustryBlacklist = new List<string>();
            if (LightIndustryWhitelist == null) LightIndustryWhitelist = new List<string>();
            if (HeavyIndustryWhitelist == null) HeavyIndustryWhitelist = new List<string>();
            if (WarehouseBlacklist == null) WarehouseBlacklist = new List<string>();
            if (WarehouseWhitelist == null) WarehouseWhitelist = new List<string>();
        }

        // Store available building names - populated by the zoning system
        [SettingsUIHidden]
        public static List<string> AvailableLightBuildingNames { get; set; } = new();

        [SettingsUIHidden]
        public static List<string> AvailableHeavyBuildingNames { get; set; } = new();

        [SettingsUIHidden]
        public static List<string> AvailableWarehouseBuildingNames { get; set; } = new();

        // ===== ZONE SETTINGS =====

        [SettingsUISection(kZoneSettings, "WarehouseZone")]
        public bool EnableWarehouseZone { get; set; } = true;

        // ===== LIGHT INDUSTRY BLACKLIST =====

        [SettingsUIHidden]
        public List<string> LightIndustryBlacklist { get; set; } = new();

        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        [SettingsUIMultilineText]
        public string LightIndustryBlacklistDisplay
        {
            get => LightIndustryBlacklist.Count > 0
                ? string.Join("\n", LightIndustryBlacklist)
                : "# No buildings blacklisted for Light Industry";
            set { }
        }

        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetLightBuildingDropdownItems))]
        public string SelectedLightBlacklistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "LightIndustryBlacklist")]
        public bool AddLightBlacklist
        {
            set
            {
                Mod.log.Info($"AddLightBlacklist button clicked! Selected: {SelectedLightBlacklistBuilding}");
                if (value && !string.IsNullOrWhiteSpace(SelectedLightBlacklistBuilding) &&
                    SelectedLightBlacklistBuilding != "None")
                {
                    if (!LightIndustryBlacklist.Contains(SelectedLightBlacklistBuilding))
                    {
                        LightIndustryBlacklist.Add(SelectedLightBlacklistBuilding);
                        Mod.log.Info($"Added to Light Industry blacklist: {SelectedLightBlacklistBuilding}");
                        ApplyAndSave();
                    }
                    else
                    {
                        Mod.log.Info($"Building already in blacklist: {SelectedLightBlacklistBuilding}");
                    }
                }
                else
                {
                    Mod.log.Info($"Cannot add - no valid building selected");
                }
            }
        }

        public static DropdownItem<string>[] GetLightBuildingDropdownItems()
        {
            var items = new List<DropdownItem<string>>();

            var count = AvailableLightBuildingNames?.Count ?? 0;
            Mod.log.Info($"GetLightBuildingDropdownItems called - AvailableLightBuildingNames has {count} items");

            if (AvailableLightBuildingNames == null || AvailableLightBuildingNames.Count == 0)
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
                Mod.log.Warn("Dropdown showing (Loading buildings...) - list is null or empty");
            }
            else if (AvailableLightBuildingNames.Count == 1 && AvailableLightBuildingNames[0] == "LoadingPlaceholder")
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
                Mod.log.Warn("Dropdown showing (Loading buildings...) - still has placeholder");
            }
            else
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "-- Select Building --" });
                foreach (var buildingName in AvailableLightBuildingNames.OrderBy(n => n))
                {
                    items.Add(new DropdownItem<string> { value = buildingName, displayName = buildingName });
                }
                Mod.log.Info($"Dropdown populated with {AvailableLightBuildingNames.Count} buildings");
            }

            return items.ToArray();
        }

        // ===== HEAVY INDUSTRY BLACKLIST =====

        [SettingsUIHidden]
        public List<string> HeavyIndustryBlacklist { get; set; } = new();

        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        [SettingsUIMultilineText]
        public string HeavyIndustryBlacklistDisplay
        {
            get => HeavyIndustryBlacklist.Count > 0
                ? string.Join("\n", HeavyIndustryBlacklist)
                : "# No buildings blacklisted for Heavy Industry";
            set { }
        }

        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHeavyBuildingDropdownItems))]
        public string SelectedHeavyBlacklistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "HeavyIndustryBlacklist")]
        public bool AddHeavyBlacklist
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
        }

        public static DropdownItem<string>[] GetHeavyBuildingDropdownItems()
        {
            var items = new List<DropdownItem<string>>();

            if (AvailableHeavyBuildingNames == null || AvailableHeavyBuildingNames.Count == 0)
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
            }
            else if (AvailableHeavyBuildingNames.Count == 1 && AvailableHeavyBuildingNames[0] == "LoadingPlaceholder")
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
            }
            else
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "-- Select Building --" });
                foreach (var buildingName in AvailableHeavyBuildingNames.OrderBy(n => n))
                {
                    items.Add(new DropdownItem<string> { value = buildingName, displayName = buildingName });
                }
            }

            return items.ToArray();
        }

        // ===== WAREHOUSE BLACKLIST =====

        [SettingsUIHidden]
        public List<string> WarehouseBlacklist { get; set; } = new();

        [SettingsUISection(kBlacklistSection, "WarehouseBlacklist")]
        [SettingsUIMultilineText]
        public string WarehouseBlacklistDisplay
        {
            get => WarehouseBlacklist.Count > 0
                ? string.Join("\n", WarehouseBlacklist)
                : "# No buildings blacklisted for Warehouse";
            set { }
        }

        [SettingsUISection(kBlacklistSection, "WarehouseBlacklist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetWarehouseBuildingDropdownItems))]
        public string SelectedWarehouseBlacklistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kBlacklistSection, "WarehouseBlacklist")]
        public bool AddWarehouseBlacklist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedWarehouseBlacklistBuilding) &&
                    SelectedWarehouseBlacklistBuilding != "None")
                {
                    if (!WarehouseBlacklist.Contains(SelectedWarehouseBlacklistBuilding))
                    {
                        WarehouseBlacklist.Add(SelectedWarehouseBlacklistBuilding);
                        Mod.log.Info($"Added to Warehouse blacklist: {SelectedWarehouseBlacklistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
        }

        public static DropdownItem<string>[] GetWarehouseBuildingDropdownItems()
        {
            var items = new List<DropdownItem<string>>();

            var count = AvailableWarehouseBuildingNames?.Count ?? 0;
            Mod.log.Info($"GetWarehouseBuildingDropdownItems called - AvailableWarehouseBuildingNames has {count} items");

            if (AvailableWarehouseBuildingNames == null || AvailableWarehouseBuildingNames.Count == 0)
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
                Mod.log.Warn("Dropdown showing (Loading buildings...) - list is null or empty");
            }
            else if (AvailableWarehouseBuildingNames.Count == 1 && AvailableWarehouseBuildingNames[0] == "LoadingPlaceholder")
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "(Loading buildings...)" });
                Mod.log.Warn("Dropdown showing (Loading buildings...) - still has placeholder");
            }
            else
            {
                items.Add(new DropdownItem<string> { value = "None", displayName = "-- Select Building --" });
                foreach (var buildingName in AvailableWarehouseBuildingNames.OrderBy(n => n))
                {
                    items.Add(new DropdownItem<string> { value = buildingName, displayName = buildingName });
                }
                Mod.log.Info($"Dropdown populated with {AvailableWarehouseBuildingNames.Count} buildings");
            }

            return items.ToArray();
        }

        // ===== LIGHT INDUSTRY WHITELIST =====

        [SettingsUIHidden]
        public List<string> LightIndustryWhitelist { get; set; } = new();

        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        [SettingsUIMultilineText]
        public string LightIndustryWhitelistDisplay
        {
            get => LightIndustryWhitelist.Count > 0
                ? string.Join("\n", LightIndustryWhitelist)
                : "# No buildings whitelisted for Light Industry";
            set { }
        }

        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetLightBuildingDropdownItems))]
        public string SelectedLightWhitelistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "LightIndustryWhitelist")]
        public bool AddLightWhitelist
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
        }

        // ===== HEAVY INDUSTRY WHITELIST =====

        [SettingsUIHidden]
        public List<string> HeavyIndustryWhitelist { get; set; } = new();

        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        [SettingsUIMultilineText]
        public string HeavyIndustryWhitelistDisplay
        {
            get => HeavyIndustryWhitelist.Count > 0
                ? string.Join("\n", HeavyIndustryWhitelist)
                : "# No buildings whitelisted for Heavy Industry";
            set { }
        }

        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHeavyBuildingDropdownItems))]
        public string SelectedHeavyWhitelistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "HeavyIndustryWhitelist")]
        public bool AddHeavyWhitelist
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
        }

        // ===== WAREHOUSE WHITELIST =====

        [SettingsUIHidden]
        public List<string> WarehouseWhitelist { get; set; } = new();

        [SettingsUISection(kWhitelistSection, "WarehouseWhitelist")]
        [SettingsUIMultilineText]
        public string WarehouseWhitelistDisplay
        {
            get => WarehouseWhitelist.Count > 0
                ? string.Join("\n", WarehouseWhitelist)
                : "# No buildings whitelisted for Warehouse";
            set { }
        }

        [SettingsUISection(kWhitelistSection, "WarehouseWhitelist")]
        [SettingsUIDropdown(typeof(Setting), nameof(GetWarehouseBuildingDropdownItems))]
        public string SelectedWarehouseWhitelistBuilding { get; set; } = "";

        [SettingsUIButton]
        [SettingsUISection(kWhitelistSection, "WarehouseWhitelist")]
        public bool AddWarehouseWhitelist
        {
            set
            {
                if (value && !string.IsNullOrWhiteSpace(SelectedWarehouseWhitelistBuilding) &&
                    SelectedWarehouseWhitelistBuilding != "None")
                {
                    if (!WarehouseWhitelist.Contains(SelectedWarehouseWhitelistBuilding))
                    {
                        WarehouseWhitelist.Add(SelectedWarehouseWhitelistBuilding);
                        Mod.log.Info($"Added to Warehouse whitelist: {SelectedWarehouseWhitelistBuilding}");
                        ApplyAndSave();
                    }
                }
            }
        }

        // ===== LIST MANAGEMENT =====

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
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

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
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

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
        public bool ClearWarehouseBlacklist
        {
            set
            {
                if (value)
                {
                    WarehouseBlacklist.Clear();
                    Mod.log.Info("Cleared Warehouse blacklist");
                    ApplyAndSave();
                }
            }
            get => false;
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
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

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
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

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kManagementSection, "ClearLists")]
        public bool ClearWarehouseWhitelist
        {
            set
            {
                if (value)
                {
                    WarehouseWhitelist.Clear();
                    Mod.log.Info("Cleared Warehouse whitelist");
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
            EnableWarehouseZone = true;
            LightIndustryBlacklist = new List<string>();
            HeavyIndustryBlacklist = new List<string>();
            LightIndustryWhitelist = new List<string>();
            HeavyIndustryWhitelist = new List<string>();
            WarehouseBlacklist = new List<string>();
            WarehouseWhitelist = new List<string>();
        }
    }
}