using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Assignment
{
    
    class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();
        List<BasicModel> modelsObstacle = new List<BasicModel>();
        List<BasicModel> modelsObstacleFirm = new List<BasicModel>();
        List<BasicModel> hampers = new List<BasicModel>();
        List<BasicModel> shots = new List<BasicModel>();
        List<BasicModel> enemyShots = new List<BasicModel>();

        List<BasicModel> enemies = new List<BasicModel>();

        List<BasicModel> enemies2 = new List<BasicModel>();

        Tank tankModel;
        PatrolEnemy penemy;

        int[,] graph_max_other;
        int[,] graph_max_player;

        Texture2D[] skyboxTextures;
        Model skyboxModel;

        Vector3 npc_position;
        Vector3 player_position;
        Vector3 player_dis;
        Vector3 player_dis_next = Vector3.Zero;
        Vector3 npc_dis;
        string text;

        private bool bPursue;
        private bool bEvade;

        

        MousePick mousepick;
        Vector3? pickposition;
        private int boundaryWidth = 40;
        private int leftBoundary = 1800;
        private int rightBoundary = -2200;

        private const int maxX = 3000;
        private const int minX = -3000;
        private const int maxY = 1000;
        private const int minY = -1000;
        private const int maxZ = 3000;
        private const int minZ = -3000;

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

        // For font
        SpriteBatch spriteBatch;
        SpriteFont font;
        Vector2 scorePosition = new Vector2(400, 0);
        Vector2 timeLeftPosition = new Vector2(800, 0);
        Vector2 levelPosition = new Vector2(10, 0);
        Vector2 gameOverPosition = new Vector2(400, 300);
        Vector2 lifePosition = new Vector2(800, 0);
        // For time counter and score
        string score;
        string timeUsed;
        string life;
        //bullet profile
        private const float shotSpeed = 0.01f;
        private const int shotDelay = 2700;
        int shotCountdown = 0;
        public Vector3 bulletPosition = Vector3.Zero;
        public Vector3 bulletDirection = Vector3.Zero;
        Vector3 temp = Vector3.Zero;

        // Sound Effect
        public SoundEffect soundFX1;
        public SoundEffectInstance BGM;
        public SoundEffect soundFX2;
        public SoundEffectInstance shotSound;
        public SoundEffect soundFX3;
        public SoundEffectInstance shotSound2;
        public static SoundEffect soundFX4;
        public static SoundEffectInstance tankTrackSound;

        bool destoryed;
        Astra instanceAstra;
        public ModelManager(Game game)
            : base(game)
        {

            bPursue = false;
            bEvade = false;
            instanceAstra = new Astra();

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
            SetNextSpawnTime();
            text = "FSM: \n";
            instanceAstra.readMapInformation();
            PlayInfo.initLife(3);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            models.Add(new Ground(
               Game.Content.Load<Model>(@"Models/Ground/Ground")));

            Model skyModel = Game.Content.Load<Model>(@"Models/SkyBox/skybox");
            models.Add(new SkyBox(skyModel));

            skyboxTextures = new Texture2D[skyModel.Meshes.Count];
            int [,] a = instanceAstra.retMapInformation();
            tankModel = new Tank(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, a, instanceAstra.retRow(), instanceAstra.retCol());
            
            models.Add(tankModel);

            
            addBoundary();


            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Arial");
            soundFX1 = Game.Content.Load<SoundEffect>(@"Sounds/Explosion");
            BGM = soundFX1.CreateInstance();
            BGM.IsLooped = true;
            BGM.Play();
            soundFX2 = Game.Content.Load<SoundEffect>(@"Sounds/shot");
            shotSound = soundFX2.CreateInstance();
            shotSound.IsLooped = false;
            soundFX3 = Game.Content.Load<SoundEffect>(@"Sounds/shot2");
            shotSound2 = soundFX3.CreateInstance();
            shotSound2.IsLooped = false;
            soundFX4 = Game.Content.Load<SoundEffect>(@"Sounds/tank_tracks");
            tankTrackSound = soundFX4.CreateInstance();
            tankTrackSound.IsLooped = false;
            base.LoadContent();
        }

        public int getPatrolNumber()
        {
            return enemies2.Count;
        }

        public override void Update(GameTime gameTime)
        {
            
            foreach (BasicModel model in models)
                {
                if (tankModel.getIsAuto() == false)
                {
                    if (!Collide())
                    {
                    }
                    else
                    {
                        model.stop();
                    }
                }

                CollideEnemy();
                model.update(gameTime);
            }

            UpdateShots(gameTime);
            enemyShot(gameTime);
            UpdateEnemyShots(gameTime);

            for (int i = 0; i < enemies.Count; ++i)
            {
                enemies[i].update(gameTime);
            }

            for (int i = 0; i < enemies2.Count; ++i)
            {
                enemies2[i].update(gameTime);
            }
            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            PlayInfo.CalculateTime((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            //update time and score
            score = PlayInfo.GetScore().ToString();
            life = PlayInfo.GetLife().ToString();
            timeUsed = PlayInfo.getTime().ToString("0.0");
            base.Update(gameTime);
        }



        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (BasicModel model in models)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }

            foreach (BasicModel model in modelsObstacle)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }
            foreach (BasicModel model in modelsObstacleFirm)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }
            foreach (BasicModel s in shots)
            {
                s.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }
            foreach (BasicModel s1 in enemyShots)
            {
                s1.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }
            foreach (BasicModel e in enemies)
            {
                e.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }
            foreach (BasicModel e2 in enemies2)
            {
                e2.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }

            ///Draw score and time

            spriteBatch.Begin();

            int temp1 = currentLevel;
            temp1++;
            int temp2 = levelInfoList[currentLevel].numberEnemies;
            temp2++;

            spriteBatch.DrawString(font, "Level: " + temp1, levelPosition, Color.YellowGreen);
            spriteBatch.DrawString(font, "Score: " + score + "/" + temp1, scorePosition, Color.YellowGreen);
            spriteBatch.DrawString(font, "Life: " + life, lifePosition, Color.YellowGreen);
            if (LevelUp())
            {
                spriteBatch.DrawString(font, "Well Done!", gameOverPosition, Color.Red);
            }
            if (isDied())
            {
                spriteBatch.DrawString(font, "You are dead!", gameOverPosition, Color.Red);
            }

            spriteBatch.End();

            //Reset device states
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        public void addBoundary()
        {

            //bottom line
            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(-2200 + ik * boundaryWidth, 0, -1000)));
            }

            //left
            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary, 0, ik * boundaryWidth - 1000)));
            }

            //one of 15
            for (int ik = 0; ik < 20; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 20 * boundaryWidth, 0, 2200 - 20 * boundaryWidth - ik * boundaryWidth)));
            }


            //five of 15
            for (int ik = 0; ik < 10; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 30 * boundaryWidth, 0, 2200 - 20 * boundaryWidth - ik * boundaryWidth)));
            }

            //second five of 15
            for (int ik = 0; ik < 10; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 39 * boundaryWidth, 0, 2200 - 30 * boundaryWidth - ik * 40)));
            }

            //first five of 15
            for (int ik = 0; ik < 10; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 30 * boundaryWidth - ik * boundaryWidth, 0, 2200 - 20 * 40)));
            }

            //second strike five of 15
            for (int ik = 0; ik < 10; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 30 * boundaryWidth - ik * boundaryWidth, 0, 2200 - 30 * 40)));
            }

            //third strike five of 15
            for (int ik = 0; ik < 10; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 30 * boundaryWidth - ik * boundaryWidth, 0, 2200 - 40 * 40)));
            }
            //top
            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(ik * 40 + rightBoundary, 0, 2200)));
            }

            //right
            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(rightBoundary, 0, ik * 40 - 1000)));
            }
        }

        public Vector3 getPointRowColTest(Grid g)
        {

            Grid retg = g;
            Vector3 retv3 = getPointCoodTest(retg);

            return retv3;
        }
        public Vector3 getPointCoodTest(Grid rc)
        {
            Vector3 retv = new Vector3();

            retv.Y = 0f;

            for (int r = 0; r < 85; r++)
            {
                if (rc.row == r)
                {
                    retv.Z = 2200 - 40 * r - 20;
                    break;
                }
            }

            for (int c = 0; c < 100; c++)
            {
                if (rc.col == c)
                {
                    retv.X = leftBoundary - 40 * c - 20;
                }
            }

            return retv;
        }

        protected bool CollideEnemy()
        {
            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                for (int i = modelsObstacleFirm.Count - 1; i >= 0; i--)
                {
                    if (modelsObstacleFirm[i].CollidesWith(
                                    modelsObstacleFirm[i].model,
                                    (modelsObstacleFirm[i].getWorld()),
                                    enemies[j].model,
                                    (enemies[j].getWorld())))
                    {
                        enemies[j].setBoolCollision();
                        return true;
                    }
                }
            }
            return false;
        }
        protected bool Collide()
        {
            for (int i = modelsObstacleFirm.Count - 1; i >= 0; i--)
            {
                if (modelsObstacleFirm[i].CollidesWith(
                                modelsObstacleFirm[i].model,
                                (modelsObstacleFirm[i].getWorld()),
                                tankModel.model,
                                (tankModel.getWorld())))
                {
                    return true;
                }
            }
            

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].CollidesWith(
                                enemies[i].model,
                                (enemies[i].getWorld()),
                                tankModel.model,
                                (tankModel.getWorld())))
                {
                    return true;
                }
            }

            for (int i = enemies2.Count - 1; i >= 0; i--)
            {
                if (enemies2[i].CollidesWith(
                                enemies2[i].model,
                                (enemies2[i].getWorld()),
                                tankModel.model,
                                (tankModel.getWorld())))
                {
                    return true;
                }
            }

            
            
            return false;
        }

        public void stopPlayer()
        {
            tankModel.setModelSpeed(0f);
        }

        private string curModel;

        /// Add bullet
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public void AddShot(Vector3 position, Vector3 direction)
        {
            direction = tankModel.getTankDirection();
            direction.Y = 0;
            position.Y = 30;    //bullet heigth
            shots.Add(new Bullet(
                Game.Content.Load<Model>(@"Models\Bullet\bullet"),
                position, direction * 50, 0, 0, 0));
        }
        public void UpdateShots(GameTime gameTime)
        {
            destoryed = false;
            // Loop through shots
            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot
                shots[i].update(gameTime);

                Vector3 range = shots[i].getWorld().Translation - Tank.tankPosition;
                if (range.X > maxX || range.X < minX
                    || range.Y > maxY || range.Y < minY
                    || range.Z > maxZ || range.Z < minZ)
                {
                    shots.RemoveAt(i);
                    i -= 1;
                    destoryed = true;
                }
                else
                {
                    for (int j = 0; j < enemies.Count; ++j)
                    {
                        if (shots[i].CollidesWith(
                            shots[i].model,
                            (shots[i].getWorld()),
                            enemies[j].model,
                            (enemies[j].getWorld())))
                        {
                            // Collision! remove the tank and the shot.
                            enemies.RemoveAt(j);
                            shots.RemoveAt(i);
                            i -= 1;
                            destoryed = true;
                            //get score
                            PlayInfo.AddScore(1);
                            break;
                        }
                    }

                    if (destoryed == false)
                    {
                        for (int j = 0; j < enemies2.Count; ++j)
                        {
                            if (shots[i].CollidesWith(
                                shots[i].model,
                                (shots[i].getWorld()),
                                enemies2[j].model,
                                (enemies2[j].getWorld())))
                            {
                                // Collision! remove the tank and the shot.
                                enemies2.RemoveAt(j);
                                shots.RemoveAt(i);
                                i -= 1;
                                destoryed = true;
                                //get score
                                PlayInfo.AddScore(1);
                                break;
                            }
                        }
                    }

                }
            }
        }
        private void SetNextSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }
        private void SpawnEnemy()
        {
            // Generate random position with random X and random Y
            // between -maxX and maxX and -maxY and maxY. Z is always
            // the same for all ships.
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, ((int)maxSpawnLocation.X+800)%1800),
                0,
                ((Game1)Game).rnd.Next(-(int)maxSpawnLocation.Z, (int)maxSpawnLocation.Z));

            // Direction will always be (0, 0, Z), where
            // Z is a random value between minSpeed and maxSpeed
            Vector3 direction = new Vector3(0, 0,
                ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpeed,
                levelInfoList[currentLevel].maxSpeed));

            position = new Vector3(-1200, 0, 1200);

            // Get a random roll rotation between -maxRollAngle and maxRollAngle
            float rollRotation = (float)((Game1)Game).rnd.NextDouble() *
                    maxRollAngle - (maxRollAngle / 2);

            // Add model to the list
            int[,] a = instanceAstra.retMapInformation();
            enemies.Add(new NpcTank(
                Game.Content.Load<Model>(@"Models/Enemy/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, a, instanceAstra.retRow(), instanceAstra.retCol(), position, direction));

            // Increment # of enemies this level and set next spawn time
            ++enemiesThisLevel;
            SetNextSpawnTime();
        }
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

        private bool isDied()
        {
            if (PlayInfo.GetLife() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void enemyShot(GameTime gameTime)
        {
            foreach (BasicModel model in enemies)
            {
                Vector3 tempPosition = model.GetModelPosition();
                Vector3 tempDirection = model.GetTankDirection();
                enemyFireShots(gameTime, tempPosition, tempDirection);
            }
        }


        protected void enemyFireShots(GameTime gameTime, Vector3 bulletPosition, Vector3 bulletDirection)
        {

            if (shotCountdown <= 0)
            {

                    this.bulletDirection = tankModel.getPosition()- bulletPosition;
                    this.bulletDirection.Y = 20;
                    this.bulletDirection.Normalize();
                    // Add a shot to the model manager
                    AddEnemyShot(
                       bulletPosition,
                        this.bulletDirection * shotSpeed);

                playEnemyShotSound();

                    // Reset the shot countdown
                shotCountdown = shotDelay;
                //}
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }
        public void AddEnemyShot(Vector3 position, Vector3 direction)
        {

            direction.Y = 0;
            position.Y = 30;    //bullet heigth
            enemyShots.Add(new Bullet(
                Game.Content.Load<Model>(@"Models\Bullet\bullet"),
                position, direction * 50, 0, 0, 0));
        }
        public void UpdateEnemyShots(GameTime gameTime)
        {
            // Loop through shots
            for (int i = 0; i < enemyShots.Count; ++i)
            {
                // Update each shot
                enemyShots[i].update(gameTime);

                Vector3 range = enemyShots[i].getWorld().Translation;
                if (range.X > maxX || range.X < minX
                    || range.Y > maxY || range.Y < minY
                    || range.Z > maxZ || range.Z < minZ)
                {
                    enemyShots.RemoveAt(i);
                    i -= 1;
                }
                else
                {

                    if (enemyShots[i].CollidesWith(
                        enemyShots[i].model,
                        (enemyShots[i].getWorld()),
                        tankModel.model,
                        (tankModel.getWorld())))
                    {
                        // Collision! remove  the shot and reduce the life
                        enemyShots.RemoveAt(i);
                        i -= 1;
                        //get score
                        PlayInfo.reduceLife(1);
                        break;
                    }
                }
            }
        }
        public void playShotSound()
        {
            shotSound.Play();
        }
        public void playEnemyShotSound()
        {
            shotSound2.Play();
        }
    }
}
