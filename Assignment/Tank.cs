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

        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone lSteerEngineBone;
        ModelBone rSteerEngineBone;
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone hatchBone;
        ModelBone tankBone;

        Vector3 distance;
        Vector3 destination;
        Vector3 direction = new Vector3(0, 0, 1);

        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix lSteerEngineTransform;
        Matrix rSteerEngineTransform;
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix hatchTransform;
        Matrix tankTransform;

        float turretRotationValue;
        float cannonRotationValue;
        float steerRotationValue;
        float wheelRotationValue;

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
        int[,] array;
        private int row;
        private int col;

        public float WheelRotationValue
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }

        public float WheelRotationValueUpDown;
        /*{
            get { return WheelRotationValueUpDown; }
            set { WheelRotationValueUpDown = value; }
        }*/

        public float SteerRotationValue
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }

        public float TurretRotation
        {
            get { return turretRotationValue; }
            set
            {
                if (value > MaxTurretAngle)
                    turretRotationValue = MaxTurretAngle;
                else if (value < MinTurretAngle)
                    turretRotationValue = MinTurretAngle;
                else
                    turretRotationValue = value;
            }
        }

        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set
            {
                if (value > MaxCannonAngle)
                    cannonRotationValue = MaxCannonAngle;
                else if (value < MinCannonAngle)
                    cannonRotationValue = MinCannonAngle;
                else
                    cannonRotationValue = value;
            }
        }


        public Tank(Model model, GraphicsDevice device, Camera camera, int[,] map, int r, int c)
            : base(model)
        {
            pickPosition = Vector3.Zero;
            curPosition = Vector3.Zero;
            WheelRotationValueUpDown = 0f;


            mousePick = new MousePick(device, camera);

            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            lSteerEngineBone = model.Bones["l_steer_geo"];
            rSteerEngineBone = model.Bones["r_steer_geo"];
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];
            hatchBone = model.Bones["hatch_geo"];
            tankBone = model.Bones["tank_geo"];


            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            lSteerEngineTransform = lSteerEngineBone.Transform;
            rSteerEngineTransform = rSteerEngineBone.Transform;
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            hatchTransform = hatchBone.Transform;
            tankTransform = tankBone.Transform;

            v = new Velocity();
            preMousePick = Vector3.Zero;
            array = map;
            row = r;
            col = c;
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
            /*int[,] array = {
                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                           { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
                           };*/

            
            lpath = new List<Point>();
            maze = new Maze(array);
        }

        public override void update(GameTime gameTime)
        {
            //base.update(gameTime);
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100.0f;
            float rotationPerFrame = timeDifference * 1f;
            float movementPerFrame = timeDifference * 1f;


            lSteerEngineBone.Transform = Matrix.CreateRotationY(wheelRotationValue) * lSteerEngineTransform;
            rSteerEngineBone.Transform = Matrix.CreateRotationY(wheelRotationValue) * rSteerEngineTransform;

            wheelRollMatrix *= Matrix.CreateRotationX(wheelRotationValue);
            leftBackWheelBone.Transform = Matrix.CreateRotationX(wheelRotationValue) * leftBackWheelTransform;
            rightBackWheelBone.Transform = Matrix.CreateRotationX(wheelRotationValue) * rightBackWheelTransform;
            leftFrontWheelBone.Transform = Matrix.CreateRotationX(wheelRotationValue) * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = Matrix.CreateRotationX(wheelRotationValue) * rightFrontWheelTransform;
            rightBackWheelBone.Transform = Matrix.CreateRotationX(WheelRotationValue) * rightBackWheelTransform;

            MouseState mousestate = Mouse.GetState();


            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;
            navigate(gameTime);



            hatchBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * hatchTransform;
            tankBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * tankTransform;

            base.update(gameTime);

        }

        public void navigate(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed
    && pickPosition.HasValue == true)
            {
                preMousePick = pickPosition;
                bStart = false;
                Vector3? cp = new Vector3?(getCurrentPosition());
                Grid currentPositionRowCol = getPointRowCol(cp);

                pickPosition = mousePick.GetCollisionPosition();
                Grid destinationPositionRowCol = getPointRowCol(pickPosition);
                //Grid destinationPositionRowCol = getPointRowColTest();

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
            }
            else
            {
                speed = Vector3.Zero;
                translation.Translation += speed;
                v.Speed = 0;
            }
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

        private int leftBoundary = 1800;
        public Grid getPointRowColTest()
        {

            Grid retg = new Grid(32, 29);
            Vector3 retv3 = getPointCoodTest(retg);

            return retg;
        }
        public Vector3 getPointCoodTest(Grid rc)
        {
            Vector3 retv = new Vector3();

            retv.Y = 0f;

            for (int r = 0; r < row; r++)
            {
                if (rc.row == r)
                {
                    retv.Z = 2200 - 40 * r - 20;
                    //retv.X = 1600 - 200 * r - 100;
                    break;
                }
            }

            for (int c = 0; c < col; c++)
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
        public Grid getPointRowCol(Vector3? pos)
        {

            int retRow = 0;
            int retCol = 0;
            Console.WriteLine(pos.Value.X);
            Console.WriteLine(pos.Value.Z);
            for (int r = 0; r < row; r++)
            {
                if (pos.Value.Z < 2400 - 40 * r && pos.Value.Z >= 2400 - 40 * (r + 1))
                {
                    retRow = r + 1;
                    break;
                }
            }

            for (int c = 0; c < col; c++)
            {
                if (pos.Value.X < leftBoundary - 40 * c && pos.Value.X >= leftBoundary - 40 * (c + 1))
                {
                    retCol = c + 1;
                    break;
                }
            }
            //Console.WriteLine("row--------------" + row);
            //Console.WriteLine("col--------------" + col);

            Grid retg = new Grid(retRow, retCol);
            return retg;
        }

        public Vector3 getPointCood(Point rc)
        {
            Vector3 retv = new Vector3();

            retv.Y = 0f;

            for (int r = 0; r < row; r++)
            {
                if (rc.X == r)
                {
                    retv.Z = 2400 - 40 * r - 20;
                    //retv.X = 1600 - 200 * r - 100;
                    break;
                }
            }

            for (int c = 0; c < col; c++)
            {
                if (rc.Y == c)
                {
                    retv.X = leftBoundary - 40 * c - 20;
                    //retv.Z = -1000 + 200 * c + 100;
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
            //wheelRotationValue = s;
            v.Speed = 0;
            speed = Vector3.Zero;
        }

        public Vector3 getPosition()
        {
            return translation.Translation;
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
