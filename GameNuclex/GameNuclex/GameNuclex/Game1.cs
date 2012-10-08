using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Nuclex.UserInterface;
using Nuclex.Game.States;
using GameNuclex.Screen;
using GameNuclex.Object;
using GameNuclex.NuclexPlus.Core;
using GameNuclex.NuclexPlus.Component;

namespace GameNuclex
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameStateManager manager;
        SoundEffect soundEffect;
        SoundEffectInstance soundEffectInstance;

        NuclexKinect nucKinect;

        Engine engine;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            // Set Static Reference for future use //
            Object.Static.Content = Content;
            Object.Static.Services = Services;
            Object.Static.SpriteBatch = spriteBatch;

            soundEffect = Content.Load<SoundEffect>("Sound/Backsound");
            soundEffectInstance = soundEffect.CreateInstance();

            nucKinect = new NuclexKinect(this.GraphicsDevice);

            soundEffectInstance.IsLooped = true;

            // Init Manager //
            manager = new GameStateManager(this.Services);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);
            this.Services.AddService(typeof(GameStateManager), manager);
            this.Services.AddService(typeof(GraphicsDevice), this.GraphicsDevice);
            this.Services.AddService(typeof(SoundEffectInstance), this.soundEffectInstance);
            this.Services.AddService(typeof(NuclexKinect), this.nucKinect);
            engine = new Engine(this);

            SplashScreen splash = new SplashScreen(engine);
            manager.Switch(splash);

            //ScoreScreen scoreScreen = new ScoreScreen(engine, 200000, 50,
            //        10, 20, 50, "Tari Lenggang Nyai");
            //engine.manager.Switch(scoreScreen);

            //PauseMenu pause = new PauseMenu(engine);
            //manager.Switch(pause);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
            manager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            base.Draw(gameTime);
            manager.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
