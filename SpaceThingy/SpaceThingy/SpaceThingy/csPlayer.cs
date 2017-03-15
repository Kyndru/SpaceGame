using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceThingy
{
    public class csPlayer
    {
        public Texture2D texture;
        public Vector2 pos;
        public Vector2 posBullet;

        public int MaxBoostSpeed;
        public double BoostTimeCurr;
        public double BoostTimeMax;
        public int MaxSpeed;
        public Vector2 speed;
        public float thrust;

        public int MaxRockets = 5;
        public int CurrRocketCount = 5;

        MouseState msOld;
        KeyboardState ksCurr;
        KeyboardState ksOld;

        double Shotinterval;
        double LastShot;

        double Rocketinterval;
        double LastRocket;

        double timeSinceLeavingStation;

        public csStation station;

        //for Collision
        public Rectangle rec;

        public float Rotation;

        public csPlayer()
        {
            LastShot = 0;
            Shotinterval = 250;

            LastRocket = 0;
            Rocketinterval = 2000;

            timeSinceLeavingStation = 0;
        }

        public void Shoot()
        {
            csBullet bullet = new csBullet();
            bullet.pos = posBullet;
            bullet.text = Game1.textureBullet1;
            bullet.Angle = Rotation;
            bullet.isHoming = false;
            bullet.FromPlayer = true;
            bullet.Damage = 5;
            bullet.lifetimeInMs = 2000;

            float test = MathHelper.ToDegrees(Rotation);

            float testRotation = MathHelper.ToRadians(test - 90);

            Vector2 direction = new Vector2((float)Math.Cos(testRotation), (float)Math.Sin(testRotation));
            bullet.direction = direction;

            Game1.bullets.Add(bullet);
        }

        public void ShootRocket()
        {
            if (CurrRocketCount == 0)
            {
                return;
            }

            CurrRocketCount--;

            csBullet bullet = new csBullet();
            bullet.pos = posBullet;
            bullet.text = Game1.textureRocket1;
            bullet.Angle = Rotation;
            bullet.isHoming = true;
            bullet.FromPlayer = true;
            bullet.Damage = 50;
            bullet.lifetimeInMs = 5000;

            bullet.targetEnemy = Game1.enemies.Where(x => x.Alive).OrderBy(y => y.distance).ToList()[0];

            Game1.bullets.Add(bullet);
        }

        public void Update(KeyboardState ks, MouseState ms, GameTime gt)
        {
            timeSinceLeavingStation += gt.ElapsedGameTime.Milliseconds;

            Vector2 mousepos = new Vector2(ms.X + Game1.ScreenWidth / 2, ms.Y + Game1.ScreenHeight / 2);

            ksCurr = ks;

            if (station != null)
            {
                timeSinceLeavingStation = 0;

                return;
            }

            csStation nearestStation = Game1.stations.OrderBy(e => e.distance).ToList()[0];
            if (nearestStation.distance <= 1200 && ksCurr.IsKeyDown(Keys.Space) && ksOld.IsKeyUp(Keys.Space) && timeSinceLeavingStation > 1000)
            {
                station = nearestStation;
                pos.X = nearestStation.position.X + (nearestStation.texture.Width / 2);
                pos.Y = nearestStation.position.Y + (nearestStation.texture.Height / 2);
                foreach (csMenuItem m in nearestStation.menuitems)
                    m.wasSelected = false;
                nearestStation.timeSinceInitializing = 0;

                csAnimation.execute(csAnimation.animationType.ZoomOut);

                return;
            }

            if (ms.LeftButton == ButtonState.Pressed && LastShot > Shotinterval)
            {
                Shoot();
                LastShot = 0;
            }
            else
            {
                LastShot += gt.ElapsedGameTime.Milliseconds;
            }

            if (msOld.RightButton == ButtonState.Released && ms.RightButton == ButtonState.Pressed)
            {
                ShootRocket();
                LastRocket = 0;
            }
            else
            {
                LastRocket += gt.ElapsedGameTime.Milliseconds;
            }

            if (ks.IsKeyDown(Keys.W))
            {
                ChangeSpeed(0, -thrust, gt);
            }
            if (ks.IsKeyDown(Keys.S))
            {
                ChangeSpeed(0, thrust, gt);
            }
            if (ks.IsKeyDown(Keys.A))
            {
                ChangeSpeed(-thrust, 0, gt);
            }
            if (ks.IsKeyDown(Keys.D))
            {
                ChangeSpeed(thrust, 0, gt);
            }

            if (ks.IsKeyDown(Keys.Space))
            {
                speed = new Vector2();
            }

            if ((!ks.IsKeyDown(Keys.W) && !ks.IsKeyDown(Keys.S)) || (!ks.IsKeyDown(Keys.LeftShift) && (speed.Y > MaxSpeed ||speed.Y < -MaxSpeed)))
            {
                if (speed.Y > -0.01 && speed.Y < 0.01)
                    speed.Y = 0;
                else
                    speed.Y *= 0.98f;
            }

            if ((!ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.D)) || (!ks.IsKeyDown(Keys.LeftShift) && (speed.X > MaxSpeed || speed.X < -MaxSpeed)))
            {
                if (speed.X > -0.01 && speed.X < 0.01)
                    speed.X = 0;
                else
                    speed.X *= 0.98f;
            }

            pos += speed;

            Game1.scrollX += (int) speed.X;
            Game1.scrollY += (int) speed.Y;

            rec = new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);


            var x = ms.X - Game1.ScreenWidth / 2 - texture.Width /2;
            var y = ms.Y - Game1.ScreenHeight / 2 - texture.Height / 2;

            Rotation = (float)Math.Atan2(x, -y);

            var x1 = (pos.X + texture.Width / 2) + 1 * Math.Cos(Rotation * Math.PI / 180);
            var y1 = (pos.Y + texture.Height / 2) + 20 * Math.Sin(Rotation * Math.PI / 180);
            posBullet = new Vector2((float)x1, (float)y1);

            ksOld = ks;
            msOld = ms;
        }

        private void ChangeSpeed(float x, float y, GameTime gt)
        {
            if (x < 0)
            {
                if (speed.X + x > -MaxSpeed || (ksCurr.IsKeyDown(Keys.LeftShift) && speed.X + x > -MaxBoostSpeed && BoostTimeCurr > 0))
                {
                    speed.X += x;
                }
            }
            else if (x > 0)
            {
                if (speed.X + x < MaxSpeed || (ksCurr.IsKeyDown(Keys.LeftShift) && speed.X + x < MaxBoostSpeed && BoostTimeCurr > 0))
                {
                    speed.X += x;
                }
            }

            if (y < 0)
            {
                if (speed.Y + y > -MaxSpeed || (ksCurr.IsKeyDown(Keys.LeftShift) && speed.Y + y > -MaxBoostSpeed && BoostTimeCurr > 0))
                {
                    speed.Y += y;
                }
            }
            else if (y > 0)
            {
                if (speed.Y + y < MaxSpeed || (ksCurr.IsKeyDown(Keys.LeftShift) && speed.Y + y < MaxBoostSpeed && BoostTimeCurr > 0))
                {
                    speed.Y += y;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, rec, Color.White);
        }
    }
}
