// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;

namespace AmazingTrack
{
    public class BallCrashedSignal
    {
    }

    public class BallMovedToNextBlockSignal
    {
        public readonly GameObject Block;
        public readonly GameObject PreviousBlock;

        public BallMovedToNextBlockSignal(GameObject block, GameObject previousBlock)
        {
            Block = block;
            PreviousBlock = previousBlock;
        }
    }

    public class BallHitCrystalSignal
    {
        public readonly GameObject Crystal;

        public BallHitCrystalSignal(GameObject crystal)
        {
            Crystal = crystal;
        }
    }

    public class GameStateChangedSignal
    {
        public readonly GameState State;

        public GameStateChangedSignal(GameState state)
        {
            State = state;
        }
    }

    public class LevelUpSignal
    {
    }
}