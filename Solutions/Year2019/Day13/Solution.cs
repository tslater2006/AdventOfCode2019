using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode.Solutions.Year2019 {

    class Day13 : ASolution {

        IntCodeVM vm;
        Dictionary<Point, long> screen = new Dictionary<Point, long>();
        long score = 0;
        Point paddleCoords;
        Point ballCoords;

        public Day13() : base(13, 2019, "Care Package") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {

            vm.RunProgram();
            screen.Clear();

            ProcessOutputs();

            var ans = screen.Values.Count(v => v == 2);

            return ans.ToString(); 
        }

        private void ProcessOutputs()
        {
            while (vm.ReadOutputs().Count > 0)
            {
                var x = vm.PopOutput();
                var y = vm.PopOutput();
                var tile = vm.PopOutput();

                if (x == -1 && y == 0)
                {
                    /* score output, not screen */
                    score = tile;
                }

                Point p = new Point((int)x, (int)y);

                if (screen.ContainsKey(p) == false)
                {
                    screen.Add(p, tile);
                }
                else
                {
                    screen[p] = tile;
                }

                if (tile == 3)
                {
                    paddleCoords = p;
                } else if (tile == 4)
                {
                    ballCoords = p;
                }
            }
        }

        protected override string SolvePartTwo() {
            vm.Reset();

            /* insert quarters... */
            vm.WriteMemory(0, 2);

            /* basic strategy is to maintain a screen, and then every time we're asked for input, move in the relative direction of the ball */
            
            while (vm.RunProgram() == HaltType.HALT_WAITING)
            {
                ProcessOutputs();

                if (ballCoords.X < paddleCoords.X)
                {
                    vm.WriteInput(-1);
                }
                else if (ballCoords.X > paddleCoords.X)
                {
                    vm.WriteInput(1);
                }
                else
                {
                    vm.WriteInput(0);
                }
            }

            ProcessOutputs();

            return score.ToString(); 
        }

    }
}
