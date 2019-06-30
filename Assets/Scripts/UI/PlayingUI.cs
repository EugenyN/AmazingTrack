// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class PlayingUI : MonoBehaviour
    {
        public Text scoreText;
        public Text highScoreText;
        public Text levelText;

        GameController gameController;

        [Inject]
        public void Construct(GameController gameController)
        {
            this.gameController = gameController;
        }

        void Update()
        {
            scoreText.text = "Score: " + gameController.Stat.Score;
            highScoreText.text = "High: " + gameController.Stat.HighScore;
            levelText.text = "Level: " + gameController.Stat.Level;
        }
    }
}