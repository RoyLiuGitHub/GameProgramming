using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Boundary : BasicModel
    {
        Matrix translation = Matrix.Identity;
        private const float boundary = 2000f;
        public Boundary(Model model, GraphicsDevice device, Camera camera, Vector3 postions)
            : base(model)
        {
            translation = Matrix.CreateTranslation(postions);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix getWorld()
        {
            return Matrix.CreateScale(2) * translation;
        }

        public static float GetBoundary()
        {
            return boundary;
        }

        public override Vector3 GetModelPosition()
        {
            return translation.Translation;
        }
    }
}
