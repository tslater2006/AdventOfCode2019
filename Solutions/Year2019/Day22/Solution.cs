using AdventOfCode.Utilities;
using System;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2019 {

    class Day22 : ASolution {
        
        public Day22() : base(22, 2019, "Slam Shuffle") {
            
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

        protected override string SolvePartTwo()
        {

            BigInteger LEN = 119315717514047;
            BigInteger TIMES = 101741582076661;

            BigInteger a = 0;
            BigInteger b = 0;


            (a, b) = (1, 0);

            foreach (var s in Input)
            {
                var parts = s.Split(' ');
                if (s.StartsWith("cut"))
                {
                    var n = BigInteger.Parse(parts.Last());
                    (a, b) = (a, FixMod((b - n), LEN) % LEN);
                }
                else if (s.Contains("increment"))
                {
                    var n = BigInteger.Parse(parts.Last());
                    (a, b) = (FixMod((a * n), LEN) % LEN, FixMod((b * n), LEN) % LEN);
                }
                else if (s.Contains("new"))
                {
                    (a, b) = (FixMod(-a, LEN) % LEN, FixMod((-b + LEN - 1), LEN) % LEN);
                }
            }

            var an = ModPow(a, TIMES, LEN);

            (a, b) = (an, FixMod(b * (an - 1) * ModInverse(FixMod(a - 1, LEN), LEN), LEN) % LEN);

            var ans = (FixMod(FixMod((2020 - b), LEN) * ModInverse(a, LEN), LEN) % LEN);

            return ans.ToString();
        }

        BigInteger FixMod(BigInteger b, BigInteger m)
        {
            while (b < 0)
            {
                b += m;
            }

            return b;
        }

        BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            return BigInteger.ModPow(a, m-2, m);
        }

        BigInteger ModPow(BigInteger b, BigInteger e, BigInteger m)
        {
            if (e == 0)
            {
                return 1;
            } else if (e % 2 == 0)
            {
                return ModPow(b * b % m, e / 2, m);
            }else
            {
                return (b * ModPow(b, e - 1, m)) % m;
            }
        }
    }
}
