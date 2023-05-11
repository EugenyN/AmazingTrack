using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace AmazingTrack
{
    public class ObjectSpawner
    {
        private readonly EcsWorld world;
        private readonly CrystalViewPool crystalViewPool;
        private readonly BlockViewPool blockViewPool;
        private readonly BlockPartViewFactory blockPartViewFactory;
        private readonly BallViewFactory ballViewFactory;

        private readonly Vector3 CrystalOffset = new Vector3(0, 0.8f, 0);
        
        private readonly EcsPool<BallComponent> ballPool;
        private readonly EcsPool<CrystalComponent> crystalPool;
        private readonly EcsPool<BlockComponent> blockPool;
        
        private readonly EcsPool<ViewLinkComponent> viewLinkPool;
        private readonly EcsFilter viewLinkFilter;

        public ObjectSpawner(EcsWorld world, BlockViewPool blockViewPool, BlockPartViewFactory blockPartViewFactory,
            CrystalViewPool crystalViewPool, BallViewFactory ballViewFactory)
        {
            this.world = world;
            this.blockViewPool = blockViewPool;
            this.blockPartViewFactory = blockPartViewFactory;
            this.crystalViewPool = crystalViewPool;
            this.ballViewFactory = ballViewFactory;

            ballPool = world.GetPool<BallComponent>();
            crystalPool = world.GetPool<CrystalComponent>();
            blockPool = world.GetPool<BlockComponent>();

            viewLinkPool = world.GetPool<ViewLinkComponent>();
            viewLinkFilter = world.Filter<ViewLinkComponent>().End();
        }

        public void Clear()
        {
            foreach (var entity in viewLinkFilter)
                DespawnObject(entity);

            crystalViewPool.Clear();
            blockViewPool.Clear();
        }

        public int SpawnBall(Vector3 pos, float speed)
        {
            var gameObject = ballViewFactory.Create().gameObject;
            var entity = CreateEntity<BallComponent>(gameObject);

            ref var ballComponent = ref world.GetPool<BallComponent>().Get(entity);
            ballComponent.Speed = speed;
            gameObject.transform.position = pos;
            return entity;
        }
        
        public int SpawnStartPlatform(Color color)
        {
            var gameObject = new GameObject("StartBlocks");
            gameObject.AddComponent<BlockView>();
            gameObject.AddComponent<EntityLinkView>();
            
            var entity = CreateEntity<BlockComponent>(gameObject);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    AddBlockChild(new Vector3(i, 0f, j), entity, color);
                }
            }

            return entity;
        }
        
        public int SpawnBlock(int count, Vector3 spawnPos, bool rightSide, Color color)
        {
            var gameObject = blockViewPool.Spawn().gameObject;
            var entity = CreateEntity<BlockComponent>(gameObject);
            
            AddChildBlocks(entity, count, rightSide, spawnPos, color);
            
            Assert.IsTrue(gameObject.transform.childCount == count);
            return entity;
        }
        
        private void AddChildBlocks(int blocks, int count, bool rightSide, Vector3 spawnPos, Color color)
        {
            Vector3 turnDirection = rightSide ? Vector3.forward : Vector3.right;
            
            ref var viewLinkComponent = ref viewLinkPool.Get(blocks);
            var parent = viewLinkComponent.Transform;
            bool childExists = parent.childCount > 0;
            
            for (int i = 0; i < count; i++)
                AddBlockChild(spawnPos + turnDirection * -i, blocks, color, 
                    childExists ? parent.GetChild(i).GetComponent<BlockPartView>() : null);
        }

        private void AddBlockChild(Vector3 position, int blocks, Color color, BlockPartView cachedBlockPartView = null)
        {
            ref var viewLinkComponent = ref viewLinkPool.Get(blocks);

            var blockView = cachedBlockPartView != null ? cachedBlockPartView : blockPartViewFactory.Create();
            
            blockView.GetComponent<Rigidbody>().isKinematic = true;
            blockView.GetComponent<Renderer>().material.color = color;
            blockView.transform.parent = viewLinkComponent.Transform;
            blockView.transform.SetPositionAndRotation(position, Quaternion.identity);
            blockView.gameObject.SetActive(true);
        }

        public int SpawnCrystal(Vector3 blockPosition)
        {
            var crystalView = crystalViewPool.Spawn();
            var entity = CreateEntity<CrystalComponent>(crystalView.gameObject);
            
            crystalView.transform.position = blockPosition + CrystalOffset;
            crystalView.GetComponent<Rigidbody>().isKinematic = true;
            crystalView.GetComponent<Rigidbody>().position = crystalView.transform.position;
            
            return entity;
        }
        
        private int CreateEntity<T>(GameObject gameObject) where T : struct
        {
            var entity = world.NewEntity();
            world.GetPool<T>().Add(entity);

            ref var viewLinkComponent = ref world.GetPool<ViewLinkComponent>().Add(entity);
            viewLinkComponent.View = gameObject;
            viewLinkComponent.View.GetComponent<EntityLinkView>().Entity = world.PackEntity(entity);
            viewLinkComponent.Transform = gameObject.transform;
            
            return entity;
        }

        public void DespawnObject(int entity)
        {
            ref var viewLinkComponent = ref viewLinkPool.Get(entity);
            
            if (ballPool.Has(entity))
            {
                Object.Destroy(viewLinkComponent.View);
            }

            if (crystalPool.Has(entity))
            {
                crystalViewPool.Despawn(viewLinkComponent.View.GetComponent<CrystalView>());
            }
            
            if (blockPool.Has(entity))
            {
                if (viewLinkComponent.Transform.childCount > 3) // start block
                    Object.Destroy(viewLinkComponent.View);
                else
                    blockViewPool.Despawn(viewLinkComponent.View.GetComponent<BlockView>());
            }

            world.DelEntity(entity);
        }
    }
}