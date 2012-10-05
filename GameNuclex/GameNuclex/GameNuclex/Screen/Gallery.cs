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
using GameNuclex.Object.Game;
using System.IO;
using GameNuclex.IO;

namespace GameNuclex.Screen
{
    class Gallery : Scene
    {
        #region InstanceVariable

        Texture2D Background;
        Texture2D ButtonBack;
        Texture2D ContentBackground;
        Texture2D CursorImage;
        Texture2D Picture;

        SpriteFont descriptionFont;
        SpriteFont titleFont;

        private KinectSensor KinectDevice;
        Cursor cursor;
        ProgressBar progressBar;
        GalleryItem Item;

        int GameHeight, GameWidth;

        private bool isCollide;

        #endregion InstanceVariable

        public Gallery(Engine engine, GalleryItem item) : base(engine) 
        {
            this.Item = item;
        }

        protected override void OnEntered()
        {
            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            //DiscoverKinectDevice();

            GameHeight = engine.graphics.Viewport.Height;
            GameWidth = engine.graphics.Viewport.Width;

            titleFont = engine.content.Load<SpriteFont>("GameFont");
            descriptionFont = engine.content.Load<SpriteFont>("FontGalleryList");

            CursorImage = engine.content.Load<Texture2D>("image/cursor");
            Background = engine.content.Load<Texture2D>("image/Gallery/background");
            ButtonBack = engine.content.Load<Texture2D>("image/btn_back");
            ContentBackground = engine.content.Load<Texture2D>("image/Gallery/gallery-bg");
            Picture = Texture2D.FromStream(engine.graphics, File.OpenRead(GameIO.GALLERY_PATH + Item.Name + "\\picture.jpg"));

            cursor = new Cursor();
            cursor.Initialize(CursorImage);

            progressBar = new ProgressBar();
            progressBar.Initialize(engine.content.Load<Texture2D>("image/progress-bar"));

            Item.Description = WrapText(descriptionFont, Item.Description, 800);

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

            UpdatePlayer();

            CheckCollition(gameTime);
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
            engine.spriteBatch.Draw(Background, new Rectangle(0, 0, Background.Width, Background.Height), Color.White);
            engine.spriteBatch.Draw(ButtonBack, new Rectangle(
                10, Background.Height - 115, ButtonBack.Width, ButtonBack.Height)
                , Color.White);

            engine.spriteBatch.Draw(ContentBackground, new Rectangle(
                GameWidth / 2 - ContentBackground.Width / 2, GameHeight / 2 - ContentBackground.Height / 2,
                ContentBackground.Width, ContentBackground.Height), Color.White);

            engine.spriteBatch.Draw(Picture, new Rectangle(engine.graphics.Viewport.Width / 2 - Picture.Width / 2, 150,
                Picture.Width, Picture.Height), Color.White);

            Vector2 danceLength = titleFont.MeasureString(Item.Name);
            engine.spriteBatch.DrawString(titleFont, Item.Name,
                new Vector2(Background.Width / 2 - (int)danceLength.X / 2, 160 + Picture.Height), Color.Yellow);

            engine.spriteBatch.DrawString(descriptionFont, Item.Description,
                new Vector2(Background.Width / 2 - 400, 170 + Picture.Height + danceLength.Y), Color.Yellow);

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
                    GalleryList galleryList = new GalleryList(engine);
                    engine.manager.Switch(galleryList);
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

        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
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
