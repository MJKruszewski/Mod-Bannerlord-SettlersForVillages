using System;
using ModLib;
using SettlersForVillages.CampaignBehavior.Castle;
using SettlersForVillages.CampaignBehavior.Village;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SettlersForVillages
{
    public class Main : MBSubModuleBase
    {
        public static SettlersForVillagesSettings Settings;

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
                FileDatabase.Initialise("SettlersForVillages");
                Settings = FileDatabase.Get<SettlersForVillagesSettings>(SettlersForVillagesSettings.InstanceId) ?? new SettlersForVillagesSettings();
                SettingsDatabase.RegisterSettings(Settings);
            }
            catch(Exception ex)
            {
                Logger.logError(ex);
            }
        }
    }
}