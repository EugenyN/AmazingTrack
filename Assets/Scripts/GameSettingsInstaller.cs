using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameSettings GameSettings;
        public AudioSettings AudioPlayer;

        public override void InstallBindings()
        {
            Container.BindInstance(GameSettings).IfNotBound();
            Container.BindInstance(AudioPlayer).IfNotBound();
        }
    }
}