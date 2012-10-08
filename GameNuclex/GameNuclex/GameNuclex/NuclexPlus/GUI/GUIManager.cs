using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameNuclex.NuclexPlus.Core;
using Microsoft.Xna.Framework;

namespace GameNuclex.NuclexPlus.GUI
{
    public class GUIManager
    {
        List<GUIUnit> guiList;
        protected Engine engine;
        public GUIManager(Engine engine) {
            this.engine = engine;
            guiList = new List<GUIUnit>(10);
        }

        public void AddGUIUnit(GUIUnit unit) {
            guiList.Add(unit);
        }
        public void RemoveGUIUnit(GUIUnit unit) {
            guiList.Remove(unit);
        }
        public void RemoveGUIUnitAt(int idx) {
            guiList.RemoveAt(idx);
        }

        public void Draw(GameTime gameTime) {
            
            for (int nn = 0; nn < guiList.Count; nn++) {
                guiList[nn].Draw(engine.spriteBatch, gameTime);
            }
            
        }
        public void Update(GameTime gameTime) {
            for (int nn = 0; nn < guiList.Count; nn++) {
                guiList[nn].Update(gameTime);
            }
        }

    }
}
