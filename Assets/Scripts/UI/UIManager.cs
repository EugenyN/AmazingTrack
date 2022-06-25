// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject titleUI;
        [SerializeField] GameObject playingUI;
        [SerializeField] GameObject gameEndUI;
        
        [Inject]
        private SignalBus signalBus;

        private void Awake()
        {
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
                case GameState.Title:
                    SwitchUI(titleUI);
                    break;
                case GameState.Playing:
                    SwitchUI(playingUI);
                    break;
                case GameState.GameEnd:
                    SwitchUI(gameEndUI);
                    break;
                default:
                    break;
            }
        }

        private void SwitchUI(GameObject ui)
        {
            foreach (var item in new[] { titleUI, playingUI, gameEndUI })
                item.SetActive(item == ui);
        }
    }
}