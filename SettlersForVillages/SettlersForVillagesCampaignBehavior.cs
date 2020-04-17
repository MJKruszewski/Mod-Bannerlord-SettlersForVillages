using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;

namespace SettlersForVillages
{
    class SettlersForVillagesCampaignBehavior : CampaignBehaviorBase
    {
        private static readonly string VillageSettlersMenu = "village_settlers_menu";
        private static readonly string VillageMilitiaMenu = "village_militia_rec_menu";

        private static readonly List<SettlerTierSetting> TierSettlersList = new List<SettlerTierSetting>
        {
            new SettlerTierSetting(2500, 10),
            new SettlerTierSetting(5000, 25),
            new SettlerTierSetting(10000, 50),
            new SettlerTierSetting(15000, 75),
        };

        private static readonly List<MilitiaTierSetting> TierMilitiaList = new List<MilitiaTierSetting>
        {
            new MilitiaTierSetting(5000, 10),
            new MilitiaTierSetting(10000, 25),
            new MilitiaTierSetting(15000, 50),
            new MilitiaTierSetting(20000, 75),
        };

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddGameMenus);
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

                    return Campaign.Current.CurrentMenuContext.StringId != VillageSettlersMenu &&
                           Settlement.CurrentSettlement.IsVillage &&
                           Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan;
                },
                args => { GameMenu.SwitchToMenu(VillageSettlersMenu); },
                false,
                4
            );
            campaignGameSystemStarter.AddGameMenuOption(
                "village",
                "village_militia_rec_menu_button",
                "Recruit to militia",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;

                    return Campaign.Current.CurrentMenuContext.StringId != VillageMilitiaMenu &&
                           Settlement.CurrentSettlement.IsVillage &&
                           Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan;
                },
                args => { GameMenu.SwitchToMenu(VillageMilitiaMenu); },
                false,
                4
            );

            campaignGameSystemStarter.AddGameMenu(
                VillageSettlersMenu,
                "The village spokesman says they can encourage townsmen and nearby villages to provide you a settlers",
                null
            );
            campaignGameSystemStarter.AddGameMenu(
                VillageMilitiaMenu,
                "The village spokesman says they can encourage some villagers to guard village",
                null
            );

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
                            AddVillagers(tier.GoldPrice, tier.VillagersToAdd);
                        }
                        catch (Exception e)
                        {
                            Logger.logError(e);
                        }
                    }
                );
            }

            foreach (var tier in TierMilitiaList)
            {
                campaignGameSystemStarter.AddGameMenuOption(
                    VillageMilitiaMenu,
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
                            AddMilitia(tier.GoldPrice, tier.MilitiaToAdd);
                        }
                        catch (Exception e)
                        {
                            Logger.logError(e);
                        }
                    }
                );
            }

            AddLeaveButtons(campaignGameSystemStarter);
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
            campaignGameSystemStarter.AddGameMenuOption(
                VillageMilitiaMenu,
                "village_militia_rec_leave",
                "Leave",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => { GameMenu.SwitchToMenu("village"); }
            );
        }

        private static void AddVillagers(int goldPrice, float villagersToAdd)
        {
            if (goldPrice > Hero.MainHero.Gold)
            {
                Logger.DisplayInfoMsg("Not enough denars to encourage settlers");
                return;
            }

            // if (Settlement.CurrentSettlement.Village.Hearth >=
            //     Settlement.CurrentSettlement.Village.Bound.Village.Hearth)
            // {
            //     Logger.DisplayInfoMsg("Not enough settlers in town");
            //     return;
            // }

            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, goldPrice);
            Settlement.CurrentSettlement.Village.Hearth += villagersToAdd;
            // todo Settlement.CurrentSettlement.Village.Bound.Village.Hearth -= villagersToAdd;
        }

        private static void AddMilitia(int goldPrice, float militiaToAdd)
        {
            if (goldPrice > Hero.MainHero.Gold)
            {
                Logger.DisplayInfoMsg("Not enough denars to recruit militia");
                return;
            }

            if (militiaToAdd >= Settlement.CurrentSettlement.Village.Hearth)
            {
                Logger.DisplayInfoMsg("Not enough villagers");
                return;
            }

            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, goldPrice);
            Settlement.CurrentSettlement.Village.Hearth -= militiaToAdd;
            Settlement.CurrentSettlement.ReadyMilitia += militiaToAdd;
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}