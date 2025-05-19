using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;

namespace Misc.Spawners
{
    public class PickupSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Inventory.Inventory inventory;
        [SerializeField] private List<PickupSpawnData> spawnDataList;

        private void Start()
        {
            spawnDataList
                .Where(spawnData => !spawnData.deferSpawn)
                .ToList()
                .ForEach(spawnData => 
                    spawnData.spawnPoints
                        .ToList()
                        .ForEach(point => Spawn(spawnData, point.position))
                );
        }

        private void Spawn(PickupSpawnData rule, Vector3 pos)
        {
            var prefab = rule.pickupItem.pickupPrefab;
            var go     = Instantiate(prefab, pos, Quaternion.identity);
        
            // configure behaviour
            var pickup = go.GetComponent<SpawnedPickup>();
            if (pickup == null) pickup = go.AddComponent<SpawnedPickup>();

            pickup.data            = rule.pickupItem;
            pickup.isPickable      = rule.startPickable;
            pickup.attractDistance = rule.attractDistance;
            pickup.attractSpeed    = rule.attractSpeed;
            pickup.spinSpeed       = rule.spinSpeed;
            pickup.hoverAmplitude  = rule.hoverAmplitude;
            pickup.hoverFrequency  = rule.hoverFrequency;

            // give it a reference to your Inventory
            pickup.playerInventory = inventory;
            pickup.onCollected?.AddListener((itemData) => inventory.Add(itemData));
        }

        public void SpawnByItemData(ItemData itemData)
        {
            spawnDataList
                .Where(s => s.pickupItem == itemData)
                .ToList()
                .ForEach(spawnData => 
                    spawnData.spawnPoints
                        .ToList()
                        .ForEach(point => Spawn(spawnData, point.position))
                );
        }
    }
}