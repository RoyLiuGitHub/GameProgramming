using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    class Bullet:BasicModel
    {
        ////bullet  change to public for shots count
        //public List<BasicModel> shots = new List<BasicModel>();
        float shotMinZ = -200;

        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;

        Vector3 position;


        public Bullet(Model model, Vector3 Position, 
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(model)
        {
            world = Matrix.CreateScale(0.1f)*Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            position = Position;
        }



        //public override void Update(GameTime gameTime)
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
