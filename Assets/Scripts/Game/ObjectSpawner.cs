// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace AmazingTrack
{
    public class ObjectSpawner
    {
        readonly Crystal.Pool crystalPool;
        readonly BlocksGroup.Pool blocksGroupPool;
        readonly Block.Factory blockFactory;
        readonly Ball.Factory ballFactory;

        readonly Vector3 CrystalOffset = new Vector3(0, 0.8f, 0);


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
                DespawnCrystal(obj);

            crystalPool.Clear();

            foreach (var obj in GameObject.FindGameObjectsWithTag(Tags.BlocksGroup))
                DespawnBlocksGroup(obj);

            blocksGroupPool.Clear();
        }

        public GameObject SpawnBall(Vector3 pos)
        {
            var ball = ballFactory.Create();
            ball.transform.position = pos;
            return ball.gameObject;
        }

        public GameObject SpawnStartPlatform(Color color)
        {
            GameObject startPlatform = new GameObject("StartBlocks");
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

        private GameObject SpawnBlock(Vector3 pos, Transform parent, Color color)
        {
            var block = blockFactory.Create();
            block.Reset(pos, parent, color);
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

        public void DespawnCrystal(GameObject gameObject)
        {
            crystalPool.Despawn(gameObject.GetComponent<Crystal>());
        }

        public GameObject SpawnBlocksGroup(int count, Vector3 spawnPos, bool rightSide, Color color)
        {
            var group = blocksGroupPool.Spawn(count, rightSide, spawnPos, color);
            group.gameObject.tag = Tags.BlocksGroup;
            return group.gameObject;
        }

        public void DespawnBlocksGroup(GameObject gameObject)
        {
            var group = gameObject.GetComponent<BlocksGroup>();
            if (group != null)
                blocksGroupPool.Despawn(group);
            else
                GameObject.Destroy(gameObject);
        }
    }
}