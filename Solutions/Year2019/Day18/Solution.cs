using AdventOfCode.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2019 {

    class Day18 : ASolution {
        char[,] map;
        int mapWidth = 0;
        int mapHeight = 0;
        bool trackCollected = false;
        DefaultDictionary<char, Point> keyCoords = new DefaultDictionary<char, Point>();

        /* 2d map that key off 2 keys, and return their distance */
        DefaultDictionary<char, DefaultDictionary<char, int>> keyDist = new DefaultDictionary<char, DefaultDictionary<char, int>>();

        /* 2d map that key off 2 keys, and return their keymask (required doors) */
        DefaultDictionary<char, DefaultDictionary<char, int>> keyDoors = new DefaultDictionary<char, DefaultDictionary<char, int>>();

        public Day18() : base(18, 2019, "Many-Worlds Interpretation") {
        }

        void ParseMap()
        {
            mapWidth = Input[0].Length;
            mapHeight = Input.Length;
            keyCoords.Clear();
            map = new char[mapHeight, mapWidth];

            for (var y = 0; y < mapHeight; y++)
            {
                var chars = Input[y].ToCharArray();
                for (var x = 0; x < mapWidth; x++)
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
            possibleStates.Enqueue(startState, startState.DistanceTraveled);
            //possibleStates.AddOrUpdate(startState, startState.DistanceTraveled);
            State workingState = new State();
            long stateCount = 0;
            
            while (possibleStates.TryDequeueMin(out workingState))
            {
                stateCount++;
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
                    newState.BotLocations = (char[])workingState.BotLocations.Clone();
                    newState.BotLocations[k.Item1] = k.Item2;
                    newState.KeyMask = workingState.KeyMask | (1 << (k.Item2 - 'a'));

                    if (trackCollected)
                    {
                        newState.Collected.AddRange(workingState.Collected);
                        newState.Collected.Add((k.Item1, k.Item2));
                    }

                    newState.DistanceTraveled = workingState.DistanceTraveled + keyDist[workingState.BotLocations[k.Item1]][k.Item2];
                    possibleStates.Enqueue(newState, newState.DistanceTraveled);
                    
                }

            }

            /* can't return null, its a struct */
            return new State();
        }

        /* For each key, we're going to calculate distance and required doors to every other key
         * this includes the '@' we allowed in as a key, it gets removed at the end of this function */
        void ProcessKeys(bool multiBot)
        {
            keyDist.Clear();
            keyDoors.Clear();

            foreach (var a in keyCoords.Keys)
            {
                int[,] FloodMap = GetFloodMap(keyCoords[a]);

                foreach(var b in keyCoords.Keys)
                {
                    if (b == a) { continue; }
                    if (multiBot && GetKeyQuadrant(a) != GetKeyQuadrant(b))
                    {
                        continue;
                    }

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
                            p = p.Around(0,0,mapWidth-1,mapHeight).Where(m => map[m.Y, m.X] != '#').OrderBy(n => FloodMap[n.Y, n.X]).First();
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
        }

        int[,] GetFloodMap(Point startPoint)
        {
            int[,] FloodLevels = new int[mapHeight, mapWidth];
            bool[,] Seen = new bool[mapHeight, mapWidth];

            Queue<Point> ToProcess = new Queue<Point>();

            ToProcess.Enqueue(startPoint);
            Seen[startPoint.Y, startPoint.X] = true;
            while (ToProcess.Count > 0)
            {
                var p = ToProcess.Dequeue();

                foreach (var point in p.Around(0, 0, mapWidth - 1, mapHeight - 1))
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



        protected override string SolvePartOne() {
            ParseMap();
            ProcessKeys(false);

            keyCoords.Remove('@');

            var startState = new State() { BotLocations = new char[] { '@' } };

            var ans = FindBestPath(startState);

            /* foreach ((int, char) c in ans.Collected)
            {
                Console.WriteLine("Bot #" + c.Item1 + " collects key: " + c.Item2);
            }*/

            return ans.DistanceTraveled.ToString(); 
        }

        protected override string SolvePartTwo() {
            ParseMap();
            /* we need to adjust the map ... */
            var origin = keyCoords['@'];
            keyCoords.Remove('@');
            /* new bot keys, !, @, $, % */
            map[origin.Y, origin.X] = '#';
            map[origin.Y, origin.X + 1] = '#';
            map[origin.Y, origin.X - 1] = '#';
            map[origin.Y + 1, origin.X] = '#';
            map[origin.Y - 1, origin.X] = '#';

            map[origin.Y-1, origin.X-1] = '!';
            map[origin.Y - 1, origin.X + 1] = '@';
            map[origin.Y + 1, origin.X + 1] = '$';
            map[origin.Y + 1, origin.X - 1] = '%';

            keyCoords.Add('!', new Point(origin.X - 1, origin.Y - 1));
            keyCoords.Add('@', new Point(origin.X + 1, origin.Y - 1));
            keyCoords.Add('$', new Point(origin.X + 1, origin.Y + 1));
            keyCoords.Add('%', new Point(origin.X - 1, origin.Y + 1));
            ProcessKeys(true);

            keyCoords.Remove('!');
            keyCoords.Remove('@');
            keyCoords.Remove('$');
            keyCoords.Remove('%');

            var ans = FindBestPath(new State() { BotLocations = new char[] { '!', '@', '$', '%' } });

            /*foreach ((int,char) c in ans.Collected)
            {
                Console.WriteLine("Bot #" + c.Item1 + " collects key: " + c.Item2);
            }*/

            return ans.DistanceTraveled.ToString(); 
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

        List<(int,char)> GetReachableKeys(State curState)
        {
            List<(int, char)> possibleKeys = new List<(int, char)>();

            /* for each robot ... */
            for(var x = 0; x < curState.BotLocations.Length; x++ )
            {
                foreach (var k in keyCoords.Keys.Where(c => (curState.KeyMask & (1 << (c - 'a'))) == 0))
                {
                    if (curState.BotLocations.Length > 1 && GetKeyQuadrant(k) != x)
                    {
                        continue;
                    } else
                    {
                        if ((curState.KeyMask & keyDoors[curState.BotLocations[x]][k]) == keyDoors[curState.BotLocations[x]][k])
                        {
                            possibleKeys.Add((x, k));
                        }
                    }
                }
            }

            return possibleKeys;
        }

        int GetKeyQuadrant(char k)
        {
            switch(k)
            {
                case '!':
                    return 0;
                case '@':
                    return 1;
                case '$':
                    return 2;
                case '%':
                    return 3;
            }

            Point keyCoord = keyCoords[k];

            var isLeft = keyCoord.X < (mapWidth / 2);
            var isTop = keyCoord.Y < (mapHeight / 2);

            if (isLeft && isTop)
            {
                return 0;
            } else if (!isLeft && isTop)
            {
                return 1;
            } else if (!isLeft && !isTop)
            {
                return 2;
            } else if (isLeft && !isTop)
            {
                return 3;
            }

            return -1;

        }

    }

    public class State
    {
        public int KeyMask;
        public int DistanceTraveled;
        public char[] BotLocations;
        public List<(int, char)> Collected = new List<(int, char)>();
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()) == false)
            {
                return false;
            }

            State cast = (State)obj;
            return (cast.BotLocations.SequenceEqual(BotLocations) && KeyMask == cast.KeyMask);
        }

        public override int GetHashCode()
        {
            if (BotLocations.Length == 1)
            {
                return HashCode.Combine<char, int>(BotLocations[0], KeyMask);
            }
            else if (BotLocations.Length == 4)
            {
                return HashCode.Combine<char, char, char, char, int>(BotLocations[0], BotLocations[1], BotLocations[2], BotLocations[3], KeyMask);
            }
            else
            {
                var locationHashCode = ((IStructuralEquatable)this.BotLocations).GetHashCode(EqualityComparer<char>.Default);
                return HashCode.Combine<int, int>(locationHashCode, KeyMask);
            }
        }

    }

}
