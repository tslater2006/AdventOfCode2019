using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day03 : ASolution {
        Dictionary<Point,int> Wire1 = null;
        Dictionary<Point,int> Wire2 = null;

        public Day03() : base(3, 2019, "Crossed Wires")
        {
            Wire1 = ParseWire(Input[0]);
            Wire2 = ParseWire(Input[1]);
        }

        internal Dictionary<Point,int> ParseWire (string path) {
            var directions = path.Split(',');
            Dictionary<Point, int> newWire = new Dictionary<Point, int>();
            int stepCount = 0;
            Point currentPoint = new Point(0, 0);
            
            foreach (var direction in directions)
            {
                var amount = int.Parse(direction.Substring(1));
                int xOffset = 0;
                int yOffset = 0;

                switch (direction[0])
                {
                    case 'U':
                        yOffset = -1;
                        break;
                    case 'D':
                        yOffset = 1;
                        break;
                    case 'L':
                        xOffset = -1;
                        break;
                    case 'R':
                        xOffset = 1;
                        break;
                }

                for (var x = 0; x < amount; x++)
                {
                    currentPoint = new Point(currentPoint.X + xOffset, currentPoint.Y + yOffset);
                    if (newWire.ContainsKey(currentPoint) == false)
                    {
                        newWire.Add(currentPoint, stepCount++);
                    }
                }
            }

            return newWire;
        }

        protected override string SolvePartOne() {

            var intersects = Wire1.Keys.Intersect(Wire2.Keys);
            
            var closest = intersects.OrderBy(p => Math.Abs(p.X) + Math.Abs(p.Y)).First();
            
            var distance = Math.Abs(closest.X) + Math.Abs(closest.Y); 

            return distance.ToString(); 
        }

        protected override string SolvePartTwo() {

            var intersects = Wire1.Keys.Intersect(Wire2.Keys);
            var closestWalk = intersects.Select(p => Wire1[p] + Wire2[p]).OrderBy(i => i).First();

            return closestWalk.ToString(); 
        }
    }
}
