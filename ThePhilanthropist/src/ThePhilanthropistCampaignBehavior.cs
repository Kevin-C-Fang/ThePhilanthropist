using Helpers;
using MCM.Abstractions.Base.Global;
using SandBox.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using MathF = TaleWorlds.Library.MathF;


namespace ThePhilanthropist.src
{
    public class ThePhilanthropistCampaignBehavior : CampaignBehaviorBase
    {
        private Dictionary<string, SettlementProsperityIncreaseFactors> SettlementProsperityIncreaseTracker = new Dictionary<string, SettlementProsperityIncreaseFactors>();
        private Settings _settings = GlobalSettings<Settings>.Instance!;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, OnHourlyTickSettlementEvent);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnDailyTickSettlementEvent);
            _settings.PropertyChanged += HandleProsperityDurationIncrease;
        }

        private void HandleProsperityDurationIncrease(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.ProsperityDurationIncrease))
            {
                PropagateDurationProsperityIncreaseChange();
            }
        }

        private void PropagateDurationProsperityIncreaseChange()
        {
            foreach (SettlementProsperityIncreaseFactors factors in SettlementProsperityIncreaseTracker.Values)
            {
                factors.UpdateProsperityIncreaseOverTimeUsingDuration(_settings);
            }
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            AddGameMenus(starter);
            PropagateDurationProsperityIncreaseChange();
        }

        private void AddGameMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("town", "settlement_donation", "Donate to townsfolk", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition),
                new GameMenuOption.OnConsequenceDelegate(settlement_donation_on_consequence), false, -1, false, null);
            starter.AddGameMenuOption("village", "settlement_donation", "Donate to villagers", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition),
                new GameMenuOption.OnConsequenceDelegate(settlement_donation_on_consequence), false, -1, false, null);

            starter.AddGameMenuOption("village_looted", "rebuild_village", "Help rebuild {VILLAGE_NAME}", new GameMenuOption.OnConditionDelegate(rebuild_village_on_condition), 
                new GameMenuOption.OnConsequenceDelegate(rebuild_village_on_consequence), false, -1, false, null);

            starter.AddWaitGameMenu("rebuild_village", GameTexts.FindText("settlement_rebuild_description").ToString(), new OnInitDelegate(rebuild_village_on_init), 
                new OnConditionDelegate(back_on_condition), new OnConsequenceDelegate(wait_menu_rebuild_village_on_consequence),
                new OnTickDelegate(wait_menu_rebuild_village_on_tick), GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption,
                GameMenu.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
            starter.AddGameMenuOption("rebuild_village", "rebuild_village_end", "End Rebuilding", new GameMenuOption.OnConditionDelegate(leave_on_condition),
                new GameMenuOption.OnConsequenceDelegate(wait_menu_end_rebuilding_on_consequence), true, -1, false, null);
        }

        private void OnDailyTickSettlementEvent(Settlement settlement)
        {
            if (_settings.EnableProsperityIncreaseOverTime && SettlementProsperityIncreaseTracker.ContainsKey(settlement.StringId))
            {
                if (SettlementProsperityIncreaseTracker[settlement.StringId].CanDecreaseProsperityCheck())
                {
                    float prosperityIncreaseAmount = SettlementProsperityIncreaseTracker[settlement.StringId].DecreaseProsperityIncreaseTotal();

                    IncreaseSettlementProsperityOrHearth(settlement, prosperityIncreaseAmount);
                }
            }
        }

        private void wait_menu_end_rebuilding_on_consequence(MenuCallbackArgs args)
        {
            PlayerEncounter.Finish(true);
        }

        private bool leave_on_condition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Leave;
            return true;
        }

        private void OnHourlyTickSettlementEvent(Settlement settlement)
        {
            Settlement currentSettlement = Settlement.CurrentSettlement;
            if (settlement == currentSettlement && settlement.IsRaided)
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(0.02f + MobileParty.MainParty.Party.EstimatedStrength/6000f, false, null);
                IncreaseSettlementHealthAction.Apply(currentSettlement, explainedNumber.ResultNumber);
            }
        }

        private void wait_menu_rebuild_village_on_tick(MenuCallbackArgs args, CampaignTime dt)
        {
            args.MenuContext.GameMenu.SetProgressOfWaitingInMenu(Settlement.CurrentSettlement.SettlementHitPoints);
        }

        private void wait_menu_rebuild_village_on_consequence(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("village");
        }

        private bool back_on_condition(MenuCallbackArgs args)
        {
            return true;
        }

        private void rebuild_village_on_init(MenuCallbackArgs args)
        {
            args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
        }

        private bool rebuild_village_on_condition(MenuCallbackArgs args)
        {
            MBTextManager.SetTextVariable("VILLAGE_NAME", Settlement.CurrentSettlement.Name, false);
            args.optionLeaveType = GameMenuOption.LeaveType.Craft;
            args.IsEnabled = !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction);
            args.Tooltip = args.IsEnabled ? null : GameTexts.FindText("enemy_settlement_rebuild_warning");
            return true;
        }

        private void rebuild_village_on_consequence(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("rebuild_village");
        }

        private bool settlement_donation_on_condition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Bribe;
            args.IsEnabled = !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction);
            args.Tooltip = args.IsEnabled ? null : GameTexts.FindText("enemy_settlement_donation_warning");
            return true;
        }

        private void settlement_donation_on_consequence(MenuCallbackArgs args)
        {
            Settlement settlement = Settlement.CurrentSettlement;
            string donationText = GameTexts.FindText("settlement_donation_description").ToString();

            if (settlement.IsTown && settlement.Town.Prosperity < _settings.DonateTownProsperityMax)
            {
                TextInquiryData data = new TextInquiryData("Donation", donationText, true, true, "Donate", "Cancel", new Action<string>(OnDonateToSettlement), 
                    null, false, new Func<string, Tuple<bool, string>>(IsDonationTextValid), "", "");

                InformationManager.ShowTextInquiry(data, false, false);
            }
            else if (settlement.IsVillage && settlement.Village.Hearth < _settings.DonateVillageProsperityMax)
            {
                TextInquiryData data = new TextInquiryData("Donation", donationText, true, true, "Donate", "Cancel", new Action<string>(OnDonateToSettlement), 
                    null, false, new Func<string, Tuple<bool, string>>(IsDonationTextValid), "", "");

                InformationManager.ShowTextInquiry(data, false, false);
            }
            else
            {
                InformationManager.ShowInquiry(new InquiryData("Thank you", GameTexts.FindText("settlement_donation_rejected_description").ToString(), true, false, "Leave", string.Empty, null, null), false, false);
            }
        }

        private void OnDonateToSettlement(string text)
        {
            if (!int.TryParse(text, out int donationAmount))
            {
                InformationManager.DisplayMessage(new InformationMessage("Invalid Input! Should not hit this point."));
                return;
            }

            Settlement settlement = Settlement.CurrentSettlement;
            float prosperityIncreaseAmount = donationAmount / (float)_settings.GoldToProsperityRatio;

            if (_settings.EnableProsperityIncreaseOverTime)
            {
                if (SettlementProsperityIncreaseTracker.ContainsKey(settlement.StringId))
                {
                    SettlementProsperityIncreaseTracker[settlement.StringId].IncreaseProsperityIncreaseTotal(prosperityIncreaseAmount, _settings);
                }
                else
                {
                    SettlementProsperityIncreaseTracker.Add(settlement.StringId, new SettlementProsperityIncreaseFactors(prosperityIncreaseAmount, _settings));
                }
            }
            else
            {
                IncreaseSettlementProsperityOrHearth(settlement, prosperityIncreaseAmount);
                Campaign.Current.CurrentMenuContext.Refresh();
            }

            Hero.MainHero.ChangeHeroGold(-donationAmount);
        }

        private Tuple<bool, string> IsDonationTextValid(string text)
        {
            bool isDonationValid = true;
            string warningText = string.Empty;

            if (string.IsNullOrEmpty(text))
            {
                warningText = "Input text cannot be empty.";
            }
            else if (text.Length > 10)
            {
                warningText = "Input text cannot be longer than 10 characters.";
            }
            else if(text.Any((c) => char.IsLetter(c) || !char.IsLetterOrDigit(c)))
            {
                warningText = "Input text cannot include letters, special characters, or spaces.";
            }
            else if (int.TryParse(text, out int donationAmount))
            {
                Settlement settlement = Settlement.CurrentSettlement;

                if (donationAmount > Hero.MainHero.Gold)
                {
                    warningText = GameTexts.FindText("str_decision_not_enough_gold").ToString();
                }
                else
                {
                    warningText = settlement.IsTown ? GetDonationWarnMessage(_settings.DonateTownProsperityMax, settlement.Town.Prosperity, donationAmount) : 
                        GetDonationWarnMessage(_settings.DonateVillageProsperityMax, settlement.Village.Hearth, donationAmount);
                }
            }

            isDonationValid = warningText.Equals(string.Empty);

            return new Tuple<bool, string>(isDonationValid, warningText);
        }

        private void IncreaseSettlementProsperityOrHearth(Settlement settlement, float prosperityIncreaseAmount)
        {
            if (settlement.IsTown)
            {
                settlement.Town.Prosperity = settlement.Town.Prosperity + prosperityIncreaseAmount > _settings.DonateTownProsperityMax ? _settings.DonateTownProsperityMax :
                    settlement.Town.Prosperity + prosperityIncreaseAmount;
            }
            else if (settlement.IsVillage)
            {
                settlement.Village.Hearth = settlement.Village.Hearth + prosperityIncreaseAmount > _settings.DonateVillageProsperityMax ? _settings.DonateVillageProsperityMax :
                    settlement.Village.Hearth + prosperityIncreaseAmount;
            }
        }

        private string GetDonationWarnMessage(float limit, float currentProsperity, int donationAmount)
        {
            float prosperityNeeded = limit - currentProsperity;
            
            int goldNeededToReachMaxProsperity = (int)Math.Round(prosperityNeeded * _settings.GoldToProsperityRatio, MidpointRounding.AwayFromZero);
            TextObject warningText = Settlement.CurrentSettlement.Owner == Hero.MainHero ? GameTexts.FindText("settlement_donation_warning_owner") : GameTexts.FindText("settlement_donation_warning_non_owner");
            warningText.SetTextVariable("GOLD_AMOUNT", goldNeededToReachMaxProsperity);

            return donationAmount <= goldNeededToReachMaxProsperity ? string.Empty : warningText.ToString();
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("ThePhilanthropist", ref SettlementProsperityIncreaseTracker);
        }
    }
}