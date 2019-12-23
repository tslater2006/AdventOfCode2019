using AdventOfCode.Utilities;
using System;

namespace AdventOfCode.Solutions.Year2019 {

    class Day19 : ASolution {
        IntCodeVM vm = null;
        public Day19() : base(19, 2019, "Tractor Beam") {
            
        }

        protected override string SolvePartOne() {
            long affected = 0;
            vm = new IntCodeVM(Input[0]);
            for(var y = 0; y < 50; y++)
            {
                for (var x = 0; x < 50; x++)
                {
                    vm.Reset();
                    vm.WriteInput(x);
                    vm.WriteInput(y);

                    var haltType = vm.RunProgram();
                    affected += vm.PopOutput();

                }
            }

            return affected.ToString(); 
        }

        protected override string SolvePartTwo() {

            vm = new IntCodeVM(Input[0]);

            int curX = 10, curY = 0;
            int lastX = 0, lastY = 0;
            long hit = 0;
            while (true)
            {
                /* wait until "Y" hits inside the beam */
                while (hit == 0)
                {
                    vm.Reset();
                    vm.WriteInput(curX);
                    vm.WriteInput(curY);
                    vm.RunProgram();
                    hit = vm.PopOutput();
                    if (hit == 0)
                    {
                        curY++;
                    }
                }
                
                /* we have a beam hit at (curX,curY)... check (curX-100,curY+100) and see if its in the grid */
                vm.Reset();
                vm.WriteInput(curX - 99);
                vm.WriteInput(curY + 99);
                vm.RunProgram();
                hit = vm.PopOutput();
                if (hit == 1)
                {
                    /* winning! */
                    var xCoord = curX - 99;

                    int i = 3;
                } else
                {
                    curX++;
                    curY -= 10 ;
                }

            }

        }
    }
}
