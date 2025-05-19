using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Misc.Spawners
{
    public class ArtifactSpawner: MonoBehaviour
    {
        [SerializeField] private List<ArtifactSpawnData> artifactSpawnDataList;
        [SerializeField] private Inventory.Inventory inventory;
        [SerializeField] private UnityEvent<string> onProximityZoneEntered;
        [SerializeField] private UnityEvent<string> onProximityZoneExited;
        [SerializeField] private KeyCode keyToInteract = KeyCode.Z;
        
        private void Start()
        {
            artifactSpawnDataList.RemoveAll(spawnData =>
                inventory.Slots.Any(slot =>
                    (spawnData.artifactType & slot.itemData.itemType) != 0
                )
            );
            Debug.Log(artifactSpawnDataList, this);
            artifactSpawnDataList.ForEach(Spawn);
        }

        private void Spawn(ArtifactSpawnData spawnData)
        {
            spawnData.spawnedGameObject = Instantiate(spawnData.artifactItem.artifactPrefab, spawnData.spawnTransform);
            if (spawnData.spawnedGameObject.TryGetComponent<SpawnedArtifact>(out var artifact))
            {
                artifact.onProximityZoneEntered?.AddListener(() => OnProximityZoneOfArtifactEntered(spawnData));
                artifact.onProximityZoneExited?.AddListener(() => OnProximityZoneOfArtifactExited(spawnData));
                artifact.onCollected?.AddListener((itemData) => AddToInventoryAndRemove(spawnData, itemData));
                artifact.keyToCollect = keyToInteract;
            }
        }

        public void OnProximityZoneOfArtifactEntered(ArtifactSpawnData spawnData)
        {
            onProximityZoneEntered?.Invoke($"{spawnData.artifactItem.itemName}::Press Z to collect the {spawnData.artifactItem.itemName}");
        }

        public void OnProximityZoneOfArtifactExited(ArtifactSpawnData spawnData)
        {
            onProximityZoneExited?.Invoke($"{spawnData.artifactItem.itemName}");
        }

        public void AddToInventoryAndRemove(ArtifactSpawnData spawnData, ItemData itemData)
        {
            inventory.Add(itemData);
            Remove(spawnData);
        }

        private void Remove(ArtifactSpawnData spawnData)
        {
            if (spawnData.spawnedGameObject != null) Destroy(spawnData.spawnedGameObject);
            artifactSpawnDataList.Remove(spawnData);
        }
    }
}