// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameController.Settings GameSettings;
        public AudioSettings AudioPlayer;

        public override void InstallBindings()
        {
            Container.BindInstance(GameSettings).IfNotBound();
            Container.BindInstance(AudioPlayer).IfNotBound();
        }
    }
}

[Serializable]
public class AudioSettings
{
    public AudioClip BallHitCrystalSound;
    public float BallHitCrystalVolume = 1.0f;
    public AudioClip GameStartSound;
    public AudioClip BallFallSound;
    public AudioClip BallTurnSound;
    public AudioClip NextLevelSound;
}