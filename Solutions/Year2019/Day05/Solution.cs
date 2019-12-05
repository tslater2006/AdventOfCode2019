using AdventOfCode.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day05 : ASolution {
        IntCodeVM vm = null;
        public Day05() : base(5, 2019, "Sunny with a Chance of Asteroids") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {
            vm.WriteInput(1);
            vm.RunProgram();
            var ans = vm.ReadOutputs().Last();
            return ans.ToString(); 
        }

        protected override string SolvePartTwo() {
            vm = new IntCodeVM(Input[0]);
            vm.WriteInput(5);
            vm.RunProgram();
            List<int> outputs = vm.ReadOutputs();
            return outputs.Last().ToString();
        }
    }
}
