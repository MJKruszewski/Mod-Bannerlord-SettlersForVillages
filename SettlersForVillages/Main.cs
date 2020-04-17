using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SettlersForVillages
{
    public class Main : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                if (!(gameStarterObject is CampaignGameStarter)) return;
                if (!(game.GameType is Campaign)) return;

                ((CampaignGameStarter) gameStarterObject).AddBehavior((CampaignBehaviorBase) new SettlersForVillagesCampaignBehavior());
            }
            catch (Exception ex)
            {
                Logger.DisplayInfoMsg(
                    "SettlersForVillages: An Error occurred, when trying to load the mod into your current game.");
                Logger.logError(ex);
            }
        }
    }
}