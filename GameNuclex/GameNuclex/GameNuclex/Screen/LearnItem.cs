﻿using System;
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
using GameNuclex.IO;
using System.Xml.Linq;
using GameNuclex.Object.Game;
using System.IO;

namespace GameNuclex.Screen
{
    class LearnItem : Scene
    {
        #region InstanceVariable

        Texture2D Background;
        //Texture2D ButtonBack;
        Texture2D ListBackground;
        Texture2D CursorImage;

        SpriteFont titleFont;

        string DanceName;

        Texture2D[] ItemPicture;
        private KinectSensor KinectDevice;
        Cursor cursor;
        ProgressBar progressBar;

        //GalleryItem[] items;
        Vector2[] itemPosition;

        List<Tuple<string, Texture2D>> allList;

        int GameHeight, GameWidth;

        private bool isCollide;

        #endregion InstanceVariable

        public LearnItem(Engine engine, string danceName)
            : base(engine)
        {
            this.DanceName = danceName;
        }

        protected override void OnEntered()
        {
            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            //DiscoverKinectDevice();

            allList = GameIO.GetLearnDanceItem(DanceName, engine.graphics);
            //items = new GalleryItem[allList.Count];
            itemPosition = new Vector2[allList.Count + 1];
            //Trace.WriteLine("HOI " + allList.Length);
            for (int i = 0; i < allList.Count; i++)
            {
                
                itemPosition[i] = new Vector2(52 + (230 * i) + 52, 156 + (227 * (i / 4)) + 20);

                //Trace.WriteLine(ss);
            }

            itemPosition[allList.Count] = new Vector2(10, engine.graphics.Viewport.Height - 115);

            GameHeight = engine.graphics.Viewport.Height;
            GameWidth = engine.graphics.Viewport.Width;

            ItemPicture = new Texture2D[allList.Count + 1];

            for (int i = 0; i < ItemPicture.Length - 1; i++)
            {
                ItemPicture[i] = allList[i].Item2;
            }

            ItemPicture[ItemPicture.Length - 1] = engine.content.Load<Texture2D>("image/btn_back");

            CursorImage = engine.content.Load<Texture2D>("image/cursor");
            Background = engine.content.Load<Texture2D>("image/Gallery/background");
            ListBackground = engine.content.Load<Texture2D>("image/Gallery/list-bg");

            titleFont = engine.content.Load<SpriteFont>("FontGalleryList");

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

            engine.spriteBatch.Draw(ItemPicture[itemPosition.Length - 1], new Rectangle(
                (int)itemPosition[ItemPicture.Length - 1].X, (int)itemPosition[ItemPicture.Length - 1].Y,
                ItemPicture[itemPosition.Length - 1].Width, ItemPicture[itemPosition.Length - 1].Height)
                , Color.White);

            engine.spriteBatch.Draw(ListBackground, new Rectangle(
                GameWidth / 2 - ListBackground.Width / 2, GameHeight / 2 - ListBackground.Height / 2,
                ListBackground.Width, ListBackground.Height), Color.White);

            for (int i = 0; i < ItemPicture.Length - 1; i++)
            {
                engine.spriteBatch.Draw(ItemPicture[i], new Rectangle(
                    (int)itemPosition[i].X, (int)itemPosition[i].Y,
                    ItemPicture[i].Width, ItemPicture[i].Height), Color.White);

                //Vector2 stringLength = titleFont.MeasureString(items[i].Name);
                //engine.spriteBatch.DrawString(titleFont, items[i].Name,
                //    new Vector2(52 + (230 * i) + (115 - stringLength.X / 2), 156 + (227 * (i / 4)) + 20 + ItemPicture[i].Height + 5), 
                //    Color.White);
            }

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
                ItemPicture[itemPosition.Length - 1].Width, ItemPicture[itemPosition.Length - 1].Height);

            for (int i = 0; i < itemPosition.Length; i++)
            {
                rectangle2 = new Rectangle((int)itemPosition[i].X, (int)itemPosition[i].Y,
                    ItemPicture[i].Width + 10, ItemPicture[i].Width + 10);

                if (rectangle1.Intersects(rectangle2))
                {
                    isCollide = true;
                    progressBar.Update(itemPosition[i], gameTime, (int)cursor.Position.X - 48,
                        (int)cursor.Position.Y + 50);

                    if (progressBar.isFull)
                    {
                        if (i != ItemPicture.Length - 1)
                        {
                            //string wa = allList[i].Item1.Substring(8, allList[i].Item1.Length - 13);
                            Learn learn = new Learn(engine, allList[i].Item1.Substring(8, allList[i].Item1.Length - 22), DanceName);
                            engine.manager.Switch(learn);
                        }
                        else
                        {
                            MainMenu menu = new MainMenu(engine);
                            engine.manager.Switch(menu);
                        }
                    }

                    return;
                }
                else
                {
                    isCollide = false;
                }
            }

            //if (rectangle1.Intersects(rectangle2))
            //{
            //    isCollide = true;
            //    progressBar.Update(new Vector2(10, Background.Height - 115),
            //        gameTime, (int)cursor.Position.X - 48,
            //        (int)cursor.Position.Y + 50);

            //    if (progressBar.isFull)
            //    {
            //        //MainMenu mainMenu = new MainMenu(engine);
            //        //engine.manager.Switch(mainMenu);

            //        Gallery gallery = new Gallery(engine);
            //        engine.manager.Switch(gallery);
            //    }

            //    return;
            //}
            //else
            //{
            //    isCollide = false;
            //}

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
