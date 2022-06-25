// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class PlayingUI : MonoBehaviour
    {
        [SerializeField] Text scoreText;
        [SerializeField] Text highScoreText;
        [SerializeField] Text levelText;

        [Inject] 
        private GameController gameController;

        private int score;
        private int highScore;
        private int level;
        
        private void Update()
        {
            if (gameController.Stat.Score != score)
            {
                scoreText.text = "Score: " + gameController.Stat.Score;
                score = gameController.Stat.Score;
            }

            if (gameController.Stat.HighScore != highScore)
            {
                highScoreText.text = "High: " + gameController.Stat.HighScore;
                highScore = gameController.Stat.HighScore;
            }

            if (gameController.Stat.Level != level)
            {
                levelText.text = "Level: " + gameController.Stat.Level;
                level = gameController.Stat.Level;
            }
        }
    }
}