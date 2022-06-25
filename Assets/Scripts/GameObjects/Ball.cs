// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using UnityEngine;
using Zenject;

namespace AmazingTrack
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        public float Speed = 5.0f;

        private SignalBus signalBus;
        private AudioPlayer audioPlayer;
        private AudioSettings audioSettings;
        private GameObject prevHitObject;
        private Vector3 direction = Vector3.zero;

        private Rigidbody rb;

        [Inject]
        public void Construct(SignalBus signalBus, AudioPlayer audioPlayer, AudioSettings audioSettings)
        {
            this.signalBus = signalBus;
            this.audioPlayer = audioPlayer;
            this.audioSettings = audioSettings;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void ChangeDirection()
        {
            if (!CheckObjectUnder(out _))
                return; // can't change direction when fall

            if (direction == Vector3.forward)
                direction = -Vector3.left;
            else
                direction = Vector3.forward;

            audioPlayer.Play(audioSettings.BallTurnSound);
        }

        private bool CheckObjectUnder(out GameObject hitObject)
        {
            const float SphereCastRadius = 0.1f;
            if (Physics.SphereCast(transform.position, SphereCastRadius, Vector3.down, out RaycastHit hit, 0.3f))
            {
                hitObject = hit.transform.gameObject;
                return true;
            }
            hitObject = null;
            return false;
        }

        private bool CheckBlockUnder(out GameObject newBlock)
        {
            newBlock = null;

            if (CheckObjectUnder(out var hitObject))
            {
                if (hitObject.CompareTag(Tags.Block))
                {
                    newBlock = hitObject;
                    return true;
                }
            }
            return false;
        }

        private void FixedUpdate()
        {
            if (direction == Vector3.zero)
                return;
            
            bool controlled = IsControlled();

            if (controlled)
            {
                if (CheckBlockUnder(out var block))
                {
                    if (block != prevHitObject)
                    {
                        OnCollideWithBlock(block, prevHitObject);
                        prevHitObject = block;
                    }
                }
                else
                {
                    OnCrashed();
                }
            }

            float speed = controlled ? Speed : 1/*Speed / 2*/;
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Crystal))
                OnCollideWithCrystal(other.gameObject);
        }

        private bool IsControlled()
        {
            return rb.isKinematic;
        }

        private void OnCrashed()
        {
            rb.isKinematic = false;
            signalBus.Fire<BallCrashedSignal>();
        }

        private void OnCollideWithCrystal(GameObject crystal)
        {
            signalBus.Fire(new BallHitCrystalSignal(crystal));
        }

        private void OnCollideWithBlock(GameObject block, GameObject previousBlock)
        {
            signalBus.Fire(new BallMovedToNextBlockSignal(block, previousBlock));
        }

        public class Factory : PlaceholderFactory<Ball>
        {
        }
    }
}