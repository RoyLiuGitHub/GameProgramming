using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        public BasicModel(Model m)
        {
            model = m;
        }
        public virtual void Update()
        { }
        public virtual void Update(GameTime gametime)
        { }
        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = Camera.projection;
                    be.View = Camera.view;
                    be.World = transforms[mesh.ParentBone.Index] * GetWorld();
                }
                mesh.Draw();
            }
        }
        /// <summary>
        /// change to public for bullet collision dection   (protected)
        /// </summary>
        /// <returns></returns>
        protected virtual Matrix GetWorld()
        {
            return world;
        }

        /// <summary>
        /// for bullet hit detection
        /// </summary>
        /// <returns></returns>

        public virtual Vector3 GetTankPosition(){
            return Vector3.Zero;
        }

        public virtual Matrix GetWorldPublic()
        {
            return world;
        }

        /// <summary>
        /// Collides
        /// Loop through each ModelMesh in both objects and compare
        /// all bounding spheres for collisions
        /// </summary>
        /// <param name="model1">The first object</param>
        /// <param name="world1">The first world</param>
        /// <param name="model2">The Second object</param>
        /// <param name="world2">The Second world</param>
        /// <returns>true or false</returns>
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

        public virtual void setHamper()
        {
        }
    }
}
