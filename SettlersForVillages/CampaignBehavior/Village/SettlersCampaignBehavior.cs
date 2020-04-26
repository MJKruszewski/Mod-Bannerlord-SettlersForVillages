using System;
using System.Collections.Generic;
using SettlersForVillages.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;

namespace SettlersForVillages.CampaignBehavior.Village
{
    class SettlersCampaignBehavior : CampaignBehaviorBase
    {
        public static readonly string TaxReliefText = "Tax relief for settlers";
        private static readonly string VillageSettlersMenu = "village_settlers_menu";
        private static readonly Random Random = new Random();

        private static readonly List<SettlerTierModel> TierSettlersList = new List<SettlerTierModel>
        {
            new SettlerTierModel(2500, 10),
            new SettlerTierModel(5000, 25),
            new SettlerTierModel(10000, 50),
            new SettlerTierModel(15000, 75),
            new SettlerTierModel(20000, 100),
        };

        public override void RegisterEvents()
        {
            //Logic for player taxes
            // CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener();

            //Ai logic
            CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, AiBehaviour);

            //Player menus
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddGameMenus);
        }

        private static void AiBehaviour(MobileParty mobileParty, Settlement settlement, Hero hero)
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
                AddVillagers(5000, 25f, hero, settlement.Village);
            }
            else if (8000 <= val && 9500 > val)
            {
                Logger.logDebug("Ai rolled 8000 <= val && 9500 > val");
                AddVillagers(10000, 50f, hero, settlement.Village);
            }
            else if (9500 <= val)
            {
                Logger.logDebug("Ai rolled 9500 <= val");
                AddVillagers(15000, 75f, hero, settlement.Village);
            }
        }

        private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
        {
            campaignGameSystemStarter.AddGameMenuOption(
                "village",
                "village_settlers_menu_button",
                "Encourage settlers",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Manage;

                    return Campaign.Current.CurrentMenuContext.GameMenu.StringId != VillageSettlersMenu &&
                           Settlement.CurrentSettlement.IsVillage &&
                           Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan;
                },
                args => { GameMenu.SwitchToMenu(VillageSettlersMenu); },
                false,
                4
            );

            campaignGameSystemStarter.AddGameMenu(
                VillageSettlersMenu,
                "The village spokesman says they can encourage townsmen and nearby villages to provide you a settlers",
                null
            );

            AddTiers(campaignGameSystemStarter);
            // AddTaxRelief(campaignGameSystemStarter);
            AddLeaveButtons(campaignGameSystemStarter);
        }

        private static void AddTaxRelief(CampaignGameStarter campaignGameSystemStarter)
        {
            //todo
            campaignGameSystemStarter.AddGameMenuOption(
                VillageSettlersMenu,
                "village_settlers_tax_relief_add",
                "Introduce tax relief to encourage settlers",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;

                    return FindTaxRelief() == null;
                },
                args =>
                {
                    try
                    {
                        Settlement.CurrentSettlement.Village.HearthChangeExplanation.AddLine(
                            TaxReliefText,
                            0.5f + (Settlement.CurrentSettlement.Village.MarketTown.Prosperity / 1000)
                        );

                        Logger.DisplayInfoMsg("Tax relief introduced in " + Settlement.CurrentSettlement.Name);
                    }
                    catch (Exception e)
                    {
                        Logger.logError(e);
                    }
                }
            );

            campaignGameSystemStarter.AddGameMenuOption(
                VillageSettlersMenu,
                "village_settlers_tax_relief_remove",
                "Cancel tax relief",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;

                    return FindTaxRelief() != null;
                },
                args =>
                {
                    try
                    {
                        Settlement.CurrentSettlement.Village.HearthChangeExplanation.Lines.Remove(FindTaxRelief());

                        Logger.DisplayInfoMsg("Tax relief canceled in " + Settlement.CurrentSettlement.Name);
                    }
                    catch (Exception e)
                    {
                        Logger.logError(e);
                    }
                }
            );
        }

        private static StatExplainer.ExplanationLine FindTaxRelief()
        {
            return Settlement.CurrentSettlement.Village.HearthChangeExplanation.Lines.Find((arg) =>
                arg.Name == TaxReliefText);
        }

        private static void AddTiers(CampaignGameStarter campaignGameSystemStarter)
        {
            foreach (var tier in TierSettlersList)
            {
                campaignGameSystemStarter.AddGameMenuOption(
                    VillageSettlersMenu,
                    tier.GetOptionId(),
                    tier.GetOptionText(),
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Trade;

                        return true;
                    },
                    args =>
                    {
                        try
                        {
                            AddVillagers(tier.GoldPrice, tier.VillagersToAdd, Hero.MainHero,
                                Settlement.CurrentSettlement.Village);
                        }
                        catch (Exception e)
                        {
                            Logger.logError(e);
                        }
                    }
                );
            }
        }

        private static void AddLeaveButtons(CampaignGameStarter campaignGameSystemStarter)
        {
            campaignGameSystemStarter.AddGameMenuOption(
                VillageSettlersMenu,
                "village_settlers_leave",
                "Leave",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => { GameMenu.SwitchToMenu("village"); }
            );
        }

        private static void AddVillagers(int goldPrice, float villagersToAdd, Hero hero,
            TaleWorlds.CampaignSystem.Village village)
        {
            if (goldPrice > Hero.MainHero.Gold)
            {
                if (hero == Hero.MainHero) Logger.DisplayInfoMsg("Not enough denars to encourage settlers");
                Logger.logDebug("Not enough gold - " + hero.Name);
                return;
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

            if (!Main.Settings.ProsperityAffection || village.MarketTown == null) return;
            village.MarketTown.Settlement.Prosperity -= villagersToAdd / 2;
            Logger.logDebug("Changed prosperity in " +
                            village.MarketTown.Name + " by " +
                            villagersToAdd / 2);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}