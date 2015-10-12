using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class Tree : BasicModel
    {

        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        float scale;
        public Tree(Model model, Vector3 position, float scale)
            : base(model)
        {
            translation = Matrix.CreateTranslation(position);
            this.scale = scale;
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
            return Matrix.CreateScale(scale) * rotation * translation;
        }

        public override Vector3 getModelPosition()
        {
            return translation.Translation* scale;
        }
        public override void treedown()
        {
            rotation *= Matrix.CreateRotationZ(MathHelper.PiOver4 / 10);
        }

        public override Matrix GetWorldPublic()
        {
            rotation = Matrix.Identity;
            return Matrix.CreateScale(scale) * rotation * translation;
        }

        //public override Vector3 GetTankPosition()
        //{
        //    return translation.Translation;
        //}
    }
}
