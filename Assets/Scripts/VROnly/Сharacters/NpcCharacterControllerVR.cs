using System.Collections.Generic;
using Electronics;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace VROnly.Сharacters
{
    public class NpcCharacterControllerVR: MonoBehaviour
    {
        public enum NpcCharacterState
        {
            Standing,
            Exasperated,
            Walking,
            Fixing
        }

        private bool _interactive = true;
        
        [Header("Items")]
        [SerializeField] private ItemType possessedItems = ItemType.None;
        [SerializeField] private ItemType requiredItems = ItemType.ArtifactItems;
        [Header("Movement Metrics")]
        [SerializeField] private Transform target;
        [SerializeField] private float minDistanceToTarget = 2f;
        [Header("Animations")]
        [SerializeField] private float fixingDuration = 3f;
        [SerializeField] private float exasperatedAnimationDuration = 3.5f;
        [Header("Audio")]
        [SerializeField] private AudioSource feetAudioSource;
        [SerializeField] private AudioSource headAudioSource;
        [SerializeField] private AudioSource handAudioSource;
        [SerializeField] private List<AudioClip> walkingSounds;
        [SerializeField] private AudioClip wrenchSound;
        [SerializeField] private AudioClip exasperationSound;
        [SerializeField] private float walkingSoundInterval = 1f;
        [SerializeField] private float wrenchSoundInterval = 1f;
        [SerializeField] private float walkingPitchVariation = 0.25f;
        [Header("Misc")]
        [SerializeField] private GameObject wrenchPrefab;
        [SerializeField] private Transform wrenchSpawnPoint;
        [SerializeField] private TvController tvToFix;
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private NpcCharacterState _currentState;
        private float _walkingSoundTimer;
        private float _wrenchSoundTimer;
        private float _stateTimer;
        private GameObject _currentWrench;


        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _currentState = NpcCharacterState.Standing;
            
            _walkingSoundTimer = walkingSoundInterval;
            _wrenchSoundTimer = wrenchSoundInterval;
        }
        
        private void Update()
        {
            switch (_currentState) {
                case NpcCharacterState.Exasperated:
                    HandleExasperatedState();
                    break;
                case NpcCharacterState.Walking:
                    HandleWalkingState();
                    break;
                case NpcCharacterState.Fixing:
                    HandleFixingState();
                    break;
                case NpcCharacterState.Standing:
                default:
                    HandleStandingState();
                    break;
            };
        }

        public void AddItem(ItemData item)
        {
            possessedItems |= item.itemType;
        }

        public void RemoveItem(ItemData item)
        {
            possessedItems &= ~item.itemType;
        }
        
        private void HandleStandingState()
        {
            if (_interactive && HasAllRequiredItems())
            {
                BecomeExasperated();
            }
        }

        private void HandleExasperatedState()
        {
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= exasperatedAnimationDuration)
            {
                TransitionToState(NpcCharacterState.Walking);
                _agent.isStopped = false;
                _agent.SetDestination(target.position);
                _animator.SetBool("IsWalking", true);
            }
        }

        private void HandleWalkingState()
        {
            if (_agent.remainingDistance <= minDistanceToTarget)
            {
                TransitionToState(NpcCharacterState.Fixing);
                _agent.isStopped = true;
                _animator.SetBool("IsWalking", false);
                _animator.SetTrigger("StartFixing");
                _currentWrench = Instantiate(wrenchPrefab, wrenchSpawnPoint);
            }
            
            _walkingSoundTimer -= Time.deltaTime;
            if (_walkingSoundTimer <= 0)
            {
                PlayRandomFootstep();
                _walkingSoundTimer = walkingSoundInterval;
            }
        }
        
        private void HandleFixingState()
        {
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= fixingDuration)
            {
                if (_currentWrench != null) Destroy(_currentWrench);
                tvToFix.Repair();
                TransitionToState(NpcCharacterState.Standing);
                _animator.SetTrigger("FinishFixing");
            }
            
            _wrenchSoundTimer -= Time.deltaTime;
            if (_wrenchSoundTimer <= 0)
            {
                handAudioSource.pitch = Random.Range(1 - walkingPitchVariation / 2, 1 + walkingPitchVariation / 2);
                handAudioSource.PlayOneShot(wrenchSound);
                _wrenchSoundTimer = wrenchSoundInterval;
            }
        }
        
        private void TransitionToState(NpcCharacterState newState)
        {
            _currentState = newState;
            _stateTimer = 0f;
            
            // Reset all animator states
            _animator.SetBool("IsStanding", false);
            _animator.SetBool("IsWalking", false);
            _animator.ResetTrigger("Exasperated");
            _animator.ResetTrigger("StartFixing");
            _animator.ResetTrigger("FinishFixing");

            // Set new state animator parameters
            switch (newState)
            {
                case NpcCharacterState.Standing:
                    _animator.SetBool("IsStanding", true);
                    break;

                case NpcCharacterState.Exasperated:
                    headAudioSource.PlayOneShot(exasperationSound);
                    break;
            
                case NpcCharacterState.Walking:
                    _walkingSoundTimer = walkingSoundInterval; // Reset timer
                    PlayRandomFootstep(); // Play immediate first step
                    break;
            
                case NpcCharacterState.Fixing:
                    _wrenchSoundTimer = wrenchSoundInterval; // Reset timer
                    handAudioSource.PlayOneShot(wrenchSound);
                    break;
            }
        }
        
        private void PlayRandomFootstep()
        {
            if (walkingSounds.Count > 0)
            {
                feetAudioSource.pitch = Random.Range(1 - walkingPitchVariation / 2, 1 + walkingPitchVariation / 2);
                int randomIndex = Random.Range(0, walkingSounds.Count);
                feetAudioSource.PlayOneShot(walkingSounds[randomIndex]);
            }
        }

        private void BecomeExasperated()
        {
            _interactive = false;
            TransitionToState(NpcCharacterState.Exasperated);
            _animator.SetTrigger("Exasperated");
        }

        private bool HasAllRequiredItems()
        {
            return (possessedItems & requiredItems) == requiredItems;
        }
    }
}