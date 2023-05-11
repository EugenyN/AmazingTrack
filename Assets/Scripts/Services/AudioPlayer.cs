using UnityEngine;

namespace AmazingTrack
{
    public class AudioPlayer
    {
        private readonly AudioSource audioSource;

        public AudioPlayer(Camera camera)
        {
            audioSource = camera.GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume = 1)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}