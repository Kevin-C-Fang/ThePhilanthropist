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

    }
}
