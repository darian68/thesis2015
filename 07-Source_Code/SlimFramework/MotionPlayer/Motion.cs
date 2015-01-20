using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    class Motion
    {
        private int numFrames;
        public int NumFrames
        {
            get { return numFrames; }
            set { numFrames = value; }
        }
        private Skeleton skeleton;
        private Posture[] posture;
        public Motion(string amcFileName, double scale, Skeleton ske)
        {
            skeleton = ske;
            numFrames = 0;
            int code = ReadAMCFile(amcFileName, scale);
        }
        public Motion(int numFr, Skeleton ske)
        {
            skeleton = ske;
            numFrames = numFr;
            posture = new Posture[numFr];
        }
        public int ReadAMCFile(string name, double scale)
        {
            StreamReader s = File.OpenText(name);
            string read = null;
            while ((read = s.ReadLine()) != null)
            {

            }
            return 1;
        }
    }
}
