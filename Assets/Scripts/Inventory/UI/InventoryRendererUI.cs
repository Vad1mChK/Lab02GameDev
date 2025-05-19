using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Inventory.UI
{ 
    public class InventoryRendererUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private InventorySlotUI slotPrefab;

        private List<InventorySlotUI> slots = new();

        private void Update()
        {
            float threshold = 0f;
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
            if (scrollAmount > threshold)
            {
                SelectNextSlot();
            }

            if (scrollAmount < -threshold)
            {
                SelectPreviousSlot();
            }
        }

        void OnEnable()
        {
            RebuildUI();
            inventory.OnInventoryUpdated.AddListener(RebuildUI);
            inventory.OnSlotSelected   .AddListener(OnSlotSelected);
        }

        void OnDisable()
        {
            inventory.OnInventoryUpdated.RemoveListener(RebuildUI);
            inventory.OnSlotSelected   .RemoveListener(OnSlotSelected);
        }

        private void SelectNextSlot()
        {
            if (inventory.Slots.Count < 2) return;
            int next = (inventory.CurrentSlot + 1) % inventory.Slots.Count;
            inventory.SelectSlot(next);
        }

        private void SelectPreviousSlot()
        {
            if (inventory.Slots.Count < 2) return;
            int prev = (inventory.CurrentSlot + inventory.Slots.Count - 1) % inventory.Slots.Count;
            inventory.SelectSlot(prev);
        }

        void RebuildUI()
        {
            // Clear or pool existing slots
            foreach (var s in slots) Destroy(s.gameObject);
            slots.Clear();

            // Recreate slots
            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                var data = inventory.Slots[i];
                InventorySlotUI uiSlot = Instantiate(slotPrefab, slotContainer);
                uiSlot.Setup(i, data, inventory.SelectSlot);
                slots.Add(uiSlot);
            }
            
            OnSlotSelected(inventory.CurrentSlot);
        }

        void OnSlotSelected(int index)
        {
            for (int i = 0; i < slots.Count; i++)
                slots[i].SetSelected(i == index);
        }
    }
}