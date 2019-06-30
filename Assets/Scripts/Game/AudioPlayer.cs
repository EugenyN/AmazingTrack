// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    public class AudioPlayer
    {
        readonly Camera _camera;

        public AudioPlayer(Camera camera)
        {
            _camera = camera;
        }

        public void Play(AudioClip clip)
        {
            Play(clip, 1);
        }

        public void Play(AudioClip clip, float volume)
        {
            _camera.GetComponent<AudioSource>().PlayOneShot(clip, volume);
        }
    }
}
