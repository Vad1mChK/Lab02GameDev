using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Electronics
{
    public class TvController : MonoBehaviour
    {
        [SerializeField] private string tvId;
        [SerializeField] private TvState state = TvState.OffBroken;

        [SerializeField] private Material materialWhenOff;
        [SerializeField] private Material materialWhenBroken;
        [SerializeField] private Material materialWhenRepaired;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClipWhenBroken;

        [SerializeField] private Image isOnIndicator;
        [SerializeField] private Image isOperationalIndicator;

        [SerializeField] private Color offColor = new Color(.8392f, 0f, .1176f);
        [SerializeField] private Color onColor = new Color(.5804f, .7373f, .0549f);

        [SerializeField] private UnityEvent onEnteredProximityZone;
        [SerializeField] private UnityEvent onExitedProximityZone;
        [SerializeField] private UnityEvent onSetStateToBroken;
        [SerializeField] private UnityEvent onSetStateToOperational;
        [SerializeField] private UnityEvent onSetStateToOff;
            
        private MeshRenderer _meshRenderer;
        
        public static Dictionary<string, TvController> _map = new();

        private enum TvState
        {
            OffBroken,
            Off,
            Broken,
            Operational,
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                _meshRenderer = meshRenderer;
            }
            
            _map[tvId] = this;
            UpdateTvLook();
        }

        // // Update is called once per frame
        // void Update()
        // {
        //     if (Input.GetKeyDown(keyToToggle))
        //     {
        //         Toggle();
        //     }
        //
        //     if (Input.GetKeyDown(keyToRepair))
        //     {
        //         Repair();
        //     }
        // }

        public void Toggle()
        {
            switch (state)
            {
                case TvState.OffBroken:
                    state = TvState.Broken;
                    break;
                case TvState.Off:
                    state = TvState.Operational;
                    break;
                case TvState.Broken:
                    state = TvState.OffBroken;
                    break;
                case TvState.Operational:
                    state = TvState.Off;
                    break;
            }
            UpdateTvLook();
        }

        public void Repair()
        {
            switch (state)
            {
                case TvState.OffBroken:
                    state = TvState.Off;
                    break;
                case TvState.Broken:
                    state = TvState.Operational;
                    break;
                default:
                    return;
            }
            UpdateTvLook();
        }

        private void UpdateMaterial()
        {
            switch (state)
            {
                case TvState.OffBroken:
                case TvState.Off:
                    _meshRenderer.material = materialWhenOff;
                    break;
                
                case TvState.Broken:
                    _meshRenderer.material = materialWhenBroken;
                    break;
                    
                case TvState.Operational:
                    _meshRenderer.material = materialWhenRepaired;
                    break;
            }
        }

        private void UpdateAudio()
        {
            if (state == TvState.Broken)
            {
                SetRandomAudioPitch();
                audioSource.clip = audioClipWhenBroken;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }

        private void UpdateIndicators()
        {
            if (isOnIndicator != null)
            {
                switch (state)
                {
                    case TvState.Off:
                    case TvState.OffBroken:
                        isOnIndicator.color = offColor;
                        break;
                    default:
                        isOnIndicator.color = onColor;
                        break;
                }
            }

            if (isOperationalIndicator != null)
            {
                switch (state)
                {
                    case TvState.OffBroken:
                    case TvState.Broken:
                        isOperationalIndicator.color = offColor;
                        break;
                    default:
                        isOperationalIndicator.color = onColor;
                        break;
                }
            }
        }

        private void InvokeEventForState()
        {
            switch (state)
            {
                case TvState.Broken:
                    onSetStateToBroken?.Invoke();
                    break;
                case TvState.Operational:
                    onSetStateToOperational?.Invoke();
                    break;
                default:
                    onSetStateToOff?.Invoke();
                    break;
            }
        }

        private void UpdateTvLook()
        {
            UpdateMaterial();
            UpdateAudio();
            UpdateIndicators();
            InvokeEventForState();
        }

        private void SetRandomAudioPitch()
        {
            float pitchModifier = Random.value - .5f;
            audioSource.pitch = 1 + pitchModifier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                onEnteredProximityZone?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                onExitedProximityZone?.Invoke();
            }
        }
        
        public static TvController Lookup(string id) =>
            (_map != null && _map.TryGetValue(id, out var tv)) ? tv : null;
    }
}