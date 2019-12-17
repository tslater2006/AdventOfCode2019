using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2019
{

    enum Direction
    {
        NORTH, SOUTH, EAST, WEST
    }

    class Day17 : ASolution
    {
        IntCodeVM vm;
        Dictionary<Point, long> points = new Dictionary<Point, long>();
        long[,] map;
        Point startLocation;
        Direction startDirection;
        public Day17() : base(17, 2019, "Set and Forget")
        {
            vm = new IntCodeVM(Input[0]);

            vm.RunProgram();

            var outputs = vm.ReadOutputs();
            int curX = 0;
            int curY = 0;
            while (outputs.Count > 0)
            {
                var temp = outputs.Dequeue();
                if (temp == 10)
                {
                    curY++;
                    curX = 0;
                }
                else
                {
                    if (temp == '<' || temp == '^' || temp == 'v' || temp == '>')
                    {
                        switch (temp)
                        {
                            case '^':
                                startDirection = Direction.NORTH;
                                break;
                            case '<':
                                startDirection = Direction.WEST;
                                break;
                            case '>':
                                startDirection = Direction.EAST;
                                break;
                            case 'v':
                                startDirection = Direction.SOUTH;
                                break;
                        }
                        startLocation = new Point(curX, curY);
                    }
                    points.Add(new Point(curX, curY), temp);
                    curX++;
                }
            }
        }

        private int CandidateLength(string candidate)
        {
            return candidate.Length + (OccurenceCount(candidate, "L") * 2) + (OccurenceCount(candidate, "R") * 2) - 1;
        }

        private string[] FactorizeMoves(string moves)
        {
            string originalMoves = moves.Substring(0);
            var parts = Regex.Split(moves, @"([RL]\d+)").Where(s => s != String.Empty).ToArray();

            string[] retVal = new string[4];
            List<string> patterns = new List<string>();

            List<string> candidates = new List<string>();
            /* generate pattern candidates ? */
            for (var x = 0; x < parts.Length - 2; x++)
            {
                for (var y = x + 1; y < parts.Length; y++)
                {
                    string candidate = String.Join("", parts, x, y - x);
                    if (CandidateLength(candidate) <= 20)
                    {
                        if (OccurenceCount(moves, candidate) > 1)
                        {
                            candidates.Add(candidate);
                        }
                    }
                }
            }

            /* get all combinations of 3 that are possible */
            var toTry = Combinations.GetCombinations(candidates.Distinct().ToArray(), 3);

            foreach (var attempt in toTry)
            {
                var sorted = attempt.OrderByDescending(l => l.Length).ToList();
                var test = moves.Replace(sorted[0], "A,").Replace(sorted[1], "B,").Replace(sorted[2], "C,");
                if (test.Contains("L") == false && test.Contains("R") == false)
                {
                    retVal[0] = test.Trim(',');
                    retVal[1] = sorted[0].Replace("R", ",R,").Replace("L", ",L,").Trim(',');
                    retVal[2] = sorted[1].Replace("R", ",R,").Replace("L", ",L,").Trim(',');
                    retVal[3] = sorted[2].Replace("R", ",R,").Replace("L", ",L,").Trim(',');
                    /*Console.WriteLine("Main program: " + retVal[0]);
                    Console.WriteLine("A: " + retVal[1]);
                    Console.WriteLine("B: " + retVal[2]);
                    Console.WriteLine("C: " + retVal[3]);
                    Console.WriteLine();*/
                    int i = 3;
                }
            }

            return retVal;
        }

        public static int OccurenceCount(string str, string val)
        {
            int occurrences = 0;
            int startingIndex = 0;

            while ((startingIndex = str.IndexOf(val, startingIndex)) >= 0)
            {
                ++occurrences;
                startingIndex += val.Length - 1;
                ++startingIndex;
            }

            return occurrences;
        }

        protected override string SolvePartOne()
        {
            long[,] map = DictionaryTo2D(points);

            List<Point> intersections = new List<Point>();

            for (var y = 0; y < map.GetLength(0); y++)
            {
                for (var x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 35)
                    {
                        int trackNeighbors = 0;

                        /* north */
                        Point possible = new Point(x, y - 1);
                        if (points.ContainsKey(possible) && map[possible.Y, possible.X] == 35)
                        {
                            trackNeighbors++;
                        }

                        /* east */
                        possible = new Point(x + 1, y);
                        if (points.ContainsKey(possible) && map[possible.Y, possible.X] == 35)
                        {
                            trackNeighbors++;
                        }

                        /* south */
                        possible = new Point(x, y + 1);
                        if (points.ContainsKey(possible) && map[possible.Y, possible.X] == 35)
                        {
                            trackNeighbors++;
                        }

                        /* west */
                        possible = new Point(x - 1, y);
                        if (points.ContainsKey(possible) && map[possible.Y, possible.X] == 35)
                        {
                            trackNeighbors++;
                        }

                        if (trackNeighbors == 4)
                        {
                            /* its an intersection point */
                            intersections.Add(new Point(x, y));
                            map[y, x] = (long)'X';
                        }
                    }
                }
            }

            /* for (var y = 0; y < map.GetLength(0); y++)
            {
                for (var x = 0; x < map.GetLength(1); x++)
                {
                    switch (map[y, x])
                    {
                        case 35:
                            Console.Write("#");
                            break;
                        case 46:
                            Console.Write(".");
                            break;
                        default:
                            Console.Write((char)map[y, x]);
                            int i = 3;
                            break;
                    }
                }
                Console.WriteLine();
            } */

            intersections = intersections.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
            var ans = intersections.Sum(p => p.X * p.Y);



            return ans.ToString();
        }

        protected override string SolvePartTwo()
        {

            long[,] map = DictionaryTo2D(points);

            string directionString = Walk(map, startLocation, startDirection);

            string[] inputs = FactorizeMoves(directionString);

            /* change the inputs to the format we need for the VM */

            vm = new IntCodeVM(Input[0]);

            /* set it to run mode */
            vm.WriteMemory(0, 2);

            vm.WriteInputString($"{inputs[0]}\n");
            vm.WriteInputString($"{inputs[1]}\n");
            vm.WriteInputString($"{inputs[2]}\n");
            vm.WriteInputString($"{inputs[3]}\n");
            vm.WriteInputString("n\n");

            vm.RunProgram();

            var ans = vm.ReadOutputs().Last().ToString();
            return ans;
        }

        long[,] DictionaryTo2D(Dictionary<Point, long> dict)
        {
            var minX = dict.Keys.Min(k => k.X);
            var maxX = dict.Keys.Max(k => k.X);
            var minY = dict.Keys.Min(k => k.Y);
            var maxY = dict.Keys.Max(k => k.Y);

            var offsetX = Math.Abs(minX);
            var offsetY = Math.Abs(minY);

            var sizeX = maxX + offsetX;
            var sizeY = maxY + offsetY;

            long[,] Maze = new long[sizeY + 1, sizeX + 1];


            foreach (var kvp in dict)
            {
                Maze[kvp.Key.Y + offsetY, kvp.Key.X + offsetX] = kvp.Value;
            }

            return Maze;
        }


        char GetTurnDirection(Direction from, Direction to)
        {
            switch (from)
            {
                case Direction.NORTH:
                    return to == Direction.EAST ? 'R' : 'L';
                case Direction.EAST:
                    return to == Direction.SOUTH ? 'R' : 'L';
                case Direction.SOUTH:
                    return to == Direction.WEST ? 'R' : 'L';
                case Direction.WEST:
                    return to == Direction.NORTH ? 'R' : 'L';

            }
            return ' ';
        }

        string Walk(long[,] map, Point location, Direction direction)
        {
            StringBuilder sb = new StringBuilder();
            int distanceWalked = 0;
            char turnDirection = ' ';
            while (true)
            {
                Point directionOffset = GetOffsetForDirection(direction);
                /* check in front of us */
                Point curPoint = new Point(location.X + directionOffset.X, location.Y + directionOffset.Y);
                if (MapContainsPoint(map, curPoint) && map[curPoint.Y, curPoint.X] == '#')
                {
                    distanceWalked++;
                    location.X += directionOffset.X;
                    location.Y += directionOffset.Y;
                }
                else
                {
                    /* nothing in front, check the other 2 directions */
                    Direction d1 = Direction.NORTH;
                    Direction d2 = Direction.NORTH;

                    switch (direction)
                    {
                        case Direction.NORTH:
                        case Direction.SOUTH:
                            d1 = Direction.WEST;
                            d2 = Direction.EAST;
                            break;
                        case Direction.EAST:
                        case Direction.WEST:
                            d1 = Direction.NORTH;
                            d2 = Direction.SOUTH;
                            break;
                    }

                    directionOffset = GetOffsetForDirection(d1);
                    curPoint = new Point(location.X + directionOffset.X, location.Y + directionOffset.Y);
                    if (MapContainsPoint(map, curPoint) && map[curPoint.Y, curPoint.X] == '#')
                    {

                        if (distanceWalked > 0)
                        {
                            sb.Append(turnDirection.ToString() + distanceWalked);
                        }
                        turnDirection = GetTurnDirection(direction, d1);
                        /* change to direction 1 */
                        direction = d1;
                        distanceWalked = 0;
                        continue;
                    }

                    directionOffset = GetOffsetForDirection(d2);
                    curPoint = new Point(location.X + directionOffset.X, location.Y + directionOffset.Y);
                    if (MapContainsPoint(map, curPoint) && map[curPoint.Y, curPoint.X] == '#')
                    {
                        if (distanceWalked > 0)
                        {
                            sb.Append(turnDirection.ToString() + distanceWalked);
                        }
                        turnDirection = GetTurnDirection(direction, d2);

                        /* change to direction 1 */
                        direction = d2;
                        distanceWalked = 0;
                        continue;
                    }

                    break;

                }
            }

            sb.Append(turnDirection.ToString() + distanceWalked);

            return sb.ToString();

        }



        bool MapContainsPoint(long[,] map, Point p)
        {
            return (p.X >= 0 && p.X < map.GetLength(1) && p.Y >= 0 && p.Y < map.GetLength(0));
        }
        Point GetOffsetForDirection(Direction direction)
        {
            Point directionOffset = new Point(0, 0);
            switch (direction)
            {
                case Direction.NORTH:
                    directionOffset.X = 0;
                    directionOffset.Y = -1;
                    break;
                case Direction.EAST:
                    directionOffset.X = 1;
                    directionOffset.Y = 0;
                    break;
                case Direction.SOUTH:
                    directionOffset.X = 0;
                    directionOffset.Y = 1;
                    break;
                case Direction.WEST:
                    directionOffset.X = -1;
                    directionOffset.Y = 0;
                    break;
            }
            return directionOffset;
        }
    }


}
