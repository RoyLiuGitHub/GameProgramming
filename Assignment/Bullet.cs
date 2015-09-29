﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Bullet:BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float bulletSize = 0.1f;
        Vector3 direction;
        Vector3 position;
        //steven
        private int testkk = 0;

        public Bullet(Model model, Vector3 Position, 
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(model)
        {
            world = Matrix.CreateScale(bulletSize) *Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            position = Position;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate model
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
                pitchAngle, rollAngle);

            // Move model
            world *= Matrix.CreateTranslation(direction);

            //calcaulate bullet position
            position += direction;

            base.Update(gameTime);
        }

        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(1f) * rotation * world;
        }
    }
}
