using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Assignment
{
    public class PlayInfo
    {
        static int score;
        static float timeUsed;


        public static void AddScore(int gain)
        {
            score += gain;
        }
        public static int GetScore()
        {
            return score;
        }

        public static void CalculateTime(float time)
        {
            timeUsed += time/1000;
        }
        public static float getTime()
        {

            return timeUsed;
        }

    }
}
