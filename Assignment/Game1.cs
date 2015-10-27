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
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ModelManager modelManager;

        //bullet profile
        private const float shotSpeed = 0.01f;
        private const int shotDelay = 2700;
        int shotCountdown = 0;
        public Vector3 bulletPosition = Vector3.Zero;
        public Vector3 bulletDirection = Vector3.Zero;

        public Camera camera { get; protected set; }
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
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 1500, 4000),
            new Vector3(0, 0, 3000), Vector3.Up);
            Components.Add(camera);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            IsMouseVisible = true;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // See if the player has fired a shot
            FireShots(gameTime);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        public Random rnd { get; protected set;}

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

                    modelManager.playShotSound();

                    // Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }

    }
}
