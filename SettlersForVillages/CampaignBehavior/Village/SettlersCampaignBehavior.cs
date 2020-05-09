using System;
using System.Collections.Generic;
using SettlersForVillages.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.SaveSystem;

namespace SettlersForVillages.CampaignBehavior.Village
{
    class SettlersCampaignBehavior : CampaignBehaviorBase
    {
        private static readonly string VillageSettlersMenu = "village_settlers_menu";

        public Dictionary<string, VillageDetailsModel> _settlersForVillagesData =
            new Dictionary<string, VillageDetailsModel>();

        private static readonly List<SettlerTierModel> TierSettlersList = new List<SettlerTierModel>
        {
            new SettlerTierModel(2500, 10),
            new SettlerTierModel(5000, 25),
            new SettlerTierModel(10000, 50),
            new SettlerTierModel(15000, 75),
            new SettlerTierModel(20000, 100),
        };

        private static readonly List<TaxReliefTierModel> TierTaxReliefList = new List<TaxReliefTierModel>
        {
            new TaxReliefTierModel(1, "10"),
            new TaxReliefTierModel(2, "25"),
            new TaxReliefTierModel(3, "50"),
        };

        public static SettlersCampaignBehavior Instance
        {
            get { return (SettlersCampaignBehavior) Campaign.Current.GetCampaignBehavior<SettlersCampaignBehavior>(); }
        }

        public override void RegisterEvents()
        {
            //Logic for player taxes
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, Tax.TaxReliefDailyTick);

            //Ai logic
            CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, Villagers.AiBehaviour);

            //Player menus
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddGameMenus);
        }

        private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
        {
            try
            {
                campaignGameSystemStarter.AddGameMenuOption(
                    "village",
                    "village_settlers_menu_button",
                    Main.Localization.GetTranslation(Localization.SettlersMenu),
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
                    Main.Localization.GetTranslation(Localization.SettlersMenuDescription),
                    null
                );

                AddSettlersMenu(campaignGameSystemStarter);
                AddTaxReliefMenu(campaignGameSystemStarter);
                AddLeaveButtons(campaignGameSystemStarter);
            }
            catch (Exception e)
            {
                Logger.logError(e);
            }
        }

        private static void AddTaxReliefMenu(CampaignGameStarter campaignGameSystemStarter)
        {
            foreach (var tier in TierTaxReliefList)
            {
                campaignGameSystemStarter.AddGameMenuOption(
                    VillageSettlersMenu,
                    "village_settlers_tax_relief_add_" + tier.Label,
                    Main.Localization.GetTranslation(Localization.SettlersMenuTaxIntroduce, tier.Label),
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Trade;

                        return false == Tax.FindTaxRelief();
                    },
                    args =>
                    {
                        try
                        {
                            if (Instance._settlersForVillagesData.TryGetValue(
                                Settlement.CurrentSettlement.StringId, out var village))
                            {
                                village.TaxReliefTier = tier.TierEnum;
                            }
                            else
                            {
                                Instance._settlersForVillagesData.Add(
                                    Settlement.CurrentSettlement.StringId,
                                    new VillageDetailsModel(Settlement.CurrentSettlement.StringId, tier.TierEnum));
                            }

                            GameMenu.SwitchToMenu(VillageSettlersMenu);
                            Logger.DisplayInfoMsg(Main.Localization.GetTranslation(
                                Localization.SettlersActionTaxIntroduce, Settlement.CurrentSettlement.Name));
                        }
                        catch (Exception e)
                        {
                            Logger.logError(e);
                        }
                    }
                );
            }


            campaignGameSystemStarter.AddGameMenuOption(
                VillageSettlersMenu,
                "village_settlers_tax_relief_remove",
                Main.Localization.GetTranslation(Localization.SettlersMenuTaxCancel),
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;

                    return Tax.FindTaxRelief();
                },
                args =>
                {
                    try
                    {
                        if (Instance._settlersForVillagesData.TryGetValue(
                            Settlement.CurrentSettlement.StringId, out var village))
                        {
                            village.TaxReliefTier = -1;
                        }

                        GameMenu.SwitchToMenu(VillageSettlersMenu);
                        Logger.DisplayInfoMsg(Main.Localization.GetTranslation(Localization.SettlersActionTaxCancel,
                            Settlement.CurrentSettlement.Name));
                    }
                    catch (Exception e)
                    {
                        Logger.logError(e);
                    }
                }
            );
        }

        private static void AddSettlersMenu(CampaignGameStarter campaignGameSystemStarter)
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
                            Villagers.Add(tier.GoldPrice, tier.VillagersToAdd, Hero.MainHero,
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
                Main.Localization.GetTranslation(Localization.MenuLeave),
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => { GameMenu.SwitchToMenu("village"); }
            );
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("settlersForVillagesData", ref _settlersForVillagesData);

            if (Main.Settings.DeleteMode)
            {
                Logger.logDebug("Removing " + _settlersForVillagesData.Count + " entries");
                _settlersForVillagesData.Clear();
            }
        }
    }

    public class CustomSaveDefiner : SaveableTypeDefiner
    {
        public CustomSaveDefiner() : base(902003041)
        {
        }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            AddClassDefinition(typeof(VillageDetailsModel), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, VillageDetailsModel>));
        }
    }
}