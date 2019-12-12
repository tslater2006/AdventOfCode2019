using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utilities
{
    public class Moon
    {
        public Point3D Position;
        public Point3D Velocity;
        public List<Moon> OrbitSiblings = new List<Moon>();


        public Moon(Point3D startingPosition)
        {
            Position = startingPosition;
            Velocity = new Point3D() { X = 0, Y = 0, Z = 0 };
        }

        public void ApplyGravity()
        {
            foreach(Moon m in OrbitSiblings)
            {
                if (Position.X != m.Position.X)
                {
                    Velocity.X += (Position.X > m.Position.X) ? -1 : 1;
                }
                if (Position.Y != m.Position.Y)
                {
                    Velocity.Y += (Position.Y > m.Position.Y) ? -1 : 1;
                }
                if (Position.Z != m.Position.Z)
                {
                    Velocity.Z += (Position.Z > m.Position.Z) ? -1 : 1;
                }
            }
        }


        public void ApplyGravity(int axis)
        {
            foreach (Moon m in OrbitSiblings)
            {
                switch (axis)
                {
                    case 0:
                        {
                            if (Position.X != m.Position.X)
                            {
                                Velocity.X += (Position.X > m.Position.X) ? -1 : 1;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (Position.Y != m.Position.Y)
                            {
                                Velocity.Y += (Position.Y > m.Position.Y) ? -1 : 1;
                            }
                        }
                        break;
                    case 2:
                        {
                            if (Position.Z != m.Position.Z)
                            {
                                Velocity.Z += (Position.Z > m.Position.Z) ? -1 : 1;
                            }
                        }
                        break;
                }
            }
        }

        public void Orbit(int axis)
        {
            switch(axis)
            {
                case 0:
                    Position.X += Velocity.X;
                    break;
                case 1:
                    Position.Y += Velocity.Y;
                    break;
                case 2:
                    Position.Z += Velocity.Z;
                    break;
            }
        }

        public void Orbit()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Position.Z += Velocity.Z;
        }

        public int TotalEnergy
        {
            get
            {
                return (Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z)) * (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z));
            }
        }

        public override string ToString()
        {
            return $"pos =< x = {Position.X}, y = {Position.Y}, z = {Position.Z} >, vel =< x = {Velocity.X}, y = {Velocity.Y}, z = {Velocity.Z} >";
        }

    }

    public struct Point3D
    {
        public int X;
        public int Y;
        public int Z;
    }

}
