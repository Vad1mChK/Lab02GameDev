using Inventory;
using UnityEngine;

namespace Misc.Spawners
{
    using System;
    using Inventory;
    using UnityEngine;

    [Serializable]
    public class PickupSpawnData
    {
        public ItemData   pickupItem;       // your ScriptableObject
        public Transform[] spawnPoints;     // where to put them
        public bool deferSpawn = false; // if set to true, only spawn when SpawnByItemData is called
        public bool       startPickable = true;
        public float      attractDistance = 5f;
        public float      attractSpeed    = 3f;
        public float      spinSpeed       = 90f;    // deg/sec
        public float      hoverAmplitude  = 0.25f;  // meters
        public float      hoverFrequency  = 1f;      // cycles/sec
    }
}