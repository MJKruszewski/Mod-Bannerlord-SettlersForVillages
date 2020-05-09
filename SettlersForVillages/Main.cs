using System;
using System.IO;
using System.Xml.Serialization;
using SettlersForVillages.CampaignBehavior.Castle;
using SettlersForVillages.CampaignBehavior.Village;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SettlersForVillages
{
    public class Main : MBSubModuleBase
    {
        public static SettlersForVillagesSettings Settings = new SettlersForVillagesSettings();
        public static Localization Localization = new Localization();

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                if (!(gameStarterObject is CampaignGameStarter)) return;
                if (!(game.GameType is Campaign)) return;

                ((CampaignGameStarter) gameStarterObject).AddBehavior(
                    (CampaignBehaviorBase) new SettlersCampaignBehavior());
                ((CampaignGameStarter) gameStarterObject).AddBehavior(
                    (CampaignBehaviorBase) new MilitiaCampaignBehavior());
                ((CampaignGameStarter) gameStarterObject).AddBehavior(
                    (CampaignBehaviorBase) new CastleMilitiaCampaignBehavior());
            }
            catch (Exception ex)
            {
                Logger.DisplayInfoMsg(
                    "SettlersForVillages: An Error occurred, when trying to load the mod into your current game.");
                Logger.logError(ex);
            }
        }

        protected override void OnSubModuleLoad()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LocalizationConfiguration));
                Stream fs = new FileStream("./../../Modules/SettlersForVillages/language.xml", FileMode.Open);

                LocalizationConfiguration localizationConfiguration =
                    (LocalizationConfiguration) serializer.Deserialize(fs);
                fs.Close();


                serializer = new XmlSerializer(typeof(LocalizationObject[]));
                fs = new FileStream("./../../Modules/SettlersForVillages/i18n/EN.xml", FileMode.Open);

                foreach (var localizationObject in ((LocalizationObject[]) serializer.Deserialize(fs)))
                {
                    Localization.DefaultDictionary.Add(localizationObject.id, localizationObject.translation);
                }

                fs.Close();

                fs = new FileStream(
                    "./../../Modules/SettlersForVillages/i18n/" + localizationConfiguration.LanguageCode + ".xml",
                    FileMode.Open);
                foreach (var localizationObject in ((LocalizationObject[]) serializer.Deserialize(fs)))
                {
                    Localization.Dictionary.Add(localizationObject.id, localizationObject.translation);
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                Logger.logError(ex);
            }
        }
    }
}