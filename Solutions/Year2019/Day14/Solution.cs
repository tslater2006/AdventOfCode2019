using AdventOfCode.Utilities;
using System.Collections.Generic;

namespace AdventOfCode.Solutions.Year2019
{

    class Day14 : ASolution
    {

        Refinery Refinery;
        public Day14() : base(14, 2019, "")
        {
            Refinery = new Refinery(Input);
        }

        protected override string SolvePartOne()
        {
            Refinery.ProduceMaterial(new RefineryProduction() { Amount = 1, Chemical = "FUEL" });

            return Refinery.OreRequired.ToString();
        }

        protected override string SolvePartTwo()
        {
            Refinery = new Refinery(Input);
            long fuelCount = 0;
            var productionFactor = 10000;
            Dictionary<string, int> oldSurplus = null;
            long oldOreRequired = 0;
            while (productionFactor >= 1)
            {
                while (Refinery.OreRequired < 1000000000000)
                {
                    oldSurplus = new Dictionary<string, int>(Refinery.Surplus);
                    oldOreRequired = Refinery.OreRequired;
                    Refinery.ProduceMaterial(new RefineryProduction() { Amount = productionFactor, Chemical = "FUEL" });
                    fuelCount += productionFactor;
                }

                /* we've gone over... */
                if (productionFactor >= 1)
                {
                    /*reset old state*/
                    Refinery.Surplus = new Dictionary<string,int>(oldSurplus);
                    Refinery.OreRequired = oldOreRequired;
                    fuelCount -= productionFactor;
                    productionFactor /= 10;
                }
            }
            return fuelCount.ToString();
        }
    }
}
