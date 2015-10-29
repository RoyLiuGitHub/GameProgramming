using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class SkyBox : BasicModel
    {
        public SkyBox(Model model)
            : base(model)
        {

        }

        /*public override void update()
        {
            base.update();
        }*/

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;
            base.Draw(device, camera);
        }

        public override Matrix getWorld()
        {
            ////return Matrix.CreateScale(2000f);
            //Vector3 movement = Camera.cameraPosition;
            //movement.Y = 0;

            //return Matrix.CreateScale(2000f) * Matrix.CreateTranslation(movement);
            return Matrix.CreateScale(3000f);
        }


    }
}
