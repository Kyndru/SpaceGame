using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceThingy
{
    public class csStation
    {
        public Guid id;

        public string name = "";

        public Vector2 position;
        public Texture2D texture;
        public Rectangle rec;

        public double Angle = 0;
        public double distance;

        KeyboardState ksOld;
        public double timeSinceInitializing = 0;

        int currSelectedItem = 0;
        public List<csMenuItem> menuitems = new List<csMenuItem>();

        public void Update(Vector2 playerpos, KeyboardState ks, GameTime gt)
        {
            timeSinceInitializing += gt.ElapsedGameTime.Milliseconds;

            distance = Math.Sqrt((playerpos.X - position.X) * (playerpos.X - position.X) + (playerpos.Y - position.Y) * (playerpos.Y - position.Y));
            rec = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            if (ksOld.IsKeyUp(Keys.S) && ks.IsKeyDown(Keys.S))
            {
                if (currSelectedItem + 1 < menuitems.Count)
                    currSelectedItem++; 
            }
            if (ksOld.IsKeyUp(Keys.W) && ks.IsKeyDown(Keys.W))
            {
                if (currSelectedItem - 1 >= 0)
                    currSelectedItem--;
            }

            if (ksOld.IsKeyUp(Keys.Space) && ks.IsKeyDown(Keys.Space) && timeSinceInitializing > 500)
            {
                menuitems[currSelectedItem].selected();
            }

            ksOld = ks;
        }

        public void Draw(SpriteBatch sb)
        {
            //sb.Draw(texture, new Vector2(position.X + (texture.Width / 2), position.Y + (texture.Height / 2)), null, Color.White, (float)Angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0);
            sb.Draw(texture, new Vector2(rec.Center.X, rec.Center.Y), null, Color.White, (float)Angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0);
        }

        public void showMenu(SpriteBatch sb)
        {
            sb.DrawString(Game1.fontbig, this.name, new Vector2(Game1.player.pos.X - Game1.player.texture.Width / 2 + 500, Game1.player.pos.Y - Game1.player.texture.Height / 2 - 60), Color.Green);

            for (int i = 0; i < menuitems.Count; i++)
            {
                sb.DrawString(Game1.fontbig, menuitems[i].displaytext, new Vector2(Game1.player.pos.X - Game1.player.texture.Width / 2 + 500, Game1.player.pos.Y - Game1.player.texture.Height / 2 + (40 * i)), i == currSelectedItem ? Color.Red : Color.White);
            }
        }
    }
}
