using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AmazingTrack
{
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] Text scoreText;

        [Inject] private PlayerStatService playerStatService;
        
        private void OnEnable()
        {
            ref var playerStatComponent = ref playerStatService.GetPlayerStat();
            
            string text = "Your score: " + playerStatComponent.Score;
            bool newRecord = playerStatComponent.Score == playerStatComponent.HighScore;
            if (newRecord)
                text += "\nNew record !";

            scoreText.text = text;
        }
    }
}