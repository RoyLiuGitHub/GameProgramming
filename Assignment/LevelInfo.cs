﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment
{
    class LevelInfo
    {
        //Spawn variables
        public int minSpawnTime { get; set; }
        public int maxSpawnTime { get; set; }

        //Enemy count variables
        public int numberEnemies { get; set; }
        public int minSpeed { get; set; }
        public int maxSpeed { get; set; }

        //misses
        public int missesAllowed { get; set; }

        public LevelInfo(int minSpawnTime, int maxSpawnTime,
            int numberEnemies, int minSpeed, int maxSpeed,
            int missesAllowed)
        {
            this.minSpawnTime = minSpawnTime;
            this.maxSpawnTime = maxSpawnTime;
            this.numberEnemies = numberEnemies;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
            this.missesAllowed = missesAllowed;

        }
    }
}
