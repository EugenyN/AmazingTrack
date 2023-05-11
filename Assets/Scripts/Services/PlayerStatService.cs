using Leopotam.EcsLite;
using UnityEngine;

namespace AmazingTrack
{
    public class PlayerStatService
    {
        private readonly EcsWorld world;
        
        public const int ScoreForCrystal = 10;
        public const int ScoreForStep = 1;
        private const int ScoreForNextLevel = 300;
        
        private readonly EcsPool<PlayerStatComponent> playerStatPool;
        private readonly EcsPool<PlayerLevelUpComponent> playerLevelUpPool;
        private readonly EcsFilter playerStatFilter;
        
        public PlayerStatService(EcsWorld world)
        {
            this.world = world;

            playerStatPool = world.GetPool<PlayerStatComponent>();
            playerLevelUpPool = world.GetPool<PlayerLevelUpComponent>();
            playerStatFilter = world.Filter<PlayerStatComponent>().End();
        }

        public ref PlayerStatComponent GetPlayerStat()
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            return ref playerStatPool.Get(playerStat);
        }

        public void AddScore(int score)
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            ref var playerStatComponent = ref playerStatPool.Get(playerStat);
            playerStatComponent.Score += score;
            if (playerStatComponent.Score > playerStatComponent.Level * ScoreForNextLevel)
            {
                playerStatComponent.Level++;
                playerLevelUpPool.Add(playerStat);
            }
        }

        public void GameStart(int level)
        {
            var playerStat = world.NewEntity();
            
            ref var playerStatComponent = ref playerStatPool.Add(playerStat);
            playerStatComponent.Level = level;
            playerStatComponent.Score = 0;
            RestoreResult(ref playerStatComponent);
        }

        public void GameEnd()
        {
            // new record
            var playerStat = playerStatFilter.GetRawEntities()[0];
            ref var playerStatComponent = ref playerStatPool.Get(playerStat);
            if (playerStatComponent.Score > playerStatComponent.HighScore)
                playerStatComponent.HighScore = playerStatComponent.Score;

            StoreResult(playerStatComponent);
        }

        public void Clear()
        {
            var playerStat = playerStatFilter.GetRawEntities()[0];
            world.DelEntity(playerStat);
        }

        private void StoreResult(in PlayerStatComponent playerStatComponent)
        {
            PlayerPrefs.SetInt("AmazingTrack_HighScore", playerStatComponent.HighScore);
        }

        private void RestoreResult(ref PlayerStatComponent playerStatComponent)
        {
            if (PlayerPrefs.HasKey("AmazingTrack_HighScore"))
                playerStatComponent.HighScore = PlayerPrefs.GetInt("AmazingTrack_HighScore");
        }
    }
}