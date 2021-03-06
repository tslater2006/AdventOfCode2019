using AdventOfCode.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day15 : ASolution {

        Dictionary<Point, long> MappedPoints = new Dictionary<Point, long>();
        long[,] Maze = null;
        long[,] FloodLevels = null;
        bool[,] Seen = null;
        IntCodeVM vm = null;
        
        Point StartPoint;
        Point TargetPoint;

        public Day15() : base(15, 2019, "Oxygen System") {
            vm = new IntCodeVM(Input[0]);
            BuildMaze();
        }

        void BuildMaze()
        {
            Console.WriteLine();
            vm.RunProgram();

            Point origin = new Point(0, 0);
            StartPoint = origin;
            WalkPoint(origin);

            var minX = MappedPoints.Keys.Min(k => k.X);
            var maxX = MappedPoints.Keys.Max(k => k.X);
            var minY = MappedPoints.Keys.Min(k => k.Y);
            var maxY = MappedPoints.Keys.Max(k => k.Y);

            var offsetX = Math.Abs(minX);
            var offsetY = Math.Abs(minY);

            var sizeX = maxX + offsetX;
            var sizeY = maxY + offsetY;

            StartPoint = new Point(StartPoint.X + offsetX, StartPoint.Y + offsetY);
            TargetPoint = new Point(TargetPoint.X + offsetX, TargetPoint.Y + offsetY);

            Maze = new long[sizeY + 1, sizeX + 1];
            FloodLevels = new long[sizeY + 1, sizeX + 1];
            Seen = new bool[sizeY + 1, sizeX + 1];

            foreach (var kvp in MappedPoints)
            {
                Maze[kvp.Key.Y + offsetY, kvp.Key.X + offsetX] = kvp.Value;
            }
            
            /* for (var y = 0; y < sizeY + 1; y++)
            {
                for (var x = 0; x < sizeX + 1; x++)
                {
                    if (StartPoint.X == x && StartPoint.Y == y)
                    {
                        Console.Write(" O ");
                    }
                    else if (TargetPoint.X == x && TargetPoint.Y == y)
                    {
                        Console.Write(FloodLevels[y, x].ToString("D3"));
                    }
                    else
                    {
                        switch (Maze[y, x])
                        {
                            case 1:
                                Console.Write(FloodLevels[y,x].ToString("D3"));
                                break;
                            case 0:
                                Console.Write(" # ");
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }*/
            
        }

        void FloodFill(Point startPoint)
        {
            Stack<Point> ToProcess = new Stack<Point>();

            ToProcess.Push(startPoint);

            while (ToProcess.Count > 0)
            {
                var p = ToProcess.Pop();

                Point[] possiblePoints = new Point[]
                {
                    new Point(p.X, p.Y - 1),
                    new Point(p.X, p.Y + 1),
                    new Point(p.X + 1, p.Y),
                    new Point(p.X - 1, p.Y)
                };

                foreach(var point in possiblePoints)
                {
                    if (point.Equals(TargetPoint))
                    {
                        int i = 3;
                    }
                    if (Maze[point.Y, point.X] != 0)
                    {
                        if (Seen[point.Y, point.X] == false)
                        {
                            FloodLevels[point.Y, point.X] = FloodLevels[p.Y, p.X] + 1;
                            ToProcess.Push(point);
                        }
                    }
                }

                Seen[p.Y, p.X] = true;
            }

        }

        void WalkPoint(Point p)
        {
            /* try north */
            var newPoint = new Point(p.X, p.Y - 1);
            if (MappedPoints.ContainsKey(newPoint) == false)
            {
                var moveResult = MoveInDirection(1);
                MappedPoints.Add(newPoint, moveResult);

                if (moveResult != 0)
                {
                    WalkPoint(newPoint);
                    /* undo the move */
                    MoveInDirection(2);
                    if (moveResult == 2)
                    {
                        MappedPoints[newPoint] = 1;
                        TargetPoint = new Point(newPoint.X, newPoint.Y);
                    }
                }
            }

            /* try east */
            newPoint = new Point(p.X + 1, p.Y);
            if (MappedPoints.ContainsKey(newPoint) == false)
            {
                var moveResult = MoveInDirection(4);
                MappedPoints.Add(newPoint, moveResult);

                if (moveResult != 0)
                {
                    WalkPoint(newPoint);
                    /* undo the move */
                    MoveInDirection(3);
                    if (moveResult == 2)
                    {
                        MappedPoints[newPoint] = 1;
                        TargetPoint = new Point(newPoint.X, newPoint.Y);
                    }
                }
            }

            /* try south */
            newPoint = new Point(p.X, p.Y + 1);
            if (MappedPoints.ContainsKey(newPoint) == false)
            {
                var moveResult = MoveInDirection(2);
                MappedPoints.Add(newPoint, moveResult);

                if (moveResult != 0)
                {
                    WalkPoint(newPoint);
                    /* undo the move */
                    MoveInDirection(1);
                    if (moveResult == 2)
                    {
                        MappedPoints[newPoint] = 1;
                        TargetPoint = new Point(newPoint.X, newPoint.Y);
                    }
                }
            }

            /* try west */
            newPoint = new Point(p.X - 1, p.Y);
            if (MappedPoints.ContainsKey(newPoint) == false)
            {
                var moveResult = MoveInDirection(3);
                MappedPoints.Add(newPoint, moveResult);

                if (moveResult != 0)
                {
                    WalkPoint(newPoint);
                    /* undo the move */
                    MoveInDirection(4);
                    if (moveResult == 2)
                    {
                        MappedPoints[newPoint] = 1;
                        TargetPoint = new Point(newPoint.X, newPoint.Y);
                    }
                }
            }

        }

        long MoveInDirection(int dir)
        {
            vm.WriteInput(dir);
            vm.RunProgram();
            return vm.PopOutput();
        }

        protected override string SolvePartOne() {

            FloodFill(StartPoint);
            return FloodLevels[TargetPoint.Y,TargetPoint.X].ToString(); 
        }

        protected override string SolvePartTwo() {

            /* reset flood fill from part 1 */
            FloodLevels = new long[Maze.GetLength(0),Maze.GetLength(1)];
            Seen = new bool[Maze.GetLength(0), Maze.GetLength(1)];

            FloodFill(TargetPoint);

            var maxFlood = FloodLevels.Cast<long>().Max();

            return maxFlood.ToString(); 
        }
    }
}
