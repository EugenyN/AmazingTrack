// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmazingTrack
{
    public class BackgroundColorScript : MonoBehaviour
    {
        public Color Color1 = Color.red;
        public Color Color2 = Color.blue;
        public float Duration = 5.0f;

        private Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
        }

        void Update()
        {
            float t = Mathf.PingPong(Time.time, Duration) / Duration;
            cam.backgroundColor = Color.Lerp(Color1, Color2, t);
        }
    }
}