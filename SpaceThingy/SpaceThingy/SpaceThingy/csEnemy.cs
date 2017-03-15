using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceThingy
{
    public class csEnemy
    {
        public Guid id;

        public Vector2 position;
        public Texture2D texture;
        public Rectangle rec;

        public double distance;
        public SpriteFont font;

        public Vector2 speed;
        public int maxSpeed;

        public int HealthCurr;
        public int HealthMax;

        public bool Alive
        {
            get { return HealthCurr > 0; }
        }

        public double Angle;

        public csEnemy()
        {
            id = Guid.NewGuid();

            HealthMax = 100;
            HealthCurr = HealthMax;
        }

        public bool IsChasing
        {
            get { return distance < 400; }
        }

        public void Update(Vector2 playerpos)
        {
            distance = Math.Sqrt((playerpos.X - position.X) * (playerpos.X - position.X) + (playerpos.Y - position.Y) * (playerpos.Y - position.Y));

            Angle = Math.Atan2(playerpos.X - position.X, -(playerpos.Y - position.Y));

            float thrust = 1;

            if (IsChasing)
            {
                if (playerpos.X < position.X)
                {
                    if (speed.X - thrust > -maxSpeed)
                        speed.X -= thrust;
                }
                if (playerpos.X > position.X)
                {
                    if (speed.X + thrust < maxSpeed)
                        speed.X += thrust;
                }

                if (playerpos.Y < position.Y)
                {
                    if (speed.Y - thrust > -maxSpeed)
                        speed.Y -= thrust;
                }
                if (playerpos.Y > position.Y)
                {
                    if (speed.Y + thrust < maxSpeed)
                        speed.Y += thrust;
                }
            }

            //if (Game1.bullets.Any(x => x.isHoming && x.targetEnemy.id == id && x.alive))
            //{
            //    csBullet bullet = Game1.bullets.First(x => x.isHoming && x.targetEnemy.id == id && x.alive);

            //    if (bullet.pos.X < position.X)
            //    {
            //        if (speed.X + thrust < maxSpeed)
            //            speed.X += thrust;
            //    }
            //    if (bullet.pos.X > position.X)
            //    {
            //        if (speed.X - thrust > -maxSpeed)
            //            speed.X -= thrust;
            //    }

            //    if (bullet.pos.Y < position.Y)
            //    {
            //        if (speed.Y + thrust < maxSpeed)
            //            speed.Y += thrust;
            //    }
            //    if (bullet.pos.Y > position.Y)
            //    {
            //        if (speed.Y - thrust > -maxSpeed)
            //            speed.Y -= thrust;
            //    }
            //}


            speed.X *= 0.98f;
            speed.Y *= 0.98f;

            position += speed;

            rec = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            List<csBullet> lst = Game1.bullets.Where(b => b.rec.Intersects(rec) && b.alive).ToList();
            foreach (csBullet b in lst)
            {
                b.alive = false;
                HealthCurr -= b.Damage;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Color txtColorDistance;
            if (IsChasing)
                txtColorDistance = Color.Red;
            else
                txtColorDistance = Color.Blue;

            Color txtColorHealth;
            if (HealthCurr > 80)
                txtColorHealth = Color.Green;
            else if (HealthCurr > 40)
                txtColorHealth = Color.Yellow;
            else
                txtColorHealth = Color.Red;

            sb.DrawString(font, "Health: " + HealthCurr, new Vector2((int)position.X, (int)position.Y - 70), txtColorHealth);
            sb.DrawString(font, "Distance: " + distance, new Vector2((int)position.X, (int)position.Y - 50), txtColorDistance);
            

            sb.Draw(texture, new Vector2(position.X + texture.Width / 2, position.Y + texture.Height / 2), null, Color.White, (float)Angle, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0);
        }
    }
}
