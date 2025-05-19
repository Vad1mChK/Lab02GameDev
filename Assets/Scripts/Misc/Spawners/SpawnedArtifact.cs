using System;
using Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Misc.Spawners
{
    public class SpawnedArtifact: MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] public InventoryItemEvent onCollected;
        [SerializeField] public UnityEvent onProximityZoneEntered;
        [SerializeField] public UnityEvent onProximityZoneExited;
        [SerializeField] public KeyCode keyToCollect;
        private bool _collectible;
        public bool Collectible => _collectible;

        private void Update()
        {
            if (_collectible && Input.GetKeyDown(keyToCollect))
            {
                Collect();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _collectible = true;
                onProximityZoneEntered?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _collectible = false;
                onProximityZoneExited?.Invoke();
            }
        }

        private void OnDisable()
        {
            onCollected?.RemoveAllListeners();
            onProximityZoneExited?.RemoveAllListeners();
            onProximityZoneEntered?.RemoveAllListeners();
            gameObject.SetActive(false);
        }

        public void Collect()
        {
            onCollected?.Invoke(itemData);
            onProximityZoneExited?.Invoke();
            _collectible = false;
        }
    }
}