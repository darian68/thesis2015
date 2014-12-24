using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    public class Bone
    {
        private int parent;
        public int Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private double[] direction = new double[3];
        public double[] Direction
        {
            get { return direction; }
            set
            { 
                direction[0] = value[0];
                direction[1] = value[1];
                direction[2] = value[2];
            }
        }
        private double length;
        public double Length
        {
            get { return length; }
            set { length = value; }
        }
        private double axisX, axisY, axisZ;
        public double AxisX
        {
            get { return axisX; }
            set { axisX = value; }
        }
        public double AxisY
        {
            get { return axisY; }
            set { axisY = value; }
        }
        public double AxisZ
        {
            get { return axisZ; }
            set { axisZ = value; }
        }
        private double aspX, aspY;
        public double AspX
        {
            get { return aspX; }
            set { aspX = value; }
        }
        public double AspY
        {
            get { return aspY; }
            set { aspY = value; }
        }
        private int dof;
        public int Dof
        {
            get { return dof; }
            set { dof = value; }
        }
        private int dofrx, dofry, dofrz;
        public int Dofrx
        {
            get { return dofrx; }
            set { dofrx = value; }
        }
        public int Dofry
        {
            get { return dofry; }
            set { dofry = value; }
        }
        public int Dofrz
        {
            get { return dofrz; }
            set { dofrz = value; }
        }
        private int doftx, dofty, doftz;
        public int Doftx
        {
            get { return doftx; }
            set { doftx = value; }
        }
        public int Dofty
        {
            get { return dofty; }
            set { dofty = value; }
        }
        public int Doftz
        {
            get { return doftz; }
            set { doftz = value; }
        }
        private int doftl;
        public int Doftl
        {
            get { return doftl; }
            set { doftl = value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double[,] rotParentCurrent = new double[4, 4];
        
        private double rx, ry, rz;
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
        private double tx, ty, tz;
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
        private double tl;
        public double Tl
        {
            get { return tl; }
            set { tl = value; }
        }
        private int[] dofo = new int[8];
        
        public Bone()
        {
            parent = -1;
        }
    }
}
