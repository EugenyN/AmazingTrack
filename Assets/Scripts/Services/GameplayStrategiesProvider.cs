namespace AmazingTrack
{
    public class GameplayStrategiesProvider
    {
        private readonly GameSettings gameSettings;
        private CrystalSpawnStrategy crystalSpawnStrategy;
        private BlockHolesStrategy blockHolesStrategy;

        public GameplayStrategiesProvider(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }

        public CrystalSpawnStrategy GetCrystalSpawnStrategy()
        {
            if (crystalSpawnStrategy == null)
            {
                if (gameSettings.RandomCrystals)
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