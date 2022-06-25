// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] Text scoreText;

        [Inject]
        private GameController gameController;

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