// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public enum GameState
    {
        Title,
        Playing,
        GameOver,
        GameEnd
    }

    public enum GameMode
    {
        Easy,
        Normal,
        Hard,
        Holes
    }

    /// <summary>
    /// Main game states, score calculation
    /// </summary>
    public class GameController : IInitializable, ITickable, IDisposable
    {
        private GameState state;

        [Inject] public readonly PlayerStat Stat;

        private readonly AmazingTrack track;
        private readonly CameraFollow camera;
        private readonly SignalBus signalBus;
        private readonly GameSettings gameSettings;
        private readonly AudioSettings audioSettings;
        private readonly AudioPlayer audioPlayer;
        
        private float gameOverTimer;

        public GameController(SignalBus signalBus, AmazingTrack track, CameraFollow camera,
            GameSettings gameSettings, AudioPlayer audioPlayer, AudioSettings audioSettings)
        {
            this.signalBus = signalBus;
            this.track = track;
            this.camera = camera;
            this.gameSettings = gameSettings;
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
                case GameState.GameOver:
                    gameOverTimer -= Time.deltaTime;
                    if (gameOverTimer <= 0)
                        ChangeState(GameState.GameEnd);
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
            Stat.GameStart(gameSettings.Level);

            track.CreateObjects(gameSettings.RandomCrystals, gameSettings.GameMode == GameMode.Holes,
                GetBlocksCountInGroup(), GetBallSpeedForCurrentLevel());

            camera.StartFollow(track.Ball.gameObject);
        }

        private void ChangeState(GameState state)
        {
            this.state = state;
            signalBus.Fire(new GameStateChangedSignal(state));
        }

        private void OnBallCrashed()
        {
            GameOver();
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
            return gameSettings.BallInitialSpeed + Stat.Level - 1;
        }

        public void ShowTitle(bool clearScene = true)
        {
            if (clearScene)
                ClearScene();
            InitScene();
            ChangeState(GameState.Title);
        }

        public void GameStart(GameMode gameMode)
        {
            bool recreate = gameSettings.GameMode != gameMode;
            gameSettings.GameMode = gameMode;
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

        private void GameOver()
        {
            Stat.GameEnd();

            camera.StopFollow();

            audioPlayer.Play(audioSettings.BallFallSound);

            gameOverTimer = 1.0f;
            ChangeState(GameState.GameOver);
        }

        private int GetBlocksCountInGroup()
        {
            switch (gameSettings.GameMode)
            {
                case GameMode.Easy:
                    return 3;
                case GameMode.Normal:
                    return 2;
                case GameMode.Hard:
                    return 1;
                case GameMode.Holes:
                    return 3;
                default:
                    return 2;
            }
        }
    }
}