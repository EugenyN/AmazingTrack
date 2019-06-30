// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;

namespace AmazingTrack
{
    public abstract class CrystalSpawnStrategy
    {
        public abstract bool ShouldSpawn();
    }

    public class RandomCrystalSpawnStrategy : CrystalSpawnStrategy
    {
        const int Chance = 5;

        public override bool ShouldSpawn()
        {
            return Random.Range(0, Chance) == 0;
        }
    }

    public class ProgressiveCrystalSpawnStrategy : CrystalSpawnStrategy
    {
        const int ProgressiveStep = 5;

        int blockCounter = -1;
        int blockWithCrystalCounter = 0;

        public override bool ShouldSpawn()
        {
            blockCounter++;
            if (blockCounter == ProgressiveStep)
            {
                blockCounter = 0;

                blockWithCrystalCounter++;
                if (blockWithCrystalCounter == ProgressiveStep)
                    blockWithCrystalCounter = 0;
            }

            return blockCounter == blockWithCrystalCounter;
        }
    }
}