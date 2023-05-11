using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class BlockPartView : MonoBehaviour
    {
    }
    
    public class BlockPartViewFactory : PlaceholderFactory<BlockPartView>
    {
    }
}