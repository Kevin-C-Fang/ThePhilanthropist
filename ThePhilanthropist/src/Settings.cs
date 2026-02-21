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
        public const float MaxVillageProsperityFromDonationMinLimit = 0f;
        public const float MaxVillageProsperityFromDonationMaxLimit = 6000f;
        public const int GoldToProsperityRatioMinLimit = 1;
        public const int GoldToProsperityRatioMaxLimit = 120;
        public const int DurationOfProsperityIncreaseMinLimit = 1;
        public const int DurationOfProsperityIncreaseMaxLimit = 14;

        private float _maxTownProsperityFromDonation = 5000f;
        private float _maxVillageProsperityFromDonation = 600f;
        private int _goldToProsperityRatio = 12;
        private int _durationOfProsperityIncrease = 3;

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

        [SettingPropertyFloatingInteger("Max Village Prosperity From Donation", MaxVillageProsperityFromDonationMinLimit, MaxVillageProsperityFromDonationMaxLimit, HintText = "Max village prosperity that can be increased through donating.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public float MaxVillageProsperityFromDonation
        {
            get => _maxVillageProsperityFromDonation;
            set
            {
                var clamped = Math.Min(Math.Max(value, MaxVillageProsperityFromDonationMinLimit), MaxVillageProsperityFromDonationMaxLimit);
                if (_maxVillageProsperityFromDonation != clamped)
                {
                    _maxVillageProsperityFromDonation = clamped;
                }
            }
        }

        [SettingPropertyInteger("Gold To Prosperity Ratio", GoldToProsperityRatioMinLimit, GoldToProsperityRatioMaxLimit, HintText = "Amount of gold required to increase prosperity by 1.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public int GoldToProsperityRatio
        {
            get => _goldToProsperityRatio;
            set
            {
                var clamped = Math.Min(Math.Max(value, GoldToProsperityRatioMinLimit), GoldToProsperityRatioMaxLimit);
                if (_goldToProsperityRatio != clamped)
                {
                    _goldToProsperityRatio = clamped;
                }
            }
        }

        [SettingPropertyBool("Husia - Enable Prosperity Increase Over Time", IsToggle = true, HintText = "Prosperity will increase over time instead of occurring immediately.", RequireRestart = false)]
        [SettingPropertyGroup("Husia - Enable Prosperity Increase Over Time")]
        public bool EnableProsperityIncreaseOverTime { get; set; } = false;

        [SettingPropertyInteger("Duration of Prosperity Increase", DurationOfProsperityIncreaseMinLimit, DurationOfProsperityIncreaseMaxLimit, HintText = "Amount of days to increase prosperity over time.", RequireRestart = false)]
        [SettingPropertyGroup("Husia - Enable Prosperity Increase Over Time")]
        public int DurationOfProsperityIncrease
        {
            get
            {
                return _durationOfProsperityIncrease;
            }
            set
            {
                var clamped = Math.Min(Math.Max(value, DurationOfProsperityIncreaseMinLimit), DurationOfProsperityIncreaseMaxLimit);
                if (_durationOfProsperityIncrease != clamped)
                {
                    _durationOfProsperityIncrease = clamped;
                    OnPropertyChanged(nameof(DurationOfProsperityIncrease));
                }
            }
        }
    }
}
