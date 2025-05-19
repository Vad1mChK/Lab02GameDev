using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Misc
{
    public class DoorOpener : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator anim;
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private float soundPitchVariation;
        [Header("Misc")] 
        [SerializeField] private string doorId;
        [SerializeField] private bool requiresProximity;
        [SerializeField] private UnityEvent<bool> OnToggleOpen;
        [SerializeField] private UnityEvent OnOpen;
        [SerializeField] private UnityEvent OnClose;
        [SerializeField] private UnityEvent OnDoorTriggerEnter;
        [SerializeField] private UnityEvent OnDoorTriggerExit;
        [SerializeField] private UnityEvent OnAttemptUnlockWrongKey;
        
        private bool _open;
        private bool _usable;
        
        public bool Open
        {
            get => _open;
            set => SetOpen(value);
        }

        public bool Usable => _usable;

        public static Dictionary<string, DoorOpener> _map = new();
        
        void Start()
        {
            anim = GetComponent<Animator>();
            if (!requiresProximity) _usable = true;
        }

        private void Awake()
        {
            if (_map == null)
            {
                _map = new Dictionary<string, DoorOpener>();
            }
            
            _map[doorId] = this;
        }

        public void SetOpen(bool isOpen)
        {
            if (!_usable)
            {
                return;
            }
            
            _open = isOpen;
            
            anim.SetBool("isOpen", isOpen);

            if (audioSource != null)
            {
                float pitchModifier = Random.value;
                audioSource.pitch = 1 + (pitchModifier - .5f) * soundPitchVariation;
                    
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                    
                if (isOpen && openSound != null)
                {
                    audioSource.resource = openSound;
                    audioSource.Play();
                }

                if (!isOpen && closeSound != null)
                {
                    audioSource.resource = closeSound;
                    audioSource.Play();
                }
            }
            
            OnToggleOpen?.Invoke(_open);
            (_open ? OnOpen : OnClose)?.Invoke();
        }

        public void ToggleOpen()
        {
            SetOpen(!_open);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && requiresProximity)
            {
                _usable = true;
                OnDoorTriggerEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && requiresProximity)
            {
                _usable = false;
                OnDoorTriggerExit?.Invoke();
            }
        }

        public static DoorOpener Lookup(string id) =>
            (_map != null && _map.TryGetValue(id, out var door)) ? door : null;
    }
}