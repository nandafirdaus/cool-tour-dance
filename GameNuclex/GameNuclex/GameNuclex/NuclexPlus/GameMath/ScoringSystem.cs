using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using GameNuclex.Data;
using GameNuclex.NuclexPlus.GameMath;
using System.Diagnostics;
namespace GameNuclex.NuclexPlus.GameMath
{
    public class ScoringSystem
    {
        public enum ScoringDegree { Max, Min, Mean };
        const int catPerfect = 3, catGood = 2, catBad = 1, catMiss = 0;
        int _Combo;
        const int totalComparation = 0;
        const int limitPerfect = 5, limitGood = 4, limitBad = 3, limitMiss = 2;

        const float permittedError = 20;
        public int Combo { get { return _Combo; } set { _Combo = value; } }

        int _TotalPerfect, _TotalGood, _TotalBad, _TotalMiss;

        public int TotalPerfect { get { return _TotalPerfect; } }
        public int TotalGood { get { return _TotalGood; } }
        public int TotalBad { get { return _TotalBad; } }
        public int TotalMiss { get { return _TotalMiss; } }

        int _TotalScore;
        public int TotalScore { get { return _TotalScore; } set { _TotalScore = value; } }

        bool _MomentPerfect, _MomentGood, _MomentBad, _MomentMiss;

        public bool MomentPerfect { get { return _MomentPerfect; } }
        public bool MomentGood { get { return _MomentGood; } }
        public bool MomentBad { get { return _MomentBad; } }
        public bool MomentMiss { get { return _MomentMiss; } }



        ScoringDegree usedScoringDegree;
        DanceData danceData;

        public ScoringSystem(DanceData danceData, ScoringDegree scoringDegree)
        {
            this.danceData = danceData;
            usedScoringDegree = scoringDegree;
            _TotalScore = 0;
            _TotalPerfect = _TotalGood = _TotalBad = _TotalMiss = 0;
        }

        public int CompareSkeleton3Dim(Vector3[] skeletonOri, Skeleton skeletonData, ScoringDegree scoringType)
        {
            Tuple<float, float, float>[] countedSkeleton = new Tuple<float, float, float>[8];
            // ElbowRight //
            countedSkeleton[0] = Geometry.Get3DPolar(skeletonOri[NuclexJoint.WristRight], skeletonOri[NuclexJoint.ElbowRight]);
            countedSkeleton[1] = Geometry.Get3DPolar(skeletonData.Joints[JointType.WristRight], skeletonData.Joints[JointType.ElbowRight]);
            // ElbowLeft//
            countedSkeleton[2] = Geometry.Get3DPolar(skeletonOri[NuclexJoint.WristLeft], skeletonOri[NuclexJoint.ElbowLeft]); ;
            countedSkeleton[3] = Geometry.Get3DPolar(skeletonData.Joints[JointType.WristLeft], skeletonData.Joints[JointType.ElbowLeft]);
            // KneeRight //
            countedSkeleton[4] = Geometry.Get3DPolar(skeletonOri[NuclexJoint.HipRight], skeletonOri[NuclexJoint.KneeRight]);
            countedSkeleton[5] = Geometry.Get3DPolar(skeletonData.Joints[JointType.HipRight], skeletonData.Joints[JointType.KneeRight]);
            // KneeLeft //
            countedSkeleton[6] = Geometry.Get3DPolar(skeletonOri[NuclexJoint.HipLeft], skeletonOri[NuclexJoint.KneeLeft]);
            countedSkeleton[7] = Geometry.Get3DPolar(skeletonData.Joints[JointType.HipLeft], skeletonData.Joints[JointType.KneeLeft]);



            int success = 0;

            if (scoringType == ScoringDegree.Max)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MaxDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        //Trace.WriteLine(Geometry.MaxDegreeError(countedSkeleton[n], countedSkeleton[n + 1]));
                        success++;
                    }
                }

            }
            else if (scoringType == ScoringDegree.Min)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MinDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        success++;
                    }
                }

            }
            else if (scoringType == ScoringDegree.Mean)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MeanDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        success++;
                    }
                }
            }


            if (success == limitPerfect)
            {
                return catPerfect;
            }
            else if (success == limitGood)
            {
                return catGood;
            }
            else if (success == limitBad)
            {
                return catBad;
            }
            else
            {
                return catMiss;
            }
        }




        public static float GetSmallestDegree(float deg1, float deg2)
        {
            if (deg1 > deg2)
            {
                float temp = deg1;
                deg1 = deg2;
                deg2 = temp;
            }
            return Math.Min(Math.Abs(360 + deg1 - deg2), Math.Abs(deg1 - deg2));
        }

        public int CompareSkeleton2D(Vector3[] skeletonOri, Skeleton skeletonData, ScoringDegree scoringType)
        {
            float[] degree = new float[12];
            degree[0] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.WristLeft]);
            degree[1] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.WristLeft]);

            degree[2] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.WristRight]);
            degree[3] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.WristRight]);

            //Trace.WriteLine("Degree 3 4 : "+degree[2]+" "+degree[3]);

            degree[4] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.AnkleLeft]);
            degree[5] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.AnkleLeft]);

            degree[6] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.AnkleRight]);
            degree[7] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.AnkleRight]);

            degree[8] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.ElbowLeft]);
            degree[9] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.ElbowLeft]);

            degree[10] = Geometry.Get2DPolar(skeletonOri[NuclexJoint.HipCenter], skeletonOri[NuclexJoint.ElbowRight]);
            degree[11] = Geometry.Get2DPolar(skeletonData.Joints[JointType.HipCenter], skeletonData.Joints[JointType.ElbowRight]);

            int success = 0;
            if (scoringType == ScoringDegree.Mean)
            {

                for (int nn = 0; nn < degree.Length; nn += 2)
                {
                    if (GetSmallestDegree(degree[nn], degree[nn + 1]) < permittedError)
                    {
                        success++;
                    }
                }
            }

            if (success >= limitPerfect)
            {
                return catPerfect;
            }
            else if (success >= limitGood)
            {
                return catGood;
            }
            else if (success >= limitBad)
            {
                return catBad;
            }
            else
            {
                return catMiss;
            }


        }
        // ga kepake susah //
        public int CompareSkeleton(Vector3[] skeletonOri, Skeleton skeletonData, ScoringDegree scoringType)
        {
            Tuple<float, float, float>[] countedSkeleton = new Tuple<float, float, float>[8];
            // ElbowRight //
            countedSkeleton[0] = Geometry.GetRadiusPolarAzimuth(skeletonOri[NuclexJoint.WristRight], skeletonOri[NuclexJoint.ElbowRight]);
            countedSkeleton[1] = Geometry.GetRadiusPolarAzimuth(skeletonData.Joints[JointType.WristRight], skeletonData.Joints[JointType.ElbowRight]);
            // ElbowLeft//
            countedSkeleton[2] = Geometry.GetRadiusPolarAzimuth(skeletonOri[NuclexJoint.WristLeft], skeletonOri[NuclexJoint.ElbowLeft]); ;
            countedSkeleton[3] = Geometry.GetRadiusPolarAzimuth(skeletonData.Joints[JointType.WristLeft], skeletonData.Joints[JointType.ElbowLeft]);
            // KneeRight //
            countedSkeleton[4] = Geometry.GetRadiusPolarAzimuth(skeletonOri[NuclexJoint.KneeRight], skeletonOri[NuclexJoint.AnkleRight]);
            countedSkeleton[5] = Geometry.GetRadiusPolarAzimuth(skeletonData.Joints[JointType.KneeRight], skeletonData.Joints[JointType.AnkleRight]);
            // KneeLeft //
            countedSkeleton[6] = Geometry.GetRadiusPolarAzimuth(skeletonOri[NuclexJoint.KneeLeft], skeletonOri[NuclexJoint.AnkleLeft]);
            countedSkeleton[7] = Geometry.GetRadiusPolarAzimuth(skeletonData.Joints[JointType.KneeLeft], skeletonData.Joints[JointType.AnkleLeft]);

            int success = 0;

            if (scoringType == ScoringDegree.Max)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MaxDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        //Trace.WriteLine(Geometry.MaxDegreeError(countedSkeleton[n], countedSkeleton[n + 1]));
                        success++;
                    }
                }

            }
            else if (scoringType == ScoringDegree.Min)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MinDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        success++;
                    }
                }

            }
            else if (scoringType == ScoringDegree.Mean)
            {
                for (int n = 0; n < countedSkeleton.Length; n += 2)
                {
                    if (Geometry.MeanDegreeError(countedSkeleton[n], countedSkeleton[n + 1]) < permittedError)
                    {
                        success++;
                    }
                }
            }


            if (success == limitPerfect)
            {
                return catPerfect;
            }
            else if (success == limitGood)
            {
                return catGood;
            }
            else if (success == limitBad)
            {
                return catBad;
            }
            else
            {
                return catMiss;
            }
        }

        public int GetScore(int result)
        {

            if (result == catPerfect)
            {
                return 400;
            }
            else if (result == catGood)
            {
                return 300;
            }
            else if (result == catBad)
            {
                return 100;
            }
            else
            {  // result == miss //
                return 0;
            }

        }

        public void Reset()
        {
            index = 0;
            TotalScore = 0;
            _TotalMiss = _TotalPerfect = _TotalGood = _TotalBad = 0;
            Combo = 0;
        }

        int index = 0;
        bool hasChecked = false;
        int MaxCorrect = 0;
        int MaxCorrectCategory;
        public void UpdateTime(Skeleton skeletonData, long time)
        {

            if (index + 1 <= danceData.listOfFrame.Count - 1)
            {

                DanceDataFrame now = danceData.listOfFrame[index];
                DanceDataFrame next = danceData.listOfFrame[index + 1];

                if (!hasChecked)
                {
                    //Trace.WriteLine("Time : " + time + " " + now.time + " " + next.time);
                    // do scoring //
                    int result = CompareSkeleton2D(now.jointPosition, skeletonData, usedScoringDegree);
                    MaxCorrect = Math.Max(MaxCorrect, GetScore(result));
                    MaxCorrectCategory = Math.Max(MaxCorrectCategory, result);
                    hasChecked = true;
                }

                if (next.time <= time)
                {

                    // Current index in dance data //
                    index++;
                    // Reset status if movement has been checked //s
                    hasChecked = false;
                    // Add total score //
                    TotalScore += (MaxCorrect);
                    // Add per category score//
                    // Reset moment //
                    _MomentMiss = _MomentBad = _MomentGood = _MomentPerfect = false;
                    switch (MaxCorrectCategory)
                    {
                        case (catPerfect):
                            {
                                _MomentPerfect = true;
                                _TotalPerfect++;
                                break;
                            }
                        case (catGood):
                            {
                                _MomentGood = true;
                                _TotalGood++;
                                break;
                            }
                        case (catBad):
                            {
                                _MomentBad = true;
                                _TotalBad++;
                                break;
                            }
                        case (catMiss):
                            {
                                _MomentMiss = true;
                                _TotalMiss++;
                                break;
                            }
                    }



                    // Reset local variable /
                    MaxCorrectCategory = 0;
                    MaxCorrect = 0;
                }
            }
            else
            {
                //Trace.WriteLine("SELESAI");
            }
        }
    }
}