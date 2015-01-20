using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    class Transform
    {
        /* Compute transpose of a matrix
        Input: matrix  a
        Output: matrix b = Transpose(a)
        */
        public void MatrixTranspose(double[,] a, double[,] b)
        {
            int i, j;
            for (i = 0; i < 4; i++)
            {
                for (j= 0; j < 4; j++)
                {
                    b[i, j] = a[j, i];
                }
            }
        }
        /* Transform the point (x,y,z) by the matrix m, which is
        assumed to be affine (last row 0 0 0 1) 
        this is just a matrix-vector multiply 
        */
        public void MatrixTransformAffine(double[,] m, double x, double y, double z, double[] pt)
        {
            pt[0] = m[0, 0] * x + m[0, 1] * y + m[0, 2] * z + m[0, 3];
            pt[1] = m[1, 0] * x + m[1, 1] * y + m[1, 2] * z + m[1, 3];
            pt[2] = m[2, 0] * x + m[2, 1] * y + m[2, 2] * z + m[2, 3];
        }
        /* cross product of two vectors: c = a x b */
        public void V3Cross(double[] a, double[] b, double[] c)
        {
            c[0] = a[1] * b[2] - a[2] * b[1];
            c[1] = a[2] * b[0] - a[0] * b[2];
            c[2] = a[0] * b[1] - a[1] * b[0];
        }
        public double V3Dot(double[] a, double[] b)
        {
            return (a[0] * b[0] + a[1] * b[1] + a[2] * b[2]);
        }
        public double V3Mag(double[] a)
        {
            return (Math.Sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]));
        }
        public void RotationZ(double[,] r , double a)
        {
            a = a * Constants.PI/180.0;
            r[0, 0] = Math.Cos(a);
            r[0, 1] = -Math.Sin(a);
            r[0, 2] = 0;
            r[0, 3] = 0;

            r[1, 0] = Math.Sin(a);
            r[1, 1] = Math.Cos(a);
            r[1, 2] = 0;
            r[1, 3] = 0;

            r[2, 0] = 0;
            r[2, 1] = 0;
            r[2, 2] = 1;
            r[2, 3] = 0;

            r[3, 0] = 0;
            r[3, 1] = 0;
            r[3, 2] = 0;
            r[3, 3] = 1;
        }
        public void RotationY(double[,] r, double a)
        {
            a = a * Constants.PI / 180.0;
            r[0, 0] = Math.Cos(a);
            r[0, 1] = 0;
            r[0, 2] = Math.Sin(a);
            r[0, 3] = 0;

            r[1, 0] = 0;
            r[1, 1] = 1;
            r[1, 2] = 0;
            r[1, 3] = 0;

            r[2, 0] = -Math.Sin(a);
            r[2, 1] = 0;
            r[2, 2] = Math.Cos(a);
            r[2, 3] = 0;

            r[3, 0] = 0;
            r[3, 1] = 0;
            r[3, 2] = 0;
            r[3, 3] = 1;
        }
        public void RotationX(double[,] r, double a)
        {
            a = a * Constants.PI / 180.0;
            a = a * Constants.PI / 180.0;
            r[0, 0] = 1;
            r[0, 1] = 0;
            r[0, 2] = 0;
            r[0, 3] = 0;

            r[1, 0] = 0;
            r[1, 1] = Math.Cos(a);
            r[1, 2] = -Math.Sin(a);
            r[1, 3] = 0;

            r[2, 0] = 0;
            r[2, 1] = Math.Sin(a);
            r[2, 2] = Math.Cos(a);
            r[2, 3] = 0;

            r[3, 0] = 0;
            r[3, 1] = 0;
            r[3, 2] = 0;
            r[3, 3] = 1;
        }
        void MatrixMult(double[,] a, double[,] b, double[,] c)
        {
            int i, j, k;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    c[i, j] = 0;
                    for (k = 0; k < 4; k++)
                        c[i, j] += a[i, k] * b[k, j];
                }
        }
        /*
        Rotate vector v by a, b, c in X,Y,Z order.
        v_out = Rz(c)*Ry(b)*Rx(a)*v_in
        */
        void VectorRotationXYZ(double[] v, double a, double b, double c)
        {
            double[,] Rx = new Double[4, 4];
            double[,] Ry = new Double[4, 4];
            double[,] Rz = new Double[4, 4];
            //Rz is a rotation matrix about Z axis by angle c, same for Ry and Rx
            RotationZ(Rz, c);
            RotationY(Ry, b);
            RotationX(Rx, a);
            //Matrix vector multiplication to generate the output vector v.
            MatrixTransformAffine(Rz, v[0], v[1], v[2], v);
            MatrixTransformAffine(Ry, v[0], v[1], v[2], v);
            MatrixTransformAffine(Rx, v[0], v[1], v[2], v);
        }
        //get the angle from vector v1 to vector v2 around the axis
        double GetAngle(double[] v1, double[] v2, double[] axis)
        {
            double dot_prod = V3Dot(v1, v2);
            double r_axis_len = V3Mag(axis);

            double theta = Math.Atan2(r_axis_len, dot_prod);

            return theta;
        }
    }
}
