using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace LightHeavyIndustry
{
    [FileLocation(nameof(LightHeavyIndustry))]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kPollutionSection = "Pollution";
        public const string kBlacklistSection = "Blacklist";
        public const string kUninstallSection = "Uninstall";
        public const string kGroup = "Actions";

        public Setting(IMod mod) : base(mod) { }

        [SettingsUISection(kSection, "Options")]
        public bool Enabled { get; set; } = true;

        [SettingsUISection(kSection, "Options")]
        public bool DryRun { get; set; } = false;

        [SettingsUISection(kPollutionSection, "Values")]
        [SettingsUISlider(min = 0, max = 100, step = 10, scalarMultiplier = 1)]
        public int LightIndustryGroundPollution { get; set; } = 30;

        [SettingsUISection(kBlacklistSection, "Buildings")]
        [SettingsUIMultilineText]
        public string LightIndustryBlacklistText { get; set; } = "";

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

        // Hidden property that parses the text into a list
        [SettingsUIHidden]
        public List<string> LightIndustryBlacklist
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LightIndustryBlacklistText))
                    return new List<string>();

                var lines = LightIndustryBlacklistText.Split('\n');
                var result = new List<string>();

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("#"))
                    {
                        result.Add(trimmed);
                    }
                }

                return result;
            }
        }

        public override void SetDefaults()
        {
            Enabled = true;
            DryRun = false;
            LightIndustryGroundPollution = 30;
            LightIndustryBlacklistText = "# Add building names here, one per line\n# Lines starting with # are comments\n# Example:\n# IndustrialManufacturing01_L1_2x2\n";
        }
    }
}