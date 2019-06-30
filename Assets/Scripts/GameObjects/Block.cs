// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class Block : MonoBehaviour
    {
        private const float FallDownDelay = 0.3f;

        public bool Falling = false;

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

        public void Reset(Vector3 position, Transform parent, Color color)
        {
            gameObject.SetActive(true);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Renderer>().material.color = color;
            transform.rotation = Quaternion.identity;
            transform.position = position;
            transform.parent = parent;
            Falling = false;
        }

        public void FallDown()
        {
            Falling = true;
            StartCoroutine(FallDownInt());
        }

        IEnumerator FallDownInt()
        {
            yield return new WaitForSeconds(FallDownDelay);
            GetComponent<Rigidbody>().isKinematic = false;
        }

        public class Factory : PlaceholderFactory<Block>
        {
        }
    }
}