﻿using System;
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

        public bool getIsAuto()
        {
            return isAuto;
        }



        private const float MaxTurretAngle = 1.0f;
        private const float MinTurretAngle = -1.0f;

        private const float MaxCannonAngle = 0.0f;
        private const float MinCannonAngle = -0.9f;

        private Vector3 relativePosition = new Vector3(3f, 0, 3f);
        private Vector3 speed;

        Vector3? pickPosition;
        public static Vector3 tankPosition;
        public static Vector3 turretDirection;

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
        //2015/10/30
        Vector3 preTankPosition;
        Vector3 preDirection;
        Vector3 displacement;
        //tank moving speed    fixed speed for WASD movement
        int WASDspeed = 10;
        float angle;
        //WASD mode or Auto mode
        bool isAuto;
        bool isNavigate;

        private float scaleRatio = 100f;
        public Tank(Model model, GraphicsDevice device, Camera camera, int[,] map, int r, int c)
            : base(model)
        {
            pickPosition = Vector3.Zero;
            tankPosition = Vector3.Zero;
            mousePick = new MousePick(device, camera);
            v = new Velocity();
            preMousePick = Vector3.Zero;
            array = map;
            row = r;
            col = c;

            isNavigate = false;

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
        
        public virtual Vector3 getCurSpeed()
        {
            return speed;
        }

        public void initMap()
        {

            lpath = new List<Point>();
            maze = new Maze(array);
        }

        public override void update(GameTime gameTime)
        {
            isAuto = true;
            preTankPosition = tankPosition;
            preDirection = direction;
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100.0f;
            float rotationPerFrame = timeDifference * 1f;
            float movementPerFrame = timeDifference * 1f;
            MouseState mousestate = Mouse.GetState();
            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;
            GetUserInput(gameTime);
            navigate(gameTime);
            LimitInBoundary();
            translation.Translation = tankPosition;

            //For bullet direction
            turretDirection = Vector3.Transform(tankPosition, turretBone.Transform);

            if (preTankPosition != tankPosition || preDirection != direction)
            {
                ModelManager.tankTrackSound.Resume();
            }
            else
            {
                ModelManager.tankTrackSound.Stop();
            }

            base.update(gameTime);
        }

        public void navigate(GameTime gameTime)
        {
            if (isAuto == true)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed
        && pickPosition.HasValue == true)
                {
                    isNavigate = true;
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
                        destination.Y = 0;
                        distance = destination - getCurrentPosition();

                        direction = distance;
                        direction.Normalize();
                    }

                }


                if (isNavigate == true)
                {

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

                        if (Math.Abs(tmpver3.X - tankPosition.X) < 20 && Math.Abs(tmpver3.Y - tankPosition.Y) < 20)
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
                                destination.Y = 0;
                                distance = destination - tankPosition;

                                direction = distance;
                                direction.Normalize();
                            }
                        }

                    }
                    if (bStart && preMousePick != Vector3.Zero)
                    {
                        destination = pickPosition.Value;

                        destination.Y = 0;
                        distance = destination - tankPosition;

                        direction = distance;
                        direction.Normalize();
                    }


                    if (!inBrakeRange(destination))
                    {
                        v.increaseVelocity(gameTime);
                        speed = direction * v.Speed;
                        tankPosition += speed;
                        angle = (float)Math.Atan2((pickPosition.Value.X - preTankPosition.X), (pickPosition.Value.Z - preTankPosition.Z));
                        rotation = Matrix.CreateRotationY(angle);
                        TankTranslation(gameTime);
                    }
                    else
                    {
                        speed = Vector3.Zero;
                        tankPosition += speed;
                        v.Speed = 0;
                        isNavigate = false;
                    }
                }
            }
        }
        private void GetUserInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                isAuto = false;
                isNavigate = false;
                displacement = direction;
                displacement.Y = 0;
                tankPosition += displacement * WASDspeed;


                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                TankTranslation(gameTime);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                isAuto = false;
                isNavigate = false;
                displacement = direction;
                displacement.Y = 0;
                tankPosition -= displacement * WASDspeed;

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                TankTranslation(gameTime);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                isAuto = false;
                isNavigate = false;
                direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50));
                rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / 50);

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                TankTranslation(gameTime);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                isAuto = false;
                isNavigate = false;
                direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver4 / 50));
                rotation *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4 / -50);

                float time = (float)gameTime.TotalGameTime.Milliseconds / 1000;
                TankTranslation(gameTime);
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
            float x = tankPosition.X;
            float z = tankPosition.Z;
            Console.WriteLine("X is " + x  + "  z is "+ z);
            float minBoundary = Boundary.GetBoundary();
            if (tankPosition.X > (1800))
                tankPosition.X = 1800;
            if (tankPosition.X < (-1900))
                tankPosition.X = -1900;
            if (tankPosition.Z > 1900)
                tankPosition.Z = 1900;
            if (tankPosition.Z < -900)
                tankPosition.Z = -900;
        }

        public Point tankFindPath(Grid dp, Grid cp)
        {
            Point start = new Point(cp.row, cp.col);
            Point end = new Point(dp.row, dp.col);
            var parent = maze.FindPath(start, end, false);

            lGarr = new List<Point>();
            while (parent != null)
            {
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
                    break;
                }
            }

            for (int c = 0; c < col; c++)
            {
                if (rc.col == c)
                {
                    retv.X = leftBoundary - 40 * c - 20;
                }
            }

            return retv;
        }
        public Grid getPointRowCol(Vector3? pos)
        {

            int retRow = 0;
            int retCol = 0;
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
                    break;
                }
            }

            for (int c = 0; c < col; c++)
            {
                if (rc.Y == c)
                {
                    retv.X = leftBoundary - 40 * c - 20;
                }
            }

            return retv;
        }

        public override Matrix getWorld()
        {
            return scale * rotation * translation;
        }

        public override Vector3 getCurrentPosition()
        {
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
        public override void setModelSpeed(float s)
        {
            v.Speed = 0;
            speed = Vector3.Zero;
        }

        public Vector3 getPosition()
        {
            return translation.Translation;
        }

        public override void stop()
        {
            tankPosition = preTankPosition;
            v.Speed = 0;
            speed = Vector3.Zero;
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
