// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class Block : MonoBehaviour
    {
        private const float FallDownDelay = 0.3f;

        public bool Falling;
        public float FallingTimer;

        public bool HasCrystal()
        {
            return gameObject.transform.childCount > 0;
        }

        public GameObject GetCrystal()
        {
            if (!HasCrystal())
                return null;
            return gameObject.transform.GetChild(0).gameObject;
        }

        public void Reinit(Vector3 position, Transform parent, Color color)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Renderer>().material.color = color;
            transform.parent = parent;
            transform.SetPositionAndRotation(position, Quaternion.identity);
            Falling = false;
            gameObject.SetActive(true);
        }

        public void FallDown()
        {
            Falling = true;
            FallingTimer = FallDownDelay;
        }

        private void Update()
        {
            if (Falling)
            {
                FallingTimer -= Time.deltaTime;
                if (FallingTimer <= 0.0f)
                    GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        public class Factory : PlaceholderFactory<Block>
        {
        }
    }
}