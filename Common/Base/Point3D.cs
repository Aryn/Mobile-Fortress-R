using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Common.Base
{
    public struct Point3D
    {
        public int X, Y, Z;
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(X, Z, -Y);
        }

        public static Point3D operator -(Point3D A, Point3D B)
        {
            return new Point3D(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
        }
        public static Point3D operator +(Point3D A, Point3D B)
        {
            return new Point3D(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
        }

        public static Point3D Zero { get { return new Point3D(0, 0, 0); } }
        public static Point3D One { get { return new Point3D(1, 1, 1); } }
    }
}
