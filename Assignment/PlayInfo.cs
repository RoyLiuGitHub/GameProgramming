using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment
{
    public class PlayInfo
    {
        static int score;
        static float timeUsed;
        static int life;


        public static void AddScore(int gain)
        {
            score += gain;
        }
        public static int GetScore()
        {
            return score;
        }

        public static void initLife(int initLife)
        {
            life= initLife;
        }
        public static void reduceLife(int loss)
        {
            life -= loss;
        }
        public static int GetLife()
        {
            return life;
        }


        public static void CalculateTime(float time)
        {
            timeUsed += time / 1000;
        }
        public static float getTime()
        {

            return timeUsed;
        }

    }
}
