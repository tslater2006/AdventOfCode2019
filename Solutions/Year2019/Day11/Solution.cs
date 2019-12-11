using AdventOfCode.Utilities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2019 {

    class Day11 : ASolution {
        IntCodeVM vm = null;
        public Day11() : base(11, 2019, "") {
            vm = new IntCodeVM(Input[0]);
        }

        protected override string SolvePartOne() {

            var points = RunPaintingRobot(0);

            return points.Count.ToString(); 
        }

        protected override string SolvePartTwo() {
            var points = RunPaintingRobot(1);

            var maxX = points.Keys.Max(k => k.X);
            var maxY = points.Keys.Max(k => k.Y);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            for (var y = 0; y < maxY + 1; y++)
            {
                for (var x = 0; x < maxX + 1; x++)
                {
                    var point = new Point(x, y);
                    if (points.ContainsKey(point))
                    {
                        sb.Append(points[point] == 1 ? '█' : ' ');
                    } else
                    {
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString(); 
        }

        protected Dictionary<Point, long> RunPaintingRobot(long startingColor)
        {
            vm.Reset();
            Dictionary<Point, long> points = new Dictionary<Point, long>();
            Point currentPoint = new Point(0, 0);
            points.Add(currentPoint, startingColor);
            int currentDirection = 0; /* 0 is up, 1 is right, 2 is down, 3 is left */

            var lastHaltType = HaltType.HALT_WAITING;

            while (lastHaltType == HaltType.HALT_WAITING)
            {

                var curPanelColor = points.ContainsKey(currentPoint) ? points[currentPoint] : 0;

                vm.WriteInput(curPanelColor);
                lastHaltType = vm.RunProgram();

                /* expect 2 outputs */
                var newColor = vm.PopOutput();
                var rotateDirection = vm.PopOutput();

                if (points.ContainsKey(currentPoint))
                {
                    points[currentPoint] = newColor;
                }
                else
                {
                    points.Add(currentPoint, newColor);
                }


                switch (rotateDirection)
                {
                    case 0:
                        /* rotate left */
                        currentDirection--;
                        if (currentDirection < 0)
                        {
                            currentDirection = 3;
                        }
                        break;
                    case 1:
                        /* rotate right */
                        currentDirection++;
                        if (currentDirection > 3)
                        {
                            currentDirection = 0;
                        }
                        break;
                }

                switch (currentDirection)
                {
                    case 0:
                        currentPoint = new Point(currentPoint.X, currentPoint.Y - 1);
                        break;
                    case 1:
                        currentPoint = new Point(currentPoint.X + 1, currentPoint.Y);
                        break;
                    case 2:
                        currentPoint = new Point(currentPoint.X, currentPoint.Y + 1);
                        break;
                    case 3:
                        currentPoint = new Point(currentPoint.X - 1, currentPoint.Y);
                        break;
                }
            }
            return points;
        }

    }
}
