﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment
{
    class MousePick
    {
        GraphicsDevice device;
        Camera camera;
        public MousePick(GraphicsDevice device, Camera camera)
        {
            this.device = device;
            this.camera = camera;
        }

        public Vector3? GetCollisionPosition()
        {
            MouseState mousestate = Mouse.GetState();

            Vector3 nearSource = new Vector3(mousestate.X, mousestate.Y, 0f);
            Vector3 farSource = new Vector3(mousestate.X, mousestate.Y, 1f);

            Vector3 nearPoint = device.Viewport.Unproject(nearSource, camera.projection, camera.view, Matrix.Identity);
            Vector3 farPoint = device.Viewport.Unproject(farSource, camera.projection, camera.view, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray pickRay = new Ray(nearPoint, direction);
            Nullable<float> result = pickRay.Intersects(new Plane(Vector3.Up, 0f));

            Vector3? resultVector = direction * result;
            Vector3? collisionPoint = resultVector + nearPoint;

            return collisionPoint;
        }
    }
}
