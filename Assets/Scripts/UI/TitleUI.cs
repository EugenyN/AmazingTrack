// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class TitleUI : MonoBehaviour
    {
        [Inject] 
        private GameController gameController;

        public void OnEasyButtonClick()
        {
            gameController.GameStart(GameMode.Easy);
        }

        public void OnNormalButtonClick()
        {
            gameController.GameStart(GameMode.Normal);
        }

        public void OnHardButtonClick()
        {
            gameController.GameStart(GameMode.Hard);
        }

        public void OnHolesButtonClick()
        {
            gameController.GameStart(GameMode.Holes);
        }
    }
}