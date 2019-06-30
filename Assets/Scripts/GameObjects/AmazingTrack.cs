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
    public class AmazingTrack : MonoBehaviour
    {
        private const int BlocksOnScreen = 30;
        private const float BallSpawnHeight = 0.75f;
        private const float FallTimeBeforeDead = 2.0f;

        private ObjectSpawner spawner;
        private SignalBus signalBus;
        private Vector3 lastSpawnPos = new Vector3(0f, 0f, 0f);
        private Queue<GameObject> liveBlocksQueue = new Queue<GameObject>();

        private CrystalSpawnStrategy crystalSpawnStrategy;
        private BlockHolesStrategy blockHolesStrategy;

        public bool RandomCrystals = false;
        public bool MakeHoles = false;
        public int BlocksInGroup = 1;

        public Ball Ball { get; private set; }

        private CrystalSpawnStrategy GetCrystalSpawnStrategy()
        {
            if (crystalSpawnStrategy == null)
            {
                if (RandomCrystals)
                    crystalSpawnStrategy = new RandomCrystalSpawnStrategy();
                else
                    crystalSpawnStrategy = new ProgressiveCrystalSpawnStrategy();
            }
            return crystalSpawnStrategy;
        }

        private BlockHolesStrategy GetBlockHolesStrategy()
        {
            if (blockHolesStrategy == null)
                blockHolesStrategy = new BlockHolesStrategy();
            return blockHolesStrategy;
        }


        [Inject]
        public void Construct(SignalBus signalBus, ObjectSpawner spawner)
        {
            this.signalBus = signalBus;
            this.spawner = spawner;

            signalBus.Subscribe<BallMovedToNextBlockSignal>(OnBallMovedToNextBlock);
            signalBus.Subscribe<BallHitCrystalSignal>(OnBallHitCrystal);
        }

        public void CreateObjects(bool randomCrystals, bool makeHoles, int blocksInGroup, float ballSpeed)
        {
            RandomCrystals = randomCrystals;
            MakeHoles = makeHoles;
            BlocksInGroup = blocksInGroup;

            GameObject startPlatform = spawner.SpawnStartPlatform(Color.gray);
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
            StopAllCoroutines();

            if (Ball != null)
                DestroyImmediate(Ball.gameObject);

            liveBlocksQueue.Clear();

            spawner.Clear();

            crystalSpawnStrategy = null;
        }

        void SpawnNextBlocks()
        {
            bool rightDirection = Random.Range(0, 2) == 0;

            lastSpawnPos += rightDirection ? Vector3.right : Vector3.forward;

            Color color = new Color(Random.value, Random.value, Random.value, 1.0f);
            GameObject group = SpawnBlocksGroupWithCrystalAndHole(lastSpawnPos, rightDirection, color);
            liveBlocksQueue.Enqueue(group);
        }

        GameObject SpawnBlocksGroupWithCrystalAndHole(Vector3 spawnPos, bool rightSide, Color color)
        {
            var group = spawner.SpawnBlocksGroup(BlocksInGroup, spawnPos, rightSide, color);
            GameObject groupObj = group.gameObject;

            if (MakeHoles)
            {
                Assert.IsTrue(groupObj.transform.childCount == 3);

                if (GetBlockHolesStrategy().IsTimeToHole())
                    groupObj.GetComponent<BlocksGroup>().MakeHole();
            }

            if (GetCrystalSpawnStrategy().ShouldSpawn())
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
            spawner.DespawnCrystal(signal.Crystal);
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
                StartCoroutine(FallDownBlocks(blockGroup));
                SpawnNextBlocks();
            }
        }

        private IEnumerator FallDownBlocks(GameObject blockGroup)
        {
            foreach (Transform child in blockGroup.transform)
            {
                var block = child.gameObject.GetComponent<Block>();
                if (child.gameObject.activeSelf)
                {
                    block.FallDown();

                    if (block.HasCrystal())
                        StartCoroutine(FallDownCrystal(block.GetCrystal()));
                }
            }

            yield return new WaitForSeconds(FallTimeBeforeDead);
            spawner.DespawnBlocksGroup(blockGroup);
        }

        private IEnumerator FallDownCrystal(GameObject crystal)
        {
            crystal.GetComponent<Crystal>().FallDown();
            yield return new WaitForSeconds(FallTimeBeforeDead);
            spawner.DespawnCrystal(crystal);
        }
    }
}