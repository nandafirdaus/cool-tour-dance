using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Nuclex.Game.States;
using Microsoft.Xna.Framework.Audio;

namespace GameNuclex.NuclexPlus.Core
{
    public class Engine
    {
        Game _game;
        SpriteBatch _spriteBatch;
        ContentManager _content;
        GameStateManager _manager;
        GraphicsDevice _graphicsDevice;
        SoundEffectInstance _soundEffectInstance;
        public Engine(Game game)
        {
            this._game = game;
            _spriteBatch = this.game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            _content = this.game.Content;
            _manager = this.game.Services.GetService(typeof(GameStateManager)) as GameStateManager;
            _graphicsDevice = this.game.Services.GetService(typeof(GraphicsDevice)) as GraphicsDevice;
            _soundEffectInstance = this.game.Services.GetService(typeof(SoundEffectInstance)) as SoundEffectInstance;
        }

        public SoundEffectInstance soundEffect
        {
            get { return _soundEffectInstance; }
        }

        public SpriteBatch spriteBatch
        {
            get { return _spriteBatch; }
        }

        public ContentManager content
        {
            get { return _content; }
        }

        public GameStateManager manager {
            get { return _manager; }
        }

        public GraphicsDevice graphics
        {
            get { return _graphicsDevice; }
        }

        public Game game
        {
            get { return _game; }
        }
    }
}
