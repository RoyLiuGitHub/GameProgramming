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

        Tank tankModel;
        npcTank npcModel;
        npcTank npcModel1;

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
//kkkfdd

        MousePick mousepick;
        Vector3? pickposition;



        //DijkstraManager dij;
        //AStra astra;

        public ModelManager(Game game)
            : base(game)
        {

            bPursue = false;
            bEvade = false;

            lState = new List<string>();

            //dij = new DijkstraManager();
            //astra = new AStra();
        }


        public override void Initialize()
        {
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

            ///astra.readMapInformation();

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

            tankModel = new Tank(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera);
            npcModel = new npcTank(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(800, 0, 1000));
            npcModel1 = new npcTank(Game.Content.Load<Model>(@"Models/Tank/tank"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(600, 0, -300));
            models.Add(tankModel);

            //Vector2 curPos = new Vector2(8, 5);
            //Vector2 destinationPos = new Vector2(1, 7);

            //dij.readMapInformation(curPos);
            //Pos111 p = new Pos111(8,5);
            //astra.findpath(p);
            //models.Add(npcModel);
            //models.Add(npcModel1);
            //npcModel.AddTarget(tankModel);
            //models.Add(new Box(Game.Content.Load<Model>(@"Box/box"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera));
            //models.Add(new Box(Game.Content.Load<Model>(@"boxtexture/boxtexture"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera));
            //modelsObstacle.Add(new Box(Game.Content.Load<Model>(@"Box/box"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera));

            //modelsObstacleFirm.Add(new boxtexture(Game.Content.Load<Model>(@"Box/box"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(400 + 100, 0, 600)));

            modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(300, 0, 100)));


            for (int ik = 0; ik < 70; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(300 + ik * 10, 0, 220)));
            }

            for (int ik = 0; ik < 50; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(850, 0, -200 + ik * 10)));
            }

            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(-200 + ik * 20, 0, -1000)));
            }

            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(1600, 0, ik * 20 - 1000)));
            }

            for (int ik = 0; ik < 110; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(ik * 20 - 400, 0, 1000)));
            }
            for (int ik = 0; ik < 100; ik++)
            {
                modelsObstacleFirm.Add(new Boundary(Game.Content.Load<Model>(@"Models/Boundary/stone"), ((Game1)Game).GraphicsDevice, ((Game1)Game).camera, new Vector3(-400, 0, ik * 20 - 1000)));
            }

            base.LoadContent();
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
                    Console.WriteLine("collision!!!");
                    stopPlayer();
                }
                model.update(gameTime);
            }
            /*
            pickPosition = mousePick.GetCollisionPosition();
            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed
                && pickPosition.HasValue == true)
            {
                tankModel.tankFindPath(maze);
            }*/

            Vector3 playerPosition = tankModel.getCurrentPosition();
            Vector3 npc0Position = npcModel.getCurrentPosition();
            Vector3 npc1Position = npcModel1.getCurrentPosition();
            float tmpx0 = (playerPosition - npc0Position).X;
            float tmpz0 = (playerPosition - npc0Position).Z;

            float tmpx1 = (playerPosition - npc1Position).X;
            float tmpz1 = (playerPosition - npc1Position).Z;

            double distancediff = Math.Sqrt(tmpx0 * tmpx0 + tmpz0 * tmpz0);
            //Console.WriteLine("00000" + distancediff);
            if (distancediff < 500)
            {
                npcModel.setbClose(true);
                npcModel.evade(gameTime.ElapsedGameTime.Milliseconds, tankModel.getCurSpeed(), tankModel.getCurrentPosition());
                curModel = lState[1];
                //bEvade = true;
            }
            else if (distancediff > 1000)
            {
                curModel = lState[0];
                //npcModel.changePortableModel(curModel);
                npcModel.setbClose(false);
            }

            distancediff = Math.Sqrt(tmpx1 * tmpx1 + tmpz1 * tmpz1);
            //Console.WriteLine("11111" + distancediff);
            if (bPursue || distancediff < 500)
            {
                npcModel1.setbClose(true);
                npcModel1.pursue(gameTime.ElapsedGameTime.Milliseconds, tankModel.getCurSpeed(), tankModel.getCurrentPosition());
                bPursue = true;
            }


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
    }
}
