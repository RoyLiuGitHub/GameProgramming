using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;


namespace Assignment
{
    public partial class ModelManager : DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();

        //bullet  change to public for shots count
        List<BasicModel> shots = new List<BasicModel>();
        List<BasicModel> enemies = new List<BasicModel>();
        List<BasicModel> boundary = new List<BasicModel>();

        //Enemy spawn variables
        Vector3 maxSpawnLocation = new Vector3(500, 0, 500);
        int nextSpawnTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.Pi / 40;
        //Enemy count
        int enemiesThisLevel = 0;
        //Misses variables
        int missedThisLevel = 0;
        //Current level
        int currentLevel = 0;
        //list of LevelInfo objects
        List<LevelInfo> levelInfoList = new List<LevelInfo>();

        int maxX = 3000;
        int minX = -3000;
        int maxY = 1000;
        int minY = -1000;
        int maxZ = 3000;
        int minZ = -3000;

        // For font
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector2 scorePosition = new Vector2(400, 0);
        Vector2 timeLeftPosition = new Vector2(800, 0);
        Vector2 levelPosition = new Vector2(10, 0);
        Vector2 gameOverPosition = new Vector2(400, 300);

        // For time counter and score
        string score;
        string timeUsed;

        // Sound Effect
        public SoundEffect soundFX1;
        public SoundEffectInstance BGM;
        public SoundEffect soundFX2;
        public SoundEffectInstance shotSound;
        public SoundEffect soundFX3;
        public SoundEffectInstance explosionSound;
        public static SoundEffect soundFX4;
        public static SoundEffectInstance tankTrackSound;

        //remove bullet
        public void UpdateShots(GameTime gameTime)
        {

            // Loop through shots
            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot
                shots[i].Update(gameTime);

                Vector3 range = shots[i].GetWorldPublic().Translation - Tank.tankPosition;
                if (range.X > maxX || range.X < minX
                    || range.Y > maxY || range.Y < minY
                    || range.Z > maxZ || range.Z < minZ)
                {
                    shots.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    for (int j = 0; j < enemies.Count; ++j)
                    {
                        if (shots[i].CollidesWith(shots[i].model, (shots[i].GetWorldPublic() * 0.0000002f),
                            enemies[j].model, (enemies[j].GetWorldPublic() * 0.0000002f)))

                        {
                            // Collision! remove the tank and the shot.
                            enemies.RemoveAt(j);
                            shots.RemoveAt(i);
                            i -= 1;

                            //get score
                            PlayInfo.AddScore(1);

                            break;
                        }
                    }
                }
            }
        }

        public ModelManager(Game game) : base(game)
        {
            // Initialize game levels
            levelInfoList.Add(new LevelInfo(1000, 3000, 1, 2, 6, 10));
            levelInfoList.Add(new LevelInfo(900, 2800, 2, 2, 6, 9));
            levelInfoList.Add(new LevelInfo(800, 2600, 3, 2, 6, 8));
            levelInfoList.Add(new LevelInfo(700, 2400, 4, 3, 7, 7));
            levelInfoList.Add(new LevelInfo(600, 2200, 5, 3, 7, 6));
            levelInfoList.Add(new LevelInfo(500, 2000, 6, 3, 7, 5));
            levelInfoList.Add(new LevelInfo(400, 1800, 7, 4, 7, 4));
            levelInfoList.Add(new LevelInfo(300, 1600, 8, 4, 8, 3));
            levelInfoList.Add(new LevelInfo(200, 1400, 9, 5, 8, 2));
            levelInfoList.Add(new LevelInfo(100, 1200, 10, 5, 9, 1));
            levelInfoList.Add(new LevelInfo(50, 1000, 11, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 800, 12, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 600, 13, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(25, 400, 14, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(0, 200, 15, 18, 20, 0));
        }
        public override void Initialize()
        {
            //Set initial spawn time
            SetNextSpawnTime();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            models.Add(new Ground(
                Game.Content.Load<Model>(@"Models\Ground\Ground")));

            models.Add(new SkyBox(
                Game.Content.Load<Model>(@"Models\SkyBox\skybox")));

            models.Add(new Tank(
                Game.Content.Load<Model>(@"Models\Tank\tank"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera)
                );
            setBoundary();

            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Arial");


            //soundFX1 = Game.Content.Load<SoundEffect>(@"Sounds/BGM");
            //BGM = soundFX1.CreateInstance();
            //BGM.IsLooped = true;
            //BGM.Play();
            soundFX2 = Game.Content.Load<SoundEffect>(@"Sounds/shot");
            shotSound = soundFX2.CreateInstance();
            shotSound.IsLooped = false;
            //shotSound.Play();
            //soundFX3 = Game.Content.Load<SoundEffect>(@"Sounds/explosion");
            //explosionSound = soundFX3.CreateInstance();
            //explosionSound.IsLooped = false;
            ////explosionSound.Play();
            soundFX4 = Game.Content.Load<SoundEffect>(@"Sounds/tank_tracks");
            tankTrackSound = soundFX4.CreateInstance();
            tankTrackSound.IsLooped = false;
            //tankTrackSound.Play();

            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            // Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i)
            {
                models[i].Update(gameTime);
            }

            UpdateShots(gameTime);

            //update enemies
            for (int i = 0; i < enemies.Count; ++i)
            {
                enemies[i].Update(gameTime);
            }

            for (int i = 0; i < boundary.Count; ++i)
            {
                boundary[i].Update(gameTime);
            }


            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            PlayInfo.CalculateTime((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            //update time and score
            score = PlayInfo.GetScore().ToString();
            timeUsed = PlayInfo.getTime().ToString("0.0");

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            // Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }

            foreach (BasicModel s in shots)
            {
                s.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }

            foreach (BasicModel e in enemies)
            {
                e.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }
            foreach (BasicModel b in boundary)
            {
                b.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }

            ///Draw score and time
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Level: " + currentLevel+1, levelPosition, Color.YellowGreen);
            spriteBatch.DrawString(font, "Score: " + score + "/" + levelInfoList[currentLevel].numberEnemies, scorePosition, Color.YellowGreen);
            spriteBatch.DrawString(font, "Time: " + timeUsed, timeLeftPosition, Color.YellowGreen);
            if (LevelUp())
            {
                spriteBatch.DrawString(font, "Well Done!", gameOverPosition, Color.Red);
            }
            spriteBatch.End();

            //Reset device states
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        //When the first enemy spawn
        private void SetNextSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }
        //spawn a new enemy
        private void SpawnEnemy()
        {
            // Generate random position with random X and random Y
            // between -maxX and maxX and -maxY and maxY. Z is always
            // the same for all ships.
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X),
                0,
                ((Game1)Game).rnd.Next(-(int)maxSpawnLocation.Z, (int)maxSpawnLocation.Z));

            // Direction will always be (0, 0, Z), where
            // Z is a random value between minSpeed and maxSpeed
            Vector3 direction = new Vector3(0, 0,
                ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpeed,
                levelInfoList[currentLevel].maxSpeed));

            // Get a random roll rotation between -maxRollAngle and maxRollAngle
            float rollRotation = (float)((Game1)Game).rnd.NextDouble() *
                    maxRollAngle - (maxRollAngle / 2);

            // Add model to the list
            enemies.Add(new Enemy(
                Game.Content.Load<Model>(@"Models\Enemy\tank"),
                position, direction, 0, 0, rollRotation));


            // Increment # of enemies this level and set next spawn time
            ++enemiesThisLevel;
            SetNextSpawnTime();
        }
        // spawn a new enemy when the time is right
        protected void CheckToSpawnEnemy(GameTime gameTime)
        {
            // Time to spawn a new enemy?
            if (enemiesThisLevel <
                levelInfoList[currentLevel].numberEnemies)
            {
                timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawn > nextSpawnTime)
                {
                    SpawnEnemy();
                }
            }
        }

        //Add bullet
        public void AddShot(Vector3 position, Vector3 direction)
        {
            direction = Camera.getCameraDirection();
            direction.Y = 0;
            position.Y = 60;
            shots.Add(new Bullet(
                //Game.Content.Load<Model>(@"Models\Bullet\ammo"),
                Game.Content.Load<Model>(@"Models\Bullet\bullet"),
                position, direction*50, 0, 0, 0));
        }

        //Test game status
        private bool LevelUp()
        {
            if (PlayInfo.GetScore() == levelInfoList[currentLevel].numberEnemies)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void playShotSound()
        {
            shotSound.Play();
        }

        public void setBoundary()
        {
            int b = (int)Boundary.GetBoundary();
            for (int x = -1 * b; x <= b; x += 30)
            {
                boundary.Add(new Boundary(
                    Game.Content.Load<Model>(@"Models/Boundary/stone"),
                    new Vector3(x, 0, b)));

                boundary.Add(new Boundary(
                    Game.Content.Load<Model>(@"Models/Boundary/stone"),
                    new Vector3(x, 0, -b)));

                boundary.Add(new Boundary(
                    Game.Content.Load<Model>(@"Models/Boundary/stone"),
                    new Vector3(b, 0, x)));

                boundary.Add(new Boundary(
                    Game.Content.Load<Model>(@"Models/Boundary/stone"),
                    new Vector3(-b, 0, x)));
            }
        }
    }
}
