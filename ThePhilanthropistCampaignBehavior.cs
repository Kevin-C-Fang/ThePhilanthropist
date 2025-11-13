using Helpers;
using SandBox.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;


namespace ThePhilanthropist
{
    public class ThePhilanthropistCampaignBehavior : CampaignBehaviorBase
    {
        private const float TownProsperityLimit = 5000f;
        private const float VillageHearthLimit = 600f;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            AddGameMenus(starter);
        }

        private void AddGameMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("town", "settlement_donation", "Donate to townsfolk", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition),
                new GameMenuOption.OnConsequenceDelegate(settlement_donation_on_consequence), false, 8, false, null);
            starter.AddGameMenuOption("village", "settlement_donation", "Donate to villagers", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition),
                new GameMenuOption.OnConsequenceDelegate(settlement_donation_on_consequence), false, 8, false, null);
        }
        private bool settlement_donation_on_condition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Bribe;
            return true;
        }

        private void settlement_donation_on_consequence(MenuCallbackArgs args)
        {
            Settlement settlement = Settlement.CurrentSettlement;
            string donationText = "After entering the settlement, you see the poor and weary souls that wander its streets. \n\n" +
                "Children covered in mud, begging for denars. Men and women alike starving, their eyes red and mad with hunger. \n\n" +
                "You can not help but feel the bleakness of life and the quiet despair that fills it. " +
                "Your resolve to help these people stokes the fire within you, your desire to be good. \n\n" +
                "You will help these people, there is no doubt. You will donate what you can.";

            if (settlement.IsTown && settlement.Town.Prosperity < TownProsperityLimit)
            {
                TextInquiryData data = new TextInquiryData("Donation", donationText, true, true, "Donate", "Cancel", new Action<string>(this.OnDonateToSettlement), 
                    null, false, new Func<string, Tuple<bool, string>>(IsDonationTextValid), "", "");

                InformationManager.ShowTextInquiry(data, false, false);
            }
            else if (settlement.IsVillage && settlement.Village.Hearth < VillageHearthLimit)
            {
                TextInquiryData data = new TextInquiryData("Donation", donationText, true, true, "Donate", "Cancel", new Action<string>(this.OnDonateToSettlement), 
                    null, false, new Func<string, Tuple<bool, string>>(IsDonationTextValid), "", "");

                InformationManager.ShowTextInquiry(data, false, false);
            }
            else
            {
                string donationRejectedText = "A crowd gathers as word spreads of your intent to donate to the welfare of this settlement. \n\n" +
                    "A representative of the people steps forward and says \"Ser, we cannot accept. Our settlement has flourished beyond imagination. " +
                    "We are content and well fed. Please spread your kindness to others as you have done here.\" \n\n" +
                    "Astonished, you look around the crowd and see in each person's face a smile that lights the sky. \n\n" +
                    "Gratified by the virtue of these people, you accept the crowds wishes and continue on for there are still those in need.";

                InformationManager.ShowInquiry(new InquiryData("Thank you", donationRejectedText, true, false, "Leave", string.Empty, null, null), false, false);
            }
        }

        private void OnDonateToSettlement(string text)
        {
            if (!int.TryParse(text, out int donationAmount))
            {
                DisplayMessage("Invalid Input! Should not hit this point.");
                return;
            }

            Settlement settlement = Settlement.CurrentSettlement;
            float prosperityIncreaseAmount = donationAmount / 12f;

            if (settlement.IsTown)
            {
                settlement.Town.Prosperity = settlement.Town.Prosperity + prosperityIncreaseAmount > TownProsperityLimit ? TownProsperityLimit : 
                    settlement.Town.Prosperity + prosperityIncreaseAmount;
            }
            else if (settlement.IsVillage)
            {
                settlement.Village.Hearth = settlement.Village.Hearth + prosperityIncreaseAmount > VillageHearthLimit ? VillageHearthLimit : 
                    settlement.Village.Hearth + prosperityIncreaseAmount;
            }

            Hero.MainHero.ChangeHeroGold(-donationAmount);
            Campaign.Current.CurrentMenuContext.Refresh();
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
            else if(text.Any((char c) => char.IsLetter(c) || !char.IsLetterOrDigit(c)))
            {
                warningText = "Input text cannot include letters, special characters, or spaces.";
            }
            else if (int.TryParse(text, out int donationAmount))
            {
                Settlement settlement = Settlement.CurrentSettlement;

                if (donationAmount > Hero.MainHero.Gold)
                {
                    warningText = "You don't have enough {GOLD_ICON}";
                }
                else
                {
                    warningText = settlement.IsTown ? GetDonationWarnMessage(TownProsperityLimit, settlement.Town.Prosperity, donationAmount) : 
                        GetDonationWarnMessage(VillageHearthLimit, settlement.Village.Hearth, donationAmount);
                }
            }

            isDonationValid = warningText.Equals(string.Empty) ? true : false;

            return new Tuple<bool, string>(isDonationValid, warningText);
        }

        private string GetDonationWarnMessage(float limit, float currentProsperity, int donationAmount)
        {
            float prosperityNeeded = limit - currentProsperity;

            int goldNeededToReachMaxProsperity = (int)Math.Round(prosperityNeeded * 12f, MidpointRounding.AwayFromZero);

            return donationAmount <= goldNeededToReachMaxProsperity ? string.Empty : $"You can only donate {goldNeededToReachMaxProsperity}" +
                " {GOLD_ICON} or else the lord of this settlement will speculate on your intentions.";
        }
        private void DisplayMessage(string text)
        {
            InformationManager.DisplayMessage(new InformationMessage(text));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}