// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;

namespace AmazingTrack
{
    public class CameraFollow : MonoBehaviour
    {
        public float LerpRate = 5.0f;
        private GameObject target;
        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        public void StartFollow(GameObject target)
        {
            enabled = true;
            this.target = target;
        }

        public void StopFollow()
        {
            enabled = false;
        }

        private void Update()
        {
            Follow();
        }

        private void Follow()
        {
            if (target == null)
                return;

            var pos = transform.position;
            var targetPos = target.transform.position + initialPosition;
            pos = Vector3.Lerp(pos, targetPos, LerpRate * Time.deltaTime);
            transform.position = pos;
        }
    }
}