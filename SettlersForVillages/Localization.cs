using System;
using System.Collections.Generic;

namespace SettlersForVillages
{
    public class Localization
    {
        public const string MenuLeave = "menu.leave";

        public const string SettlersMenu = "settlers.menu";
        public const string SettlersMenuTiers = "settlers.menu.tiers";
        public const string SettlersMenuDescription = "settlers.menu.description";
        public const string SettlersMenuTaxCancel = "settlers.menu.tax.cancel";
        public const string SettlersMenuTaxIntroduce = "settlers.menu.tax.introduce";

        public const string SettlersActionTooManyCalls = "settlers.action.tooManyCalls";
        public const string SettlersActionLackOfGold = "settlers.action.lackOfGold";
        public const string SettlersActionLackOfSettlers = "settlers.action.lackOfSettlers";
        public const string SettlersActionTaxMigration = "settlers.action.tax.migration";
        public const string SettlersActionTaxCancel = "settlers.action.tax.cancel";
        public const string SettlersActionTaxIntroduce = "settlers.action.tax.introduce";

        public const string MilitiaActionMoveLackOfVillagers = "militia.action.castle.move.lackOfVillagers";
        public const string MilitiaActionMove = "militia.action.castle.move";
        public const string MilitiaActionLackOfGold = "militia.action.village.lackOfGold";
        public const string MilitiaActionLackOfVillagers = "militia.action.village.lackOfVillagers";

        public const string MilitiaMenuTiers = "militia.menu.tiers";
        public const string MilitiaMenuMove = "militia.menu.move";
        public const string MilitiaMenuCastle = "militia.menu.castle";
        public const string MilitiaMenuCastleDescription = "militia.menu.castle.description";
        public const string MilitiaMenuVillage = "militia.menu.village";
        public const string MilitiaMenuVillageDescription = "militia.menu.village.description";

        public string TranslationCode { get; set; } = "EN";
        public Dictionary<string, string> Dictionary { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DefaultDictionary = new Dictionary<string, string>();


        public string GetTranslation(string key, params object[] values)
        {
            try
            {
                if (Dictionary.TryGetValue(key, out var value))
                {
                    return string.Format(value, values);
                }

                if (DefaultDictionary.TryGetValue(key, out var defaultValue))
                {
                    return string.Format(defaultValue, values);
                }
            }
            catch (Exception e)
            {
                Logger.logDebug("Problem with translation " + key);
                Logger.logError(e);
            }

            return key;
        }
    }
}