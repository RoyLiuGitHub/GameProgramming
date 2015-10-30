using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;


namespace Assignment
{
    class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();
        List<BasicModel> modelsObstacle = new List<BasicModel>();
        List<BasicModel> modelsObstacleFirm = new List<BasicModel>();
        List<BasicModel> hampers = new List<BasicModel>();

        //bullet  change to public for shots count
        List<BasicModel> shots = new List<BasicModel>();

        List<BasicModel> enemies = new List<BasicModel>();

        Tank tankModel;
        //NpcTank npcModel;
        //NpcTank npcModel1;

        int[,] graph_max_other;
        int[,] graph_max_player;

        Texture2D[] skyboxTextures;
        Model skyboxModel;

        Vector3 npc_position;
        Vector3 player_position;
        Vector3 player_dis;
        Vector3 player_dis_next = Vector3.Zero;
        Vector3 npc_dis;

        SpriteFont font;
        Vector2 fontPosition;
        string text;
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        MousePick mousePick;

        private bool bPursue;
        private bool bEvade;

        List<string> lState;


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

        //DijkstraManager dij;
        //Astra astra;
        Astra instanceAstra;

        public ModelManager(Game game)
            : base(game)
        {

            bPursue = false;
            bEvade = false;

            lState = new List<string>();

            //dij = new DijkstraManager();
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
            //Set initial spawn time
            SetNextSpawnTime();

            spriteBatch = new SpriteBatch(((Game1)Game).GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Arial");
            fontPosition = new Vector2(((Game1)Game).GraphicsDevice.Viewport.Width / 2, ((Game1)Game).GraphicsDevice.Viewport.Height / 2);

            text = "FSM: \n";

            XElement xml = XElement.Load(@"Content/fsm_npc1.xml");
            foreach (XElement state in xml.Elements())
            {
                text += "state: " + state.Attribute("fromState").Value + "\n";
                lState.Add(state.Attribute("fromState").Value.ToString());
            }
            curModel = lState[0];

            instanceAstra.readMapInformation();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //models.Add(new BasicModel(Game.Content.Load<Model>(@"Ground/Ground")));


            models.Add(new Ground(
               Game.Content.Load<Model>(@"Models/Ground/Ground")));

            Model skyModel = Game.Content.Load<Model>(@"Models/SkyBox/skybox");
            models.Add(new SkyBox(skyModel));

            skyboxTextures = new Texture2D[skyModel.Meshes.Count];

            int i = 0;
            foreach (ModelMesh mesh in skyModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    skyboxTextures[i++] = currentEffect.Texture;

            /*foreach (ModelMesh mesh in skyModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();*/
            int [,] a = instanceAstra.retMapInformation();
            tankModel = new Tank(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, a, instanceAstra.retRow(), instanceAstra.retCol());
            
            models.Add(tankModel);

            
            addBoundary();


            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (BasicModel model in models)
            {
                if (!Collide())
                {
                    //Console.WriteLine("no collision~~~~");
                }
                else
                {
                    //Console.WriteLine("collision!!!");
                    //stopPlayer();
                }
                model.update(gameTime);
            }

            for (int i = 0; i < enemies.Count; ++i)
            {
                enemies[i].update(gameTime);
            }
            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            UpdateShots(gameTime);



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
            foreach (BasicModel e in enemies)
            {
                e.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            }
            /*spriteBatch.Begin();
            Vector2 fontOrigin = font.MeasureString(text) / 2;
            spriteBatch.DrawString(
                font,
                text,
                fontPosition,
                Color.Red,
                0,
                fontOrigin,
                0.0f,
                SpriteEffects.None,
                0f);
            spriteBatch.End();*/


            base.Draw(gameTime);
        }

        public void addBoundary()
        {
            /*for (int ik = 0; ik < 70; ik++)
{
    modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(300 + ik * 10, 0, 220)));
}

for (int ik = 0; ik < 50; ik++)
{
    modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(850, 0, -200 + ik * 10)));
}*/

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
            //stones
            for (int ik = 0; ik < 2; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(leftBoundary - 38 * boundaryWidth, 0, 2200 - 60 * boundaryWidth - ik * boundaryWidth)));
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
                    //retv.X = 1600 - 200 * r - 100;
                    break;
                }
            }

            for (int c = 0; c < 100; c++)
            {
                if (rc.col == c)
                {
                    retv.X = leftBoundary - 40 * c - 20;
                    //retv.Z = -1000 + 200 * c + 100;
                }
            }
            //Console.WriteLine("row--------------" + row);
            //Console.WriteLine("col--------------" + col);

            return retv;
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

            return false;
        }

        public void stopPlayer()
        {
            //tankModel.WheelRotationValue = 0f;
            tankModel.setModelSpeed(0f);
        }

        private string curModel;

        /// Add bullet
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public void AddShot(Vector3 position, Vector3 direction)
        {
            //direction = Camera.getCameraDirection();
            direction = tankModel.getTankDirection();
            direction.Y = 0;
            position.Y = 30;    //bullet heigth
            shots.Add(new Bullet(
                //Game.Content.Load<Model>(@"Models\Bullet\ammo"),
                Game.Content.Load<Model>(@"Models\Bullet\bullet"),
                position, direction * 50, 0, 0, 0));
        }
        public void UpdateShots(GameTime gameTime)
        {

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
                }
                //else
                //{
                //    for (int j = 0; j < npcModel.Count; ++j)
                //    {
                //        //if (playerArr[i].CollidesWith(modelsObstacleTree[j].model, modelsObstacleTree[j].GetworldWithoutDistance() * Matrix.CreateTranslation(modelsObstacleTree[j].GetModelPosition())))
                //        if (shots[i].CollidesWith(
                //            shots[i].model,
                //            (shots[i].getWorld()),
                //            npcModel[j].model,
                //            (npcModel[j].GetTankPosition())))
                //        {
                //            // Collision! remove the tank and the shot.
                //            npcModel.RemoveAt(j);
                //            shots.RemoveAt(i);
                //            i -= 1;
                //            ////get score
                //            //PlayInfo.AddScore(1);
                //            break;
                //        }
                //    }
                //}
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
            int[,] a = instanceAstra.retMapInformation();
            enemies.Add(new NpcTank(
                Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, a, instanceAstra.retRow(), instanceAstra.retCol(), position, direction));



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

    }
}
