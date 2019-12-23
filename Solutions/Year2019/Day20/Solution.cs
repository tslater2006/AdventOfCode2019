using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdventOfCode.Solutions.Year2019
{

    class Day20 : ASolution
    {
        int mapHeight;
        int mapWidth;
        char[,] map = null;
        Dictionary<string, Point> Portals = new Dictionary<string, Point>();
        DefaultDictionary<Point, char> PortalLetters = new DefaultDictionary<Point, char>();
        Dictionary<Point, string> PointToGate = new Dictionary<Point, string>();
        Dictionary<string, List<Point>> GateToPoints = new Dictionary<string, List<Point>>();

        DefaultDictionary<Point, bool> InsideGates = new DefaultDictionary<Point, bool>();

        public Day20() : base(20, 2019, "Donut Maze")
        {
            mapHeight = Input.Length;
            mapWidth = Input[0].Length;
        }

        protected override string SolvePartOne()
        {
            ParseMap();
            ProcessGates();
            var ans = WalkMaze(new MazeState() { Location = GateToPoints["AA"][0], Distance = 0, Level = 0 });
            StringBuilder sb = new StringBuilder();
            sb.Append(ans.Distance.ToString());
            sb.AppendLine("AA -> ");
            ans.Gates.ForEach(g => sb.Append(g.Item1).Append(" -> "));
            sb.AppendLine("ZZ");
            return sb.ToString();
        }

        MazeState WalkMaze(MazeState state, bool recursive = false)
        {
            Queue<MazeState> ToProcess = new Queue<MazeState>();
            HashSet<(Point, int)> visited = new HashSet<(Point, int)>();

            ToProcess.Enqueue(state);
            MazeState current;
            while (ToProcess.Count > 0)
            {
                current = ToProcess.Dequeue();
                visited.Add((current.Location, current.Level));

                if (PointToGate.ContainsKey(current.Location))
                {
                    if (PointToGate[current.Location].Equals("ZZ") && current.Level == 0)
                    {
                        return current;
                    }
                    else
                    {

                        /* jump the gate and add + 1 to our distance */
                        var otherGatePoint = GateToPoints[PointToGate[current.Location]].Where(p => p.Equals(current.Location) == false).FirstOrDefault();
                        if (otherGatePoint.X != 0 && otherGatePoint.Y != 0)
                        {
                            /* have we been to the other side of the jump before? */
                            (Point, int) otherSide = (otherGatePoint, current.Level);
                            if (recursive)
                            {
                                otherSide.Item2 += (InsideGates[current.Location] ? 1 : -1);
                            }

                            if (visited.Contains(otherSide) == false)
                            {
                                if (GateOpen(current, otherGatePoint, recursive))
                                {
                                    var nextState = new MazeState() { Location = otherGatePoint, Distance = current.Distance + 1, Level = otherSide.Item2 };
                                    nextState.Gates.AddRange(current.Gates);
                                    nextState.Gates.Add((PointToGate[current.Location], otherSide.Item2));
                                    ToProcess.Enqueue(nextState);
                                    continue;
                                }
                            }
                        }


                    }
                }
                var nextPoints = current.Location.Around(0, 0, mapWidth - 1, mapHeight - 1).Where(p => map[p.Y, p.X] == '.' && visited.Contains((p, current.Level)) == false).ToList();

                foreach (var p in nextPoints)
                {
                    ToProcess.Enqueue(new MazeState() { Location = p, Distance = current.Distance + 1, Level = current.Level , Gates = current.Gates});
                }
            }
            return null;

        }

        bool GateOpen(MazeState state, Point to, bool recursive)
        {
            if (!recursive)
            {
                return true;
            } else
            {
                var otherGateName = PointToGate[to];

                if (otherGateName == "AA" || otherGateName == "ZZ") {
                    /* AA and ZZ only open on level 0 */
                    return state.Level == 0;
                } else
                {
                    /* some other gate.. */
                    if (state.Level == 0 && InsideGates[state.Location] == false)
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                }
            }
        }

        void ParseMap()
        {
            map = new char[mapHeight, mapWidth];

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    char curChar = Input[y][x];
                    if (curChar == ' ')
                    {
                        map[y, x] = '#';
                    }
                    if (curChar >= 'A' && curChar <= 'Z')
                    {

                        if (new Point(x, y).Around().Select(p => PortalLetters.ContainsKey(p) ? 1 : 0).Sum() == 0)
                        {
                            PortalLetters.Add(new Point(x, y), curChar);
                        }
                        map[y, x] = curChar;
                    }
                    else
                    {
                        map[y, x] = curChar;
                    }
                }
            }
        }

        void PrintMap()
        {
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    Console.Write(map[y, x].ToString());
                }
                Console.WriteLine();
            }
        }

        int CleanDeadEnds()
        {
            int height = map.GetLength(0);
            int width = map.GetLength(1);
            int deadEndCount = 0;
            /* Fill dead ends */
            Queue<Point> pointsToBackfill = new Queue<Point>();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Point p = new Point(x, y);
                    if (map[y, x] == '#')
                    {
                        continue;
                    }
                    var wallsAround = p.Around(0, 0, width - 1, height - 1).Count(p => map[p.Y, p.X] == '#');
                    if (map[y, x] == '.' && wallsAround == 3)
                    {
                        deadEndCount++;
                        map[y, x] = '#';
                        pointsToBackfill.Enqueue(p);

                        while (pointsToBackfill.Count > 0)
                        {
                            Point pz = pointsToBackfill.Dequeue();
                            var opens = pz.Around(0, 0, width - 1, height - 1).Where(z => map[z.Y, z.X] == '.').Where(q => q.Around(0, 0, width - 1, height - 1).Count(p => map[p.Y, p.X] == '#') == 3).ToList();
                            if (opens.Count == 1)
                            {
                                deadEndCount++;
                                map[opens[0].Y, opens[0].X] = '#';
                                pointsToBackfill.Enqueue(opens[0]);
                            }
                        }

                    }
                }
            }



            return deadEndCount;
        }

        void ProcessGates()
        {
            foreach(var kvp in PortalLetters)
            {
                /* this is the 'start' of a gate */
                var otherPoint = kvp.Key.Around(0, 0, mapWidth - 1, mapHeight - 1).Where(p => map[p.Y,p.X] >= 'A' && map[p.Y,p.X] <= 'Z').First();

                var diff = otherPoint.Subtract(kvp.Key);

                var entrance = otherPoint.Add(diff);
                if (entrance.X >= mapWidth || entrance.Y >= mapHeight || map[entrance.Y,entrance.X] != '.' )
                {
                    /* go the other direction... */
                    entrance = kvp.Key.Subtract(diff);
                }

                string gateName = new string(new char[] { map[kvp.Key.Y, kvp.Key.X], map[otherPoint.Y, otherPoint.X] });
                PointToGate.Add(entrance, gateName) ;

                if (GateToPoints.ContainsKey(gateName) == false)
                {
                    GateToPoints.Add(gateName, new List<Point>());
                }

                GateToPoints[gateName].Add(entrance);

                if (entrance.Y > 3 && entrance.Y < mapHeight - 3 && entrance.X > 3 && entrance.X < mapWidth - 3)
                {
                    InsideGates.Add(entrance, true);
                }

            }
        }
        protected override string SolvePartTwo()
        {
            MazeState ans = WalkMaze(new MazeState() { Location = GateToPoints["AA"][0], Distance = 0, Level = 0 },true);

            StringBuilder sb = new StringBuilder();
            sb.Append(ans.Distance.ToString());
            sb.AppendLine();
            sb.Append("(AA,0) -> ");
            ans.Gates.ForEach(g => sb.Append("(").Append(g.Item1).Append(",").Append(g.Item2.ToString()).Append(") -> "));
            sb.AppendLine("(ZZ,0)");

            return sb.ToString();
        }
    }


    class MazeState
    {
        public Point Location;
        public int Distance;
        public int Level;
        public List<(string,int)> Gates = new List<(string,int)>();
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()) == false)
            {
                return false;
            }

            var cast = (MazeState)obj;
            return cast.Location.Equals(this.Location) && cast.Distance == Distance && cast.Level == Level;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine<Point, int, int>(Location, Distance, Level);
        }

    }
}
