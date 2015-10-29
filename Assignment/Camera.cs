using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public static Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;
        Vector3 cameraUpInit;
        Vector3 cameraTarget;

        MouseState prevMouseState;
        Vector3 velocity;
        bool hasJump;
        bool hasFall;

        Vector3 preCameraPosition;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            //view = Matrix.CreateLookAt(pos, target, up);
            hasJump = false;
            hasFall = false;
            velocity = new Vector3(0, 0, 0);
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            cameraUpInit = up;
            cameraTarget = target;
            CreateLookAt();
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / Game.Window.ClientBounds.Height, 1, 8000);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }

        public override void Initialize()
        {
            prevMouseState = Mouse.GetState();
            base.Initialize();
        }

        private void fallToGround()
        {

        }

        public override void Update(GameTime gameTime)
        {
            //preCameraPosition = cameraPosition;
            //Vector3 temp = Tank.tankPosition;

            ////Yaw rotation
            //cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp,
            //    (-MathHelper.PiOver4 / 70) *
            //    (Mouse.GetState().X - prevMouseState.X)));


            ////Reset prevMouseState
            //prevMouseState = Mouse.GetState();


            CreateLookAt();

            base.Update(gameTime);
        }


    }
}
