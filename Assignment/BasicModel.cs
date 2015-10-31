using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class BasicModel
    {

        public Model model
        {
            get;
            protected set;
        }

        protected Matrix world = Matrix.Identity;

        public BasicModel(Model model)
        {
            this.model = model;
        }
        public virtual void setBoolCollision()
        {

        }
        public virtual Vector3 getCurSpeed()
        {
            return Vector3.Zero;
        }
        public virtual void update(GameTime gameTime)
        {

        }
        public virtual Vector3 getCurDirection()
        {
            return Vector3.Zero;
        }
        public virtual bool inBrakeRange(Vector3 destination)
        {

            return false;
        }
        public virtual Vector3 GetTankDirection()
        {
            return Vector3.Zero;
        }
        public virtual Vector3 GetModelPosition()
        {
            return Vector3.Zero;
        }

        public virtual Vector3 getCurrentPosition()
        {
            return Vector3.Zero;
        }

        public virtual void setModelSpeed(float s)
        {

        }

        public virtual void changePortableModel(string m)
        {
        }


        public bool CollidesWith(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            foreach (ModelMesh m1 in model1.Meshes)
            {
                foreach (ModelMesh m2 in model2.Meshes)
                {
                    if (m1.BoundingSphere.Transform(world1).Intersects(m2.BoundingSphere.Transform(world2)))
                        return true;
                }
            }
            return false;
        }

        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * getWorld();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.TextureEnabled = true;
                    effect.Alpha = 1;
                }
                mesh.Draw();
            }
        }
        public virtual Matrix getWorld()
        {
            return world;
        }

        public virtual Matrix GetScale()
        {
            return Matrix.Identity;
        }

        public virtual void stop()
        {
            
        }
    }
}
