using System.Collections;
using System.Linq;
using Inventory;
using UnityEngine;

namespace Misc.Spawners
{
    public class SpawnedPickup : MonoBehaviour
    {
        [SerializeField] public ItemData data;
        [SerializeField] public bool isPickable = true;
        [SerializeField] public float attractDistance = 5f;
        [SerializeField] public float attractSpeed = 3f;
        [SerializeField] public float spinSpeed = 90f;
        [SerializeField] public float hoverAmplitude = 0.25f;
        [SerializeField] public float hoverFrequency = 1f;
        [SerializeField] public Inventory.Inventory playerInventory;
        [SerializeField] public InventoryItemEvent onCollected;
        [SerializeField] private GameObject innerObject;
        [Header("Pickup VFX")]
        [Tooltip("Optional particle effect to play on pickup")]
        [SerializeField] private ParticleSystem pickupEffectPrefab;
        private Vector3 _initialPosition;

        private Transform _playerTransform;

        private void Awake()
        {
            _initialPosition = transform.position;
            if (!GetComponent<Collider>().isTrigger)
                GetComponent<Collider>().isTrigger = true;
        }

        private void Start()
        {
            // assume your player GameObject is tagged "Player"
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                _playerTransform = playerGO.transform;

            StartCoroutine(SpinAndHover());
            StartCoroutine(AttractToPlayer());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isPickable) return;
            if (!other.CompareTag("Player")) return;

            onCollected?.Invoke(data);
            Debug.Log("DESTROYING", this);
            Destroy(gameObject);
            // StartCoroutine(PlayEffectAndDestroy());
        }

        private IEnumerator SpinAndHover()
        {
            while (true)
            {
                // spin
                transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

                // hover
                var y = _initialPosition.y +
                        Mathf.Sin(Time.time * hoverFrequency * Mathf.PI * 2f)
                        * hoverAmplitude;
                var p = transform.position;
                transform.position = new Vector3(p.x, y, p.z);

                yield return null; // wait one frame
            }
        }

        private IEnumerator AttractToPlayer()
        {
            while (true)
            {
                if (_playerTransform != null)
                {
                    var dist = Vector3.Distance(transform.position, _playerTransform.position);
                    if (dist <= attractDistance)
                    {
                        // move on XZ plane toward player
                        var target = _playerTransform.position;
                        target.y = transform.position.y;
                        transform.position = Vector3.MoveTowards(
                            transform.position, target, attractSpeed * Time.deltaTime
                        );
                    }
                }

                yield return null;
            }
        }

        private void DisableAllColliders()
        {
            GetComponentsInChildren<Collider>().ToList()
                .ForEach(it => it.enabled = false);
        }
        
        private IEnumerator PlayEffectAndDestroy()
        {
            // Step 1: Immediately hide and disable interactions
            Destroy(innerObject);
            DisableAllColliders();

            // Step 2: Trigger collection event AFTER visual cleanup
            onCollected?.Invoke(data);

            // Step 3: Play VFX (if any)
            ParticleSystem ps = null;
            if (pickupEffectPrefab != null)
            {
                ps = Instantiate(
                    pickupEffectPrefab, 
                    transform.position, 
                    Quaternion.identity
                );
                ps.Play();
            }

            // Step 4: Wait for VFX (or minimum delay)
            float delay = ps != null ? 
                ps.main.duration + ps.main.startLifetime.constantMax : 
                0.2f;
    
            yield return new WaitForSeconds(delay);

            // Step 5: Finally destroy the pickup GameObject
            Destroy(gameObject);
        }
    }
}