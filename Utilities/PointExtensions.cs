using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdventOfCode.Utilities
{
    static class PointExtensions
    {
        public static IEnumerable<Point> Around(this Point p)
        {
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X, p.Y + 1);
            yield return new Point(p.X, p.Y - 1);
        }

        public static Point Add(this Point p, Point p2)
        {
            return new Point(p.X + p2.X, p.Y + p2.Y);
        }

        public static Point Subtract(this Point p, Point p2)
        {
            return new Point(p.X - p2.X, p.Y - p2.Y);
        }

        public static IEnumerable<Point> Around(this Point p, int minX, int minY, int maxX, int maxY)
        {
            if (p.X + 1 >= minX && p.X + 1 <= maxX)
            {
                yield return new Point(p.X + 1, p.Y);
            }
            if (p.X - 1 >= minX && p.X - 1 <= maxX)
            {
                yield return new Point(p.X - 1, p.Y);
            }
            if (p.Y + 1 >= minY && p.Y + 1 <= maxY)
            {
                yield return new Point(p.X, p.Y + 1);
            }
            if (p.Y - 1 >= minY && p.Y - 1 <= maxY)
            {
                yield return new Point(p.X, p.Y - 1);
            }
        }
    }
}
