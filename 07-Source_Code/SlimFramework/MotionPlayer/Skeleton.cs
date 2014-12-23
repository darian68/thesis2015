using System;
using System.Collections.Generic;
using System.IO;
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
            set 
            { 
                rootPosition[0] = value[0];
                rootPosition[1] = value[1];
                rootPosition[2] = value[2];
            }
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
        private Bone rootBone;
        public Bone RootBone
        {
            get { return rootBone; }
            set { rootBone = value; }
        }
        private Bone[] listBone = new Bone[Constants.MAX_BONE];
        public Bone GetBoneByIdx(int index)
        {
            for (int i = 0; i < listBone.Length; i++)
            {
                if (listBone[i].Idx == index) return listBone[i];
            }
            return null;
        }
        public int NumberBoneInSkel(Bone bone)
        {
            return listBone.Length;
        }
        public int Name2Idx(string name)
        {
            for (int i = 0; i < listBone.Length; i++)
            {
                if (string.Compare(listBone[i].Name, name) == 0)
                {
                    return listBone[i].Idx;
                }
            }
            return 0;
        }
        public string Idx2Name(int idx)
        {
            for (int i = 0; i < listBone.Length; i++)
            {
                if (listBone[i].Idx == idx) return listBone[i].Name;
            }
            return String.Empty;
        }
        public int ReadASFFile(string fileName, double scale)
        {
            StreamReader s = File.OpenText(fileName);
            string read = null;
            while ((read = s.ReadLine()) != null)
            {
                Console.WriteLine(read);
                read = read.Trim();
                if (String.Compare(read, ":bonedata") == 0)
                {
                    break;
                }
            }
            // 
            bool done = false;
            // loop for all bone
            // exit when hit :hierarchy
            string keyword = String.Empty;
            string data = String.Empty;
            string[] stringArray = new String[4];
            double[] doubleArray = new Double[3];
            for (int i = 1; (!done) && (i < Constants.MAX_BONE); i++ )
            {
                listBone[i] = new Bone();
                listBone[i].Dof = 0;
                listBone[i].Dofrx = 0;
                listBone[i].Dofry = 0;
                listBone[i].Dofrz = 0;
                listBone[i].Doftx = 0;
                listBone[i].Dofty = 0;
                listBone[i].Doftz = 0;
                listBone[i].Doftl = 0;
                listBone[i].Sibling = 0;
                listBone[i].Child = 0;
                // Loop for each bone
                while (true)
                {
                    read = s.ReadLine().Trim();
                    if (String.Compare(read, "begin") == 0)
                    {
                        continue;
                    }
                    if (String.Compare(read, "end") == 0)
                    {
                        break;
                    }
                    if (String.Compare(read, ":hierarchy") == 0)
                    {
                        // Stop here
                        done = true;
                        break;
                    }
                    keyword = read.Substring(0, read.IndexOf(" "));
                    data = read.Substring(read.IndexOf(" "), read.Length - keyword.Length).Trim();
                    // id of bone
                    if (String.Compare(keyword, "id") == 0)
                    {
                        listBone[i].Idx = Int32.Parse(data);
                    }
                    // name of bone
                    if (String.Compare(keyword, "name") == 0)
                    {
                        
                        listBone[i].Name = data;
                    }
                    // Direction
                    if (String.Compare(keyword, "direction") == 0)
                    {
                        stringArray = data.Split(' ');
                        doubleArray[0] = double.Parse(stringArray[0]);
                        doubleArray[1] = double.Parse(stringArray[1]);
                        doubleArray[2] = double.Parse(stringArray[2]);
                        listBone[i].Direction = doubleArray;
                    }
                    // length
                    if (String.Compare(keyword, "length") == 0)
                    {
                        listBone[i].Length = double.Parse(data);
                    }
                    // axis
                    if (String.Compare(keyword, "axis") == 0)
                    {
                        stringArray = data.Split(' ');
                        listBone[i].AxisX = double.Parse(stringArray[0]);
                        listBone[i].AxisY = double.Parse(stringArray[1]);
                        listBone[i].AxisZ = double.Parse(stringArray[2]);

                    }
                    // dof
                    if (String.Compare(keyword, "dof") == 0)
                    {
                        stringArray = data.Split(' ');
                        for (int j= 0; j < stringArray.Length; j++)
                        {
                            if (String.Compare(stringArray[j], "rx") == 0) {
                                listBone[i].Dofrx = 1;
                            } else if (String.Compare(stringArray[j], "ry") == 0) {
                                listBone[i].Dofry = 1;
                            } else if (String.Compare(stringArray[j], "rz") == 0) {
                                listBone[i].Dofrz = 1;
                            } else if (String.Compare(stringArray[j], "tx") == 0) {
                                listBone[i].Doftx = 1;
                            } else if (String.Compare(stringArray[j], "ty") == 0) {
                                listBone[i].Dofty = 1;
                            } else if (String.Compare(stringArray[j], "tz") == 0) {
                                listBone[i].Doftz = 1;
                            } else if (String.Compare(stringArray[j], "l") == 0) {
                                listBone[i].Doftl = 1;
                            }
                        }
                    }
                }
            }
            s.Close();
            return 0;
        }
    }
}
