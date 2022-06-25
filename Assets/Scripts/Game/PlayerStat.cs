// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class PlayerStat
    {
        public const int ScoreForCrystal = 10;
        public const int ScoreForStep = 1;
        private const int ScoreForNextLevel = 300;

        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public int Level { get; private set; } = 1;

        [Inject]
        private SignalBus signalBus;

        public void AddScore(int score)
        {
            Score += score;

            if (Score > Level * ScoreForNextLevel)
            {
                Level++;
                OnNextLevel();
            }
        }

        public void GameStart(int level)
        {
            Level = level;
            Score = 0;
            RestoreResult();
        }

        public void GameEnd()
        {
            // new record
            if (Score > HighScore)
                HighScore = Score;

            StoreResult();
        }

        private void OnNextLevel()
        {
            signalBus.Fire(new LevelUpSignal());
        }

        private void StoreResult()
        {
            PlayerPrefs.SetInt("AmazingTrack_HighScore", HighScore);
        }

        private void RestoreResult()
        {
            if (PlayerPrefs.HasKey("AmazingTrack_HighScore"))
                HighScore = PlayerPrefs.GetInt("AmazingTrack_HighScore");
        }
    }
}