using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Ground : BasicModel
    {
        public Ground(Model model)
            : base(model)
        {

        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearWrap;
            base.Draw(device, camera);
        }
    }
}
