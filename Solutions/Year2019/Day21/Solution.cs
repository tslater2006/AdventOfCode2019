using AdventOfCode.Utilities;
using System;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day21 : ASolution {

        IntCodeVM vm = null;
        public Day21() : base(21, 2019, "Springdroid Adventure") {
            
        }

        protected override string SolvePartOne() {

            vm = new IntCodeVM(Input[0]);

            var springScript = "NOT A T\nOR T J\nNOT B T\nOR T J\nNOT C T\nOR T J\nAND D J\nWALK\n";

            vm.WriteInputString(springScript);

            var haltType = vm.RunProgram();

            var ans = vm.ReadOutputs().Last();

            return ans.ToString(); 
        }

        protected override string SolvePartTwo() {

            vm = new IntCodeVM(Input[0]);

            var springScript = "OR A J\nAND B J\nAND C J\nNOT J J\nAND D J\nOR E T\nOR H T\nAND T J\nRUN\n";

            vm.WriteInputString(springScript);

            var haltType = vm.RunProgram();

            foreach(var l in vm.ReadOutputs())
            {
                Console.Write((char)l);
            }

            var ans = vm.ReadOutputs().Last();

            return ans.ToString();

        }
    }
}
