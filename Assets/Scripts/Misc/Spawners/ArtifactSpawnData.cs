using System;
using Inventory;
using UnityEngine;

namespace Misc.Spawners
{
    [Serializable]
    public class ArtifactSpawnData
    {
        [SerializeField] public ItemType artifactType;
        [SerializeField] public ItemData artifactItem;
        [SerializeField] public Transform spawnTransform;
        [SerializeField] public GameObject spawnedGameObject;
    }
}