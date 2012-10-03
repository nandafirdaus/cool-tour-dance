using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.Game.States;
using GameNuclex.NuclexPlus.Core;

namespace GameNuclex.NuclexPlus.GameFlow
{
    public abstract class Scene : DrawableGameState
    {
        protected Engine engine;
        
        public Scene(Engine engine) {
            this.engine = engine;
        }
    }
}
