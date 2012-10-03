using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameNuclex.NuclexPlus.Component;

namespace GameNuclex.NuclexPlus.Animation
{
    public class Sprite
    {
        Texture2D Image;
        Vector2 Pos;
        float Scale;
        Rectangle[] FrameRect;

        public int Height
        {
            get { return FrameRect[0].Height; }
        }

        public int Width
        {
            get { return FrameRect[0].Width; }
        }

        public int SpriteSize
        {
            get { return FrameRect.Length; }
        }

        NuclexTimer Timer;

        public int FrameIdx;
        int TotalFrame;
        public Sprite(Texture2D image, int totalFrame, int fps) {
            Image = image;
            TotalFrame = totalFrame;
            FrameRect = new Rectangle[totalFrame];
            int width = image.Width / totalFrame, height = image.Height;
            for (int n = 0; n < totalFrame; n++) {
                FrameRect[n] = new Rectangle(width * n, 0, width, height);
            }

            Timer = new NuclexTimer(1000 / fps);

            Scale = 1f;
        }

        public Sprite(Texture2D image, int height, int width, int fps)
        {
            this.Pos = new Vector2(-200, 0);
            Image = image;
            int x = (image.Width / width);
            int y = (image.Height / height);
            TotalFrame = x * y;
            FrameRect = new Rectangle[x * y];

            for (int i = 0; i < y; i++)
            {
                for (int n = 0; n < x; n++)
                {
                    FrameRect[(i) * x + n] = new Rectangle(width * n, height * i, width, height);
                }
            }

            Timer = new NuclexTimer(1000 / fps);

            Scale = 1f;
        }

        public Sprite(Texture2D image, int height, int width, int x, int y, int fps, int totalFrame)
        {
            this.Pos = new Vector2(-200, 0);
            Image = image;
            TotalFrame = totalFrame;
            FrameRect = new Rectangle[totalFrame];

            for (int i = 0; i < y; i++)
            {
                for (int n = 0; n < x; n++)
                {
                    if (i != y || n != totalFrame - 1)
                    {
                        FrameRect[(i) * x + n] = new Rectangle(width * n, height * i, width, height); 
                    }
                }
            }

            Timer = new NuclexTimer(1000 / fps);

            Scale = 1f;
        }

        public void SetPosition(int X, int Y) {
            this.Pos.X = X;
            this.Pos.Y = Y;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            
            spriteBatch.Draw(Image, Pos, FrameRect[FrameIdx], Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            
        }

        public void Update(GameTime gameTime) {
            if (Timer.isTicked(gameTime)) {
                FrameIdx = ((FrameIdx+1) >= TotalFrame) ? TotalFrame-1 : FrameIdx + 1;
            }
        }
    }
}
