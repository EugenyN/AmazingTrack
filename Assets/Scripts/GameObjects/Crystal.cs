// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class Crystal : MonoBehaviour
    {
        private const float FallDownDelay = 0.4f;
        private const float RotationSpeed = 60.0f;

        void Start()
        {
        }

        void Update()
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        }

        public void Take()
        {
            transform.parent = null;
            gameObject.SetActive(false);
        }

        public void FallDown()
        {
            StartCoroutine(FallDownInt());
        }

        IEnumerator FallDownInt()
        {
            yield return new WaitForSeconds(FallDownDelay);
            GetComponent<Rigidbody>().isKinematic = false;

            transform.parent = null;
        }

        public void Reset(Vector3 position, Transform parent)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = position;
            transform.parent = parent;
        }

        public class Pool : MonoMemoryPool<Vector3, Transform, Crystal>
        {
            protected override void Reinitialize(Vector3 position, Transform parent, Crystal crystal)
            {
                crystal.Reset(position, parent);
            }
        }
    }
}