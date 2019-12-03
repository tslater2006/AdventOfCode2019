using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day03 : ASolution {
        List<Point> Wire1 = null;
        List<Point> Wire2 = null;
        public Day03() : base(3, 2019, "Crossed Wires")
        {
            Wire1 = ParseWire(Input[0]);
            Wire2 = ParseWire(Input[1]);
        }

        internal List<Point> ParseWire (string path) {
            var directions = path.Split(',');
            List<Point> newWire = new List<Point>();

            newWire.Add(new Point(0, 0));

            foreach (var direction in directions)
            {
                var currentPoint = newWire.Last();
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
                    newWire.Add(currentPoint);
                }
            }

            return newWire;
        }

        protected override string SolvePartOne() {

            var intersects = Wire1.Skip(1).Intersect(Wire2.Skip(1));
            
            var closest = intersects.OrderBy(p => Math.Abs(p.X) + Math.Abs(p.Y)).First();
            
            var distance = Math.Abs(closest.X) + Math.Abs(closest.Y); 

            return distance.ToString(); 
        }

        protected override string SolvePartTwo() {

            var intersects = Wire1.Skip(1).Intersect(Wire2.Skip(1));

            var closestWalk = intersects.Select(p => Wire1.IndexOf(p) + Wire2.IndexOf(p)).OrderBy(i => i).First();

            return closestWalk.ToString(); 
        }
    }
}
