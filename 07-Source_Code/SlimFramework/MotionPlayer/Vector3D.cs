using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    class Vector3D
    {
        // Coordinates
        private double[] p = new double[3];
        public double x
        {
            get
            {
                return p[0];
            }
            protected set
            {
                p[0] = value;
            }
        }
        public double y
        {
            get
            {
                return p[1];
            }
            protected set
            {
                p[1] = value;
            }
        }
        public double z
        {
            get
            {
                return p[2];
            }
            protected set
            {
                p[2] = value;
            }
        }
        public double[] xyz
        {
            get
            {
                return p;
            }
            protected set
            {
                p = value;
            }
        }
        public Vector3D() { }
        public Vector3D(double x, double y, double z)
        {
            p[0] = x;
            p[1] = y;
            p[2] = z;
        }
        public Vector3D(double[] a)
        {
            p[0] = a[0];
            p[1] = a[1];
            p[2] = a[2];
        }
        public double Length()
        {
            return Math.Sqrt(p[0] * p[0] + p[1] * p[1] + p[2] * p[2]);
        }
        // Operators
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            Vector3D c = new Vector3D(a.p[0] + b.p[0], a.p[1] + b.p[1], a.p[2] + b.p[2]);
            return c;
        }
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            Vector3D c = new Vector3D(a.p[0] - b.p[0], a.p[1] - b.p[1], a.p[2] - b.p[2]);
            return c;
        }
        public static Vector3D operator *(Vector3D a, double b)
        {
            Vector3D c = new Vector3D(a.p[0] * b, a.p[1] * b, a.p[2] * b);
            return c;
        }
        public static Vector3D operator /(Vector3D a, double b)
        {
            Vector3D c = new Vector3D(a.p[0] / b, a.p[1] / b, a.p[2] / b);
            return c;
        }
        // cross product - tich co huong
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            Vector3D c = new Vector3D(  a.p[1] * b.p[2] - a.p[2] * b.p[1],
                                        a.p[2] * b.p[0] - a.p[0] * b.p[2],
                                        a.p[0] * b.p[1] - a.p[1] * b.p[0]
                                     );
            return c;
        }
        // dot product
        public static double operator %(Vector3D a, Vector3D b)
        {
            return (a.p[0] * b.p[0] + a.p[1] * b.p[1] + a.p[2] * b.p[2]);
        }
    }
}
