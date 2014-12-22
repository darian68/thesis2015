using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    class Skeleton
    {
        private double[] rootPosition = new double[3];
        public double[] RootPosition
        {
            get { return rootPosition; }
            set { rootPosition = value; }
        }
        private double tx, ty, tz, rx, ry, rz;
        public double Tx
        {
            get { return tx; }
            set { tx = value; }
        }
        public double Ty
        {
            get { return ty; }
            set { ty = value; }
        }
        public double Tz
        {
            get { return tz; }
            set { tz = value; }
        }
        public double Rx
        {
            get { return rx; }
            set { rx = value; }
        }
        public double Ry
        {
            get { return ry; }
            set { ry = value; }
        }
        public double Rz
        {
            get { return rz; }
            set { rz = value; }
        }
    }
}
