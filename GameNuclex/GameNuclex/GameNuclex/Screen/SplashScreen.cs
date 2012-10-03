using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nuclex.Game.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameNuclex.Object;

using GameNuclex.NuclexPlus;
using GameNuclex.NuclexPlus.Component;
using GameNuclex.NuclexPlus.Core;
using GameNuclex.NuclexPlus.GameFlow;
using System.Diagnostics;
namespace GameNuclex.Screen
{
    public class SplashScreen : Scene
    {


        public SplashScreen(Engine engine)
            : base(engine)
        {

        }

        Texture2D splashImage;
        NuclexTimer timer;

        protected override void OnEntered()
        {
            
            splashImage = base.engine.content.Load<Texture2D>("image/splash-screen");

            timer = new NuclexTimer(1000);
        }
        protected override void OnLeaving()
        {
            base.OnLeaving();
        }

        Vector2 place = new Vector2(0, 0);
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            engine.spriteBatch.Draw(splashImage, place, Color.White);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //place.X += 0.5f;
            //place.Y += 0.5f;
            //Trace.WriteLine(gameTime.TotalGameTime.TotalMilliseconds);
            if (timer.isTicked((long) (gameTime.TotalGameTime.TotalMilliseconds)))
            {
                MainMenu mainMenu = new MainMenu(engine);
                engine.manager.Switch(mainMenu);

                //Play play = new Play(engine);
                //engine.manager.Switch(play);

                //ScoreScreen score = new ScoreScreen(engine, 30, 25, 50, 14, 5);
                //engine.manager.Switch(score);
            }
        }
    }
}
