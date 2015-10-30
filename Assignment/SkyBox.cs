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
            translation.Translation = new Vector3(0, 300, 0);
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


            //return Matrix.CreateScale(3000f);

            return Matrix.CreateScale(3600f) * translation;
        }


    }
}
