using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class SkyBox : BasicModel
    {
        float skyBoxSize = 4000f;
        public SkyBox(Model model)
            : base(model)
        {
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;
            base.Draw(device, camera);
        }
        protected override Matrix GetWorld()
        {
            //Make skyBox follow the camera
            Vector3 movement = Camera.cameraPosition;
            movement.Y = 0;
            return Matrix.CreateScale(skyBoxSize) * Matrix.CreateTranslation(movement);
        }
    }
}
