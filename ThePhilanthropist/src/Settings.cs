using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.LinQuick;
using static TaleWorlds.Library.VirtualFolders.Win64_Shipping_Client;

namespace ThePhilanthropist.src
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public const float MaxTownProsperityFromDonationMinLimit = 0f;
        public const float MaxTownProsperityFromDonationMaxLimit = 50000f;

        private float _maxTownProsperityFromDonation = 5000f;

        private int _prosperityDurationIncrease = 3;

        public override string Id => "ThePhilanthropist";
        public override string DisplayName => "The Philanthropist";
        public override string FolderName => "ThePhilanthropist";
        public override string FormatType => "json";

        [SettingPropertyFloatingInteger("Max Town Prosperity From Donation", MaxTownProsperityFromDonationMinLimit, MaxTownProsperityFromDonationMaxLimit, HintText = "Max town prosperity that can be increased through donating.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public float MaxTownProsperityFromDonation
        {
            get => _maxTownProsperityFromDonation;
            set
            {
                var clamped = Math.Min(Math.Max(value, MaxTownProsperityFromDonationMinLimit), MaxTownProsperityFromDonationMaxLimit);
                if (_maxTownProsperityFromDonation != clamped)
                {
                    _maxTownProsperityFromDonation = clamped;
                }
            }
        }

        [SettingPropertyFloatingInteger("Donate Village Prosperity Max", 0f, 6000f, HintText = "Max village prosperity that can be increased through donating.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public float DonateVillageProsperityMax { get; set; } = 600f;

        [SettingPropertyInteger("Gold To Prosperity Ratio", 1, 120, HintText = "Amount of gold required to increase prosperity by 1.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public int GoldToProsperityRatio { get; set; } = 12;

        [SettingPropertyBool("Husia - Enable Prosperity Increase Over Time", IsToggle = true, HintText = "Prosperity will increase over time instead of occurring immediately.", RequireRestart = false)]
        [SettingPropertyGroup("Husia - Enable Prosperity Increase Over Time")]
        public bool EnableProsperityIncreaseOverTime { get; set; } = false;

        [SettingPropertyInteger("Duration of Prosperity Increase", 1, 14, HintText = "Amount of days to increase prosperity over time.", RequireRestart = false)]
        [SettingPropertyGroup("Husia - Enable Prosperity Increase Over Time")]
        public int ProsperityDurationIncrease {
            get
            {
                return _prosperityDurationIncrease;
            }
            set
            {
                if (_prosperityDurationIncrease != value)
                {
                    _prosperityDurationIncrease = value;
                    OnPropertyChanged(nameof(ProsperityDurationIncrease));
                }
            }
        }
    }
}
