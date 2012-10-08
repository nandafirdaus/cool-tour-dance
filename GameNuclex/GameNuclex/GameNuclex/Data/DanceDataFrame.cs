using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace GameNuclex.Data
{
    public class DanceDataFrame
    {
        public string danceName;
        public long time;
        public Vector3[] jointPosition;
        public NuclexEnum.PlayerPose pose;
        public DanceDataFrame(string danceName, long time, Vector3[] skeleton, NuclexEnum.PlayerPose pose)
        {
            this.danceName = danceName;
            this.time = time;
            this.jointPosition = skeleton;
            this.pose = pose;
        }
    }
}
