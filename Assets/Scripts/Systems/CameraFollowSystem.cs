using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class CameraFollowSystem : ITickable
    {
        private readonly CameraView cameraView;
        private readonly GameSettings gameSettings;

        private readonly Vector3 initialPosition;

        private readonly EcsFilter followingBallFilter;
        
        private readonly EcsPool<ViewLinkComponent> viewLinkPool;
        
        public CameraFollowSystem(CameraView cameraView, EcsWorld world, GameSettings gameSettings)
        {
            this.cameraView = cameraView;
            this.gameSettings = gameSettings;

            viewLinkPool = world.GetPool<ViewLinkComponent>();

            followingBallFilter = world.Filter<BallComponent>().Exc<FallingComponent>().End();
            
            initialPosition = cameraView.transform.position;
        }
        
        public void Tick()
        {
            foreach (var followingBall in followingBallFilter)
            {
                ref var viewLinkComponent = ref viewLinkPool.Get(followingBall);
            
                var pos = cameraView.transform.position;
                var targetPos = viewLinkComponent.Transform.position + initialPosition;
                pos = Vector3.Lerp(pos, targetPos, gameSettings.CameraLerpRate * Time.deltaTime);
                cameraView.transform.position = pos;
            }
        }
    }
}