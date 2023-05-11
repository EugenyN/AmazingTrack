using UnityEngine;

namespace AmazingTrack
{
    public abstract class CrystalSpawnStrategy
    {
        public abstract bool ShouldSpawn();
    }

    public class RandomCrystalSpawnStrategy : CrystalSpawnStrategy
    {
        private const int Chance = 5;

        public override bool ShouldSpawn()
        {
            return Random.Range(0, Chance) == 0;
        }
    }

    public class ProgressiveCrystalSpawnStrategy : CrystalSpawnStrategy
    {
        private const int ProgressiveStep = 5;

        private int blockCounter = -1;
        private int blockWithCrystalCounter = 0;

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