using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;

namespace AdventOfCode.Solutions.Year2019 {

    class Day07 : ASolution {

        IntCodeVM[] Amplifiers = new IntCodeVM[5];

        public Day07() : base(7, 2019, "Amplification Circuit") {
            for(var x = 0; x < 5; x++)
            {
                Amplifiers[x] = new IntCodeVM(Input[0]);
            }
        }

        protected override string SolvePartOne() {

            var possibleCombinations = Enumerable.Range(0, 5).Permutations().ToList();
            long maxScore = 0;
            foreach (var combo in possibleCombinations)
            {
                int[] phases = combo.ToArray();

                foreach(var amp in Amplifiers)
                {
                    amp.Reset();
                }

                long ampOutput = 0;

                for (var x = 0; x < 5; x++)
                {
                    Amplifiers[x].WriteInput(phases[x]);
                    Amplifiers[x].WriteInput(ampOutput);
                    Amplifiers[x].RunProgram();

                    var outputs = Amplifiers[x].ReadOutputs();

                    ampOutput = outputs.First();
                }

                if (ampOutput > maxScore)
                {
                    maxScore = ampOutput;
                }

            }


            return maxScore.ToString(); 
        }

        protected override string SolvePartTwo() {
            var possibleCombinations = Enumerable.Range(5, 5).Permutations().ToList();
            long maxScore = 0;
            foreach (var combo in possibleCombinations)
            {
                int[] phases = combo.ToArray();
                phases = new int[] { 9,8,7,6,5 };
                foreach (var amp in Amplifiers)
                {
                    amp.Reset();
                }

                for(var x = 0; x < 5; x++)
                {
                    Amplifiers[x].WriteInput(phases[x]);
                }

                Queue<IntCodeVM> vmLoop = new Queue<IntCodeVM>(Amplifiers);
                long ampOutput = 0;
                int ampTerminations = 0;
                while (vmLoop.Count > 0)
                {
                    IntCodeVM curAmp = vmLoop.Dequeue();
                    int AmpIndex = Array.IndexOf(Amplifiers, curAmp);
                    curAmp.WriteInput(ampOutput);
                    if (curAmp.RunProgram() == HaltType.HALT_TERMINATE)
                    {
                        ampTerminations++;
                    } else
                    {
                        /* its waiting for more input */
                        vmLoop.Enqueue(curAmp);
                    }

                    ampOutput = curAmp.PopOutput();
                    if (AmpIndex == 0) { Console.WriteLine(); }
                    Console.Write(ampOutput.ToString() + ", ");
                }

                if (ampOutput > maxScore)
                {
                    maxScore = ampOutput;
                }
                break;
            }
            return maxScore.ToString(); 
        }



    }

    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> values)
        {
            if (values.Count() == 1)
                return new[] { values };
            return values.SelectMany(v => Permutations(values.Where(x => x.Equals(v) == false)), (v, p) => p.Prepend(v));
        }
    }

}
