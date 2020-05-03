using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace SettlersForVillages.CampaignBehavior.Village
{
    public static class Villagers
    {
        private static readonly Random Random = new Random();

        public static void Add(int goldPrice, float villagersToAdd, Hero hero,
            TaleWorlds.CampaignSystem.Village village)
        {
            if (goldPrice > Hero.MainHero.Gold)
            {
                if (hero == Hero.MainHero) Logger.DisplayInfoMsg("Not enough denars to encourage settlers");
                Logger.logDebug("Not enough gold - " + hero.Name);
                return;
            }

            if (hero == Hero.MainHero && Main.Settings.MaxCallsForSettlersPerDayForVillageEnabled)
            {
                if (SettlersCampaignBehavior.Instance._settlersForVillagesData.ContainsKey(Settlement.CurrentSettlement.StringId))
                {
                    if (SettlersCampaignBehavior.Instance._settlersForVillagesData[Settlement.CurrentSettlement.StringId].SettlementToday >= Main.Settings.MaxCallsForSettlersPerDayForVillage)
                    {
                        Logger.DisplayInfoMsg("Too many calls for settlers, wait until tomorrow...");

                        return;
                    }
                }
            }

            if (
                village.MarketTown != null
                && villagersToAdd >= village.MarketTown.Prosperity
            )
            {
                if (hero == Hero.MainHero) Logger.DisplayInfoMsg("Not enough settlers in town");
                return;
            }

            GiveGoldAction.ApplyForCharacterToSettlement(hero, village.Settlement, goldPrice);
            village.Hearth += villagersToAdd;

            if (hero == Hero.MainHero && Main.Settings.MaxCallsForSettlersPerDayForVillageEnabled)
            {
                if (SettlersCampaignBehavior.Instance._settlersForVillagesData.ContainsKey(Settlement.CurrentSettlement.StringId))
                {
                    SettlersCampaignBehavior.Instance._settlersForVillagesData[Settlement.CurrentSettlement.StringId].SettlementToday++;
                }
                else
                {
                    SettlersCampaignBehavior.Instance._settlersForVillagesData.Add(Settlement.CurrentSettlement.StringId,
                        new VillageDetailsModel(Settlement.CurrentSettlement.StringId, -1));
                    SettlersCampaignBehavior.Instance._settlersForVillagesData[Settlement.CurrentSettlement.StringId].SettlementToday++;
                }
            }

            if (!Main.Settings.ProsperityAffection || village.MarketTown == null) return;
            village.MarketTown.Settlement.Prosperity -= villagersToAdd / 2;
            Logger.logDebug("Changed prosperity in " +
                            village.MarketTown.Name + " by " +
                            villagersToAdd / 2);
        }
        
        
        public static void AiBehaviour(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (!Main.Settings.AiEnabled || settlement == null || hero == null) return;

            if (
                !settlement.IsVillage
                || settlement.OwnerClan != hero.Clan
                || hero == Hero.MainHero
                || 10000 > hero.Gold
            ) return;

            var val = Random.Next(10000);
            val += (hero.Gold / 1000);

            if (5000 > val)
            {
                Logger.logDebug("Ai rolled less than 5000");
                return;
            }

            if (5000 <= val && 8000 > val)
            {
                Logger.logDebug("Ai rolled 5000 <= val && 8000 > val");
                Add(5000, 25f, hero, settlement.Village);
            }
            else if (8000 <= val && 9500 > val)
            {
                Logger.logDebug("Ai rolled 8000 <= val && 9500 > val");
                Add(10000, 50f, hero, settlement.Village);
            }
            else if (9500 <= val)
            {
                Logger.logDebug("Ai rolled 9500 <= val");
                Add(15000, 75f, hero, settlement.Village);
            }
        }
    }
}