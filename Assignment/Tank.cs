using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    class Tank : BasicModel
    {
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;

        MousePick mousePick;

        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;

        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;

        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;

        //tank speed.
        int speed = 3;


        float movingTime = 0;
        float angle;

        //Vector3 tankPosition;
        Vector3 distance;
        Vector3 destination;
        Vector3 direction = new Vector3(0,0,1);

        Vector3 displacement;

        Vector3 preTankPosition;
        Vector3 preDirection;


        private float currentMoveSpeed = 0;
        private float maxMoveSpeed = 0.4f;
        private float tankAcceleration = 0.01f;
        private Vector3 currentVelocity;
        private float minStopSpeed = 0.1f;
        private float scaleRatio = 100f;

        private bool bhamper = false;


        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }

        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }

        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }

        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }

        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }

        //get tank position for bullet start point
        public static Vector3 tankPosition;
        //{
        //    get { return tankPosition; }
        //    protected set { tankPosition = value; }
        //}
        //get turret for bullet direction
        public static Vector3 turretDirection;
        //{
        //    get { return turretDirection; }
        //    set { turretDirection = value; }
        //}

        public Tank(Model model, GraphicsDevice device, Camera camera)
            : base(model)
        {
            mousePick = new MousePick(device, camera);
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];
            leftSteerBone = model.Bones["l_steer_geo"];
            rightSteerBone = model.Bones["r_steer_geo"];
            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            hatchBone = model.Bones["hatch_geo"];

            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;
        }

        public override void Update(GameTime gameTime)
        {


            preTankPosition = tankPosition;
            preDirection = direction;



            //float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;

            ////Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            ////Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            ////Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            ////Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            ////Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);



            //TankTranslation(time);
            Navigate(gameTime);
            GetUserInput(gameTime);

            TankTranslation(gameTime);
            LimitInBoundary();

            //if (Keyboard.GetState().IsKeyDown(Keys.W))
            //{
            //    displacement = direction;
            //    displacement.Y = 0;
            //    tankPosition += displacement * speed;

            //    WheelRotation = time * 5;
            //    SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
            //    CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            //    HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            //    turretBone.Transform *= turretRotation * turretTransform;


            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.S))
            //{
            //    displacement = direction;
            //    displacement.Y = 0;
            //    tankPosition -= displacement * speed;

            //    WheelRotation = time * 5;
            //    SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;

            //    CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            //    HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            //    turretBone.Transform *= turretRotation * turretTransform;


            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //{
            //    direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50));
            //    rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50);

            //    WheelRotation = time * 5;
            //    SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;

            //    CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            //    HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            //    turretBone.Transform *= turretRotation * turretTransform;



            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.D))
            //{
            //    direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver4 / 50));
            //    rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / -50);

            //    WheelRotation = time * 5;
            //    SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
            //    //TurretRotation = (float)Math.Sin(time * 0.333f) * 1.25f;
            //    CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            //    HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            //    turretBone.Transform *= turretRotation * turretTransform;


            //}


            //TurretRotation = (float)Math.Atan2(Camera.getCameraDirection().X - tankPosition.X, Camera.getCameraDirection().Z - tankPosition.Z);



            translation.Translation = tankPosition;

            //leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            //rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            //leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            //rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            //leftSteerBone.Transform = steerRotation * leftSteerTransform;
            //rightSteerBone.Transform = steerRotation * rightSteerTransform;
            //turretBone.Transform = turretRotation * turretTransform;
            //cannonBone.Transform = cannonRotation * cannonTransform;
            //hatchBone.Transform = hatchRotation * hatchTransform;



            //For bullet direction
            turretDirection = Vector3.Transform(tankPosition, turretBone.Transform);


            //play track music
            if (preTankPosition != tankPosition || preDirection != direction)
            {
                ModelManager.tankTrackSound.Resume();
            }
            else
            {
                ModelManager.tankTrackSound.Stop();
            }

            base.Update(gameTime);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }
        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(0.20f) * rotation * translation;
        }

        public override Matrix GetWorldPublic()
        {
            return Matrix.CreateScale(0.2f) * rotation * translation;
        }

        private void GetUserInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                displacement = direction;
                displacement.Y = 0;
                tankPosition += displacement * speed;


                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                //TankTranslation(time);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                displacement = direction;
                displacement.Y = 0;
                tankPosition -= displacement * speed;

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                //TankTranslation(time);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50));
                rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50);

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                //TankTranslation(time);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver4 / 50));
                rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / -50);

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                //TankTranslation(time);
            }
        }

        private void TankTranslation(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;

            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            WheelRotation = time * 5;
            SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;

            CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            turretBone.Transform *= turretRotation * turretTransform;

            TurretRotation = (float)Math.Atan2(Camera.getCameraDirection().X - tankPosition.X, Camera.getCameraDirection().Z - tankPosition.Z);

            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;
        }
        private void NaturallySlowDown(int elapsedFrameTime)
        {
            if (Math.Abs(currentMoveSpeed) < minStopSpeed)
            {
                currentMoveSpeed = 0;
                currentVelocity = Vector3.Zero;
            }
            else
            {
                // no matter currentMoveSpeed is positive or negative
                // -currentMoveSpeed/Math.Abs(currentMoveSpeed) return opposite 1 of it
                currentMoveSpeed -= currentMoveSpeed / Math.Abs(currentMoveSpeed) * tankAcceleration / 2;
                currentVelocity = displacement * currentMoveSpeed;
            }
            tankPosition += currentVelocity * elapsedFrameTime;
        }

        private void LimitInBoundary()
        {
            float minBoundary = Boundary.GetBoundary() -  scaleRatio;
            if (tankPosition.X > minBoundary)
                tankPosition.X = minBoundary;
            if (tankPosition.X < -minBoundary)
                tankPosition.X = -minBoundary;
            if (tankPosition.Z > minBoundary)
                tankPosition.Z = minBoundary;
            if (tankPosition.Z < -minBoundary)
                tankPosition.Z = -minBoundary;
        }

        private void Navigate(GameTime gameTime)
        {
            Vector3? pickPosition = mousePick.GetCollisionPosition();
            float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
            if (Mouse.GetState().RightButton == ButtonState.Pressed
                && pickPosition.HasValue == true)
            {

                destination = pickPosition.Value;
                destination.Y = 0;

                distance = destination - tankPosition;
                direction = distance;
                direction.Normalize();

                angle = (float)Math.Atan2((pickPosition.Value.X - tankPosition.X), (pickPosition.Value.Z - tankPosition.Z));

                rotation = Matrix.CreateRotationY(angle);

                if (distance.X != 0)
                {
                    movingTime = distance.X / direction.X;
                }
                else
                {
                    movingTime = distance.Z / direction.Z;
                }
            }

            if (bhamper)
            {
                tankPosition = destination;
                movingTime = 0f;
                bhamper = false;
            }

            if (tankPosition != destination)
            {

                WheelRotation = time * 5;
                SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
                CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
                HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

                if (movingTime > (gameTime.ElapsedGameTime.Milliseconds))
                {
                    tankPosition += direction * gameTime.ElapsedGameTime.Milliseconds;
                    translation.Translation = tankPosition + direction * gameTime.ElapsedGameTime.Milliseconds;
                    movingTime -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (movingTime != 0 && movingTime <= gameTime.ElapsedGameTime.Milliseconds)
                {
                    translation.Translation = direction * movingTime;
                    tankPosition += direction * movingTime;
                    movingTime = 0;
                }
            }

            if (movingTime == 0f)
            {
                WheelRotation = 0;
                SteerRotation = 0;
                CannonRotation = 0;
                HatchRotation = 0;
                translation.Translation = destination;
            }
        }

        public override void setHamper()
        {
            bhamper = true;
        }
    }
}
