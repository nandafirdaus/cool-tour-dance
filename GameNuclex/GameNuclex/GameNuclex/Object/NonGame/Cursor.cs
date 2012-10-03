using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameNuclex.Object.NonGame
{
    class Cursor
    {
        #region Property

        private Texture2D Texture;
        public Vector2 Position;

        public int Width
        {
            get { return Texture.Width; }
        }

        public int Height
        {
            get { return Texture.Height; }
        }

        #endregion Property

        public void Initialize(Texture2D texture)
        {
            this.Texture = texture;
            this.Position = new Vector2();
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, Position, null, Color.White, 0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
    }
}
