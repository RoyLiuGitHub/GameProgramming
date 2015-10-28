using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Assignment
{
    class npcTank : BasicModel
    {
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(.3f);

        MousePick mousePick;

        float movingTime = 0f;

        float movement = 0;
        float rotation1 = 0;

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

        Vector3? pickPosition;
        Vector3 curPosition = Vector3.Zero;

        const float round = 10f;

        private Velocity v;

        private const float MaxMoveSpeed = 20.0f;
        private Vector3 speed;
        private bool bClose = false;

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


        public npcTank(Model model, GraphicsDevice device, Camera camera, Vector3 pos)
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

            translation.Translation = pos;
            v = new Velocity();

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

        public override bool inBrakeRange(Vector3 destination)
        {
            Vector3 curPos = getCurrentPosition();
            if (Math.Abs(curPos.X - destination.X) + Math.Abs(curPos.Z - destination.Z) < 10)
            {
                return true;
            }

            return false;
        }

        public void patrol()
        {

        }
        public void pursue(int elapseTime, Vector3 curSpeed, Vector3 t)
        {
            target = t;
            bEvadePursue = true;
            //translation.Translation += curSpeed;
        }

        private Vector3 target;
        private bool bEvadePursue;

        public void evade(int elapseTime, Vector3 curSpeed, Vector3 t)
        {
            target = t;
            bEvadePursue = false;
            //translation.Translation += curSpeed;
        }

        public override void update(GameTime gameTime)
        {
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

            hatchBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * hatchTransform;
            tankBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * tankTransform;

            if (bClose)
            {
                pickPosition = target;

                destination = pickPosition.Value;
                destination.Y = 0;
                if (!bEvadePursue)
                    distance = getCurrentPosition() - destination;
                else
                    distance = destination - getCurrentPosition();

                direction = distance;
                direction.Normalize();

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


            base.update(gameTime);

        }
        private string curState;
        public override void changePortableModel(string curMode)
        {
            curState = curMode;

        }
        public void setbClose(bool b)
        {
            bClose = b;
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
    }
}
