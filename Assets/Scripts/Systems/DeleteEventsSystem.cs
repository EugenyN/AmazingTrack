using Leopotam.EcsLite;
using Zenject;

namespace AmazingTrack
{
    public sealed class DeleteEventsSystem<T> : ITickable where T: struct
    {
        private readonly EcsFilter filter;
        private readonly EcsPool<T> pool;

        public DeleteEventsSystem(EcsWorld world)
        {
            filter = world.Filter<T>().End();
            pool = world.GetPool<T>();
        }

        public void Tick()
        {
            foreach (var entity in filter)
                pool.Del(entity);
        }
    }
}