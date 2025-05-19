using System;
using System.Collections.Generic;
using Electronics;
using Inventory;
using Inventory.Special;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Characters
{
    public class NpcCharacterController : MonoBehaviour
    {
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
        [SerializeField] private KeyCode keyToActivate = KeyCode.K;

        [Header("Requirements")]
        [SerializeField] public ItemType obtainedArtifacts = ItemType.ArtifactItems;
        [SerializeField] public ItemType requiredArtifacts = ItemType.None;

        [SerializeField] private NpcInventoryRendererUI npcInventoryRendererUI;
 
        public UnityEvent OnTriggerEnterEvent;
        public UnityEvent OnTriggerExitEvent;
        public UnityEvent OnUpdateObtainedArtifactsEvent;
        public UnityEvent OnShowObtainedArtifactsInventoryEvent;
        public UnityEvent OnHideObtainedArtifactsInventoryEvent;
        
        private bool _interactive;
        public bool Interactive => _interactive;
        
        private float walkingSoundTimer;
        private float wrenchSoundTimer;

        private NavMeshAgent agent;
        private Animator animator;
        private NpcCharacterState currentState;
        private float stateTimer;
        private GameObject currentWrench;

        private enum NpcCharacterState 
        {
            Standing,
            Exasperated,
            Walking,
            Fixing
        }

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            currentState = NpcCharacterState.Standing;
            InitializeAnimator();
            
            walkingSoundTimer = walkingSoundInterval;
            wrenchSoundTimer = wrenchSoundInterval;
        }

        void Update()
        {
            switch (currentState)
            {
                case NpcCharacterState.Standing:
                    HandleStandingState();
                    break;
                
                case NpcCharacterState.Exasperated:
                    HandleExasperatedState();
                    break;
                
                case NpcCharacterState.Walking:
                    HandleWalkingState();
                    break;
                
                case NpcCharacterState.Fixing:
                    HandleFixingState();
                    break;
            }
        }

        private void InitializeAnimator()
        {
            // animator.SetBool("IsStanding", true);
        }

        private void HandleStandingState()
        {
            if (_interactive && HasAllNecessaryArtifacts() && Input.GetKeyDown(keyToActivate))
            {
                _interactive = false;
                TransitionToState(NpcCharacterState.Exasperated);
                OnTriggerExitEvent?.Invoke();
                animator.SetTrigger("Exasperated");
            }
        }

        private void HandleExasperatedState()
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= exasperatedAnimationDuration)
            {
                TransitionToState(NpcCharacterState.Walking);
                agent.isStopped = false;
                agent.SetDestination(target.position);
                animator.SetBool("IsWalking", true);
            }
        }

        private void HandleWalkingState()
        {
            if (agent.remainingDistance <= minDistanceToTarget)
            {
                TransitionToState(NpcCharacterState.Fixing);
                agent.isStopped = true;
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("StartFixing");
                currentWrench = Instantiate(wrenchPrefab, wrenchSpawnPoint);
            }
            
            walkingSoundTimer -= Time.deltaTime;
            if (walkingSoundTimer <= 0)
            {
                PlayRandomFootstep();
                walkingSoundTimer = walkingSoundInterval;
            }
        }

        private void HandleFixingState()
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= fixingDuration)
            {
                if (currentWrench != null) Destroy(currentWrench);
                tvToFix.Repair();
                TransitionToState(NpcCharacterState.Standing);
                animator.SetTrigger("FinishFixing");
            }
            
            wrenchSoundTimer -= Time.deltaTime;
            if (wrenchSoundTimer <= 0)
            {
                handAudioSource.pitch = Random.Range(1 - walkingPitchVariation / 2, 1 + walkingPitchVariation / 2);
                handAudioSource.PlayOneShot(wrenchSound);
                wrenchSoundTimer = wrenchSoundInterval;
            }

            _interactive = true;
        }

        private void TransitionToState(NpcCharacterState newState)
        {
            currentState = newState;
            stateTimer = 0f;
            
            // Reset all animator states
            animator.SetBool("IsStanding", false);
            animator.SetBool("IsWalking", false);
            animator.ResetTrigger("Exasperated");
            animator.ResetTrigger("StartFixing");
            animator.ResetTrigger("FinishFixing");

            // Set new state animator parameters
            switch (newState)
            {
                case NpcCharacterState.Standing:
                    animator.SetBool("IsStanding", true);
                    OnShowObtainedArtifactsInventoryEvent?.Invoke();
                    break;

                case NpcCharacterState.Exasperated:
                    headAudioSource.PlayOneShot(exasperationSound);
                    OnHideObtainedArtifactsInventoryEvent?.Invoke();
                    break;
            
                case NpcCharacterState.Walking:
                    walkingSoundTimer = walkingSoundInterval; // Reset timer
                    PlayRandomFootstep(); // Play immediate first step
                    break;
            
                case NpcCharacterState.Fixing:
                    wrenchSoundTimer = wrenchSoundInterval; // Reset timer
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && currentState == NpcCharacterState.Standing)
            {
                _interactive = true;
                Debug.Log("Player has entered the trigger", this);
                OnTriggerEnterEvent?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _interactive = false;
                Debug.Log("Player has exited the trigger", this);
                OnTriggerExitEvent?.Invoke();
            }
        }

        public void OnReceiveArtifact(ItemData itemData)
        {
            obtainedArtifacts |= itemData.itemType;
            OnUpdateObtainedArtifactsEvent?.Invoke();
        }

        private bool HasAllNecessaryArtifacts() =>
            (obtainedArtifacts & requiredArtifacts) == requiredArtifacts;
    }
}