using AdventOfCode.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2019 {

    class Day12 : ASolution {
        string[] testInput = new string[] { "<x=-1, y=0, z=2>", "<x=2, y=-10, z=-7>", "<x=4, y=-8, z=8>", "<x=3, y=5, z=-1>" };

        List<Moon> Moons = new List<Moon>();

        public Day12() : base(12, 2019, "The N-Body Problem") {
            /* no setup here since both parts need clean state */
        }

        protected override string SolvePartOne() {
            Moons.Clear();
            Regex numberRegex = new Regex(@"-?\d+");
            foreach (var line in Input)
            {
                var matches = numberRegex.Matches(line);
                Moon m = new Moon(new Point3D() { X = int.Parse(matches[0].Value), Y = int.Parse(matches[1].Value), Z = int.Parse(matches[2].Value) });
                Moons.Add(m);
            }

            foreach (Moon m in Moons)
            {
                m.OrbitSiblings.AddRange(Moons.Where(moon => moon != m));
            }

            for (var x = 0; x < 1000; x++)
            {
                foreach (Moon m in Moons)
                {
                    m.ApplyGravity();
                }

                foreach (Moon m in Moons)
                {
                    m.Orbit();
                }
            }
            var totalEnergy = Moons.Sum(s => s.TotalEnergy);
            return totalEnergy.ToString(); 
        }

        protected override string SolvePartTwo() {
            Moons.Clear();
            Regex numberRegex = new Regex(@"-?\d+");
            foreach (var line in Input)
            {
                var matches = numberRegex.Matches(line);
                Moon m = new Moon(new Point3D() { X = int.Parse(matches[0].Value), Y = int.Parse(matches[1].Value), Z = int.Parse(matches[2].Value) });
                Moons.Add(m);
            }

            foreach (Moon m in Moons)
            {
                m.OrbitSiblings.AddRange(Moons.Where(moon => moon != m));
            }

            /* find the period for "X" axis (0) */
            HashSet<(int, int, int, int, int, int, int, int)> states = new HashSet<(int, int, int, int, int, int, int, int)>();
            int xSteps = 0;
            while (true)
            {
                var curXState = (Moons[0].Position.X, Moons[1].Position.X, Moons[2].Position.X, Moons[3].Position.X, Moons[0].Velocity.X, Moons[1].Velocity.X, Moons[2].Velocity.X, Moons[3].Velocity.X);
                if (states.Contains(curXState))
                {
                    break;
                } else
                {
                    states.Add(curXState);
                    foreach (Moon m in Moons)
                    {
                        m.ApplyGravity(0);
                    }

                    foreach (Moon m in Moons)
                    {
                        m.Orbit(0);
                    }
                    xSteps++;
                }

            }

            states.Clear();
            int ySteps = 0;
            while (true)
            {
                var curYState = (Moons[0].Position.Y, Moons[1].Position.Y, Moons[2].Position.Y, Moons[3].Position.Y, Moons[0].Velocity.Y, Moons[1].Velocity.Y, Moons[2].Velocity.Y, Moons[3].Velocity.Y);
                if (states.Contains(curYState))
                {
                    break;
                }
                else
                {
                    states.Add(curYState);
                    foreach (Moon m in Moons)
                    {
                        m.ApplyGravity(1);
                    }

                    foreach (Moon m in Moons)
                    {
                        m.Orbit(1);
                    }
                    ySteps++;
                }

            }

            states.Clear();
            int zSteps = 0;
            while (true)
            {
                var curZState = (Moons[0].Position.Z, Moons[1].Position.Z, Moons[2].Position.Z, Moons[3].Position.Z, Moons[0].Velocity.Z, Moons[1].Velocity.Z, Moons[2].Velocity.Z, Moons[3].Velocity.Z);
                if (states.Contains(curZState))
                {
                    break;
                }
                else
                {
                    states.Add(curZState);
                    foreach (Moon m in Moons)
                    {
                        m.ApplyGravity(2);
                    }

                    foreach (Moon m in Moons)
                    {
                        m.Orbit(2);
                    }
                    zSteps++;
                }

            }


            long ans = GetGCF(xSteps, GetGCF(ySteps, zSteps));

            return ans.ToString(); 
        }

        static long GetGCF(long a, long b)
        {
            return (a * b) / GetGCD(a, b);
        }

        static long GetGCD(long a, long b)
        {
            while (a != b)
                if (a < b) b = b - a;
                else a = a - b;
            return (a);
        }

    }


}
