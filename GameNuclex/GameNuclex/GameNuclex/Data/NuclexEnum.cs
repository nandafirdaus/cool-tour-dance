using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GameNuclex.Data
{
    public class NuclexEnum
    {
        public enum PlayerPose { Default, SeatMode };
        public const int
            Head = 0,
            ShoulderCenter = 1,
            ShoulderRight = 2,
            ElbowRight = 3,
            WristRight = 4,
            HandRight = 5,
            ShoulderLeft = 6,
            ElbowLeft = 7,
            WristLeft = 8,
            HandLeft = 9,
            Spine = 10,
            HipCenter = 11,
            HipRight = 12,
            KneeRight = 13,
            AnkleRight = 14,
            FootRight = 15,
            HipLeft = 16,
            KneeLeft = 17,
            AnkleLeft = 18,
            FootLeft = 19;
    }
}
