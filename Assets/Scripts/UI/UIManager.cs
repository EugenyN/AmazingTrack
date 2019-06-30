// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class UIManager : MonoBehaviour
    {
        SignalBus signalBus;

        public GameObject titleUI;
        public GameObject playingUI;
        public GameObject gameEndUI;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            this.signalBus = signalBus;

            signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<GameStateChangedSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedSignal signal)
        {
            switch (signal.State)
            {
                case GameController.GameState.Title:
                    SwitchUI(titleUI);
                    break;
                case GameController.GameState.Playing:
                    SwitchUI(playingUI);
                    break;
                case GameController.GameState.GameEnd:
                    SwitchUI(gameEndUI);
                    break;
                default:
                    break;
            }
        }

        private void SwitchUI(GameObject ui)
        {
            foreach (var item in new GameObject[] { titleUI, playingUI, gameEndUI })
                item.SetActive(item == ui);
        }
    }
}