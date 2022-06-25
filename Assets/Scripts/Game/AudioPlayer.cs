// Copyright 2019 Eugeny Novikov. Code under MIT license.

using UnityEngine;

namespace AmazingTrack
{
    public class AudioPlayer
    {
        private readonly Camera camera;

        public AudioPlayer(Camera camera)
        {
            this.camera = camera;
        }

        public void Play(AudioClip clip, float volume = 1)
        {
            camera.GetComponent<AudioSource>().PlayOneShot(clip, volume);
        }
    }
}