using System;
using UnityEngine;

namespace AmazingTrack
{
    [Serializable]
    public class GameSettings
    {
        public float BallInitialSpeed = 5f;
        public GameMode GameMode = GameMode.Normal;
        [Range(1, 10)] public int Level = 1;
        public bool RandomCrystals;
        
        public Color BackgroundColor1 = Color.red;
        public Color BackgroundColor2 = Color.blue;
        public float BackgroundChangeDuration = 5.0f;
        
        public float CameraLerpRate = 5.0f;
        public LayerMask CrystalMask; 
    }
}