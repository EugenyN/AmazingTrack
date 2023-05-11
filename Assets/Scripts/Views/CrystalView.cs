using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class CrystalView : MonoBehaviour
    {
    }
    
    public class CrystalViewPool : MonoMemoryPool<CrystalView>
    {
    }
}