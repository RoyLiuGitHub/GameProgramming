using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public static Vector3 cameraPosition { get; protected set; }
        public static Vector3 skyBoxMove;
        public static Matrix view { get; protected set; }
        public static Matrix projection { get; protected set; }
        static Vector3 cameraDirectionStatic;

        /// <summary>
        /// Get Camera Direction
        /// </summary>
        /// <returns></returns>
        public static Vector3 getCameraDirection()
        {
            return cameraDirectionStatic;
        }

        Vector3 cameraDirection;
        Vector3 cameraUp;
        Vector3 preCameraPosition;
        Vector3 jumpSpeed = new Vector3(0, 5, 0);
        MouseState prevMouseState;
        Matrix test = Matrix.Identity;
        Vector3 cameraDestination;
        //Vector3 cameraHeight = new Vector3(0, 250, 0);

        MousePick mousePick;

        //For camera rotate tank
        float angle;
        int distance = 300;

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                Tank.tankPosition + new Vector3(0, 130, 0), cameraUp);
        }

        public Camera(Game1 game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            mousePick = new MousePick(game.GraphicsDevice, game.camera);
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 8000);
        }
        /// <summary>
        /// Set Mouse Position and do initial get state
        /// </summary>
        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            preCameraPosition = cameraPosition;
            cameraDestination = cameraPosition;
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            //calculate distance for skyBox movement
            skyBoxMove = preCameraPosition - cameraPosition;
            preCameraPosition = cameraPosition;

            //Yaw rotation
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp,
                (-MathHelper.PiOver4 / 70) *
                (Mouse.GetState().X - prevMouseState.X)));

            
            angle = (float)Math.Atan2(-cameraDirection.Z, -cameraDirection.X);

            Vector3 temp = Tank.tankPosition;

            temp.X += (float)(Math.Cos(angle) * distance);
            temp.Z += (float)(Math.Sin(angle) * distance);
            temp.Y += cameraPosition.Y;
            cameraPosition = temp;


            //Pitch rotation
            Vector3 cross = Vector3.Normalize(Vector3.Cross(cameraUp, cameraDirection));
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cross,
                (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));

            //Reset prevMouseState
            prevMouseState = Mouse.GetState();

            //get static value for tank TurretRotation
            cameraDirectionStatic = cameraDirection;

            CreateLookAt();
            base.Update(gameTime);
        }
    }
}
