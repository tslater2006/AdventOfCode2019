using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Year2019 {

    class Day02 : ASolution {
        IntCodeVM vm = null;
        public Day02() : base(2, 2019, "") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {

            /* set the IntCode computer to error state 1202 */
            vm.SetMemory(1, 12);
            vm.SetMemory(2, 02);

            /* run the program */
            vm.RunProgram();

            /* read out memory 0 */
            int result = vm.ReadMemory(0);

            return result.ToString(); 
        }

        protected override string SolvePartTwo() {
            return null; 
        }
    }
}
