using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

using Nuclex.Game.States;
using Nuclex.UserInterface.Controls.Desktop;
using Microsoft.Xna.Framework.Content;
using GameNuclex.NuclexPlus.GameFlow;
using GameNuclex.NuclexPlus.Core;
using GameNuclex.NuclexPlus.Animation;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using GameNuclex.Object.NonGame;
using Microsoft.Xna.Framework.Input;
using GameNuclex.IO;
using GameNuclex.Data;
using GameNuclex.NuclexPlus.GameMath;
using GameNuclex.NuclexPlus.Component;
using System.Diagnostics;
using System.Threading;

namespace GameNuclex.Screen
{
    class Play : Scene
    {
        Thread backgroundThread;

        string DanceName;

        Video video;
        VideoPlayer player;
        Texture2D videoTexture;
        KinectSensor KinectDevice;
        ScoringSystem scoringSystem;
        NuclexTimer gameTimer;

        NuclexTimer categoryTimer;
        bool canDrawCategory;
        bool finishLoading;

        Texture2D scoreBackground;
        Texture2D perfectImage;
        Texture2D goodImage;
        Texture2D badImage;
        Texture2D missImage;
        Texture2D LoadingBackground;
        //Texture2D MenuBackground;

        SpriteFont gameFont;

        bool hasPlayed;

        public Play(Engine engine, string danceName)
            : base(engine)
        {
            this.DanceName = danceName;
        }

        protected override void OnEntered()
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            DiscoverKinectDevice();

            LoadingBackground = engine.content.Load<Texture2D>("image/PlayScreen/loadings");

            // Init scoring system //
            DanceData dance = IO.GameIO.LoadDance(DanceName);
            scoringSystem = new ScoringSystem(dance, ScoringSystem.ScoringDegree.Mean);
            gameTimer = new NuclexTimer(200);
            
            player = new VideoPlayer();

            backgroundThread = new Thread(LoadGame);
            backgroundThread.Start();
            
            //MenuBackground = engine.content.Load<Texture2D>("image/PlayScreen/menu-background");

            categoryTimer = new NuclexTimer(1000);
            categoryTimer.stop();
            canDrawCategory = true;
            finishLoading = true;

            base.OnEntered();
        }

        void LoadGame()
        {
            perfectImage = engine.content.Load<Texture2D>("image/PlayScreen/keren");
            goodImage = engine.content.Load<Texture2D>("image/PlayScreen/bagus");
            badImage = engine.content.Load<Texture2D>("image/PlayScreen/buruk");
            missImage = engine.content.Load<Texture2D>("image/PlayScreen/lewat");
            gameFont = engine.content.Load<SpriteFont>("SpriteFont1");
            scoreBackground = engine.content.Load<Texture2D>("image/PlayScreen/score-background");

            video = base.engine.content.Load<Video>("data/Dance/Play/"+ DanceName +"/video");

            // Video Player //
            player.IsLooped = false;
            player.Play(video);
        }

        protected override void OnLeaving()
        {
            base.OnLeaving();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Rectangle screen = new Rectangle(engine.graphics.Viewport.X,
                engine.graphics.Viewport.Y,
                engine.graphics.Viewport.Width,
                engine.graphics.Viewport.Height);

            if (player.State != MediaState.Playing)
            {
                engine.spriteBatch.Draw(LoadingBackground, screen, Color.White);
            }
            else
            {
                if (player.State != MediaState.Stopped)
                {
                    videoTexture = player.GetTexture();
                }

                if (videoTexture != null)
                {
                    engine.spriteBatch.Draw(videoTexture, screen, Color.White);
                }

                //base.engine.spriteBatch.Draw(MenuBackground, new Rectangle(
                //    engine.graphics.Viewport.Width - MenuBackground.Width,
                //    0,
                //    MenuBackground.Width, 
                //    MenuBackground.Height), Color.White);

                base.engine.spriteBatch.Draw(scoreBackground, new Rectangle(0, 0, scoreBackground.Width, scoreBackground.Height), Color.White);

                base.engine.spriteBatch.DrawString(gameFont, "Score: " + scoringSystem.TotalScore,
                    new Vector2(15.0f, 3.0f), Color.Yellow);

                //base.engine.spriteBatch.DrawString(base.engine.content.Load<SpriteFont>("SpriteFont1"), "Menu",
                //        new Vector2(engine.graphics.Viewport.Width - 125, 10.0f), Color.Yellow); 
                // If already preview for sometime, stop preview //

                if (scoringSystem.MomentPerfect)
                {
                    base.engine.spriteBatch.Draw(perfectImage, new Rectangle(15, 200, perfectImage.Width, perfectImage.Height),
                        Color.White);
                }
                else if (scoringSystem.MomentGood)
                {
                    base.engine.spriteBatch.Draw(goodImage, new Rectangle(15, 200, goodImage.Width, goodImage.Height),
                        Color.White);
                }
                else if (scoringSystem.MomentBad)
                {
                    base.engine.spriteBatch.Draw(badImage, new Rectangle(15, 200, badImage.Width, badImage.Height),
                        Color.White);
                }
                else if (scoringSystem.MomentMiss)
                {
                    base.engine.spriteBatch.Draw(missImage, new Rectangle(15, 200, missImage.Width, missImage.Height),
                        Color.White);
                }
            }

            //if (!hasPlayed && player.State == MediaState.Stopped)
            //{
            //    base.engine.spriteBatch.Draw(LoadingBackground, new Rectangle(0, 0, LoadingBackground.Width, LoadingBackground.Height),
            //        Color.White);
            //}



            //if (player.State != MediaState.Stopped)
            //{
            //    Trace.WriteLine("Wawawawa");
            //}

            //Trace.WriteLine(scoringSystem.TotalScore);

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            gameTimer.isTicked(gameTime);
            categoryTimer.isTicked(gameTime);

            DetectKeyboard();

            if (!hasPlayed && player.State == MediaState.Playing)
            {
                hasPlayed = true;
            }

            if (player != null && hasPlayed && player.State == MediaState.Stopped)
            {
                ScoreScreen scoreScreen = new ScoreScreen(engine, scoringSystem.TotalScore, scoringSystem.TotalPerfect,
                    scoringSystem.TotalGood, scoringSystem.TotalBad, scoringSystem.TotalMiss);
                engine.manager.Switch(scoreScreen);
            }
        }

        public void DetectKeyboard()
        {
            KeyboardState newState = Keyboard.GetState();

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.Space))
            {
                gameTimer.reset();
                scoringSystem.Reset();
            }
        }

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
            // Colorstream
            this.KinectDevice.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            // Skeletalstream
            //this.KinectDevice.SkeletonStream.Enable(new TransformSmoothParameters()
            //    {
            //        Smoothing = 0.5f,
            //        Correction = 0.5f,
            //        Prediction = 0.5f,
            //        JitterRadius = 0.05f,
            //        MaxDeviationRadius = 0.04f
            //    });

            this.KinectDevice.SkeletonStream.Enable();

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
                        Trace.WriteLine(Geometry.Get2DPolar(playerSkeleton.Joints[JointType.HipCenter], playerSkeleton.Joints[JointType.ElbowRight]));
                        scoringSystem.UpdateTime(playerSkeleton, gameTimer.totalTime);
                    }
                }
            }
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
    }
}
