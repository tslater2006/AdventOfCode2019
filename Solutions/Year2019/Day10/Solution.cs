using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day10 : ASolution {

        bool[,] AsteroidMap = null;

        int Rows;
        int Cols;
        int BestR = 0, BestC = 0;
        public Day10() : base(10, 2019, "") {

            Rows = Input.Length;
            Cols = Input[0].Length;

            AsteroidMap = new bool[Rows, Cols];

            for (var r = 0; r < Rows; r++)
            {
                var row = Input[r];

                for (var c = 0; c < Cols; c++)
                {
                    AsteroidMap[r, c] = row[c] == '#';
                }
            }
        }

        public int GetVisibleCount(bool[,] OriginalMap, int col, int row)
        {
            var tempMap = (bool[,])OriginalMap.Clone();
            int visibleCount = 0;
            HashSet<(int,int)> SeenSlopes = new HashSet<(int,int)>();
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    if (col == c && row == r)
                    {
                        continue;
                    }
                    if (OriginalMap[r,c])
                    {
                        var slope = GetSlope(row, col, r, c);
                        if (SeenSlopes.Contains(slope) == false)
                        {
                            visibleCount++;
                            SeenSlopes.Add(slope);
                        }
                    }

                }
            }

            return visibleCount;
        }

        private (int,int) GetSlope(int row1, int col1, int row2, int col2)
        {
            var rise = row1 - row2;
            var run = col1 - col2;

            if (rise != 0 && run != 0)
            {
                /* try to reduce... */
                var gcd = GetGCD(Math.Abs(rise), Math.Abs(run));
                if (gcd > 1)
                {
                    rise = rise / gcd;
                    run = run / gcd;
                }
            } else
            {
                if (rise == 0 && run > 0)
                {
                    return (0, 1);
                } else if (rise == 0 && run < 0)
                {
                    return (0, -1);
                } else if (rise > 0 && run == 0)
                {
                    return (1, 0);
                }
                else if (rise < 0 && run == 0)
                {
                    return (-1, 0);
                }
            }

            return (rise, run);
        }


        static int GetGCD(int a, int b)
        {
            while (a != b)
                if (a < b) b = b - a;
                else a = a - b;
            return (a);
        }

        protected override string SolvePartOne() {

            int maxSeen = 0;
            int maxR = 0;
            int maxC = 0;

            for(var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    if (AsteroidMap[r,c])
                    {
                        var visibleCount = GetVisibleCount(AsteroidMap, c, r);
                        if (visibleCount > maxSeen)
                        {
                            maxSeen = visibleCount;
                            maxR = r;
                            maxC = c;
                        }
                    }
                }
            }
            BestR = maxR;
            BestC = maxC;
            return maxSeen.ToString(); 
        }
        protected double Dist(int y1, int x1, int y2, int x2)
        {
            return (Math.Sqrt(Math.Pow(x2 - x1,2) + Math.Pow(y2 - y1,2)));
        }
        protected Dictionary<(int, int), (int, int)> GetVisibleAsteroids(bool[,] AsteroidMap, int row, int col)
        {
            int visibleCount = 0;
            Dictionary<(int, int), (int, int)> VisibleAsteroids = new Dictionary<(int, int), (int, int)>();
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    if (col == c && row == r)
                    {
                        continue;
                    }
                    if (AsteroidMap[r, c])
                    {
                        var slope = GetSlope(row, col, r, c);

                        if (VisibleAsteroids.ContainsKey(slope))
                        {
                            /* determine which is closer */
                            if (Dist(row,col, r,c) < Dist(row,col,VisibleAsteroids[slope].Item1, VisibleAsteroids[slope].Item2))
                            {
                                VisibleAsteroids[slope] = (r, c);
                            }
                        } else
                        {
                            VisibleAsteroids.Add(slope, (r, c));
                        }
                    }
                }
            }
            return VisibleAsteroids;
        }

        protected override string SolvePartTwo() {

            var asteroids = GetVisibleAsteroids(AsteroidMap, BestR, BestC);

            /* asteroids has Key (rise,run) and value (row,col) */

            List<(double, int, int)> flatList = new List<(double, int, int)>();
            /* list items are (rise,run,row,col) */
            foreach(var a in asteroids)
            {
                flatList.Add((Math.Atan2(a.Key.Item2, a.Key.Item1), a.Value.Item1, a.Value.Item2));
            }

            /* sort by atan2 of run (x), and rise (y) */
            flatList = flatList.OrderBy(f => f.Item1).ToList();
            var startIndex = 0;
            for (var x = 0; x < flatList.Count; x++)
            {
                if (flatList[x].Item1 >= 0)
                {
                    startIndex = x;
                    break;
                }
            }

            var leftOver = 200 - startIndex;

            var item = flatList[flatList.Count - (leftOver) + 1];

            var ans = item.Item3 * 100 + item.Item2;

            return ans.ToString(); 
        }
    }
}
