using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameNuclex.NuclexPlus.Animation;

namespace GameNuclex.Object.NonGame
{
    class ProgressBar
    {
        Texture2D Texture;
        Sprite sprite;
        Vector2 currentObject;
        public bool isFull;

        public int Height
        {
            get { return sprite.Height; }
        }

        public int Width
        {
            get { return sprite.Width; }
        }

        public void Initialize(Texture2D texture)
        {
            this.Texture = texture;
            sprite = new Sprite(texture, 42, 140, 15);
            //sprite = new Sprite(texture, 47, 47, 7, 9, 15, 60);
        }

        public void Update(Vector2 menu, GameTime time, int x, int y)
        {
            sprite.SetPosition(x, y);

            if (currentObject != menu)
            {
                sprite = new Sprite(Texture, 42, 140, 15);
            }

            currentObject = menu;

            if (sprite.SpriteSize - 1 == sprite.FrameIdx)
            {
                isFull = true;
            }
            else
            {
                isFull = false;
            }

            sprite.Update(time);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
                sprite.Draw(spriteBatch, gameTime);
        }
    }
}
