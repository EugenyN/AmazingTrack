// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    /// <summary>
    /// Main game states, score calculation
    /// </summary>
    public class GameController : IInitializable, ITickable, IDisposable
    {
        public enum GameState
        {
            Title, Playing, GameEnd
        }

        public enum Mode
        {
            Easy,
            Normal,
            Hard,
            Holes
        }

        GameState state;

        public GameState State
        {
            get { return state; }
        }

        [Inject]
        public readonly PlayerStat Stat;

        readonly AmazingTrack track;
        readonly CameraFollow camera;
        readonly SignalBus signalBus;
        readonly Settings settings;
        readonly AudioSettings audioSettings;
        readonly AudioPlayer audioPlayer;

        public GameController(SignalBus signalBus, AmazingTrack track, 
            CameraFollow camera, Settings settings, AudioPlayer audioPlayer, AudioSettings audioSettings)
        {
            this.signalBus = signalBus;
            this.track = track;
            this.camera = camera;
            this.settings = settings;
            this.audioPlayer = audioPlayer;
            this.audioSettings = audioSettings;
        }

        public void Initialize()
        {
            signalBus.Subscribe<BallCrashedSignal>(OnBallCrashed);
            signalBus.Subscribe<BallMovedToNextBlockSignal>(OnBallMovedToNextBlock);
            signalBus.Subscribe<BallHitCrystalSignal>(OnBallHitCrystal);
            signalBus.Subscribe<LevelUpSignal>(OnLevelUp);

            ShowTitle(false);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<BallCrashedSignal>(OnBallCrashed);
            signalBus.Unsubscribe<BallMovedToNextBlockSignal>(OnBallMovedToNextBlock);
            signalBus.Unsubscribe<BallHitCrystalSignal>(OnBallHitCrystal);
            signalBus.Unsubscribe<LevelUpSignal>(OnLevelUp);
        }

        public void Tick()
        {
            switch (state)
            {
                case GameState.Title:
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                            Application.Quit();
                    }
                    break;
                case GameState.Playing:
                    if (Input.GetKeyDown(KeyCode.Escape))
                        ShowTitle();

                    if (Input.GetMouseButtonDown(0))
                        track.Ball.ChangeDirection();

                    break;
                case GameState.GameEnd:
                    {
                        if (Input.GetMouseButtonDown(0))
                            GameStart(true);

                        if (Input.GetKeyDown(KeyCode.Escape))
                            ShowTitle();
                    }
                    break;
                default:
                    break;
            }
        }

        private void ClearScene()
        {
            track.DestroyObjects();
        }

        private void InitScene()
        {
            Stat.GameStart(settings.Level);

            track.CreateObjects(settings.RandomCrystals, settings.GameMode == Mode.Holes, 
                GetBlocksCountInGroup(), GetBallSpeedForCurrentLevel());

            camera.StartFolow(track.Ball.gameObject);
        }

        private void ChangeState(GameState state)
        {
            this.state = state;
            signalBus.Fire(new GameStateChangedSignal(state));
        }

        private void OnBallCrashed()
        {
            GameEnd();
        }

        private void OnBallMovedToNextBlock(BallMovedToNextBlockSignal signal)
        {
            if (signal.PreviousBlock == null)
                return;

            Stat.AddScore(PlayerStat.ScoreForStep);
        }

        private void OnBallHitCrystal(BallHitCrystalSignal signal)
        {
            Stat.AddScore(PlayerStat.ScoreForCrystal);

            audioPlayer.Play(audioSettings.BallHitCrystalSound, audioSettings.BallHitCrystalVolume);
        }

        private void OnLevelUp()
        {
            audioPlayer.Play(audioSettings.NextLevelSound);

            track.Ball.Speed = GetBallSpeedForCurrentLevel();
        }

        public float GetBallSpeedForCurrentLevel()
        {
            return settings.BallInitialSpeed + Stat.Level - 1;
        }

        public void ShowTitle(bool clearScene = true)
        {
            if (clearScene)
                ClearScene();
            InitScene();
            ChangeState(GameState.Title);
        }

        public void GameStart(Mode mode)
        {
            bool recreate = settings.GameMode != mode;
            settings.GameMode = mode;
            GameStart(recreate);
        }

        public void GameStart(bool recreateScene)
        {
            if (recreateScene)
            {
                ClearScene();
                InitScene();
            }

            ChangeState(GameState.Playing);

            audioPlayer.Play(audioSettings.GameStartSound);
        }

        private void GameEnd()
        {
            Stat.GameEnd();

            camera.StopFollow();

            audioPlayer.Play(audioSettings.BallFallSound);

            // swith to game end after 1 sec
            track.StartCoroutine(SwithToGameEndState());
        }

        private IEnumerator SwithToGameEndState()
        {
            yield return new WaitForSeconds(1.0f);
            ChangeState(GameState.GameEnd);
        }

        private int GetBlocksCountInGroup()
        {
            switch (settings.GameMode)
            {
                case Mode.Easy:
                    return 3;
                case Mode.Normal:
                    return 2;
                case Mode.Hard:
                    return 1;
                case Mode.Holes:
                    return 3;
                default:
                    return 2;
            }
        }

        [Serializable]
        public class Settings
        {
            public float BallInitialSpeed = 5f;
            public Mode GameMode = Mode.Normal;
            [Range(1, 10)]
            public int Level = 1;
            public bool RandomCrystals = false;
        }
    }
}