using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using GameNuclex.NuclexPlus.Core;
using System.Diagnostics;
namespace GameNuclex.NuclexPlus.Component
{
    public class NuclexKinect
    {
        public KinectSensor Kinect {
            get { return this._Kinect; }
            set 
            {
                if (this._Kinect != value)
                {
                    if (this._Kinect != null) {
                        UninitializeKinectSensor(this._Kinect);
                        this._Kinect = null;
                    }

                    if (value != null && value.Status == KinectStatus.Connected) {
                        this._Kinect = value;
                        InitializeKinectSensor(this._Kinect);
                    }
                }
            }
        }

        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null) {
                kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);
                kinectSensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinectSensor_DepthFrameReady);
                kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSensor_SkeletonFrameReady);
                kinectSensor.Start();
            }
        }

        private void UninitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null) {
                kinectSensor.Stop();
                kinectSensor.ColorFrameReady -= kinectSensor_ColorFrameReady;
                kinectSensor.DepthFrameReady -= kinectSensor_DepthFrameReady;
                kinectSensor.SkeletonFrameReady -= kinectSensor_SkeletonFrameReady;
            }
        }
        public void StopKinect() {
            UninitializeKinectSensor(_Kinect);
        }
        void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null) {
                    Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    Skeleton nearest = null;
                    for (int ii = 0; ii < skeletons.Length; ii++)
                    {
                        if (skeletons[ii].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (nearest == null)
                            {
                                nearest = skeletons[ii];
                            }
                            else
                            {
                                nearest = (skeletons[ii].Position.Z < nearest.Position.Z) ? skeletons[ii] : nearest;
                            }
                        }
                    }

                    _MainSkeleton = nearest;
                }
            }
        }

        void kinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame()) { 
            
            }
        }


        byte red, green, blue, alpha;
        void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            return;
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {
                    
                    byte[] pixelData = new byte[colorImageFrame.PixelDataLength];
                    colorImageFrame.CopyPixelDataTo(pixelData);
                    // remove blue layer //
                    for (int ii = 0; ii < pixelData.Length; ii+=4) {
                        red = pixelData[ii];
                        green = pixelData[ii + 1];
                        blue = pixelData[ii + 2];
                        alpha = (byte) 255;
                        pixelData[ii] = blue;
                        pixelData[ii + 1] = green;
                        pixelData[ii + 2] = red;
                        pixelData[ii + 3] = alpha;
                    }

                    if (_ColorImage == null)
                    {
                        _ColorImage = new Texture2D(graphicsDevice, colorImageFrame.Width, colorImageFrame.Height);
                    }
                    graphicsDevice.Textures[0] = null;
                    _ColorImage.SetData(pixelData);
                }
            }
        }

        
        KinectSensor _Kinect;
        Texture2D _ColorImage;
        public Texture2D ColorImage { get { return _ColorImage; } }
        Texture2D _DepthImage;
        public Texture2D DepthImage { get { return _DepthImage; } }
        Skeleton _MainSkeleton;
        public Skeleton MainSkeleton { get { return _MainSkeleton; } }

        GraphicsDevice graphicsDevice;
        public NuclexKinect(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            DiscoverKinectSensor();
            
        }
        public void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);            
            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.Kinect == null)
                    {
                        this.Kinect = e.Sensor;
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (this.Kinect == e.Sensor)
                    {
                        this.Kinect = null;
                        this.Kinect = KinectSensor.KinectSensors
                        .FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (this.Kinect == null)
                        {
                            //Notify the user that the sensor is disconnected 
                        }
                    }
                    break;
                //Handle all other statuses according to needs 
            }
        }

        

    }
}
