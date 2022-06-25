// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class Crystal : MonoBehaviour
    {
        private const float FallDownDelay = 0.4f;
        private const float RotationSpeed = 60.0f;
        
        private bool falling;
        private float fallingTimer;
        
        private void Update()
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);

            if (falling)
            {
                fallingTimer -= Time.deltaTime;
                if (fallingTimer <= 0.0f)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                    transform.parent = null;
                }
            }
        }

        public void Take()
        {
            transform.parent = null;
            gameObject.SetActive(false);
        }

        public void FallDown()
        {
            falling = true;
            fallingTimer = FallDownDelay;
        }

        private void Reinit(Vector3 position, Transform parent)
        {
            falling = false;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = position;
            transform.parent = parent;
        }

        public class Pool : MonoMemoryPool<Vector3, Transform, Crystal>
        {
            protected override void Reinitialize(Vector3 position, Transform parent, Crystal crystal)
            {
                crystal.Reinit(position, parent);
            }
        }
    }
}