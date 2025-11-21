using System.Collections.Generic;
using TaleWorlds.SaveSystem;


namespace ThePhilanthropist.src
{
    public class ThePhilanthropistSaveableTypeDefiner : SaveableTypeDefiner
    {
        public ThePhilanthropistSaveableTypeDefiner() : base(125-734-093)
        {

        }

        protected override void DefineClassTypes()
        {
            base.DefineClassTypes();

            AddClassDefinition(typeof(SettlementProsperityIncreaseFactors), 1);
        }

        protected override void DefineContainerDefinitions()
        {
            base.DefineContainerDefinitions();

            ConstructContainerDefinition(typeof(Dictionary<string, SettlementProsperityIncreaseFactors>));
        }
    }
}