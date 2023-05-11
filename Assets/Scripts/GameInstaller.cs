using Zenject;
using Leopotam.EcsLite;

namespace AmazingTrack
{
    public class GameInstaller : MonoInstaller
    {
        public PrefabsSettings Prefabs;

        public override void InstallBindings()
        {
            // ecs
            
            Container.BindInstance(new EcsWorld());
            Container.BindInterfacesAndSelfTo<EcsSystems>().AsSingle();
            Container.BindInterfacesTo<EcsStartup>().AsSingle();
            
            // settings

            Container.BindInstance(Prefabs);

            // systems
            
            Container.BindInterfacesTo<AndroidRefreshRateFix>().AsSingle();
            Container.BindInterfacesTo<CameraFollowSystem>().AsSingle();
            Container.BindInterfacesTo<BackgroundColorSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ObjectSpawner>().AsSingle();
            Container.Bind<GameplayStrategiesProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<BallSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BlockSystem>().AsSingle();
            Container.BindInterfacesTo<FallingSystem>().AsSingle();
            Container.Bind<AudioPlayer>().AsSingle();
            Container.Bind<PlayerStatService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<CrystalSystem>().AsSingle();
            
            Container.BindInterfacesTo<DeleteEventsSystem<BallPassedComponent>>().AsSingle();
            Container.BindInterfacesTo<DeleteEventsSystem<BallHitComponent>>().AsSingle();
            Container.BindInterfacesTo<DeleteEventsSystem<PlayerLevelUpComponent>>().AsSingle();

            // factories

            Container.BindFactory<BallView, BallViewFactory>().FromComponentInNewPrefab(Prefabs.BallPrefab);
            Container.BindFactory<BlockPartView, BlockPartViewFactory>().FromComponentInNewPrefab(Prefabs.BlockPartPrefab);

            // pools

            Container.BindMemoryPool<BlockView, BlockViewPool>()
                .WithInitialSize(30).FromComponentInNewPrefab(Prefabs.BlockPrefab)
                .UnderTransformGroup("ObjectsPool");
            Container.BindMemoryPool<CrystalView, CrystalViewPool>()
                .WithInitialSize(5).FromComponentInNewPrefab(Prefabs.CrystalPrefab)
                .UnderTransformGroup("ObjectsPool");
        }
    }
}