using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameNuclex.NuclexPlus.GameFlow;
using GameNuclex.NuclexPlus.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameNuclex.Object.NonGame;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using GameNuclex.IO;
using System.IO;

namespace GameNuclex.Screen
{
    class HowToPlay : Scene
    {
        #region InstanceVariable

        //Texture2D Background;
        Texture2D BackButton;
        //Texture2D DanceListBackground;
        Texture2D RightButton;
        Texture2D LeftButton;
        //Texture2D PageTitle;
        //Texture2D DancePicture;
        private KinectSensor KinectDevice;
        Vector2[] ControlPosition;

        //string[] allList;

        Texture2D[] Pictures;

        SpriteFont fontType;

        int currentIndex;
        int totalPage;

        Cursor cursor;
        ProgressBar progressBar;

        bool isCollide;

        #endregion InstanceVariable

        public HowToPlay(Engine engine)
            : base(engine)
        {

        }

        protected override void OnEntered()
        {
            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            //DiscoverKinectDevice();

            //allList = GameIO.GetPlayDanceInfo();

            //Pictures = new Texture2D[allList.Length];

            totalPage = 4;

            for (int i = 0; i < totalPage; i++)
            {
                Pictures[i] = engine.content.Load<Texture2D>("image/HowToPlay/" + (i + 1));
            }

            //Background = engine.content.Load<Texture2D>("image/HowToPlay/1");
            BackButton = engine.content.Load<Texture2D>("image/btn_back");
            //DanceListBackground = engine.content.Load<Texture2D>("image/SelectDance/pilih_screen");
            RightButton = engine.content.Load<Texture2D>("image/SelectDance/btn_right");
            LeftButton = engine.content.Load<Texture2D>("image/SelectDance/btn_left");
            //PageTitle = engine.content.Load<Texture2D>("image/SelectDance/page-title");
            //DancePicture = engine.content.Load<Texture2D>("image/SelectDance/tari-lenggang-nyai");

            fontType = engine.content.Load<SpriteFont>("GameFont");

            cursor = new Cursor();
            cursor.Initialize(engine.content.Load<Texture2D>("image/cursor"));

            progressBar = new ProgressBar();
            progressBar.Initialize(engine.content.Load<Texture2D>("image/progress-bar"));

            ControlPosition = new Vector2[3];
            // Back button
            ControlPosition[0] = new Vector2(5, engine.graphics.Viewport.Height - 115);
            // Left button
            ControlPosition[1] = new Vector2(5, engine.graphics.Viewport.Height / 2 - LeftButton.Height / 2);
            // Right button
            ControlPosition[2] = new Vector2(engine.graphics.Viewport.Width - 5 - LeftButton.Width,
                engine.graphics.Viewport.Height / 2 - LeftButton.Height / 2);

            base.OnEntered();
        }

        protected override void OnLeaving()
        {
            base.OnLeaving();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            MouseState currentMouse = Mouse.GetState();

            cursor.Position.X = currentMouse.X;
            cursor.Position.Y = currentMouse.Y;

            //UpdatePlayer();

            CheckCollision(gameTime);
        }

        private void UpdatePlayer()
        {
            Skeleton playerSkeleton = engine.nuclexKinect.MainSkeleton;

            // Update player position
            if (playerSkeleton != null)
            {
                Joint hand = playerSkeleton.Joints[JointType.HandRight];
                Joint chest = playerSkeleton.Joints[JointType.ShoulderCenter];
                Point point = GetJointPoint(hand, chest);
                cursor.Position = new Vector2(point.X, point.Y);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Pictures != null)
            {
                engine.spriteBatch.Draw(Pictures[currentIndex],
                        new Rectangle(0,0,
                            Pictures[currentIndex].Width, Pictures[currentIndex].Height), Color.White);
            }

            engine.spriteBatch.Draw(BackButton, new Rectangle(
                (int)ControlPosition[0].X, (int)ControlPosition[0].Y, BackButton.Width, BackButton.Height), Color.White);
            engine.spriteBatch.Draw(LeftButton,
                new Rectangle((int)ControlPosition[1].X, (int)ControlPosition[1].Y, LeftButton.Width, LeftButton.Height), Color.White);
            engine.spriteBatch.Draw(RightButton,
                new Rectangle((int)ControlPosition[2].X, (int)ControlPosition[2].Y, RightButton.Width, RightButton.Height), Color.White);

            cursor.Draw(engine.spriteBatch);
            if (isCollide)
            {
                progressBar.Draw(engine.spriteBatch, gameTime);
            }
        }

        void CheckCollision(GameTime gameTime)
        {
            Rectangle rectangle1;
            Rectangle rectangle2;

            rectangle1 = new Rectangle((int)cursor.Position.X, (int)cursor.Position.Y,
                cursor.Width / 2, cursor.Height / 2);

            //MouseState currentMouse = Mouse.GetState();

            rectangle2 = new Rectangle((int)ControlPosition[0].X, (int)ControlPosition[0].Y,
                BackButton.Width, BackButton.Height);

            if (rectangle1.Intersects(rectangle2))
            {
                isCollide = true;
                progressBar.Update(ControlPosition[0], gameTime, (int)cursor.Position.X - 48,
                    (int)cursor.Position.Y + 50);

                if (progressBar.isFull)
                {
                    MainMenu mainMenu = new MainMenu(engine);
                    engine.manager.Switch(mainMenu);
                }

                return;

            }

            rectangle2 = new Rectangle((int)ControlPosition[1].X, (int)ControlPosition[1].Y,
                LeftButton.Width, LeftButton.Height);

            if (rectangle1.Intersects(rectangle2))
            {
                isCollide = true;
                progressBar.Update(ControlPosition[1], gameTime, (int)cursor.Position.X - 48,
                    (int)cursor.Position.Y + 50);

                if (progressBar.isFull)
                {
                    if (currentIndex > 0)
                    {
                        currentIndex--;

                        progressBar.Update(new Vector2(), gameTime, (int)cursor.Position.X - 48,
                        (int)cursor.Position.Y + 50);
                    }
                }

                return;

            }

            rectangle2 = new Rectangle((int)ControlPosition[2].X, (int)ControlPosition[2].Y,
                RightButton.Width, RightButton.Height);

            if (rectangle1.Intersects(rectangle2))
            {
                isCollide = true;
                progressBar.Update(ControlPosition[2], gameTime, (int)cursor.Position.X - 48,
                    (int)cursor.Position.Y + 50);

                if (progressBar.isFull)
                {
                    if (currentIndex < totalPage - 1)
                    {
                        currentIndex++;

                        progressBar.Update(new Vector2(), gameTime, (int)cursor.Position.X - 48,
                        (int)cursor.Position.Y + 50);
                    }
                }

                return;

            }

            isCollide = false;

            progressBar.Update(new Vector2(), gameTime, (int)cursor.Position.X - 48,
                        (int)cursor.Position.Y + 50);
        }

        #region Kinect Method

        private void DiscoverKinectDevice()
        {
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

            if (this.KinectDevice == null)
            {
                return;
            }

            InitializeKinectDevice();
        }

        private void InitializeKinectDevice()
        {
            this.KinectDevice.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            //this.KinectDevice.SkeletonStream.Enable();

            this.KinectDevice.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(KinectDevice_SkeletonFrameReady);

            try
            {
                this.KinectDevice.Start();
            }
            catch (Exception)
            {

            }
        }

        void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = skeletonData.FirstOrDefault(x =>
                        x.TrackingState == SkeletonTrackingState.Tracked);

                    // Update player position
                    if (playerSkeleton != null)
                    {
                        Joint hand = playerSkeleton.Joints[JointType.HandRight];
                        Joint chest = playerSkeleton.Joints[JointType.ShoulderCenter];
                        Point point = GetJointPoint(hand, chest);
                        cursor.Position = new Vector2(point.X, point.Y);
                    }
                }
            }
        }

        private Point GetJointPoint(Joint position, Joint chest)
        {
            /*
            DepthImagePoint point = this.KinectDevice.MapSkeletonPointToDepth(position.Position
                , DepthImageFormat.Resolution640x480Fps30);

            point.X *= (engine.graphics.Viewport.Width / this.KinectDevice.DepthStream.FrameWidth);
            point.Y *= (engine.graphics.Viewport.Height / this.KinectDevice.DepthStream.FrameHeight);

            return new Point(point.X, point.Y);
            */

            float diffX = (position.Position.X - chest.Position.X);
            float diffY = (position.Position.Y - chest.Position.Y);

            diffX = (diffX / 0.2f) * 320;
            diffY = (diffY / 0.2f) * 240 * -1;

            return new Point((int)(diffX + 320), (int)(diffY + 240));
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.KinectDevice == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.KinectDevice = null;
                    DiscoverKinectDevice();
                }
            }
        }

        #endregion Kinect Method
    }
}
