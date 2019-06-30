// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmazingTrack
{
    public class CameraFollow : MonoBehaviour
    {
        public float LerpRate = 5.0f;
        private GameObject target;
        private Vector3 initialPosition;

        void Start()
        {
            initialPosition = transform.position;
        }

        public void StartFolow(GameObject target)
        {
            enabled = true;
            this.target = target;
        }

        public void StopFollow()
        {
            enabled = false;
        }

        void Update()
        {
            Follow();
        }

        private void Follow()
        {
            if (target == null)
                return;

            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position + initialPosition;
            pos = Vector3.Lerp(pos, targetPos, LerpRate * Time.deltaTime);
            transform.position = pos;
        }
    }
}