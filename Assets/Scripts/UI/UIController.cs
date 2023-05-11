using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] GameObject titleUI;
        [SerializeField] GameObject playingUI;
        [SerializeField] GameObject gameEndUI;
        
        [Inject]
        private GameSystem gameSystem;

        private GameState gameState;
        private Dictionary<GameState, GameObject> uiDictionary;

        private void Awake()
        {
            uiDictionary = new Dictionary<GameState, GameObject>
            {
                { GameState.Title, titleUI }, { GameState.Playing, playingUI }, { GameState.GameEnd, gameEndUI }
            };
        }

        private void Update()
        {
            var gameStateComponent = gameSystem.GetGameState();
            if (gameStateComponent.State != gameState)
            {
                if (uiDictionary.TryGetValue(gameState, out var oldUi))
                    oldUi.SetActive(false);
                
                gameState = gameStateComponent.State;
                
                if (uiDictionary.TryGetValue(gameState, out var newUi))
                    newUi.SetActive(true);
            }
        }
    }
}