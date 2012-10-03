﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using System.Diagnostics;

namespace GameNuclex.NuclexPlus.GameMath
{
    public class GameMath { 
        const double Normalize = 180.0 / Math.PI;
        public static double CosDeg(double degree) {
            return Math.Cos(Normalize * degree);
        }
        public static double SinDeg(double degree) {
            return Math.Sin(Normalize * degree);
        }
        public static double TanDeg(double degree) {
            return Math.Tan(Normalize * degree);
        }
        public static double AtanDeg(double degree) {
            return Math.Atan(Normalize * degree);
        }
        public static double AcosDeg(double degree) {
            return Math.Acos(Normalize * degree);
        }
        public static double AsinDeg(double degree) {
            return Math.Asin(Normalize * degree);
        }

    }

    public class Geometry
    {
        public static Tuple<float, float, float> Get3DPolar(Vector3 input1, Vector3 input2) {
            float diffX = input2.X - input1.X;
            float diffY = input2.Y - input1.Y;
            float diffZ = input2.Z - input1.Z;
            float xy = (float)Math.Atan(diffY / diffX);
            float xz = (float)Math.Atan(diffZ / diffX);
            float yz = (float)Math.Atan(diffY / diffZ);
            xy *= (float)(180.0 / Math.PI);
            xz *= (float)(180.0 / Math.PI);
            yz *= (float)(180.0 / Math.PI);
            return new Tuple<float, float, float>(xy, xz, yz);
        }

        public static float Get2DPolar(Joint input1, Joint input2) {
            Vector3 i1 = new Vector3(input1.Position.X, input1.Position.Y, input1.Position.Z);
            Vector3 i2 = new Vector3(input2.Position.X, input2.Position.Y, input2.Position.Z);
            return Get2DPolar(i1, i2);
        }

        // The centre of degree is input1 //
        public static float Get2DPolar(Vector3 input1, Vector3 input2) {
            float diffX = input2.X - input1.X;
            float diffY = input2.Y - input1.Y;

            if (diffX == 0) {
                if (diffY >= 0)
                {
                    return 90.0f;
                }
                else 
                {
                    return 270.0f;
                }
            }
            
            float degree = (float) (Math.Atan(diffY / diffX) * 180.0f / Math.PI);
            Trace.WriteLine("DIFF " + diffX + " " + diffY + " " + degree);
            degree = Math.Abs(degree);

            if (diffX >= 0.0 && diffY >= 0.0) {
                return degree;
            }
            else if (diffX <= 0.0 && diffY >= 0.0) {
                return 180 - degree;
            }
            else if (diffX <= 0.0 && diffY <= 0.0) {
                return 180 + degree;
            }
            else {
                return 360 - degree;
            }


        }

        public static Tuple<float, float, float> Get3DPolar(Joint input1, Joint input2)
        {
            Vector3 i1 = new Vector3(input1.Position.X, input1.Position.Y, input1.Position.Z);
            Vector3 i2 = new Vector3(input2.Position.X, input2.Position.Y, input2.Position.Z);
            return Get3DPolar(i1, i2);
        }

        public static Tuple<float, float, float> GetRadiusPolarAzimuth(Vector3 input1, Vector3 input2) {
            float diffX = input2.X - input1.X;
            float diffY = input2.Y - input1.Y;
            float diffZ = input2.Z - input1.Z;
            float radius = (float) Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
            float polar = (float) Math.Acos(diffZ / radius);
            float azimuth = (float) Math.Atan(diffY / diffX);
            // to degree again //
            
            polar *= (float) (180.0 / Math.PI);
            azimuth *= (float) (180.0 / Math.PI);
            return new Tuple<float, float, float>(radius, polar, azimuth);
        }
        public static Tuple<float, float, float> GetRadiusPolarAzimuth(Joint input1, Joint input2)
        {
            Vector3 i1 = new Vector3(input1.Position.X, input1.Position.Y, input1.Position.Z);
            Vector3 i2 = new Vector3(input2.Position.X, input2.Position.Y, input2.Position.Z);
            return GetRadiusPolarAzimuth(i1, i2);
        }

        public static float MaxDegreeError(Tuple<float, float, float> input1, Tuple<float, float, float> input2) {
            return Math.Max(Math.Abs(input1.Item2 - input2.Item2), Math.Abs(input1.Item3 - input2.Item3)); 
        }
        public static float MinDegreeError(Tuple<float, float, float> input1, Tuple<float, float, float> input2)
        {
            return Math.Min(Math.Abs(input1.Item2 - input2.Item2), Math.Abs(input1.Item3 - input2.Item3));
        }
        public static float MeanDegreeError(Tuple<float, float, float> input1, Tuple<float, float, float> input2)
        {
            float mean = Math.Abs(input1.Item2 - input2.Item2) + Math.Abs(input1.Item3- input2.Item3);
            mean /= 2;
            return mean;
        }
    }

    
}