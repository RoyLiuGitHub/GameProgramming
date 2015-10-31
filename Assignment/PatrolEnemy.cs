using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class PatrolEnemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;

        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;

        Vector3 position;
        Vector3 distance;
        Vector3 seekSpeed=new Vector3(10,0,-10);

        float angle;

        int directionChangeCount = 0;

        Vector3 destination;


        public PatrolEnemy(Model m, Vector3 Position)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            position = Position;
        }

        public override void update(GameTime gameTime)
        {

            //seek
            if (position.X < -1000)
            {

                position += seekSpeed;
                world = Matrix.CreateTranslation(position);
                angle = (float)Math.Atan2(1, -1);
                rotation = Matrix.CreateRotationY(angle);


            }
            //Flee
            else
            {

                position -= seekSpeed;
                distance += seekSpeed;
                world = Matrix.CreateTranslation(position);
                angle = (float)Math.Atan2(-1, 1);
                rotation = Matrix.CreateRotationY(angle);
            }
            

            base.update(gameTime);
        }

        public override Matrix getWorld()
        {
            return Matrix.CreateScale(0.30f) * rotation * world;
        }
    }
}
