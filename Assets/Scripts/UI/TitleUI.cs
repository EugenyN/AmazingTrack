// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class TitleUI : MonoBehaviour
    {
        GameController gameController;

        [Inject]
        public void Construct(GameController gameController)
        {
            this.gameController = gameController;
        }

        public void OnEasyButtonClick()
        {
            gameController.GameStart(GameController.Mode.Easy);
        }

        public void OnNormalButtonClick()
        {
            gameController.GameStart(GameController.Mode.Normal);
        }

        public void OnHardButtonClick()
        {
            gameController.GameStart(GameController.Mode.Hard);
        }

        public void OnHolesButtonClick()
        {
            gameController.GameStart(GameController.Mode.Holes);
        }
    }
}