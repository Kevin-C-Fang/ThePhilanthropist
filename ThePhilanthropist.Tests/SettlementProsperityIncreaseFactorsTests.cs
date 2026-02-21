using ThePhilanthropist;
using ThePhilanthropist.src;
using Xunit;

namespace ThePhilanthropist.Tests
{
    public class SettlementProsperityIncreaseFactorsTests
    {
        [Theory]
        [InlineData(-100, 1, 0f, 0f)]
        [InlineData(0, 0, 0f, 0f)]
        [InlineData(50f, 1, 50f, 50f)]
        [InlineData(10000f, 7, 10000f, 1428.571428571429f)]
        public void IncreaseProsperityIncreaseTotal_IncreaseProsperityAndCalculateIncreaseOverTime_ProsperityTotalIncreasedAndIncreaseOverTimeIsCorrect(
            float prosperityIncreaseTotal,
            int settingsDurationOfProsperityIncrease,
            float expectedProsperityIncreaseTotal,
            float expectedProsperityIncreaseOverTime)
        {
            var settings = new Settings()
            {
                DurationOfProsperityIncrease = settingsDurationOfProsperityIncrease
            };

            var factorsObject = new SettlementProsperityIncreaseFactors(0, settings);
            factorsObject.IncreaseProsperityIncreaseTotal(prosperityIncreaseTotal, settings);

            Assert.Equal(expectedProsperityIncreaseTotal, factorsObject.ProsperityIncreaseTotal);
            Assert.Equal(expectedProsperityIncreaseOverTime, factorsObject.ProsperityIncreaseOverTime);
        }

        [Theory]
        [InlineData(-100, 1, 0f, 0f, 0f)]
        [InlineData(0, 0, 0f, 0f, 0f)]
        [InlineData(50f, 1, 0f, 0f, 50f)]
        [InlineData(10000f, 7, 8571.428571428571f, 1428.571428571429f, 1428.571428571429f)]
        public void DecreaseProsperityIncreaseTotal_CheckProsperityDecreaseAndIncreaseOverTimeIsCorrect_ProsperityTotalAndIncreaseOverTimeIsCorrect(
            float prosperityIncreaseTotal,
            int settingsDurationOfProsperityIncrease,
            float expectedProsperityIncreaseTotal,
            float expectedProsperityIncreaseOverTime,
            float expectedResult)
        {
            var settings = new Settings()
            {
                DurationOfProsperityIncrease = settingsDurationOfProsperityIncrease
            };

            var factorsObject = new SettlementProsperityIncreaseFactors(prosperityIncreaseTotal, settings);
            float result = factorsObject.DecreaseProsperityIncreaseTotal();

            Assert.Equal(expectedProsperityIncreaseTotal, factorsObject.ProsperityIncreaseTotal);
            Assert.Equal(expectedProsperityIncreaseOverTime, factorsObject.ProsperityIncreaseOverTime);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(-100, 1, 0f)]
        [InlineData(0, 0, 0f)]
        [InlineData(50f, 1, 50f)]
        [InlineData(10000f, 7, 1428.571428571429f)]
        public void UpdateProsperityIncreaseOverTimeUsingDuration_CheckIfProsperityIncreaseOverTimeIsCorrect_ProsperityIncreaseOverTimeIsCorrect(
            float prosperityIncreaseTotal,
            int settingsDurationOfProsperityIncrease,
            float expectedProsperityIncreaseOverTime)
        {
            var settings = new Settings()
            {
                DurationOfProsperityIncrease = settingsDurationOfProsperityIncrease
            };

            var factorsObject = new SettlementProsperityIncreaseFactors(prosperityIncreaseTotal, settings);
            factorsObject.UpdateProsperityIncreaseOverTimeUsingDuration(settings);

            Assert.Equal(expectedProsperityIncreaseOverTime, factorsObject.ProsperityIncreaseOverTime);
        }

        [Theory]
        [InlineData(-100, 1, false)]
        [InlineData(0, 0, false)]
        [InlineData(500f, 10, true)]
        public void CanDecreaseProsperityCheck_ConfirmDecreaseProsperityCheckWorks_DecreaseProsperityCheckWorks(
            float prosperityIncreaseTotal,
            int settingsDurationOfProsperityIncrease,
            bool expectedResult)
        {
            var settings = new Settings()
            {
                DurationOfProsperityIncrease = settingsDurationOfProsperityIncrease
            };

            var factorsObject = new SettlementProsperityIncreaseFactors(prosperityIncreaseTotal, settings);
            bool result = factorsObject.CanDecreaseProsperityCheck();

            Assert.Equal(expectedResult, result);
        }
    }
}
