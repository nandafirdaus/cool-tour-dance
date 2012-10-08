using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nuclex.Game.States;
using Nuclex.UserInterface.Controls.Desktop;
using Microsoft.Xna.Framework.Content;
using GameNuclex.NuclexPlus.GameFlow;
using GameNuclex.NuclexPlus.Core;
using GameNuclex.NuclexPlus.Animation;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameNuclex.Object.NonGame;
using Microsoft.Xna.Framework.Input;
using GameNuclex.IO;
using GameNuclex.Data;
using System.Diagnostics;
using Microsoft.Kinect;

namespace GameNuclex.Screen
{
    public class MainMenu : Scene
    {
        #region Instance Variable

        int ScreenWidth;
        int ScreenHeight;

        Texture2D[] MenuTextures;
        Texture2D CursorImage;
        Texture2D Background;

        Vector2[] MenuPosition;
        ProgressBar progressBar;

        private KinectSensor KinectDevice;

        Cursor cursor;

        bool isCollide = false;

        #endregion Instance Variable

        public MainMenu(Engine engine) : base(engine) 
        {
            
        }

        protected override void OnEntered()
        {
            //string[] allList = GameIO.GetDanceInfo();
            //Trace.WriteLine("HOI "+allList.Length);
            //foreach (string ss in allList) {
            //    Trace.WriteLine(ss);
            //}

            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            //DiscoverKinectDevice();

            if (engine.soundEffect.State != Microsoft.Xna.Framework.Audio.SoundState.Playing)
            {
                engine.soundEffect.Play();
            }

            ScreenWidth = 1024;
            ScreenHeight = 768;

            cursor = new Cursor();
            CursorImage = base.engine.content.Load<Texture2D>("image/cursor");
            cursor.Initialize(CursorImage);

            //MouseState currentMouse = Mouse.GetState();
            
            progressBar = new ProgressBar();
            progressBar.Initialize(engine.content.Load<Texture2D>("image/progress-bar"));
            
            Background = base.engine.content.Load<Texture2D>("image/MainMenu/bg_menu");

            MenuTextures = new Texture2D[6];

            // Menu Start
            MenuTextures[0] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_mulai");
            // Menu Learn
            MenuTextures[1] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_belajar");
            // Menu Setting
            MenuTextures[2] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_petunjuk");
            // Menu Gallery
            MenuTextures[3] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_galeri");
            // Menu Info
            MenuTextures[4] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_info");
            // Exit
            MenuTextures[5] = base.engine.content.Load<Texture2D>("image/MainMenu/btn_close");

            MenuPosition = new Vector2[6];

            // Menu Start
            MenuPosition[0] = new Vector2(ScreenWidth / 2 - MenuTextures[0].Width / 2, ScreenHeight / 2 - MenuTextures[0].Height / 2);
            // Menu Learn
            MenuPosition[1] = new Vector2(ScreenWidth / 4 - MenuTextures[1].Width, ScreenHeight / 4 - MenuTextures[1].Height / 2);
            // Menu Setting
            MenuPosition[2] = new Vector2(ScreenWidth / 4 - MenuTextures[2].Width, ScreenHeight / 4 * 3 - MenuTextures[2].Height / 2);
            // Menu Gallery
            MenuPosition[3] = new Vector2(ScreenWidth / 4 * 3, ScreenHeight / 4 - MenuTextures[3].Height / 2);
            // Menu Info
            MenuPosition[4] = new Vector2(ScreenWidth / 4 * 3, ScreenHeight / 4 * 3 - MenuTextures[4].Height / 2);
            // Exit
            MenuPosition[5] = new Vector2(ScreenWidth - MenuTextures[5].Width, 5);

            base.OnEntered();
        }

        protected override void OnLeaving()
        {
            base.OnLeaving();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            engine.spriteBatch.Draw(Background, new Rectangle(0, 0, Background.Width, Background.Height), Color.White);
            for (int i = 0; i < MenuTextures.Length; i++)
            {   
                engine.spriteBatch.Draw(MenuTextures[i], MenuPosition[i], Color.White);                
            }

            cursor.Draw(engine.spriteBatch);
            if (isCollide)
            {
                progressBar.Draw(engine.spriteBatch, gameTime);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //MouseState currentMouse = Mouse.GetState();

            //cursor.Position.X = currentMouse.X;
            //cursor.Position.Y = currentMouse.Y;

            UpdatePlayer();

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

        void CheckCollision(GameTime gameTime)
        {
            Rectangle rectangle1;
            Rectangle rectangle2;

            rectangle1 = new Rectangle((int)cursor.Position.X, (int)cursor.Position.Y,
                cursor.Width / 2, cursor.Height / 2);

            //MouseState currentMouse = Mouse.GetState();

            for (int i = 0; i < MenuPosition.Length; i++)
            {
                rectangle2 = new Rectangle((int)MenuPosition[i].X, (int)MenuPosition[i].Y,
                    MenuTextures[i].Width, MenuTextures[i].Width);

                if (rectangle1.Intersects(rectangle2))
                {
                    isCollide = true;
                    progressBar.Update(MenuPosition[i], gameTime, (int)cursor.Position.X - 48,
                        (int)cursor.Position.Y + 50);

                    if (progressBar.isFull)
                    {
                        switch (i)
                        {
                            case 0:
                                SelectDance selectDance = new SelectDance(engine);
                                engine.manager.Switch(selectDance);
                                break;
                            case 1: 
                                SelectDanceLearn selectDanceLearn = new SelectDanceLearn(engine);
                                engine.manager.Switch(selectDanceLearn);
                                break;
                            case 2:
                                HowToPlay howToPlay = new HowToPlay(engine);
                                engine.manager.Switch(howToPlay);
                                break;
                            case 3:
                                GalleryList gallery = new GalleryList(engine);
                                engine.manager.Switch(gallery);
                                break;
                            case 4:
                                About about = new About(engine);
                                engine.manager.Switch(about);
                                break;
                            case 5:
                                engine.nuclexKinect.StopKinect();
                                engine.game.Exit();
                                break;
                            default:
                                break;
                        }
                    }

                    return;
                }
                else {
                    isCollide = false;
                }
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
