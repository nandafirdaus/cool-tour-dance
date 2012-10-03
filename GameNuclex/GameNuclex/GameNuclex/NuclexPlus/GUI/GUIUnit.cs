using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameNuclex.NuclexPlus.Core;
using Microsoft.Xna.Framework.Graphics;

namespace GameNuclex.NuclexPlus.GUI
{
    abstract public class GUIUnit
    {
        public GUIUnit() { }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
        public virtual void Update(GameTime gameTime) { }
    }
}
