using System;
using System.Collections.Generic;
using SettlersForVillages.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace SettlersForVillages.CampaignBehavior.Castle
{
    class CastleMilitiaCampaignBehavior : CampaignBehaviorBase
    {
        private static readonly string CastleMilitiaMenu = "castle_militia_rec_menu";

        private static readonly List<MilitiaMoveTierModel> TierMilitiaList = new List<MilitiaMoveTierModel>
        {
            new MilitiaMoveTierModel(10),
            new MilitiaMoveTierModel(25),
            new MilitiaMoveTierModel(50),
            new MilitiaMoveTierModel(75),
            new MilitiaMoveTierModel(100),
        };

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddGameMenus);
        }

        private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
        {
            campaignGameSystemStarter.AddGameMenuOption(
                "castle",
                "village_militia_rec_menu_button",
                Main.Localization.GetTranslation(Localization.MilitiaMenuCastle),
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;

                    return
                        Campaign.Current.CurrentMenuContext != null &&
                        Campaign.Current.CurrentMenuContext.GameMenu != null &&
                        Campaign.Current.CurrentMenuContext.GameMenu.StringId != CastleMilitiaMenu &&
                        Settlement.CurrentSettlement != null &&
                        Settlement.CurrentSettlement.IsCastle &&
                        (Main.Settings.DebugMode || Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan);
                },
                args => { GameMenu.SwitchToMenu(CastleMilitiaMenu); },
                false,
                4
            );

            campaignGameSystemStarter.AddGameMenu(
                CastleMilitiaMenu,
                Main.Localization.GetTranslation(Localization.MilitiaMenuCastleDescription),
                null
            );

            AddTiers(campaignGameSystemStarter);
            AddLeaveButtons(campaignGameSystemStarter);
        }

        private static void AddTiers(CampaignGameStarter campaignGameSystemStarter)
        {
            foreach (var tier in TierMilitiaList)
            {
                campaignGameSystemStarter.AddGameMenuOption(
                    CastleMilitiaMenu,
                    tier.GetOptionId(),
                    tier.GetOptionText(),
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Recruit;

                        return true;
                    },
                    args =>
                    {
                        try
                        {
                            MoveMilitia(tier.MilitiaToAdd);
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
                CastleMilitiaMenu,
                "castle_militia_rec_leave",
                Main.Localization.GetTranslation(Localization.MenuLeave),
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => { GameMenu.SwitchToMenu("castle"); }
            );
        }

        private static void MoveMilitia(float militiaToAdd)
        {
            bool moved = false;

            foreach (var village in Settlement.CurrentSettlement.BoundVillages)
            {
                if (
                    militiaToAdd > village.Militia
                    || village.Settlement.MilitaParty == null
                    || village.Settlement.MilitaParty.MemberRoster == null
                    || militiaToAdd > village.Settlement.MilitaParty.MemberRoster.TotalManCount
                ) continue;

                village.Settlement.MilitaParty.MemberRoster.KillNumberOfMenRandomly((int) militiaToAdd, false);
                Settlement.CurrentSettlement.ReadyMilitia += militiaToAdd;
                moved = true;
                Logger.DisplayInfoMsg(Main.Localization.GetTranslation(Localization.MilitiaActionMove, militiaToAdd,
                    village.Name));

                break;
            }

            if (!moved)
            {
                Logger.DisplayInfoMsg(Main.Localization.GetTranslation(Localization.MilitiaActionMoveLackOfVillagers));
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}