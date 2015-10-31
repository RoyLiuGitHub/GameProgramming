using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;

namespace Assignment
{
    class TankState
    {
        public string fromState;
        public List<string> condition;
        public List<string> toState;
    };


    class NpcTank : BasicModel
    {
        List<TankState> listtankstate;
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
        private const int MIN_DISTANCE = 500;
        private const int DISCOVERY_DISTANCE = 800;
        private bool bCollision;

        int reNavigateTime = 0;
        bool isPURSUE;

        SteerMode steerMode;

        enum SteerMode
        {
            IDLE,
            PURSUE,
            EVADE
        }

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

        private const float MaxTurretAngle = 1.0f;
        private const float MinTurretAngle = -1.0f;

        private const float MaxCannonAngle = 0.0f;
        private const float MinCannonAngle = -0.9f;

        private Vector3 relativePosition = new Vector3(3f, 0, 3f);
        private Vector3 speed;

        Vector3? pickPosition;
        Vector3 tankPosition;
        //Vector3 tankPosition;
        Vector3 turretDirection;

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
        Vector3 displacement;
        //tank moving speed    fixed speed for WASD movement
        int WASDspeed = 10;
        float angle;
        //WASD mode or Auto mode
        bool isNavigate;

        private float scaleRatio = 100f;



        public NpcTank(Model model, GraphicsDevice device, Camera camera, int[,] map, int r, int c, Vector3 Position,
            Vector3 Direction)
            : base(model)
        {
            pickPosition = Tank.tankPosition;
            tankPosition = Position;
            //mousePick = new MousePick(device, camera);
            v = new Velocity();
            //preMousePick = Vector3.Zero;
            array = map;
            row = r;
            col = c;

            isNavigate = false;
            bCollision = false;

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

            XElement xml = XElement.Load(@"Content/fsm_npc1.xml");
            listtankstate = new List<TankState>();

            int i = 0;
            foreach (XElement state in xml.Elements())
            {
                listtankstate.Add(new TankState());
                //text += "state: " + state.Attribute("fromState").Value + "\n";
                listtankstate[i].fromState = state.Attribute("fromState").Value.ToString();
                listtankstate[i].condition = new List<string>();
                listtankstate[i].toState = new List<string>();
                foreach (XElement transition in state.Elements())
                {
                    listtankstate[i].condition.Add(transition.Attribute("condition").Value.ToString());
                    listtankstate[i].toState.Add(transition.Attribute("toState").Value.ToString());
                }
                i++;

            }
        }



        public void initMap()
        {

            lpath = new List<Point>();
            maze = new Maze(array);


        }

        public override void update(GameTime gameTime)
        {
            preTankPosition = tankPosition;
            //pickPosition = Tank.tankPosition;


            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100.0f;
            float rotationPerFrame = timeDifference * 1f;
            float movementPerFrame = timeDifference * 1f;



            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;



            //navigate(gameTime);
            //LimitInBoundary();
            //translation.Translation = tankPosition;

            selectMode(gameTime);

            //For bullet direction
            turretDirection = Vector3.Transform(tankPosition, turretBone.Transform);


            //turretDirection.X = preTankPosition.X - tankPosition.X;
            //turretDirection.Z = preTankPosition.Z - tankPosition.Z;

            //turretDirection = Matrix.Invert(rotation).Translation;

            base.update(gameTime);

        }


        /// <summary>
        /// old version
        /// </summary>
        /// <param name="gameTime"></param>
        //public void navigate(GameTime gameTime)
        //{

        //    if (reNavigateTime == 0)
        //    {

        //        isNavigate = true;
        //        preMousePick = pickPosition;
        //        bStart = false;
        //        Vector3? cp = new Vector3?(getCurrentPosition());
        //        Grid currentPositionRowCol = getPointRowCol(cp);

        //        pickPosition = Tank.tankPosition;
        //        Grid destinationPositionRowCol = getPointRowCol(pickPosition);
        //        //Grid destinationPositionRowCol = getPointRowColTest();

        //        initMap();
        //        Point p = tankFindPath(destinationPositionRowCol, currentPositionRowCol);

        //        reNavigateTime = 3000;

        //        if (p != null)
        //        {
        //            Vector3 nextcood = getPointCood(p);
        //            destination = nextcood;
        //            //destination = pickPosition.Value;

        //            destination.Y = 0;
        //            distance = destination - getCurrentPosition();

        //            direction = distance;
        //            direction.Normalize();
        //        }

        //    }


        //    if (isNavigate == true)
        //    {

        //        if (lGarr != null)
        //        {
        //            Vector3 tmpver3 = Vector3.Zero;
        //            if (lGarr.Count >= 2)
        //            {
        //                tmpver3 = getPointCood(lGarr[lGarr.Count - 2]);
        //            }
        //            else if (lGarr.Count == 1)
        //            {
        //                tmpver3 = getPointCood(lGarr[0]);
        //            }
        //            if (Math.Abs(tmpver3.X - tankPosition.X) < 20 && Math.Abs(tmpver3.Y - tankPosition.Y) < 20)
        //            {
        //                if (lGarr.Count > 1) lGarr.RemoveAt(lGarr.Count - 1);
        //                else if (lGarr.Count == 1)
        //                {
        //                    lGarr.RemoveAt(0);
        //                    bStart = true;
        //                }

        //                if (lGarr.Count >= 2)
        //                {
        //                    destination = getPointCood(lGarr[lGarr.Count - 2]);
        //                    //destination = pickPosition.Value;

        //                    destination.Y = 0;
        //                    distance = destination - tankPosition;

        //                    direction = distance;
        //                    direction.Normalize();
        //                }
        //            }

        //        }
        //        if (bStart && preMousePick != Vector3.Zero)
        //        {
        //            destination = pickPosition.Value;

        //            destination.Y = 0;
        //            distance = destination - tankPosition;

        //            direction = distance;
        //            direction.Normalize();
        //        }


        //        if (!inBrakeRange(destination))
        //        {
        //            v.increaseVelocity(gameTime);
        //            speed = direction * v.Speed;
        //            tankPosition += speed;

        //            angle = (float)Math.Atan2((pickPosition.Value.X - preTankPosition.X), (pickPosition.Value.Z - preTankPosition.Z));

        //            rotation = Matrix.CreateRotationY(angle);
        //            reNavigateTime -= gameTime.TotalGameTime.Milliseconds;
        //            TankTranslation(gameTime);
        //        }
        //        else
        //        {
        //            speed = Vector3.Zero;
        //            tankPosition += speed;
        //            v.Speed = 0;
        //            isNavigate = false;
        //            reNavigateTime = 0;

        //        }

        //    }
        //}

       /// new version
        public void selectMode(GameTime gameTime)
        {
            float distance = Vector3.Distance(Tank.tankPosition, tankPosition);
            //PURSUE
            if (distance < DISCOVERY_DISTANCE)
            {
                isPURSUE = true;
                navigate(gameTime, isPURSUE);
                LimitInBoundary();
                translation.Translation = tankPosition;
            }
            //EVADE
            else if (distance > MIN_DISTANCE)
            {
                isPURSUE = false;
                navigate(gameTime, isPURSUE);
                LimitInBoundary();
                translation.Translation = tankPosition;
            }
            //IDLE  do nothind
            else
            { }
        }

        public override void setBoolCollision()
        {
            bCollision = true;
        }

        public void navigate(GameTime gameTime, bool PURSUE)
        {
            //public void navigate(GameTime gameTime)
            //{
            preTankPosition = translation.Translation;
            Grid destinationPositionRowCol = new Grid(0, 0);
            if (reNavigateTime == 0)
            {

                if (PURSUE == true)
                {
                    pickPosition = Tank.tankPosition;
                    destinationPositionRowCol = getPointRowCol(pickPosition);
                }
                if (PURSUE == false)
                {
                    if (Tank.tankPosition.Z > 0)
                    { pickPosition = new Vector3(-500, 0, -500); }
                    else
                    { pickPosition = new Vector3(-500, 0, 500); }
                    destinationPositionRowCol = getPointRowCol(pickPosition);
                }

                isNavigate = true;
                preMousePick = pickPosition;
                bStart = false;
                
                

                reNavigateTime = 3000;


            }

            
            if (bCollision)
            {
                bCollision = false;
                string tm = changeModel();
                if (tm.CompareTo("IDLE") == 0)
                {
                    v.Speed = 0;
                    speed = Vector3.Zero;
                    translation.Translation = preTankPosition;
                }
            }
            else
            {
                isNavigate = false;
            }


            
            //streeing
            float distance = Vector3.Distance(Tank.tankPosition, tankPosition);
            if (distance > MIN_DISTANCE)
            {
                Vector3 pursueDirect =  Tank.tankPosition - tankPosition;
                pursueDirect.Normalize();
                v.increaseVelocity(gameTime);
                speed = pursueDirect * v.Speed;
                tankPosition += speed;
            }
        }

        public string changeModel(){
            foreach(TankState ts in listtankstate){
                if(ts.fromState.CompareTo("PURSUE") == 0 || ts.fromState.CompareTo("EVADE") == 0){
                    for (int i = 0; i < ts.condition.Count; i++ )
                    {
                        if (ts.condition[i].CompareTo("PLAYER_FAR") == 0)
                        {
                            return ts.toState[i];
                        }
                    }
                    
                }
            }

            return "IDLE";
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
            float x = tankPosition.X;
            float z = tankPosition.Z;
            Console.WriteLine("X is " + x + "  z is " + z);

            //float minBoundary = Boundary.GetBoundary() - scaleRatio;
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
            //Console.WriteLine("Print path:");
            while (parent != null)
            {
                //Console.WriteLine(parent.X + ", " + parent.Y);
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
            //Console.WriteLine(pos.Value.X);
            //Console.WriteLine(pos.Value.Z);
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

        public override Vector3 GetModelPosition()
        {
            return translation.Translation;
        }

        public override Vector3 GetTankDirection()
        {
            return turretDirection;
        }

    }
    //class Grid
    //{
    //    public int row;
    //    public int col;
    //    public Grid(int r, int c)
    //    {
    //        this.row = r;
    //        this.col = c;
    //    }
    //}
}
