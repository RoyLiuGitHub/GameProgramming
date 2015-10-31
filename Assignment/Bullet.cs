using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Assignment
{
    class Bullet : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float bulletSize = 0.1f;
        Vector3 direction;
        Vector3 position;

        public Bullet(Model model, Vector3 Position,
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(model)
        {
            world = Matrix.CreateScale(bulletSize) * Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            position = Position;
            translation = Matrix.CreateTranslation(position);
        }

        public override void update(GameTime gameTime)
        {
            // Rotate model
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
                pitchAngle, rollAngle);
            // Move model
            Vector3 temp = direction;
            temp.Normalize();
            temp *= 10;

            world *= Matrix.CreateTranslation(temp);
            //calcaulate bullet position
            position += direction;

            base.update(gameTime);
        }
        public override Matrix getWorld()
        {
            return Matrix.CreateScale(1f) * rotation * world;
        }

    }
}
