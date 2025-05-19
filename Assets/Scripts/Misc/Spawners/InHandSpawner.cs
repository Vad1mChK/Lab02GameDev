using System.Collections.Generic;
using Inventory;
using NUnit.Framework;
using UnityEngine;

namespace Misc.Spawners
{
    public class InHandSpawner : MonoBehaviour
    {
        [Header("Inject via Inspector")]
        [SerializeField] private Inventory.Inventory inventory;   // your Inventory component
        [SerializeField] private Transform handParent;           // where in the hierarchy the item should live

        // Cache one spawned GameObject per ItemData
        private readonly Dictionary<ItemData, GameObject> _cache = new Dictionary<ItemData, GameObject>();

        // The currently active in-hand GameObject
        private GameObject _currentGO;

        private void OnEnable()
        {
            inventory.OnSlotSelected.AddListener(HandleSlotSelected);
        }

        private void OnDisable()
        {
            inventory.OnSlotSelected.RemoveListener(HandleSlotSelected);
        }

        private void HandleSlotSelected(int slotIndex)
        {
            // 1) Hide the previous one
            if (_currentGO != null)
                _currentGO.SetActive(false);

            // 2) Get the newly selected slot
            if (slotIndex < 0 || slotIndex >= inventory.Slots.Count)
                return;

            var slot = inventory.Slots[slotIndex];
            var data = slot.itemData;

            // 3) Spawn & cache if we haven't already
            if (!_cache.TryGetValue(data, out var go))
            {
                // assume you’ve added an `inHandPrefab` to ItemData
                go = Instantiate(data.inHandPrefab, handParent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                _cache[data] = go;
            }

            // 4) Activate & track
            go.SetActive(true);
            _currentGO = go;
        }
    }

}