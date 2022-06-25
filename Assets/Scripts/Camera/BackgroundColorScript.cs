// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;

namespace AmazingTrack
{
    public class BackgroundColorScript : MonoBehaviour
    {
        [SerializeField] Color color1 = Color.red;
        [SerializeField] Color color2 = Color.blue;
        [SerializeField] float duration = 5.0f;

        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
        }

        private void Update()
        {
            float t = Mathf.PingPong(Time.time, duration) / duration;
            cam.backgroundColor = Color.Lerp(color1, color2, t);
        }
    }
}