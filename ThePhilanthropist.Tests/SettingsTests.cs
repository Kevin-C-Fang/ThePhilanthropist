using ThePhilanthropist;
using ThePhilanthropist.src;
using Xunit;

namespace ThePhilanthropist.Tests
{
    public class SettingsTests
    {
        [Theory]
        [InlineData(-1, Settings.MaxTownProsperityFromDonationMinLimit)]
        [InlineData(0, 0)]
        [InlineData(5, 5)]
        [InlineData(50000, 50000)]
        [InlineData(50001, Settings.MaxTownProsperityFromDonationMaxLimit)]
        public void MaxTownProsperityFromDonation_StoreValueWithinLimits_ValueIsWithinLimits(
            float maxTownProsperityFromDonation,
            float expectedMaxTownProsperityFromDonation)
        {
            var settings = new Settings();
            settings.MaxTownProsperityFromDonation = maxTownProsperityFromDonation;

            Assert.Equal(expectedMaxTownProsperityFromDonation, settings.MaxTownProsperityFromDonation);
        }

        [Theory]
        [InlineData(-1, Settings.MaxVillageProsperityFromDonationMinLimit)]
        [InlineData(0, 0)]
        [InlineData(5, 5)]
        [InlineData(6000, 6000)]
        [InlineData(6001, Settings.MaxVillageProsperityFromDonationMaxLimit)]
        public void MaxVillageProsperityFromDonation_StoreValueWithinLimits_ValueIsWithinLimits(
            float maxVillageProsperityFromDonation,
            float expectedMaxVillageProsperityFromDonation)
        {
            var settings = new Settings();
            settings.MaxVillageProsperityFromDonation = maxVillageProsperityFromDonation;

            Assert.Equal(expectedMaxVillageProsperityFromDonation, settings.MaxVillageProsperityFromDonation);
        }

        [Theory]
        [InlineData(0, Settings.GoldToProsperityRatioMinLimit)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(120, 120)]
        [InlineData(121, Settings.GoldToProsperityRatioMaxLimit)]
        public void GoldToProsperityRatio_StoreValueWithinLimits_ValueIsWithinLimits(
            int goldToProsperityRatio,
            int expectedGoldToProsperityRatio)
        {
            var settings = new Settings();
            settings.GoldToProsperityRatio = goldToProsperityRatio;

            Assert.Equal(expectedGoldToProsperityRatio, settings.GoldToProsperityRatio);
        }

        [Theory]
        [InlineData(0, Settings.DurationOfProsperityIncreaseMinLimit)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(14, 14)]
        [InlineData(15, Settings.DurationOfProsperityIncreaseMaxLimit)]
        public void DurationOfProsperityIncrease_StoreValueWithinLimits_ValueIsWithinLimits(
            int durationOfProsperityIncrease,
            int expectedDurationOfProsperityIncrease)
        {
            var settings = new Settings();
            settings.DurationOfProsperityIncrease = durationOfProsperityIncrease;

            Assert.Equal(expectedDurationOfProsperityIncrease, settings.DurationOfProsperityIncrease);
        }
    }
}
