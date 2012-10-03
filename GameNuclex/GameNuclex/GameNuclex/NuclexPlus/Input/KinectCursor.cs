using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameNuclex.NuclexPlus.Core;

namespace GameNuclex.NuclexPlus.Input
{
    public class KinectCursor
    {
        Rectangle rectArea;
        
        Engine engine;
        public KinectCursor(Engine engine) {
            rectArea = new Rectangle(0, 0, 3, 3);
            this.engine = engine;
        }

        public KinectCursor(Engine engine, int x, int y) {
            rectArea = new Rectangle(x, y, 3, 3);
            this.engine = engine;
            
        }

        public Rectangle Area { 
            get { return rectArea; }
        }

    }
}
