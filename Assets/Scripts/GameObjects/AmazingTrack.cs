// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace AmazingTrack
{
    /// <summary>
    /// AmazingTrack
    /// 
    /// Encapsulates the behavior of the entire track: rules of creation 
    /// and fall/removing blocks and crystals on the track.
    /// 
    /// uses ObjectSpawner to create and delete (place in a pool) game objects
    /// </summary>
    public class AmazingTrack
    {
        private const int BlocksOnScreen = 30;
        private const float BallSpawnHeight = 0.75f;

        private readonly ObjectSpawner spawner;
        private readonly GameplayStrategiesProvider gameplayStrategies;

        private Vector3 lastSpawnPos = new Vector3(0f, 0f, 0f);
        private readonly Queue<GameObject> liveBlocksQueue = new Queue<GameObject>();

        private int BlocksInGroup = 1;

        public Ball Ball { get; private set; }

        public AmazingTrack(SignalBus signalBus, ObjectSpawner spawner,
            GameplayStrategiesProvider gameplayStrategies)
        {
            this.spawner = spawner;
            this.gameplayStrategies = gameplayStrategies;

            signalBus.Subscribe<BallMovedToNextBlockSignal>(OnBallMovedToNextBlock);
            signalBus.Subscribe<BallHitCrystalSignal>(OnBallHitCrystal);
        }

        public void CreateObjects(bool randomCrystals, bool makeHoles, int blocksInGroup, float ballSpeed)
        {
            gameplayStrategies.RandomCrystals = randomCrystals;
            gameplayStrategies.MakeHoles = makeHoles;
            BlocksInGroup = blocksInGroup;

            var startPlatform = spawner.SpawnStartPlatform(Color.gray);
            liveBlocksQueue.Enqueue(startPlatform);

            lastSpawnPos = new Vector3(1f, 0f, 1f);

            for (int i = 0; i < BlocksOnScreen; i++)
                SpawnNextBlocks();

            var ballObj = spawner.SpawnBall(new Vector3(0, BallSpawnHeight, 0));
            Ball = ballObj.GetComponent<Ball>();
            Ball.Speed = ballSpeed;
        }

        public void DestroyObjects()
        {
            if (Ball != null)
                Object.DestroyImmediate(Ball.gameObject);

            liveBlocksQueue.Clear();
            spawner.Clear();

            gameplayStrategies.Reset();
        }

        private void SpawnNextBlocks()
        {
            bool rightDirection = Random.Range(0, 2) == 0;

            lastSpawnPos += rightDirection ? Vector3.right : Vector3.forward;

            var color = new Color(Random.value, Random.value, Random.value, 1.0f);
            var group = SpawnBlocksGroupWithCrystalAndHole(lastSpawnPos, rightDirection, color);
            liveBlocksQueue.Enqueue(group);
        }

        private GameObject SpawnBlocksGroupWithCrystalAndHole(Vector3 spawnPos, bool rightSide, Color color)
        {
            var group = spawner.SpawnBlocksGroup(BlocksInGroup, spawnPos, rightSide, color);
            var groupObj = group.gameObject;

            Assert.IsTrue(groupObj.transform.childCount == BlocksInGroup);
            
            if (gameplayStrategies.MakeHoles)
            {
                if (gameplayStrategies.GetBlockHolesStrategy().IsTimeToHole())
                    groupObj.GetComponent<BlocksGroup>().MakeHole();
            }

            if (gameplayStrategies.GetCrystalSpawnStrategy().ShouldSpawn())
            {
                var child = groupObj.transform.GetChild(Random.Range(0, BlocksInGroup - 1));
                if (child.gameObject.activeSelf)
                    spawner.SpawnOrResetCrystal(child);
            }

            return groupObj;
        }

        private void OnBallHitCrystal(BallHitCrystalSignal signal)
        {
            signal.Crystal.GetComponent<Crystal>().Take();
            spawner.DespawnObject(signal.Crystal);
        }

        private void OnBallMovedToNextBlock(BallMovedToNextBlockSignal signal)
        {
            if (signal.PreviousBlock == null)
                return;

            bool isSameGroup = signal.Block.IsSiblingObject(signal.PreviousBlock);
            if (!isSameGroup)
                OnMovedToBlockInNewGroup(signal.Block, signal.PreviousBlock);
        }

        private void OnMovedToBlockInNewGroup(GameObject block, GameObject previousBlock)
        {
            var previousBlockGroup = previousBlock.ParentObject();

            if (block.GetComponent<Block>().Falling)
                return; // ignore falling blocks

            if (previousBlock.GetComponent<Block>().Falling)
                return; // ignore falling blocks

            Assert.IsTrue(liveBlocksQueue.Contains(previousBlockGroup));

            GameObject blockGroup = null;
            while (blockGroup != previousBlockGroup)
            {
                blockGroup = liveBlocksQueue.Dequeue();
                FallDownBlocks(blockGroup);
                SpawnNextBlocks();
            }
        }

        private void FallDownBlocks(GameObject blockGroup)
        {
            spawner.DespawnObjectDelayed(blockGroup, 2.0f);
            
            foreach (Transform child in blockGroup.transform)
            {
                var block = child.gameObject.GetComponent<Block>();
                if (child.gameObject.activeSelf)
                {
                    block.FallDown();

                    if (block.HasCrystal())
                    {
                        var crystal = block.GetCrystal().GetComponent<Crystal>();
                        crystal.FallDown();
                        spawner.DespawnObjectDelayed(block.GetCrystal(), 2.0f);
                    }
                }
            }
        }
    }
}