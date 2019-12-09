using AdventOfCode.Utilities;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day09 : ASolution {
        IntCodeVM vm = null;
        public Day09() : base(9, 2019, "Sensor Boost") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {
            vm.WriteInput(1);

            vm.RunProgram();
            var outs = vm.ReadOutputs();

            return outs.First().ToString(); 
        }

        protected override string SolvePartTwo() {
            vm.Reset();
            vm.WriteInput(2);

            vm.RunProgram();
            var outs = vm.ReadOutputs();

            return outs.First().ToString();
        }
    }
}
