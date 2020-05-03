using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace SettlersForVillages.CampaignBehavior.Village
{
    public static class Tax
    {
        private static readonly Random Random = new Random();
        public static bool FindTaxRelief()
        {
            try
            {
                return Settlement.CurrentSettlement != null 
                       && Settlement.CurrentSettlement.StringId != null
                       && SettlersCampaignBehavior.Instance._settlersForVillagesData.TryGetValue(Settlement.CurrentSettlement.StringId, out var village) 
                       && village.TaxReliefTier != -1;
            }
            catch (KeyNotFoundException e)
            {
                Logger.logError(e);
            }

            return false;
        }

        public static void TaxReliefDailyTick()
        {
            float settled = 0f;
            int goldTaken = 0;
            
            foreach (var detailsModel in SettlersCampaignBehavior.Instance._settlersForVillagesData)
            {
                detailsModel.Value.SettlementToday = 0;

                if (detailsModel.Value.TaxReliefTier == -1)
                {
                    continue;
                }

                Settlement settlement =
                    Settlement.FindFirst((args) => args.StringId == detailsModel.Value.SettlementId);
                
                if (settlement == null)
                {
                    Logger.logDebug("Settlement " + detailsModel.Value.SettlementId + " not found");

                    continue;
                }

                var village = settlement.Village;
                var tax = detailsModel.Value.TaxReliefAmount();
                
                var villagersToAdd = (tax / Random.Next(5, 25)) / Random.Next(1, 10);
                goldTaken += (int) tax;
                settled += villagersToAdd;
                
                village.Hearth += villagersToAdd;
                GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, settlement, (int) tax, true);

                if (!Main.Settings.ProsperityAffection || village.MarketTown == null) continue;
                village.MarketTown.Settlement.Prosperity -= villagersToAdd / 2;
                Logger.logDebug("Changed prosperity in " +
                                village.MarketTown.Name + " by " +
                                villagersToAdd / 2);
            }

            if (goldTaken != 0f || settled != 0f)
            {
                Logger.DisplayInfoMsg((int) settled + " settlers moved to villages, for tax relief in amount of " + goldTaken);
            }
        }
    }
}