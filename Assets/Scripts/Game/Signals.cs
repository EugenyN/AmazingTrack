// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmazingTrack
{
    public class BallCrashedSignal
    {
    }

    public class BallMovedToNextBlockSignal
    {
        public GameObject Block;
        public GameObject PreviousBlock;

        public BallMovedToNextBlockSignal(GameObject block, GameObject previousBlock)
        {
            Block = block;
            PreviousBlock = previousBlock;
        }
    }

    public class BallHitCrystalSignal
    {
        public GameObject Crystal;

        public BallHitCrystalSignal(GameObject crystal)
        {
            Crystal = crystal;
        }
    }

    public class GameStateChangedSignal
    {
        public GameController.GameState State;

        public GameStateChangedSignal(GameController.GameState state)
        {
            State = state;
        }
    }

    public class LevelUpSignal
    {
    }
}