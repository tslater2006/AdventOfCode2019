using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2019 {

    class Day18 : ASolution {
        char[,] map;
        DefaultDictionary<char, Point> keyCoords = new DefaultDictionary<char, Point>();

        /* 2d map that key off 2 keys, and return their distance */
        DefaultDictionary<char, DefaultDictionary<char, int>> keyDist = new DefaultDictionary<char, DefaultDictionary<char, int>>();

        /* 2d map that key off 2 keys, and return their keymask (required doors) */
        DefaultDictionary<char, DefaultDictionary<char, int>> keyDoors = new DefaultDictionary<char, DefaultDictionary<char, int>>();

        public Day18() : base(18, 2019, "") {
            ParseMap();
        }

        void ParseMap()
        {
            int width = Input[0].Length;
            int height = Input.Length;

            map = new char[height, width];

            for (var y = 0; y < height; y++)
            {
                var chars = Input[y].ToCharArray();
                for (var x = 0; x < width; x++)
                {
                    if ((chars[x] >= 'a' && chars[x] <= 'z') || chars[x] == '@')
                    {
                        /* allow for '@' to be a key, so we generate distances and keymasks between the keys and it */
                        keyCoords[chars[x]] = new Point(x, y);
                        map[y, x] = chars[x];
                    }
                    else
                    {
                        map[y, x] = chars[x];
                    }
                }
            }

            var origWalkable = map.Cast<char>().Count(c => c != '#');
            //CleanDeadEnds(); /* used to fill in the dead ends to make flood fill a bit faster, but turns out to not be a big deal... */
            ProcessKeys();

            /* lets make sure everything is in the maps */
            for(var a = 'a'; a <= 'z'; a++)
            {
                for (var b = 'a'; b <= 'z'; b++)
                {
                    if (a == b) { continue; }
                    if (!keyDist.ContainsKey(a) && !keyDist[a].ContainsKey(b))
                    {
                        keyDist[a][b] = 0;
                        keyDist[b][a] = 0;
                    }
                    if (!keyDoors.ContainsKey(a) && !keyDoors[a].ContainsKey(b))
                    {
                        keyDoors[b][a] = 0;
                    }
                }
            }

        }
        
        string KeyMaskToString(int keyMask)
        {
            StringBuilder sb = new StringBuilder();
            int bitNumber = 0;
            while (keyMask > 0)
            {
                if (keyMask %2 == 1)
                {
                    sb.Append((char)(65 + bitNumber) + ", ");
                }
                keyMask >>= 1;
                bitNumber++;
            }

            return (string)sb.ToString();
        }

        State FindBestPath(State startState)
        {
            HashSet<State> seenStates = new HashSet<State>();
            PriorityQueue<State> possibleStates = new PriorityQueue<State>();
            var totalMask = (1 << keyCoords.Keys.Count) - 1;
            possibleStates.AddOrUpdate(startState, startState.DistanceTraveled);
            State workingState = new State();
            while (possibleStates.TryDequeueMin(out workingState))
            {
                if (workingState.KeyMask == totalMask)
                {
                    return workingState;
                }

                if (seenStates.Contains(workingState))
                {
                    continue;
                } else
                {
                    seenStates.Add(workingState);
                }

                /* not gotten all of the keys yet...*/
                foreach(var k in GetReachableKeys(workingState))
                {
                    var newState = new State();
                    newState.CurSpot = k;
                    newState.KeyMask = workingState.KeyMask | (1 << (k - 'a'));
                    newState.DistanceTraveled = workingState.DistanceTraveled + keyDist[workingState.CurSpot][k];
                    
                    possibleStates.AddOrUpdate(newState, newState.DistanceTraveled);
                    
                }

            }

            /* can't return null, its a struct */
            return new State();
        }

        /* For each key, we're going to calculate distance and required doors to every other key
         * this includes the '@' we allowed in as a key, it gets removed at the end of this function */
        void ProcessKeys()
        {
            foreach(var a in keyCoords.Keys)
            {
                int[,] FloodMap = GetFloodMap(keyCoords[a]);

                foreach(var b in keyCoords.Keys)
                {
                    if (b == a) { continue; }
                    bool copiedDist = false;
                    bool copiedDoors = false;

                    if ((!keyDist.ContainsKey(a) && !keyDist[a].ContainsKey(b)) && keyDist.ContainsKey(b) && keyDist[b].ContainsKey(a))
                    {
                        copiedDist = true;
                        keyDist[a][b] = keyDist[b][a];
                    }

                    if ((!keyDoors.ContainsKey(a) && !keyDoors[a].ContainsKey(b)) && keyDoors.ContainsKey(b) && keyDoors[b].ContainsKey(a))
                    {
                        copiedDoors = true;
                        keyDoors[a][b] = keyDoors[b][a];
                    }

                    if (copiedDist == false)
                    {
                        /* we haven't found this yet... */
                        keyDist[a][b] = FloodMap[keyCoords[b].Y, keyCoords[b].X];
                    }

                    if (copiedDoors == false)
                    {
                        /* start at destination and walk backwards to the source, tracking the doors */
                        Point p = keyCoords[b];
                        var dest = p;
                        while (p != keyCoords[a])
                        {
                            /* walks backwards to a lower cell, until  we get to our source */
                            p = p.Around(0,0,FloodMap.GetLength(1)-1,FloodMap.GetLength(0)).Where(m => map[m.Y, m.X] != '#').OrderBy(n => FloodMap[n.Y, n.X]).First();
                            var mapChar = map[p.Y, p.X];
                            if (mapChar >= 'A' && mapChar <= 'Z')
                            {
                                /* found a door! */
                                var curMask = keyDoors[a][b];
                                curMask |= (1 << (mapChar - 'A'));
                                keyDoors[a][b] = curMask;
                            }
                        }
                        //Console.WriteLine($"{a} <-> {b}: {KeyMaskToString(keyDoors[a][b])}");
                    }
                }
            }

            /* remove the '@' key that was added */
            keyCoords.Remove('@');

        }

        int[,] GetFloodMap(Point startPoint)
        {
            int[,] FloodLevels = new int[map.GetLength(0), map.GetLength(1)];
            bool[,] Seen = new bool[map.GetLength(0), map.GetLength(1)];

            Queue<Point> ToProcess = new Queue<Point>();

            ToProcess.Enqueue(startPoint);
            Seen[startPoint.Y, startPoint.X] = true;
            while (ToProcess.Count > 0)
            {
                var p = ToProcess.Dequeue();

                foreach (var point in p.Around(0, 0, FloodLevels.GetLength(1) - 1, FloodLevels.GetLength(0) - 1))
                {
                    if (map[point.Y, point.X] != '#')
                    {
                        if (Seen[point.Y, point.X] == false)
                        {
                            FloodLevels[point.Y, point.X] = FloodLevels[p.Y, p.X] + 1;
                            ToProcess.Enqueue(point);
                            Seen[point.Y, point.X] = true;
                        }
                    }
                }

            }
            return FloodLevels;
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


        protected override string SolvePartOne() {
            var startState = new State() { CurSpot = '@' };

            var ans = FindBestPath(startState);
            return ans.DistanceTraveled.ToString(); 
        }

        protected override string SolvePartTwo() {
            return null; 
        }

        IEnumerable<char> GetReachableKeys(State curState)
        {
            foreach(var k in keyCoords.Keys.Where(c => (curState.KeyMask & (1 << (c - 'a'))) == 0))
            {
                if (k == curState.CurSpot)
                {
                    continue;
                } else
                {
                    if ((curState.KeyMask & keyDoors[curState.CurSpot][k]) == keyDoors[curState.CurSpot][k]) { 
                        yield return k;
                    }
                }
            }
        }
    }

    public struct State
    {
        public int KeyMask;
        public int DistanceTraveled;
        public char CurSpot;
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()) == false)
            {
                return false;
            }

            State cast = (State)obj;
            return (CurSpot == cast.CurSpot && DistanceTraveled == cast.DistanceTraveled && KeyMask == cast.KeyMask);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine<char,int, int>(CurSpot, DistanceTraveled, KeyMask);
        }

    }

}
