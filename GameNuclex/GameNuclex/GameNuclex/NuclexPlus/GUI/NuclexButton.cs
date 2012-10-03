using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameNuclex.NuclexPlus.Input;
using GameNuclex.NuclexPlus.Animation;

namespace GameNuclex.NuclexPlus.GUI
{
    public class NuclexButton : GUIUnit
    {
        Rectangle rectArea;
        Vector2 position;
        Texture2D texture;
        Sprite sprite;
        public NuclexButton(int x, int y, Texture2D texture) {

            rectArea = new Rectangle(x, y, texture.Width, texture.Height);
            position = new Vector2(x, y);
            this.texture = texture;
        }

        public bool OnHoverCondition(KinectCursor cursor) {
            return (rectArea.Contains(cursor.Area));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (sprite != null) 
            {
                
            }
            else if (texture != null) 
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
}
