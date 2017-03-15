using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceThingy
{
    public class csBullet
    {
        public bool FromPlayer;
        public Vector2 pos;
        public Texture2D text;
        public float Angle;
        public int speed;
        public Rectangle rec;
        public Vector2 direction;
        public int Damage;

        public bool isHoming;
        public csEnemy targetEnemy;
        public csPlayer targetPlayer;

        public double lifetimeCurr;
        public double lifetimeInMs;
        public bool alive;

        public csBullet()
        {
            direction = new Vector2(0, 0);
            speed = 60;
            alive = true;
            lifetimeCurr = 0;
        }

        public void Update(GameTime gt)
        {
            if (alive)
            {
                lifetimeCurr += gt.ElapsedGameTime.Milliseconds;

                if (lifetimeCurr > lifetimeInMs)
                {
                    alive = false;
                    return;
                }

                if (!isHoming)
                {
                    pos += direction * speed;
                }
                else
                {
                    if (FromPlayer)
                    {
                        //Follow target Enemy
                        if (targetEnemy.position.X > pos.X)
                            pos.X += speed / 4;
                        if (targetEnemy.position.X < pos.X)
                            pos.X -= speed / 4;

                        if (targetEnemy.position.Y > pos.Y)
                            pos.Y += speed / 4;
                        if (targetEnemy.position.Y < pos.Y)
                            pos.Y -= speed / 4;

                        //var x = targetX - this.x,
                        //y = targetY - this.y;
                        //return Math.atan2(x, -y);

                        Angle = (float)Math.Atan2(targetEnemy.position.X - pos.X, -(targetEnemy.position.Y - pos.Y));
                    }
                    else
                    {
                        //Follow Player
                    }
                }

                rec = new Rectangle((int)pos.X, (int)pos.Y, text.Width, text.Height);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (alive)
            {
                sb.Draw(text, new Vector2(pos.X + text.Width / 2, pos.Y + text.Height / 2), null, Color.White, Angle, new Vector2(text.Width / 2, text.Height / 2), 1f, SpriteEffects.None, 0);
            }
        }
    }
}
