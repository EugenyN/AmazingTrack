// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AmazingTrack
{
    public class BlocksGroup : MonoBehaviour
    {
        private Block.Factory blockFactory;
        
        [Inject]
        public void Construct(Block.Factory blockFactory)
        {
            this.blockFactory = blockFactory;
        }

        private void ReinitBlocks(int count, bool rightSide, Vector3 spawnPos, Color color)
        {
            Vector3 turnDirection = rightSide ? Vector3.forward : Vector3.right;
            bool childExists = transform.childCount > 0;

            for (int i = 0; i < count; i++)
            {
                var child = childExists ? transform.GetChild(i).GetComponent<Block>() : blockFactory.Create();
                child.Reinit(spawnPos + turnDirection * -i, gameObject.transform, color);
            }
        }

        public void MakeHole()
        {
            int childIndex = Random.Range(0, 1) == 0 ? 0 : 2;
            var child = transform.GetChild(childIndex);
            child.gameObject.SetActive(false);
        }

        public class Pool : MonoMemoryPool<int, bool, Vector3, Color, BlocksGroup>
        {
            protected override void Reinitialize(int count, bool rightSide, Vector3 spawnPos, Color color, BlocksGroup item)
            {
                base.Reinitialize(count, rightSide, spawnPos, color, item);
                item.ReinitBlocks(count, rightSide, spawnPos, color);
            }
        }
    }
}