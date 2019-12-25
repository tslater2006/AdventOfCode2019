using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day24 : ASolution {

        bool[,] Eris = null;
        int mapHeight;
        int mapWidth;
        public Day24() : base(24, 2019, "") {
            mapHeight = Input.Length;
            mapWidth = Input[0].Length;
            ParseMap();
        }

        void ParseMap()
        {
            Eris = new bool[mapHeight, mapWidth];
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    Eris[y, x] = (Input[y][x] == '#');
                }
            }
        }

        void PrintMap(bool [,] map)
        {
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    Console.Write((map[y, x] ? '#' : '.').ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        long ErisHash()
        {
            long hash = 0;
            int power = 0;
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    if (Eris[y,x])
                    {
                        hash += (long)Math.Pow(2, power);
                    }
                    power++;
                }
            }

            return hash;
        }

        void ErisEvolve()
        {
            bool[,] newEris = new bool[mapHeight, mapWidth];

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {

                    var curPoint = new Point(x, y);
                    var neighbors = curPoint.Around(0, 0, mapWidth - 1, mapHeight - 1);

                    var bugCount = neighbors.Count(p => Eris[p.Y, p.X]);

                    if (Eris[y, x]) /* currently has a bug */
                    {

                        if (bugCount != 1)
                        {
                            newEris[y, x] = false;
                        } else
                        {
                            newEris[y, x] = true;
                        }
                    } else
                    {
                        if (bugCount == 1 || bugCount == 2)
                        {
                            newEris[y, x] = true;
                        } else
                        {
                            newEris[y, x] = false;
                        }
                    }

                }
            }

            Eris = newEris;
        }

        protected override string SolvePartOne() {
            HashSet<long> seenHashes = new HashSet<long>();
            //PrintMap(Eris);
            var hash = ErisHash();
            seenHashes.Add(hash);
            do
            {
                seenHashes.Add(hash);
                ErisEvolve();
                //PrintMap(Eris);
                hash = ErisHash();
                
            } while (seenHashes.Contains(hash) == false);



            return hash.ToString(); 
        }

        protected override string SolvePartTwo() {
            ParseMap();

            Dictionary<int,bool[,]> Levels = new Dictionary<int, bool[,]>();

            Levels.Add(0, Eris);

            for (var x = 0; x < 200; x++)
            {
                EvolveWithLevels(Levels);
            }
            var totalBugs = 0;
            foreach (var kvp in Levels.OrderBy(kvp => kvp.Key))
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    for (var x = 0; x < mapWidth; x++)
                    {
                        if (kvp.Value[y, x])
                        {
                            totalBugs++;
                        }
                    }
                }
                //Console.WriteLine("Level: " + kvp.Key);
                //PrintMap(kvp.Value);
            }
                
            

            return totalBugs.ToString(); 
        }
        static Point[] outerEdgePoints = new Point[]
        {
                new Point(0,0),
                new Point(0,1),
                new Point(0,2),
                new Point(0,3),
                new Point(0,4),

                new Point(1,0),
                new Point(2,0),
                new Point(3,0),
                new Point(4,0),

                new Point(4,1),
                new Point(2,2),
                new Point(3,3),
                new Point(4,4),

                new Point(1,4),
                new Point(2,4),
                new Point(3,4)
        };

        static Point[] innerEdgePoints = new Point[]
        {
                new Point(1,1),
                new Point(2,1),
                new Point(3,1),
                new Point(3,2),
                new Point(3,3),

                new Point(2,3),
                new Point(1,3),
                new Point(1,2)
        };


        bool HasOuterEdgeBugs (bool[,] level)
        {

            return outerEdgePoints.Count(p => level[p.Y, p.X]) > 0;
        }

        bool HasInnerEdgeBugs(bool[,] level)
        {
            return innerEdgePoints.Count(p => level[p.Y, p.X]) > 0;
        }

        void EvolveWithLevels(Dictionary<int,bool[,]> levels)
        {
            Dictionary<int, bool[,]> newLevels = new Dictionary<int, bool[,]>();
            /* determine if we need to add new levels */
            var minLevel = levels.Keys.Min();
            var maxLevel = levels.Keys.Max();

            if (HasInnerEdgeBugs(levels[minLevel]))
            {
                levels.Add(minLevel - 1, new bool[mapHeight, mapWidth]);
            }

            if (HasOuterEdgeBugs(levels[maxLevel]))
            {
                levels.Add(maxLevel + 1, new bool[mapHeight, mapWidth]);
            }

            foreach(var kvp in levels)
            {
                int curLevel = kvp.Key;
                bool[,] map = kvp.Value;
                bool[,] newMap = new bool[mapHeight, mapWidth];
                for(var y = 0; y < map.GetLength(0); y++)
                {
                    for (var x = 0; x < map.GetLength(1); x++)
                    {
                        if (x == 2 && y == 2)
                        {
                            /* skip the center */
                            continue;
                        }
                        // do stuff here
                        var curPoint = new Point(x, y);
                        int bugsAroundCount = curPoint.Around(0, 0, mapWidth - 1, mapHeight - 1).Count(p => levels[curLevel][p.Y, p.X]);

                        if (levels[curLevel][2,2])
                        {
                            bugsAroundCount--;
                        }

                        var parentLevel = curLevel - 1;

                        /* if we are on the outside border */
                        if (x == 0) /* left column */
                        {
                            if (levels.ContainsKey(parentLevel))
                            {
                                bugsAroundCount += levels[parentLevel][2, 1] == true ? 1 : 0;
                            }

                        }
                        else if (x == mapWidth - 1) /* right column */
                        {
                            if (levels.ContainsKey(parentLevel))
                            {
                                bugsAroundCount += levels[parentLevel][2, 3] == true ? 1 : 0;
                            }
                        }
                        
                        if (y == 0) /* top row */
                        {
                            if (levels.ContainsKey(parentLevel))
                            {
                                bugsAroundCount += levels[parentLevel][1, 2] == true ? 1 : 0;
                            }
                        } else if (y == mapHeight - 1) /* bottom row */
                        {
                            if (levels.ContainsKey(parentLevel))
                            {
                                bugsAroundCount += levels[parentLevel][3, 2] == true ? 1 : 0;
                            }
                        }

                        /* check the inside border */
                        var childLevel = curLevel + 1;

                        if (x == 2 && y == 1) /* top cell inner ring */
                        {
                            /* whole top row of child */
                            if (levels.ContainsKey(childLevel))
                            {
                                Point[] childPoints = new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0) };
                                bugsAroundCount += childPoints.Count(p => levels[childLevel][p.Y, p.X]);
                            }
                        } else if (x == 2 && y == 3) /* bottom cell inner ring */
                        {
                            /* whole bottom row of child */
                            if (levels.ContainsKey(childLevel))
                            {
                                Point[] childPoints = new Point[] { new Point(0, 4), new Point(1, 4), new Point(2, 4), new Point(3, 4), new Point(4, 4) };
                                bugsAroundCount += childPoints.Count(p => levels[childLevel][p.Y, p.X]);
                            }
                        } else if (x == 1 && y == 2) /* left cell of inner ring */
                        {
                            /* left column of child */
                            if (levels.ContainsKey(childLevel))
                            {
                                Point[] childPoints = new Point[] { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(0, 4) };
                                bugsAroundCount += childPoints.Count(p => levels[childLevel][p.Y, p.X]);
                            }
                        }
                        else if (x == 3 && y == 2) /* right cell of inner ring */
                        {
                            /* right column of child */
                            if (levels.ContainsKey(childLevel))
                            {
                                Point[] childPoints = new Point[] { new Point(4, 0), new Point(4, 1), new Point(4, 2), new Point(4, 3), new Point(4, 4) };
                                bugsAroundCount += childPoints.Count(p => levels[childLevel][p.Y, p.X]);
                            }
                        }


                        if (levels[curLevel][y, x]) /* currently has a bug */
                        {

                            if (bugsAroundCount != 1)
                            {
                                newMap[y, x] = false;
                            }
                            else
                            {
                                newMap[y, x] = true;
                            }
                        }
                        else
                        {
                            if (bugsAroundCount == 1 || bugsAroundCount == 2)
                            {
                                newMap[y, x] = true;
                            }
                            else
                            {
                                newMap[y, x] = false;
                            }
                        }

                    }
                }

                newLevels.Add(curLevel,newMap);
            }

            foreach(var kvp in newLevels)
            {
                levels[kvp.Key] = kvp.Value;
            }
        }
    }
}
