using System;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day01 : ASolution {
        int[] masses = null;
        public Day01() : base(1, 2019, "The Tyranny of the Rocket Equation") {
            masses = Input.Select(s => int.Parse(s)).ToArray();
        }

        protected override string SolvePartOne() {
            var answ = masses.Select(m => GetFuelForMass(m)).Sum().ToString();
            return answ;
        }

        protected override string SolvePartTwo() {
            return masses.Select(m => GetRecursiveFuelForMass(m)).Sum().ToString(); 
        }

        private int GetFuelForMass(int m)
        {
            return m / 3 - 2;
        }

        private int GetRecursiveFuelForMass(int m)
        {
            /* determine the main fuel requirement */
            var initialFuel = GetFuelForMass(m);

            /* if we need more fuel, we need to return that fuel + the fuel required to lift the fuel */
            if (initialFuel > 0)
            {
                return initialFuel + GetRecursiveFuelForMass(initialFuel);
            } else
            {
                /* this is the recursion escape clause, 
                 * if we're called with an initial fuel requirement of 0, just return 0 
                 */
                return 0;
            }
        }

    }
}
