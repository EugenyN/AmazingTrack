// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class BlocksGroup : MonoBehaviour
    {
        Block.Factory blockFactory;

        [Inject]
        public void Construct(Block.Factory blockFactory)
        {
            this.blockFactory = blockFactory;
        }

        public void ResetBlocks(int count, bool rightSide, Vector3 spawnPos, Color color)
        {
            Vector3 turnDirection = rightSide ? Vector3.forward : Vector3.right;
            bool childExists = transform.childCount > 0;

            for (int i = 0; i < count; i++)
            {
                var child = childExists ? transform.GetChild(i).GetComponent<Block>()
                    : blockFactory.Create();

                child.Reset(spawnPos + turnDirection * -i, gameObject.transform, color);
            }
        }

        public void MakeHole()
        {
            //int childIndex = 1;
            int childIndex = Random.Range(0, 1) == 0 ? 0 : 2;
            var child = transform.GetChild(childIndex);
            child.gameObject.SetActive(false);
        }

        public class Pool : MonoMemoryPool<int, bool, Vector3, Color, BlocksGroup>
        {
            protected override void Reinitialize(int count, bool rightSide, Vector3 spawnPos, Color color, BlocksGroup item)
            {
                base.Reinitialize(count, rightSide, spawnPos, color, item);
                item.ResetBlocks(count, rightSide, spawnPos, color);
            }
        }
    }
}