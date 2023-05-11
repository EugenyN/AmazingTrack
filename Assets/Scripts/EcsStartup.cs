using System;
using Zenject;
using Leopotam.EcsLite;

namespace AmazingTrack
{
    public class EcsStartup : IDisposable, ILateTickable, IInitializable
    {
        private readonly EcsWorld world;

        private IEcsSystems systems;

        public EcsStartup(EcsWorld world)
        {
            this.world = world;
        }

        void IInitializable.Initialize()
        {
            systems = new EcsSystems(world);
#if UNITY_EDITOR
            systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif
            systems.Init();
        }

        void IDisposable.Dispose()
        {
            systems.Destroy();
        }

        void ILateTickable.LateTick()
        {
            systems.Run();
        }
    }
}