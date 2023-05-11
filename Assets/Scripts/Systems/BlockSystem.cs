using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace AmazingTrack
{
    public class BlockSystem : ITickable
    {
        private const int BlocksOnScreen = 30;

        private readonly EcsWorld world;
        private readonly ObjectSpawner spawner;
        private readonly GameplayStrategiesProvider gameplayStrategies;
     
        private readonly GameSettings gameSettings;

        private Vector3 lastSpawnPos;
        private readonly Queue<int> liveBlocksEntities = new Queue<int>();
        
        private readonly EcsPool<BlockComponent> blockPool;
        private readonly EcsPool<FallingComponent> fallingPool;
        private readonly EcsPool<ViewLinkComponent> viewLinkPool;

        private readonly EcsFilter ballPassedFilter;

        private int BlockPartsCount = 1;

        public BlockSystem(EcsWorld world, ObjectSpawner spawner,
            GameplayStrategiesProvider gameplayStrategies, GameSettings gameSettings)
        {
            this.world = world;
            this.spawner = spawner;
            this.gameplayStrategies = gameplayStrategies;
            this.gameSettings = gameSettings;

            blockPool = world.GetPool<BlockComponent>();
            fallingPool = world.GetPool<FallingComponent>();
            viewLinkPool = world.GetPool<ViewLinkComponent>();
            
            ballPassedFilter = world.Filter<BallPassedComponent>()
                .Exc<FallingComponent>()
                .End();
        }
        
        public void CreateStartBlocks(int blockPartsCount)
        {
            BlockPartsCount = blockPartsCount;

            var startPlatform = spawner.SpawnStartPlatform(Color.gray);
            liveBlocksEntities.Enqueue(startPlatform);

            lastSpawnPos = new Vector3(1f, 0f, 1f);

            for (int i = 0; i < BlocksOnScreen; i++)
                SpawnNextBlocks();
        }
        
        public void ClearBlocks()
        {
            liveBlocksEntities.Clear();
            gameplayStrategies.Reset();
        }
        
        public void Tick()
        {
            foreach (var block in ballPassedFilter)
                OnMovedToNextBlock(block);
        }
        
        private void OnMovedToNextBlock(int block)
        {
            Assert.IsTrue(liveBlocksEntities.Contains(block));

            int block1 = -1;
            while (block1 != block)
            {
                block1 = liveBlocksEntities.Dequeue();
                FallDownBlock(block1);
                SpawnNextBlocks();
            }
        }

        private void SpawnNextBlocks()
        {
            bool rightDirection = Random.value > 0.5f;

            lastSpawnPos += rightDirection ? Vector3.right : Vector3.forward;
            
            var color = new Color(Random.value, Random.value, Random.value, 1.0f);
            var group = SpawnBlockWithCrystalAndHole(lastSpawnPos, rightDirection, color);
            liveBlocksEntities.Enqueue(group);
        }

        private int SpawnBlockWithCrystalAndHole(Vector3 spawnPos, bool rightSide, Color color)
        {
            var block = spawner.SpawnBlock(BlockPartsCount, spawnPos, rightSide, color);

            if (gameSettings.GameMode == GameMode.Holes)
            {
                if (gameplayStrategies.GetBlockHolesStrategy().IsTimeToHole())
                    MakeHole(block);
            }

            if (gameplayStrategies.GetCrystalSpawnStrategy().ShouldSpawn())
                SpawnCrystal(block);

            return block;
        }
        
        private void MakeHole(int block)
        {
            int childIndex = Random.Range(0, 1) == 0 ? 0 : 2;
            ref var viewLinkComponent = ref viewLinkPool.Get(block);
            var child = viewLinkComponent.Transform.GetChild(childIndex);
            child.gameObject.SetActive(false);
        }

        private void SpawnCrystal(int block)
        {
            ref var viewLinkComponent = ref viewLinkPool.Get(block);
            var child = viewLinkComponent.Transform.GetChild(Random.Range(0, BlockPartsCount - 1));
            if (child.gameObject.activeSelf)
            {
                int crystal = spawner.SpawnCrystal(child.position);
                ref var blockComponent = ref blockPool.Get(block);
                blockComponent.Crystal = world.PackEntity(crystal);
            }
        }

        private void FallDownBlock(int block)
        {
            fallingPool.Add(block) = new FallingComponent { FallingDelay = 0.3f };

            ref var blockComponent = ref blockPool.Get(block);
            if (blockComponent.Crystal != null && blockComponent.Crystal.Value.Unpack(world, out int crystal))
                fallingPool.Add(crystal) = new FallingComponent { FallingDelay = 0.4f };
        }
    }
}