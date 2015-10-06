using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    /// <summary>
    ///  unfinished   it used for tank sight
    /// </summary>
    //class SightFocus
    //{
    //    GraphicsDevice device;
    //    Camera camera;

    //    //public SightFocus(GraphicsDevice device, Camera camera)
    //    public SightFocus()
    //    {
    //    }

    //    public Vector3 GetCollisionPosition()
    //    {
    //        Vector3 sightOnScreen;
    //        //Vector3 direction = farPoint - nearPoint;
    //        Vector3 direction = Tank.turretDirection;
    //        direction.Normalize();
    //        Vector3 startPoint = Tank.tankPosition;
    //        Ray pickRay = new Ray(startPoint, direction);
    //        Nullable<float> result = pickRay.Intersects(new Plane(Vector3.Up, 0f));

    //        Vector3? resultVector = direction * result;
    //        Vector3? collisionPoint = resultVector + startPoint;

    //        //graphics.GraphicsDevice.Viewport.Unproject(collisionPoint, camera.ProjectionMatrix, camera.ViewMatrix, camera.WorldMatrix);
    //        if (collisionPoint.HasValue == true)
    //        {
    //            sightOnScreen = device.Viewport.Project(
    //                collisionPoint.Value, 
    //                Camera.projection,
    //                Camera.view,
    //                Matrix.Identity
    //                );
    //        }
    //        else
    //        {
    //            sightOnScreen = Vector3.Zero;
    //        }

    //        return sightOnScreen;
    //    }
    //}
}
