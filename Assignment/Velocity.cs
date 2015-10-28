using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;


namespace Assignment
{
    class Velocity
    {
        private const float acceleration = 0.01f;
        public float Acceleration
        {
            get { return acceleration; }
        }

        private const float max_speed = 10f;
        public float maxSpeed
        {
            get { return max_speed; }
        }

        private float speed = 0;
        public float Speed
        {
            get { return speed; }
            set { speed = 0; }
        }

        private float edge = 10;
        public float Edge
        {
            get { return edge; }
        }

        public void increaseVelocity(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;
            float tmps;
            if ((tmps = speed + acceleration * time) < max_speed)
                speed = tmps;
            else
                speed = max_speed;
        }

        public void decreaseVelocity(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.Milliseconds / 100;
            float tmps;
            if ((tmps = speed - acceleration * time) > 0)
                speed = tmps;
            else
                speed = 0;
        }
    }
}
