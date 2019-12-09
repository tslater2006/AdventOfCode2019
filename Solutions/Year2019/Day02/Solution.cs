using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Year2019 {

    class Day02 : ASolution {
        IntCodeVM vm = null;
        public Day02() : base(2, 2019, "1202 Program Alarm") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {

            /* set the IntCode computer to error state 1202 */
            vm.WriteMemory(1, 12);
            vm.WriteMemory(2, 02);

            /* run the program */
            vm.RunProgram();

            /* read out memory 0 */
            long result = vm.ReadMemory(0);

            return result.ToString(); 
        }

        protected override string SolvePartTwo() {

            bool answerFound = false;
            int noun = 0;
            int verb = 0;

            for (var x = 0; x <= 99; x++)
            {
                for (var y = 0; y <= 99; y++)
                {
                    vm.Reset();
                    vm.WriteMemory(1, x);
                    vm.WriteMemory(2, y);

                    vm.RunProgram();

                    if (vm.ReadMemory(0) == 19690720)
                    {
                        answerFound = true;
                    }
                    if (answerFound)
                    {
                        verb = y;
                        break;
                    }
                }
                if (answerFound) {
                    noun = x;
                    break;
                }
            }

            return (100 * noun + verb).ToString(); 
        }
    }
}
