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

namespace SpaceThingy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Texture2D[,] worldtextures;
        bool definetextures;
        public static csPlayer player;
        public static List<csEnemy> enemies;
        public static List<csBullet> bullets = new List<csBullet>();
        public static List<csStation> stations = new List<csStation>();
        public static SpriteFont font;
        public static SpriteFont fontbig;
        //public static csAnimation animationHandler;
        public static float zoomScaling;

        csCamera camera;
        Camera2D cam;

        KeyboardState ksOld;

        Texture2D textureBackground1;
        Texture2D textureBackground2;
        Texture2D textureBackground3;
        Texture2D textureBackground4;
        public static Texture2D textureBullet1;
        public static Texture2D textureRocket1;
        Texture2D textureArrow;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int scrollX;
        public static int scrollY;

        public static int ScreenWidth;
        public static int ScreenHeight;

        float scaling = 1f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cam = new Camera2D();
            zoomScaling = 1f;
            
            player = new csPlayer();
            player.pos = new Vector2(250000, 250000);// new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            player.texture = Content.Load<Texture2D>("ships/ship1");
            player.MaxSpeed = 26;
            player.MaxBoostSpeed = 510;
            player.thrust = 1;
            player.BoostTimeMax = 10000;
            player.BoostTimeCurr = player.BoostTimeMax;

            definetextures = true;
            worldtextures = new Texture2D[500300 / 300, 500300 / 300];

            enemies = new List<csEnemy>();

            Random r = new Random();
            int enemycount = r.Next(500, 1000);

            for (int i = 0; i < enemycount; i++)
            {
                csEnemy enemy = new csEnemy();
                enemy.position = new Vector2(r.Next(0,500000), r.Next(0,500000));
                enemy.texture = Content.Load<Texture2D>("ships/ship2");
                enemy.font = Content.Load<SpriteFont>("fonts/text");
                enemy.maxSpeed = 21;
                enemies.Add(enemy);
            }

            stations = new List<csStation>();
            int stationcount = 10;

            List<csMenuItem> lstMenu = new List<csMenuItem>();

            csMenuItem item = new csMenuItem();
            item.text = "Open Shop";
            item.textafteraction = "Not implemented yet";
            item.eventItem = csEvent.events.OpenShop;
            

            csMenuItem item2 = new csMenuItem();
            item2.text = "Refill Rockets";
            item2.textafteraction = "Refilled!";
            item2.eventItem = csEvent.events.RefreshRocketAmmo;
            

            csMenuItem item3 = new csMenuItem();
            item3.text = "Leave Station";
            item3.textafteraction = "Leave Station";
            item3.eventItem = csEvent.events.ReleasePlayerFromStation;

            lstMenu.Add(item);
            lstMenu.Add(item2);
            lstMenu.Add(item3);

            for (int i = 0; i < stationcount; i++)
            {
                csStation station = new csStation();
                station.position = new Vector2(r.Next(0, 500000), r.Next(0, 500000));
                station.name = "Spartan Mk " + i;
                station.texture = Content.Load<Texture2D>("stations/spartan");
                station.menuitems = lstMenu;
                stations.Add(station);
            }

            textureBackground1 = Content.Load<Texture2D>("backgrounds/background1");
            textureBackground2 = Content.Load<Texture2D>("backgrounds/background2");
            textureBackground3 = Content.Load<Texture2D>("backgrounds/background3");
            textureBackground4 = Content.Load<Texture2D>("backgrounds/background4");
            textureBullet1 = Content.Load<Texture2D>("weapons/bullet1");
            textureRocket1 = Content.Load<Texture2D>("weapons/rocket1");
            textureArrow = Content.Load<Texture2D>("tools/arrow");
            fontbig = Content.Load<SpriteFont>("fonts/textbig");
            camera = new csCamera();


            for (int ix = 0; ix < 500000; ix += 300)
            {
                for (int iy = 0; iy < 500000; iy += 300)
                {
                    Texture2D txt;

                    txt = worldtextures[ix / 300, iy / 300];

                    if (txt == null)
                    {
                        int val = r.Next(0, 100);
                        if (val > 50)
                            txt = textureBackground2;
                        else if (val > 5)
                            txt = textureBackground1;
                        else
                            txt = textureBackground3;

                        worldtextures[ix / 300, iy / 300] = txt;
                    }
                }
            }


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

            font = Content.Load<SpriteFont>("fonts/text");
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

            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            player.Update(ks, ms, gameTime);

            if (ksOld.IsKeyUp(Keys.Z) && ks.IsKeyDown(Keys.Z))
            {
                csAnimation.execute(csAnimation.animationType.ZoomOut);
            }

            if (ksOld.IsKeyUp(Keys.U) && ks.IsKeyDown(Keys.U))
            {
                csAnimation.execute(csAnimation.animationType.ZoomIn);
            }

            foreach (csEnemy e in enemies.Where(x => x.Alive))
            {
                e.Update(player.pos);
            }

            foreach (csStation s in stations)
            {
                s.Update(player.pos, ks, gameTime);
            }

            foreach (csBullet b in bullets)
            {
                b.Update(gameTime);
            }

            //drawMinX = player.pos.X / 300 - 600;
            //drawMaxX = player.pos.X / 300 + 600;
            //drawMinY = player.pos.Y / 300 - 600;
            //drawMaxY = player.pos.Y / 300 + 600;

            ksOld = ks;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.White);

            //float scaling = 1f;
            //if (player.station == null)
            //    scaling = 1f;
            //else
            //    scaling = 0.5f;
            
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(-player.pos.X + Game1.ScreenWidth / 2, -player.pos.Y + Game1.ScreenHeight / 2, 0)) * Matrix.CreateScale(zoomScaling));


            Random r = new Random();

            for(int ix = 0; ix < 500000; ix += 300)
            {
                for (int iy = 0; iy < 500000; iy += 300)
                {
                    Texture2D txt;

                    int DrawRadius = 2000;

                    if (ix > player.pos.X - DrawRadius && ix < player.pos.X + DrawRadius && iy > player.pos.Y - DrawRadius && iy < player.pos.Y + DrawRadius)
                    {
                        txt = worldtextures[ix / 300, iy / 300];

                        spriteBatch.Draw(txt, new Rectangle(ix, iy, textureBackground1.Width, textureBackground1.Height), Color.White);
                    }
                }
            }
            
            //definetextures = false;

            foreach (csEnemy e in enemies.Where(x => x.Alive))
            {
                if (e.distance < 10000)
                    e.Draw(spriteBatch);
            }

            foreach (csBullet b in bullets)
            {
                b.Draw(spriteBatch);
            }

            spriteBatch.Draw(player.texture, new Vector2(player.pos.X + player.texture.Width / 2, player.pos.Y + player.texture.Height / 2), null, Color.White, player.Rotation, new Vector2(player.texture.Width / 2, player.texture.Height / 2), 1f, SpriteEffects.None, 0);

            foreach (csStation s in stations)
            {
                if (s.distance < 10000)
                    s.Draw(spriteBatch);
            }

            //csEnemy nearestEnemy = enemies.Where(x => x.Alive).OrderBy(e => e.distance).ToList()[0];
            csStation nearestStation = stations.OrderBy(e => e.distance).ToList()[0];
            //var arrowAngle = Math.Atan2(nearestEnemy.position.X - player.pos.X, -(nearestEnemy.position.Y - player.pos.Y));
            var arrowAngle = Math.Atan2(nearestStation.rec.Center.X - player.pos.X, -(nearestStation.rec.Center.Y - player.pos.Y));
            var arrowX = player.pos.X + player.texture.Width/2 * Math.Cos(arrowAngle * Math.PI / 180);
            var arrowY = player.pos.Y + 1 * Math.Sin(arrowAngle * Math.PI / 180);
            spriteBatch.Draw(textureArrow, new Vector2((float)arrowX, (float)arrowY - 80), null, Color.White, (float)arrowAngle, new Vector2(textureArrow.Width / 2, textureArrow.Height / 2), 1f, SpriteEffects.None, 0);
            
            spriteBatch.DrawString(font, "Pos X:" + player.pos.X + " Y:" + player.pos.Y, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 200), Color.Blue);
            spriteBatch.DrawString(font, "Speed X:" + player.speed.X + " Y:" + player.speed.Y, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 180), Color.Blue);
            spriteBatch.DrawString(font, "Rotation: " + player.Rotation, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 160), Color.Blue);
            spriteBatch.DrawString(font, "Enemies: " + enemies.Where(x => x.Alive).ToList().Count, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 140), Color.Blue);
            spriteBatch.DrawString(font, "Rockets: " + player.CurrRocketCount + "/" + player.MaxRockets, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 120), Color.Blue);
            //spriteBatch.DrawString(font, "Nearest Enemy: " + nearestEnemy.distance, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 120), Color.Blue);
            spriteBatch.DrawString(font, "Nearest Station: " + nearestStation.distance, new Vector2(player.pos.X - player.texture.Width / 2, player.pos.Y - player.texture.Height / 2 - 100), Color.Blue);


            if (player.station != null)
            {
                nearestStation.showMenu(spriteBatch);
            }
            else
            {
                if (nearestStation.distance <= 1200)
                    spriteBatch.DrawString(font, "Press Space to Enter Station \"" + nearestStation.name + "\"", new Vector2(player.pos.X - player.texture.Width / 2 - 80, player.pos.Y - player.texture.Height / 2 - 40), Color.Red);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
