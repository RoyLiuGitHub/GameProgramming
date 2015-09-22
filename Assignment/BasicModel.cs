using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //change to public for bullet collision dection   (protected)
        protected virtual Matrix GetWorld()
        {
            return world;
        }


        //for bullet hit detection
        public Matrix GetWorldPublic()
        {
            return world;
        }


        //public bool CollidesWith(Model otherModel, Matrix otherWorld)
        public bool CollidesWith(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            // Loop through each ModelMesh in both objects and compare
            // all bounding spheres for collisions
            //foreach (ModelMesh myModelMeshes in model.Meshes)
            foreach (ModelMesh m1 in model1.Meshes)
            {
                //foreach (ModelMesh hisModelMeshes in otherModel.Meshes) 
                foreach (ModelMesh m2 in model2.Meshes)
                {
                    if (m1.BoundingSphere.Transform(world1).Intersects(m2.BoundingSphere.Transform(world2)))
                        return true;
                }
            }
            return false;
            
        }
    }
}
