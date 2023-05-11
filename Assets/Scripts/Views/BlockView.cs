using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class BlockView : MonoBehaviour
    {
    }

    public class BlockViewPool : MonoMemoryPool<BlockView>
    {
    }
}