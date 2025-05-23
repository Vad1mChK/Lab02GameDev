using UnityEngine;
using Inventory;

namespace VROnly.Items
{
    [RequireComponent(typeof(Rigidbody))] // Ensures Rigidbody exists
    public class HeldItem : MonoBehaviour
    {
        [SerializeField] private ItemData data;
        [SerializeField] private Transform respawnPoint; // Assign in inspector
        
        private Rigidbody _rb;
        
        public ItemType Type => data.itemType;
        public ItemData Data => data;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            // Null check for critical components
            if (respawnPoint == null)
            {
                Debug.LogError("Respawn point not assigned!", this);
                // Optionally create default respawn point
                // respawnPoint = new GameObject("DefaultRespawn").transform;
                // respawnPoint.position = Vector3.zero;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("KillArea")) return;
            
            ResetPosition();
        }

        public void ResetPosition()
        {
            // Move to respawn point
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
            
            // Reset physics state
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            Debug.Log($"{name} respawned at {respawnPoint.position}");
        }
    }
}