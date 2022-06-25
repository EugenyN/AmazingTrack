// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class ObjectSpawner : ITickable
    {
        private readonly Crystal.Pool crystalPool;
        private readonly BlocksGroup.Pool blocksGroupPool;
        private readonly Block.Factory blockFactory;
        private readonly Ball.Factory ballFactory;

        private readonly Vector3 CrystalOffset = new Vector3(0, 0.8f, 0);

        private readonly List<(GameObject, float)> objectsForDespawn = new();

        public ObjectSpawner(BlocksGroup.Pool blocksGroupPool, Block.Factory blockFactory,
            Crystal.Pool crystalPool, Ball.Factory ballFactory)
        {
            this.blocksGroupPool = blocksGroupPool;
            this.blockFactory = blockFactory;
            this.crystalPool = crystalPool;
            this.ballFactory = ballFactory;
        }

        public void Clear()
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag(Tags.Crystal))
                DespawnObject(obj);

            crystalPool.Clear();

            foreach (var obj in GameObject.FindGameObjectsWithTag(Tags.BlocksGroup))
                DespawnObject(obj);

            blocksGroupPool.Clear();

            objectsForDespawn.Clear();
        }

        public GameObject SpawnBall(Vector3 pos)
        {
            var ball = ballFactory.Create();
            ball.transform.position = pos;
            return ball.gameObject;
        }

        public GameObject SpawnStartPlatform(Color color)
        {
            var startPlatform = new GameObject("StartBlocks");
            startPlatform.tag = Tags.BlocksGroup;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    SpawnBlock(new Vector3(i, 0f, j), startPlatform.transform, color);
                }
            }

            return startPlatform;
        }

        public GameObject SpawnBlock(Vector3 pos, Transform parent, Color color)
        {
            var block = blockFactory.Create();
            block.Reinit(pos, parent, color);
            return block.gameObject;
        }

        public GameObject SpawnOrResetCrystal(Transform parent)
        {
            return SpawnOrResetCrystal(parent.position + CrystalOffset, parent);
        }

        public GameObject SpawnOrResetCrystal(Vector3 pos, Transform parent)
        {
            var crystal = crystalPool.Spawn(pos, parent);
            return crystal.gameObject;
        }
        
        public GameObject SpawnBlocksGroup(int count, Vector3 spawnPos, bool rightSide, Color color)
        {
            var group = blocksGroupPool.Spawn(count, rightSide, spawnPos, color);
            group.gameObject.tag = Tags.BlocksGroup;
            return group.gameObject;
        }

        public void DespawnObject(GameObject gameObject)
        {
            var group = gameObject.GetComponent<BlocksGroup>();
            if (group != null)
            {
                blocksGroupPool.Despawn(group);
                return;
            }
            
            var crystal = gameObject.GetComponent<Crystal>();
            if (crystal != null)
            {
                crystalPool.Despawn(crystal);
                return;
            }
            
            Object.Destroy(gameObject);
        }

        public void DespawnObjectDelayed(GameObject gameObject, float delay)
        {
            objectsForDespawn.Add((gameObject, delay));
        }

        public void Tick()
        {
            for (int i = objectsForDespawn.Count - 1; i >= 0; i--)
            {
                var (gameObject, time) = objectsForDespawn[i];
                time -= Time.deltaTime;
                objectsForDespawn[i] = (gameObject, time);
                
                if (time <= 0)
                {
                    objectsForDespawn.RemoveAt(i);
                    DespawnObject(gameObject);
                }
            }
        }
    }
}