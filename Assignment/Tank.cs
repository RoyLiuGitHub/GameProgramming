using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Assignment
{
    class Tank : BasicModel
    {

        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(.3f);

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

        Vector3 distance;
        Vector3 destination;
        Vector3 direction = new Vector3(0, 0, 1);

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

        private const float MaxTurretAngle = 1.0f;
        private const float MinTurretAngle = -1.0f;

        private const float MaxCannonAngle = 0.0f;
        private const float MinCannonAngle = -0.9f;

        Matrix wheelRollMatrix;
        private Vector3 relativePosition = new Vector3(3f, 0, 3f);
        private Vector3 speed;

        Vector3? pickPosition;
        Vector3 curPosition = Vector3.Zero;

        const float round = 10f;

        private const float MaxMoveSpeed = 20.0f;

        private Velocity v;
        List<Point> lpath;
        Maze maze;
        List<Point> lGarr;
        Vector3? preMousePick;
        bool bStart;


        private float scaleRatio = 100f;
        Vector3 displacement;
        Vector3 preTankPosition;
        Vector3 preDirection;
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


        float movingTime = 0;
        float angle;
        //tank moving speed    fixed speed for WASD movement
        int WASDspeed = 10;

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

        public Vector3 getTankDirection()
        {
            return direction;
        }




        public float WheelRotationValueUpDown;
        /*{
            get { return WheelRotationValueUpDown; }
            set { WheelRotationValueUpDown = value; }
        }*/


        public Tank(Model model, GraphicsDevice device, Camera camera)
            : base(model)
        {
            pickPosition = Vector3.Zero;
            curPosition = Vector3.Zero;
            WheelRotationValueUpDown = 0f;

            v = new Velocity();
            preMousePick = Vector3.Zero;

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

        struct WorldObject
        {
            public Vector3 position;
            public Vector3 velocity;
            public Vector3 lastPosition;
            public void MoveForward()
            {
                lastPosition = position;
                position += velocity;
            }
            public void Backup()
            {
                position -= velocity;
            }
            public void ReverseVelocity()
            {
                velocity.X = -velocity.X;
            }
        }
        /*
        static void CheckForCollisions(ref WorldObject c1, ref WorldObject c2)
        {
            for (int i = 0; i < c1.model.Meshes.Count; i++)
            {
                // Check whether the bounding boxes of the two cubes intersect.
                BoundingSphere c1BoundingSphere = c1.model.Meshes[i].BoundingSphere;
                c1BoundingSphere.Center += c1.position;

                for (int j = 0; j < c2.model.Meshes.Count; j++)
                {
                    BoundingSphere c2BoundingSphere = c2.model.Meshes[j].BoundingSphere;
                    c2BoundingSphere.Center += c2.position;

                    if (c1BoundingSphere.Intersects(c2BoundingSphere))
                    {
                        c2.ReverseVelocity();
                        c1.Backup();
                        c1.ReverseVelocity();
                        return;
                    }
                }
            }
        }*/
        public virtual Vector3 getCurSpeed()
        {
            return speed;
        }

        public void initMap()
        {
            int[,] array = {
                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
                           };
            lpath = new List<Point>();
            maze = new Maze(array);
        }

        public override void update(GameTime gameTime)
        {
            preTankPosition = tankPosition;
            preDirection = direction;



            Navigate(gameTime);
            GetUserInput(gameTime);

            TankTranslation(gameTime);
            LimitInBoundary();


            translation.Translation = tankPosition;


            //For bullet direction
            turretDirection = Vector3.Transform(tankPosition, turretBone.Transform);


            ////play track music
            //if (preTankPosition != tankPosition || preDirection != direction)
            //{
            //    ModelManager.tankTrackSound.Resume();
            //}
            //else
            //{
            //    ModelManager.tankTrackSound.Stop();
            //}

        }

        public Point tankFindPath(Grid dp, Grid cp)
        {
            Point start = new Point(cp.row, cp.col);
            Point end = new Point(dp.row, dp.col);
            var parent = maze.FindPath(start, end, false);

            lGarr = new List<Point>();
            //Console.WriteLine("Print path:");
            while (parent != null)
            {
                Console.WriteLine(parent.X + ", " + parent.Y);
                lGarr.Add(parent);
                parent = parent.ParentPoint;
            }

            Point retp = null;
            int len;
            if ((len = lGarr.Count) >= 2)
            {
                retp = lGarr[len - 2];

            }

            return retp;
        }

        public override bool inBrakeRange(Vector3 destination)
        {
            Vector3 curPos = getCurrentPosition();
            if (Math.Abs(curPos.X - destination.X) + Math.Abs(curPos.Z - destination.Z) < 10)
            {
                return true;
            }

            return false;
        }

        public Grid getPointRowCol(Vector3? pos)
        {

            int row = 0;
            int col = 0;

            Console.WriteLine(pos.Value.X);
            Console.WriteLine(pos.Value.Z);
            for (int r = 0; r < 10; r++)
            {
                if (pos.Value.X < 1600 - 200 * r && pos.Value.X >= 1600 - 200 * (r + 1))
                {
                    row = r + 1;
                    break;
                }
            }

            for (int c = 0; c < 10; c++)
            {
                if (pos.Value.Z >= -1000 + 200 * c && pos.Value.Z < -1000 + 200 * (c + 1))
                {
                    col = c + 1;
                    break;
                }
            }
            //Console.WriteLine("row--------------" + row);
            //Console.WriteLine("col--------------" + col);

            Grid retg = new Grid(row, col);
            return retg;
        }

        public Vector3 getPointCood(Point rc)
        {
            Vector3 retv = new Vector3();

            retv.Y = 0f;

            for (int r = 0; r < 10; r++)
            {
                if (rc.X == r)
                {
                    retv.X = 1600 - 200 * r - 100;
                    break;
                }
            }

            for (int c = 0; c < 10; c++)
            {
                if (rc.Y == c)
                {
                    retv.Z = -1000 + 200 * c + 100;
                }
            }
            //Console.WriteLine("row--------------" + row);
            //Console.WriteLine("col--------------" + col);

            return retv;
        }

        public override Matrix getWorld()
        {
            return scale * rotation * translation;
        }

        public override Vector3 getCurrentPosition()
        {
            //return base.getCurrentPosition();
            return translation.Translation;
        }

        public override Vector3 getCurDirection()
        {
            return direction;
        }
        public override Matrix GetScale()
        {
            return scale;
        }
        /*protected override Matrix GetWorld()
        {
            return base.GetWorld();
        }*/

        public override void setModelSpeed(float s)
        {
            //base.setModelSpeed(s);
            wheelRotationValue = s;
        }

        public Vector3 getPosition()
        {
            return translation.Translation;
        }
        private void Navigate(GameTime gameTime)
        {


            //float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
            //if (Mouse.GetState().RightButton == ButtonState.Pressed
            //    && pickPosition.HasValue == true)
            //{
            //    Vector3? pickPosition = mousePick.GetCollisionPosition();
            //    destination = pickPosition.Value;
            //    destination.Y = 0;


            //    Grid currentPositionRowCol = getPointRowCol(tankPosition);
            //    Grid destinationPositionRowCol = getPointRowCol(pickPosition);
            //    initMap();
            //    Point p = tankFindPath(destinationPositionRowCol, currentPositionRowCol);

            //    if (p != null)
            //    {

            //        p = p.ParentPoint;
            //        Vector3 nextcood = getPointCood(p);
            //        destination = nextcood;
            //        destination.Y = 0;
            //        distance = destination - getCurrentPosition();

            //        direction = distance;
            //        direction.Normalize();
            //    }

            //}


            //angle = (float)Math.Atan2((pickPosition.Value.X - tankPosition.X), (pickPosition.Value.Z - tankPosition.Z));

            //rotation = Matrix.CreateRotationY(angle);

            //if (distance.X != 0)
            //{
            //    movingTime = distance.X / direction.X;
            //}
            //else
            //{
            //    movingTime = distance.Z / direction.Z;
            //}


            //if (tankPosition != destination)
            //{

            //    WheelRotation = time * 5;
            //    SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
            //    CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            //    HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            //    if (movingTime > (gameTime.ElapsedGameTime.Milliseconds))
            //    {
            //        tankPosition += direction * gameTime.ElapsedGameTime.Milliseconds;
            //        translation.Translation = tankPosition + direction * gameTime.ElapsedGameTime.Milliseconds;
            //        movingTime -= gameTime.ElapsedGameTime.Milliseconds;
            //    }
            //    else if (movingTime != 0 && movingTime <= gameTime.ElapsedGameTime.Milliseconds)
            //    {
            //        translation.Translation = direction * movingTime;
            //        tankPosition += direction * movingTime;
            //        movingTime = 0;
            //    }
            //}

            //if (movingTime == 0f)
            //{
            //    WheelRotation = 0;
            //    SteerRotation = 0;
            //    CannonRotation = 0;
            //    HatchRotation = 0;
            //    translation.Translation = destination;
            //}

            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;
            if (Mouse.GetState().RightButton == ButtonState.Pressed
                && pickPosition.HasValue == true)
            {
                preMousePick = pickPosition;
                bStart = false;
                Vector3? cp = new Vector3?(getCurrentPosition());
                Grid currentPositionRowCol = getPointRowCol(cp);

                pickPosition = mousePick.GetCollisionPosition();
                Grid destinationPositionRowCol = getPointRowCol(pickPosition);

                initMap();
                Point p = tankFindPath(destinationPositionRowCol, currentPositionRowCol);

                if (p != null)
                {
                    Vector3 nextcood = getPointCood(p);
                    destination = nextcood;
                    //destination = pickPosition.Value;

                    destination.Y = 0;
                    distance = destination - getCurrentPosition();

                    direction = distance;
                    direction.Normalize();
                }

            }

            if (lGarr != null)
            {
                Vector3 tmpver3 = Vector3.Zero;
                if (lGarr.Count >= 2)
                {
                    tmpver3 = getPointCood(lGarr[lGarr.Count - 2]);
                }
                else if (lGarr.Count == 1)
                {
                    tmpver3 = getPointCood(lGarr[0]);
                }

                //Console.WriteLine(tmpver3.X + " " + tmpver3.Y + " " + tmpver3.Z);
                //Console.WriteLine(translation.Translation.X + " " + translation.Translation.Y + " " + translation.Translation.Z);
                if (Math.Abs(tmpver3.X - translation.Translation.X) < 20 && Math.Abs(tmpver3.Y - translation.Translation.Y) < 20)
                {
                    if (lGarr.Count > 1) lGarr.RemoveAt(lGarr.Count - 1);
                    else if (lGarr.Count == 1)
                    {
                        lGarr.RemoveAt(0);
                        bStart = true;
                    }

                    if (lGarr.Count >= 2)
                    {
                        destination = getPointCood(lGarr[lGarr.Count - 2]);
                        //destination = pickPosition.Value;

                        destination.Y = 0;
                        distance = destination - getCurrentPosition();

                        direction = distance;
                        direction.Normalize();
                    }
                }

            }
            if (bStart && preMousePick != Vector3.Zero)
            {
                destination = pickPosition.Value;

                destination.Y = 0;
                distance = destination - getCurrentPosition();

                direction = distance;
                direction.Normalize();
            }


            if (!inBrakeRange(destination))
            {
                v.increaseVelocity(gameTime);
                speed = direction * v.Speed;
                translation.Translation += speed;
                //tankPosition += speed;
            }
            else
            {
                speed = Vector3.Zero;
                translation.Translation += speed;
                translation.Translation += direction;
                //tankPosition += direction;
                v.Speed = 0;
            }
            base.update(gameTime);
        }
        private void GetUserInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                displacement = direction;
                displacement.Y = 0;
                tankPosition += displacement * WASDspeed;


                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                //TankTranslation(time);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                displacement = direction;
                displacement.Y = 0;
                tankPosition -= displacement * WASDspeed;

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

            //TurretRotation = (float)Math.Atan2(Camera.getCameraDirection().X - tankPosition.X, Camera.getCameraDirection().Z - tankPosition.Z);

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
        private void LimitInBoundary()
        {
            float minBoundary = Boundary.GetBoundary() - scaleRatio;
            if (tankPosition.X > minBoundary)
                tankPosition.X = minBoundary;
            if (tankPosition.X < -minBoundary)
                tankPosition.X = -minBoundary;
            if (tankPosition.Z > minBoundary)
                tankPosition.Z = minBoundary;
            if (tankPosition.Z < -minBoundary)
                tankPosition.Z = -minBoundary;
        }



    }

    class Grid
    {
        public int row;
        public int col;
        public Grid(int r, int c)
        {
            this.row = r;
            this.col = c;
        }
    }
}
