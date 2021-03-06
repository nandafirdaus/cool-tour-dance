﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using GameNuclex.Data;
using GameNuclex.NuclexPlus.GameMath;
using System.Diagnostics;
using GameNuclex.NuclexPlus.Component;

namespace GameNuclex.NuclexPlus.GameMath
{
    public class ScoringSystem
    {
        public enum ScoringDegree { Max, Min, Mean };

        const int catPerfect = 3, catGood = 2, catBad = 1, catMiss = 0;
        int _Combo;
        bool _MomentCombo;
        const int totalComparation = 0;
        const int limitPerfect = 20, limitGood = 16, limitBad = 12;

        const float permittedError = 20;
        public int Combo { get { return _Combo; } }
        public bool MomentCombo { get { return _MomentCombo; } }
        int _TotalPerfect, _TotalGood, _TotalBad, _TotalMiss;
        int _TotalFrame;
        int _spriteTimer;
        int _comboTimer;
        public int TotalPerfect { get { return _TotalPerfect; } }
        public int TotalGood { get { return _TotalGood; } }
        public int TotalBad { get { return _TotalBad; } }
        public int TotalMiss { get { return _TotalMiss; } }
        public int TotalFrame { get { return _TotalFrame; } }
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
            _TotalFrame = danceData.listOfFrame.Count;
            _TotalPerfect = _TotalGood = _TotalBad = _TotalMiss = _TotalScore = 0;
        }

        public int CompareSkeleton3Dim(Vector3[] skeletonOri, Skeleton skeletonData, ScoringDegree scoringType)
        {
            Tuple<float, float, float>[] countedSkeleton = new Tuple<float, float, float>[8];
            // ElbowRight //
            countedSkeleton[0] = Geometry.Get3DPolar(skeletonOri[NuclexEnum.WristRight], skeletonOri[NuclexEnum.ElbowRight]);
            countedSkeleton[1] = Geometry.Get3DPolar(skeletonData.Joints[JointType.WristRight], skeletonData.Joints[JointType.ElbowRight]);
            // ElbowLeft//
            countedSkeleton[2] = Geometry.Get3DPolar(skeletonOri[NuclexEnum.WristLeft], skeletonOri[NuclexEnum.ElbowLeft]); ;
            countedSkeleton[3] = Geometry.Get3DPolar(skeletonData.Joints[JointType.WristLeft], skeletonData.Joints[JointType.ElbowLeft]);
            // KneeRight //
            countedSkeleton[4] = Geometry.Get3DPolar(skeletonOri[NuclexEnum.HipRight], skeletonOri[NuclexEnum.KneeRight]);
            countedSkeleton[5] = Geometry.Get3DPolar(skeletonData.Joints[JointType.HipRight], skeletonData.Joints[JointType.KneeRight]);
            // KneeLeft //
            countedSkeleton[6] = Geometry.Get3DPolar(skeletonOri[NuclexEnum.HipLeft], skeletonOri[NuclexEnum.KneeLeft]);
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

        public void updateSpriteScore()
        {
            if (this._spriteTimer > 100)
            {
                _MomentPerfect = false;
                _MomentGood = false;
                _MomentBad = false;
                _MomentMiss = false;

                this._spriteTimer = 0;
            }
            else
            {
                this._spriteTimer++;
            }
        }

        public void updateCombo()
        {
            if (this._comboTimer > 100)
            {
                _MomentCombo = false;
                this._comboTimer = 0;
            }
            else
            {
                this._spriteTimer++;
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

        public int CompareSkeleton2D(Vector3[] skeletonOri, Skeleton skeletonData, NuclexEnum.PlayerPose pose, ScoringDegree scoringType)
        {
            float[] degree = new float[16];
            degree[0] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.ElbowLeft], skeletonOri[NuclexEnum.WristLeft], skeletonOri[NuclexEnum.ShoulderLeft]);
            degree[1] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.ElbowLeft], skeletonData.Joints[JointType.WristLeft], skeletonData.Joints[JointType.ShoulderLeft]);
            Trace.WriteLine("Siku Kiri : " + degree[1]);
            degree[2] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.ElbowRight], skeletonOri[NuclexEnum.WristRight], skeletonOri[NuclexEnum.ShoulderRight]);
            degree[3] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.ElbowRight], skeletonData.Joints[JointType.WristRight], skeletonData.Joints[JointType.ShoulderRight]);
            Trace.WriteLine("Siku Kanan : " + degree[3]);
            degree[4] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.ShoulderLeft], skeletonOri[NuclexEnum.ElbowLeft], skeletonOri[NuclexEnum.ShoulderCenter]);
            degree[5] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.ShoulderLeft], skeletonData.Joints[JointType.ElbowLeft], skeletonData.Joints[JointType.ShoulderCenter]);
            Trace.WriteLine("Bahu Kiri : " + degree[5]);
            degree[6] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.ShoulderRight], skeletonOri[NuclexEnum.ElbowRight], skeletonOri[NuclexEnum.ShoulderCenter]);
            degree[7] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.ShoulderRight], skeletonData.Joints[JointType.ElbowRight], skeletonData.Joints[JointType.ShoulderCenter]);
            Trace.WriteLine("Bahu Kanan : " + degree[7]);
            degree[8] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.KneeRight], skeletonOri[NuclexEnum.AnkleRight], skeletonOri[NuclexEnum.HipRight]);
            degree[9] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.KneeRight], skeletonData.Joints[JointType.AnkleRight], skeletonData.Joints[JointType.HipRight]);
            Trace.WriteLine("Lutut Kanan : " + degree[9]);
            degree[10] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.KneeLeft], skeletonOri[NuclexEnum.AnkleLeft], skeletonOri[NuclexEnum.HipLeft]);
            degree[11] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.KneeLeft], skeletonData.Joints[JointType.AnkleLeft], skeletonData.Joints[JointType.HipLeft]);
            Trace.WriteLine("Lutut Kiri : " + degree[11]);
            degree[12] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.HipRight], skeletonOri[NuclexEnum.KneeRight], skeletonOri[NuclexEnum.HipCenter]);
            degree[13] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.HipRight], skeletonData.Joints[JointType.KneeRight], skeletonData.Joints[JointType.HipCenter]);
            Trace.WriteLine("Pinggang Kanan : " + degree[13]);
            degree[14] = Geometry.Get3JointPolar(skeletonOri[NuclexEnum.HipLeft], skeletonOri[NuclexEnum.KneeLeft], skeletonOri[NuclexEnum.HipCenter]);
            degree[15] = Geometry.Get3JointPolar(skeletonData.Joints[JointType.HipLeft], skeletonData.Joints[JointType.KneeLeft], skeletonData.Joints[JointType.HipCenter]);
            Trace.WriteLine("Pinggang Kiri : " + degree[15]);

            bool correctElbowLeft = GetSmallestDegree(degree[0], degree[1]) <= 20;
            bool correctElbowRight = GetSmallestDegree(degree[2], degree[3]) <= 20;
            bool correctShoulderLeft = GetSmallestDegree(degree[4], degree[5]) <= 20;
            bool correctShoulderRight = GetSmallestDegree(degree[6], degree[7]) <= 20;
            bool correctKneeRight = GetSmallestDegree(degree[8], degree[9]) <= 10;
            bool correctKneeLeft = GetSmallestDegree(degree[10], degree[11]) <= 10;
            bool correctHipRight = GetSmallestDegree(degree[12], degree[13]) <= 15;
            bool correctHipLeft = GetSmallestDegree(degree[14], degree[15]) <= 15;

            int success = 0;
            // Add success point //
            if (correctShoulderLeft)
            {
                if (correctElbowLeft)
                {
                    success += 7;
                }
                else
                {
                    success += 3;
                }
            }
            else if (correctShoulderLeft == false)
            {
                if (correctElbowLeft)
                {
                    success += 1;
                }
                else
                {
                    success += 0;
                }
            }

            if (correctShoulderRight)
            {
                if (correctElbowRight)
                {
                    success += 7;
                }
                else
                {
                    success += 3;
                }
            }
            else if (correctShoulderRight == false)
            {
                if (correctElbowRight)
                {
                    success += 1;
                }
                else
                {
                    success += 0;
                }
            }

            if (correctHipLeft)
            {
                if (correctKneeLeft)
                {
                    success += 5;
                }
                else
                {
                    success += 3;
                }
            }
            else if (correctHipLeft == false)
            {
                if (correctKneeLeft)
                {
                    success += 1;
                }
                else
                {
                    success += 0;
                }
            }

            if (correctHipRight)
            {
                if (correctKneeRight)
                {
                    success += 5;
                }
                else
                {
                    success += 3;
                }
            }
            else if (correctHipRight== false)
            {
                if (correctKneeRight)
                {
                    success += 1;
                }
                else
                {
                    success += 0;
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
            _Combo = 0;
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
                        int result = CompareSkeleton2D(now.jointPosition, skeletonData, now.pose, usedScoringDegree);
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
                        // Add per category score//
                        // Reset moment //
                        _MomentMiss = _MomentBad = _MomentGood = _MomentPerfect = false;
                        switch (MaxCorrectCategory)
                        {
                            case (catPerfect):
                                {
                                    _MomentPerfect = true;
                                    _TotalPerfect++;
                                    _Combo++;
                                    _comboTimer = 0;
                                    _spriteTimer = 0;
                                    _MomentCombo = true;
                                    break;
                                }
                            case (catGood):
                                {
                                    _MomentGood = true;
                                    _TotalGood++;
                                    _Combo++;
                                    _comboTimer = 0;
                                    _spriteTimer = 0;
                                    _MomentCombo = true;
                                    break;
                                }
                            case (catBad):
                                {
                                    _MomentBad = true;
                                    _TotalBad++;
                                    _Combo = 0;
                                    _spriteTimer = 0;
                                    _MomentCombo = false;
                                    break;
                                }
                            case (catMiss):
                                {
                                    _MomentMiss = true;
                                    _TotalMiss++;
                                    _spriteTimer = 0;
                                    _Combo = 0;
                                    _MomentCombo = false;
                                    break;
                                }
                        }

                        // Add total score //
                        TotalScore += (MaxCorrect * (Combo > 0 ? Combo : 1));


                        // Reset local variable /
                        MaxCorrectCategory = 0;
                        MaxCorrect = 0;
                    }
                }
                
            }
        }
    }
}