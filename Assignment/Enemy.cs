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
        Vector3 direction;

        Vector3 position;
        Vector3 distance;
        Vector3 seekSpeed;

        float angle;

        int directionChangeCount = 0;

        Vector3 destination; 

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

            distance = Tank.tankPosition - position;
            seekSpeed = distance;
            seekSpeed.Normalize();

            //seek
            if (distance.X > 700 || distance.X < -700 || distance.Z > 700 || distance.Z < -700)
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
            //return rotation * world;
            return Matrix.CreateScale(0.20f) * rotation * world;
        }

    }
}
