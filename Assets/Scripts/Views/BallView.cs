using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallView : MonoBehaviour
    {
    }
    
    public class BallViewFactory : PlaceholderFactory<BallView>
    {
    }
}