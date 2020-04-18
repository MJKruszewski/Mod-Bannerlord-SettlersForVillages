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
        public static SettlersForVillagesSettings settings;
        private static string SETTINGS_PATH = "./../../Modules/SettlersForVillages/settings.xml";

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
                XmlSerializer serializer = new XmlSerializer(typeof(SettlersForVillagesSettings));
                if (!File.Exists(SETTINGS_PATH))
                {
                    var stream = new FileStream(SETTINGS_PATH, FileMode.Create);
                    serializer.Serialize(stream, new SettlersForVillagesSettings());
                    stream.Close();
                }

                SettlersForVillagesSettings _settings;

                using (Stream reader = new FileStream(SETTINGS_PATH, FileMode.Open))
                {
                    _settings = (SettlersForVillagesSettings) serializer.Deserialize(reader);
                }

                settings = _settings;
            }
            catch (Exception ex)
            {
                Logger.logError(ex);
            }
        }
    }
}