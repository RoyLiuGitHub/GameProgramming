using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Boundary:BasicModel
    {
        Matrix translation = Matrix.Identity;
        private const float boundary = 4000f;
        public Boundary(Model model, Vector3 position)
            : base(model)
        {
            translation = Matrix.CreateTranslation(position);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }
        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(1) * translation;
        }
        public static float GetBoundary()
        {
            return boundary;
        }
    }
}
