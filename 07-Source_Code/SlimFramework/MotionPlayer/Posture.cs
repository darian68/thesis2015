using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionPlayer
{
    class Posture
    {
        public Vector3D rootPosition;
        public Vector3D[] boneRotation = new Vector3D[Constants.MAX_BONE];
        public Vector3D[] boneTranslation = new Vector3D[Constants.MAX_BONE];
        public Vector3D[] boneLength = new Vector3D[Constants.MAX_BONE];
    }
}
