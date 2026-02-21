using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;


namespace ThePhilanthropist.src
{
    public class SettlementProsperityIncreaseFactors
    {
        [SaveableField(1)]
        public float _prosperityIncreaseTotal;

        [SaveableField(2)]
        public float _prosperityIncreaseOverTime;
        
        public float ProsperityIncreaseTotal
        {
            get => _prosperityIncreaseTotal;
            set
            {
                value = value < 0 ? 0 : value;
                if (_prosperityIncreaseTotal != value)
                {
                    _prosperityIncreaseTotal = value;
                }
            }
        }

        public float ProsperityIncreaseOverTime
        {
            get => _prosperityIncreaseOverTime;
            set
            {
                if (_prosperityIncreaseOverTime != value)
                {
                    _prosperityIncreaseOverTime = value;
                }
            }
        }

        public SettlementProsperityIncreaseFactors(float prosperityIncreaseTotal, Settings settings)
        {
            IncreaseProsperityIncreaseTotal(prosperityIncreaseTotal, settings);
        }

        public void IncreaseProsperityIncreaseTotal(float prosperityIncreaseTotal, Settings settings)
        {
            ProsperityIncreaseTotal += prosperityIncreaseTotal;
            UpdateProsperityIncreaseOverTimeUsingDuration(settings);
        }

        public float DecreaseProsperityIncreaseTotal()
        {
            float decreasedProsperityIncreaseTotal = ProsperityIncreaseTotal - ProsperityIncreaseOverTime;

            if (decreasedProsperityIncreaseTotal <= 0f)
            {
                ProsperityIncreaseTotal = 0f;
                float currProsperityIncreaseOverTime = ProsperityIncreaseOverTime;
                ProsperityIncreaseOverTime = 0f;
                return currProsperityIncreaseOverTime;
            }
            else
            {
                ProsperityIncreaseTotal -= ProsperityIncreaseOverTime;
                return ProsperityIncreaseOverTime;
            }
        }

        public void UpdateProsperityIncreaseOverTimeUsingDuration(Settings settings)
        {
            ProsperityIncreaseOverTime = ProsperityIncreaseTotal / settings.DurationOfProsperityIncrease;
        }

        public bool CanDecreaseProsperityCheck()
        {
            return ProsperityIncreaseTotal > 0f && ProsperityIncreaseOverTime > 0f;
        }
    }
}