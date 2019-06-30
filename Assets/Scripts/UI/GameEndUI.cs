// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameEndUI : MonoBehaviour
    {
        public Text scoreText;

        GameController gameController;

        [Inject]
        public void Construct(GameController gameController)
        {
            this.gameController = gameController;
        }

        private void OnEnable()
        {
            string text = "Your score: " + gameController.Stat.Score;
            bool newRecord = gameController.Stat.Score == gameController.Stat.HighScore;
            if (newRecord)
                text += "\nNew record !";

            scoreText.text = text;
        }
    }
}