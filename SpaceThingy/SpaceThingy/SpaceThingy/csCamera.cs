using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceThingy
{
    public class csCamera
    {
        /// <summary>
        /// Position der Kamera.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 position;

        public void Update(GameTime gameTime)
        {
            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //KeyboardState kbState = Keyboard.GetState();

            //// Kamera bewegen
            //if (kbState.IsKeyDown(Keys.A))
            //{
            //    position.X += 50.0f * elapsed;
            //}
            //if (kbState.IsKeyDown(Keys.D))
            //{
            //    position.X -= 50.0f * elapsed;
            //}
            //if (kbState.IsKeyDown(Keys.W))
            //{
            //    position.Y += 50.0f * elapsed;
            //}
            //if (kbState.IsKeyDown(Keys.S))
            //{
            //    position.Y -= 50.0f * elapsed;
            //}

            //position = 
        }

        public Matrix GetMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(position.X + Game1.ScreenWidth/2, position.Y + Game1.ScreenHeight/2, 0));
        }
    }
}
