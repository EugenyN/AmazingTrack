// Copyright 2019 Eugeny Novikov. Code under MIT license.

namespace AmazingTrack
{
    public class GameplayStrategiesProvider
    {
        private CrystalSpawnStrategy crystalSpawnStrategy;
        private BlockHolesStrategy blockHolesStrategy;

        //TODO: reset cache when change.
        public bool RandomCrystals = false;
        public bool MakeHoles = false;

        public CrystalSpawnStrategy GetCrystalSpawnStrategy()
        {
            if (crystalSpawnStrategy == null)
            {
                if (RandomCrystals)
                    crystalSpawnStrategy = new RandomCrystalSpawnStrategy();
                else
                    crystalSpawnStrategy = new ProgressiveCrystalSpawnStrategy();
            }

            return crystalSpawnStrategy;
        }

        public BlockHolesStrategy GetBlockHolesStrategy()
        {
            return blockHolesStrategy ??= new BlockHolesStrategy();
        }

        public void Reset()
        {
            crystalSpawnStrategy = null;
            blockHolesStrategy = null;
        }
    }
}