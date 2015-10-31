using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class SkyBox : BasicModel
    {
        Matrix translation = Matrix.Identity;
        public SkyBox(Model model)
            : base(model)
        {
            translation.Translation = new Vector3(0, 3000, 0);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;
            base.Draw(device, camera);
        }

        public override Matrix getWorld()
        {
            return Matrix.CreateScale(2000f) * translation;
        }


    }
}
