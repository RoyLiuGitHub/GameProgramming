using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Assignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ModelManager modelManager;
        //DijkstraManager dijkstradanager;


        Vector3 cameraPosition = new Vector3(0, 25, 15);
        //public 
        public GraphicsDevice device { get; protected set; }
        public Camera camera { get; protected set; }


        //bullet profile
        private const float shotSpeed = 0.01f;
        private const int shotDelay = 2700;
        int shotCountdown = 0;
        public Vector3 bulletPosition = Vector3.Zero;
        public Vector3 bulletDirection = Vector3.Zero;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //random number
            rnd = new Random();

            //Resolution
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
#if !DEBUG
                graphics.IsFullScreen = true;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //camera = new Camera(this, cameraPosition, new Vector3(0, 25, 0), Vector3.Up);
            //camera = new Camera(this, new Vector3(0, 2500, 500), new Vector3(500, 200, 500), Vector3.Up);

            //camera = new Camera(this, new Vector3(0, 1500, 4000),new Vector3(0, 0, 3000), Vector3.Up);
            camera = new Camera(this, new Vector3(0, 2000, -3000), new Vector3(0, 0, 1000), Vector3.Up);

            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            this.IsMouseVisible = true;
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

            device = graphics.GraphicsDevice;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here

            // See if the player has fired a shot
            FireShots(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)

                {
                    bulletPosition = Tank.tankPosition;
                    bulletDirection = Tank.turretDirection;
                    // Add a shot to the model manager
                    modelManager.AddShot(
                       bulletPosition,
                        bulletDirection * shotSpeed);

                    //modelManager.playShotSound();

                    // Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }
        public Random rnd { get; protected set; }
    }
}
