using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameNuclex.NuclexPlus.GameFlow;
using Microsoft.Xna.Framework.Graphics;
using GameNuclex.NuclexPlus.Core;
using Microsoft.Xna.Framework;
using GameNuclex.Object.NonGame;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;

namespace GameNuclex.Screen
{
    class LearnScreen : Scene
    {
        #region InstanceVariable

        Texture2D Background;
        Texture2D ButtonBack;
        Texture2D CursorImage;
        private KinectSensor KinectDevice;
        Cursor cursor;
        ProgressBar progressBar;

        private bool isCollide;

        #endregion InstanceVariable

        public LearnScreen(Engine engine) : base(engine) 
        {
            
        }

        protected override void OnEntered()
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            DiscoverKinectDevice();

            CursorImage = engine.content.Load<Texture2D>("image/cursor");
            Background = engine.content.Load<Texture2D>("image/under-const");
            ButtonBack = engine.content.Load<Texture2D>("image/btn_back");

            cursor = new Cursor();
            cursor.Initialize(CursorImage);

            progressBar = new ProgressBar();
            progressBar.Initialize(engine.content.Load<Texture2D>("image/progress-bar"));

            base.OnEntered();
        }

        protected override void OnLeaving()
        {
            base.OnLeaving();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //MouseState currentMouse = Mouse.GetState();

            //cursor.Position.X = currentMouse.X;
            //cursor.Position.Y = currentMouse.Y;

            CheckCollition(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            engine.spriteBatch.Draw(Background, new Rectangle(0, 0, Background.Width, Background.Height), Color.White);
            engine.spriteBatch.Draw(ButtonBack, new Rectangle(
                10, Background.Height - 115, ButtonBack.Width, ButtonBack.Height)
                , Color.White);

            cursor.Draw(engine.spriteBatch);

            if (isCollide)
            {
                progressBar.Draw(engine.spriteBatch, gameTime);
            }
        }

        private void CheckCollition(GameTime gameTime)
        {
            Rectangle rectangle1;
            Rectangle rectangle2;

            rectangle1 = new Rectangle((int)cursor.Position.X, (int)cursor.Position.Y,
                cursor.Width / 2, cursor.Height / 2);

            //MouseState currentMouse = Mouse.GetState();

            rectangle2 = new Rectangle(10, Background.Height - 115,
                ButtonBack.Width, ButtonBack.Height);

            if (rectangle1.Intersects(rectangle2))
            {
                isCollide = true;
                progressBar.Update(new Vector2(10, Background.Height - 115), 
                    gameTime, (int)cursor.Position.X - 48,
                    (int)cursor.Position.Y + 50);

                if (progressBar.isFull)
                {
                    MainMenu mainMenu = new MainMenu(engine);
                    engine.manager.Switch(mainMenu);
                }

                return;
            }
            else
            {
                isCollide = false;
            }

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
