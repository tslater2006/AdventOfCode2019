using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Year2019 {

    class Day14 : ASolution {

        Refinery Refinery;
        public Day14() : base(14, 2019, "") {
            Refinery = new Refinery(Input);
        }

        protected override string SolvePartOne() {
            Refinery.ProduceMaterial(new RefineryProduction() { Amount = 1, Chemical = "FUEL" });
            return null; 
        }

        protected override string SolvePartTwo() {
            return null; 
        }
    }
}
