using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class BackgroundColorSystem : ITickable, IInitializable
    {
        private readonly GameSettings gameSettings;
        private Camera cam;

        public BackgroundColorSystem(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }
        
        public void Initialize()
        {
            cam = Camera.main;
            cam.clearFlags = CameraClearFlags.SolidColor;
        }
        
        public void Tick()
        {
            float d = gameSettings.BackgroundChangeDuration;
            float t = Mathf.PingPong(Time.time, d) / d;
            cam.backgroundColor = Color.Lerp(gameSettings.BackgroundColor1, gameSettings.BackgroundColor2, t);
        }
    }
}