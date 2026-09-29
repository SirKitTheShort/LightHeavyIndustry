using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace LightHeavyIndustry
{
    public class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleFR(Setting setting) => m_Setting = setting;

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Mod title
                { m_Setting.GetSettingsLocaleID(), "Zonage Industriel Étendu" },

                // Sections
                { m_Setting.GetOptionTabLocaleID(Setting.kZoneSettings), "Paramètres de zonage" },
                { m_Setting.GetOptionTabLocaleID(Setting.kBlacklistSection), "Listes noires de bâtiments" },
                { m_Setting.GetOptionTabLocaleID(Setting.kWhitelistSection), "Listes blanches de bâtiments" },
                { m_Setting.GetOptionTabLocaleID(Setting.kManagementSection), "Effacer les listes" },
                { m_Setting.GetOptionTabLocaleID(Setting.kUninstallSection), "Désinstallation sécurisée" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID("WarehouseZone"), "Zone d'entrepôt" },
                { m_Setting.GetOptionGroupLocaleID("LightIndustryBlacklist"), "Liste noire - Industrie légère" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryBlacklist"), "Liste noire - Industrie lourde" },
                { m_Setting.GetOptionGroupLocaleID("WarehouseBlacklist"), "Liste noire - Entrepôt" },
                { m_Setting.GetOptionGroupLocaleID("LightIndustryWhitelist"), "Liste blanche - Industrie légère" },
                { m_Setting.GetOptionGroupLocaleID("HeavyIndustryWhitelist"), "Liste blanche - Industrie lourde" },
                { m_Setting.GetOptionGroupLocaleID("WarehouseWhitelist"), "Liste blanche - Entrepôt" },
                { m_Setting.GetOptionGroupLocaleID("ClearLists"), "Effacer les listes" },
                { m_Setting.GetOptionGroupLocaleID("SafeUninstall"), "Avant de désinstaller le mod" },

                // Warehouse Zone Setting
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableWarehouseZone)), "Activer la zone d'entrepôt" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableWarehouseZone)), "Lorsque cette option est activée, les entrepôts obtiennent leur propre type de zone distinct. Si elle est désactivée, les entrepôts apparaîtront dans les zones d'industrie légère/lourde. REMARQUE : nécessite un redémarrage du jeu pour prendre effet." },

                // Light Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Bâtiments actuellement sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryBlacklistDisplay)), "Bâtiments actuellement exclus des zones d'industrie légère. Ces bâtiments du jeu de base ne seront PAS clonés dans l'industrie légère." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Sélectionner un bâtiment à mettre sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightBlacklistBuilding)), "Choisissez un bâtiment du jeu de base à exclure des zones d'industrie légère." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddLightBlacklist)), "Ajouter à la liste noire de l'industrie légère" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddLightBlacklist)), "Ajoute le bâtiment sélectionné à la liste noire de l'industrie légère." },

                // Heavy Industry Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Bâtiments actuellement sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryBlacklistDisplay)), "Bâtiments actuellement exclus des zones d'industrie lourde. Ces bâtiments du jeu de base ne seront PAS clonés dans l'industrie lourde." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Sélectionner un bâtiment à mettre sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyBlacklistBuilding)), "Choisissez un bâtiment du jeu de base à exclure des zones d'industrie lourde." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddHeavyBlacklist)), "Ajouter à la liste noire de l'industrie lourde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddHeavyBlacklist)), "Ajoute le bâtiment sélectionné à la liste noire de l'industrie lourde." },

                // Warehouse Blacklist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WarehouseBlacklistDisplay)), "Bâtiments actuellement sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WarehouseBlacklistDisplay)), "Bâtiments actuellement exclus des zones d'entrepôt. Ces bâtiments du jeu de base ne seront PAS clonés dans les entrepôts." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedWarehouseBlacklistBuilding)), "Sélectionner un bâtiment à mettre sur liste noire" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedWarehouseBlacklistBuilding)), "Choisissez un bâtiment du jeu de base à exclure des zones d'entrepôt." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddWarehouseBlacklist)), "Ajouter à la liste noire des entrepôts" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddWarehouseBlacklist)), "Ajoute le bâtiment sélectionné à la liste noire des entrepôts." },

                // Light Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Bâtiments actuellement sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LightIndustryWhitelistDisplay)), "Bâtiments forcés à apparaître dans les zones d'industrie légère. La liste blanche prime sur la liste noire." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Sélectionner un bâtiment à mettre sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedLightWhitelistBuilding)), "Choisissez un bâtiment du jeu de base à inclure de force dans les zones d'industrie légère." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddLightWhitelist)), "Ajouter à la liste blanche de l'industrie légère" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddLightWhitelist)), "Ajoute le bâtiment sélectionné à la liste blanche de l'industrie légère (prime sur la liste noire)." },

                // Heavy Industry Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Bâtiments actuellement sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HeavyIndustryWhitelistDisplay)), "Bâtiments forcés à apparaître dans les zones d'industrie lourde. La liste blanche prime sur la liste noire." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Sélectionner un bâtiment à mettre sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedHeavyWhitelistBuilding)), "Choisissez un bâtiment du jeu de base à inclure de force dans les zones d'industrie lourde." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddHeavyWhitelist)), "Ajouter à la liste blanche de l'industrie lourde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddHeavyWhitelist)), "Ajoute le bâtiment sélectionné à la liste blanche de l'industrie lourde (prime sur la liste noire)." },

                // Warehouse Whitelist
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WarehouseWhitelistDisplay)), "Bâtiments actuellement sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WarehouseWhitelistDisplay)), "Bâtiments forcés à apparaître dans les zones d'entrepôt. La liste blanche prime sur la liste noire." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedWarehouseWhitelistBuilding)), "Sélectionner un bâtiment à mettre sur liste blanche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedWarehouseWhitelistBuilding)), "Choisissez un bâtiment du jeu de base à inclure de force dans les zones d'entrepôt." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddWarehouseWhitelist)), "Ajouter à la liste blanche des entrepôts" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddWarehouseWhitelist)), "Ajoute le bâtiment sélectionné à la liste blanche des entrepôts (prime sur la liste noire)." },

                // Clear buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightBlacklist)), "Effacer la liste noire de l'industrie légère" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightBlacklist)), "Retire tous les bâtiments de la liste noire de l'industrie légère." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightBlacklist)), "Cette action effacera TOUS les bâtiments de la liste noire de l'industrie légère !" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Effacer la liste noire de l'industrie lourde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Retire tous les bâtiments de la liste noire de l'industrie lourde." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyBlacklist)), "Cette action effacera TOUS les bâtiments de la liste noire de l'industrie lourde !" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearWarehouseBlacklist)), "Effacer la liste noire des entrepôts" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearWarehouseBlacklist)), "Retire tous les bâtiments de la liste noire des entrepôts." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearWarehouseBlacklist)), "Cette action effacera TOUS les bâtiments de la liste noire des entrepôts !" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearLightWhitelist)), "Effacer la liste blanche de l'industrie légère" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearLightWhitelist)), "Retire tous les bâtiments de la liste blanche de l'industrie légère." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearLightWhitelist)), "Cette action effacera TOUS les bâtiments de la liste blanche de l'industrie légère !" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Effacer la liste blanche de l'industrie lourde" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Retire tous les bâtiments de la liste blanche de l'industrie lourde." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearHeavyWhitelist)), "Cette action effacera TOUS les bâtiments de la liste blanche de l'industrie lourde !" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearWarehouseWhitelist)), "Effacer la liste blanche des entrepôts" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearWarehouseWhitelist)), "Retire tous les bâtiments de la liste blanche des entrepôts." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearWarehouseWhitelist)), "Cette action effacera TOUS les bâtiments de la liste blanche des entrepôts !" },

                // Uninstall settings
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConvertToVanillaZones)), "Convertir tous les bâtiments en version d'origine" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConvertToVanillaZones)), "Cliquez AVANT de désinstaller pour reconvertir tous les bâtiments d'industrie légère/lourde/entrepôt en industrie manufacturière du jeu de base." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ConvertToVanillaZones)), "Cette action convertira TOUS les bâtiments d'industrie légère/lourde/entrepôt en industrie manufacturière du jeu de base !" },
            };
        }

        public void Unload() { }
    }
}
