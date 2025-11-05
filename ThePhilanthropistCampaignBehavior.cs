using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;


namespace ThePhilanthropist
{
    public class ThePhilanthropistCampaignBehavior : CampaignBehaviorBase
    {
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
            starter.AddGameMenuOption("town", "settlement_donation", "Donate to town.", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition), null, true, -1, false, null);
            starter.AddGameMenuOption("castle", "settlement_donation", "Donate to castle.", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition), null, true, -1, false, null);
            starter.AddGameMenuOption("village", "settlement_donation", "Donate to village.", new GameMenuOption.OnConditionDelegate(settlement_donation_on_condition), null, true, -1, false, null);
        }
        private bool settlement_donation_on_condition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Bribe;
            return true;
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new System.NotImplementedException();
        }
    }
}