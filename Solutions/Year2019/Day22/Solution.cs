using AdventOfCode.Utilities;
using System;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day22 : ASolution {
        
        public Day22() : base(22, 2019, "") {
            
        }

        protected override string SolvePartOne() {

            SpaceCards cards = new SpaceCards(10007);
                foreach (var s in Input)
                {
                    var parts = s.Split(' ');
                    if (s.StartsWith("cut"))
                    {
                        cards.CutCards(int.Parse(parts.Last()));
                    }
                    else if (s.Contains("increment"))
                    {
                        cards.DealByIncrement(int.Parse(parts.Last()));
                    }
                    else if (s.Contains("new"))
                    {
                        cards.DealNewStack();
                    }

                }
                int[] result = cards.Cards;
                Console.WriteLine(result[2020]);
            
            var ans = Array.IndexOf(result, 2019);

            return ans.ToString();
        }

        protected override string SolvePartTwo() {
            return null; 
        }
    }
}
