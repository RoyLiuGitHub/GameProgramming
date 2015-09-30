using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Enemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;

        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float angle;
        Vector3 direction;
        Vector3 position;
        Vector3 distance;
        Vector3 seekSpeed;
        private const int seekDistance = 700;
        private bool bhamper = false;

        public Enemy(Model m, Vector3 Position,
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(m)
            {
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            position = Position;
            }

        public override void Update(GameTime gameTime)
        {
            
            if (bhamper)
            {
                seekSpeed = Vector3.Zero;
                seekSpeed.Normalize();
                bhamper = false;
                base.Update(gameTime);
                return;
            }

            distance = Tank.tankPosition - position;
            seekSpeed = distance;
            seekSpeed.Normalize();

            //seek
            if (distance.X > seekDistance || distance.X < -seekDistance || distance.Z > seekDistance || distance.Z < -seekDistance)
            {
                
                position +=  seekSpeed;
                distance -= seekSpeed;
                world = Matrix.CreateTranslation(position);

                angle = (float)Math.Atan2((Tank.tankPosition.X - position.X), (Tank.tankPosition.Z - position.Z));
                rotation = Matrix.CreateRotationY(angle);

            }
            //Flee
            else
            {
                position -= seekSpeed;
                distance += seekSpeed;
                world = Matrix.CreateTranslation(position);
                angle = (float)Math.Atan2((Tank.tankPosition.X - position.X), (Tank.tankPosition.Z - position.Z));
                rotation = Matrix.CreateRotationY(angle);
            }
            base.Update(gameTime);
        }
        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(0.2f) * rotation * world;
        }
        public override Matrix GetWorldPublic()
        {
            return Matrix.CreateScale(0.2f) * rotation * world;
        }
        public override Vector3 GetTankPosition()
        {
            return world.Translation;
        }
        public override void setHamper()
        {
            bhamper = true;
        }
    }
}
