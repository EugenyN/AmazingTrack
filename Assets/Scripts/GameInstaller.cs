// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class GameInstaller : MonoInstaller
    {
        public PrefabsSettings Prefabs;

        public override void InstallBindings()
        {
            // settings

            Container.BindInstance(Prefabs);

            // types

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();

            Container.Bind<ObjectSpawner>().AsSingle();

            Container.Bind<AudioPlayer>().AsSingle();

            Container.Bind<PlayerStat>().AsSingle();

            // factories

            Container.BindFactory<Ball, Ball.Factory>()
                .FromComponentInNewPrefab(Prefabs.BallPrefab);

            Container.BindFactory<Block, Block.Factory>()
                .FromComponentInNewPrefab(Prefabs.BlockPrefab);

            // pools

            Container.BindMemoryPool<BlocksGroup, BlocksGroup.Pool>()
                .WithInitialSize(30)
                .FromComponentInNewPrefab(Prefabs.BlocksGroupPrefab);

            Container.BindMemoryPool<Crystal, Crystal.Pool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(Prefabs.CrystalPrefab);

            // signals

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<BallCrashedSignal>();
            Container.DeclareSignal<BallMovedToNextBlockSignal>();
            Container.DeclareSignal<BallHitCrystalSignal>();
            Container.DeclareSignal<GameStateChangedSignal>();
            Container.DeclareSignal<LevelUpSignal>();
        }
    }

    [Serializable]
    public class PrefabsSettings
    {
        public GameObject BallPrefab;
        public GameObject BlockPrefab;
        public GameObject BlocksGroupPrefab;
        public GameObject CrystalPrefab;
    }
}