using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("SlotData")]
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        [Header("Events")]
        [SerializeField] public InventoryUpdatedEvent OnInventoryUpdated;

        [SerializeField] public InventorySlotEvent OnSlotSelected;
        [SerializeField] public UnityEvent<string> OnInHandItemUpdated;
        [SerializeField] public UnityEvent OnInHandItemCleared;
        
        public int CurrentSlot { get; private set; } = -1;
        public List<InventorySlot> Slots => slots;

        public void Start()
        {
            OnInventoryUpdated?.Invoke();
            if (slots.Count > 0)
            {
                SelectSlot(0);
            }
        }

        public void Add(ItemData newItemData)
        {
            var slot = slots.FirstOrDefault(s => s.itemData == newItemData);
            
            if (slot == null)
            {
                slots.Add(new InventorySlot(newItemData));
            }
            else
            {
               ++slot.count; 
            }
            
            Debug.Log($"New item added: {newItemData.itemName}, slots: {slots.Count}, current slot: {CurrentSlot}", this);
            
            OnInventoryUpdated?.Invoke();
            if (CurrentSlot < 0 && slots.Count > 0)
            {
                SelectSlot(0);
            }
        }

        public bool Remove(ItemData itemDataToDelete)
        {
            Debug.Log($"Removing item {itemDataToDelete.itemName}", this);
            var slot = slots.FirstOrDefault(s => s.itemData == itemDataToDelete);

            if (slot == null)
            {
                Debug.LogWarning("No slot found to remove", this);
                return false;
            }
            
            if (--slot.count <= 0)
            {
                slots.Remove(slot);
            }
            
            OnInventoryUpdated?.Invoke();
            if (CurrentSlot >= slots.Count)
            {
                SelectSlot(slots.Count - 1);
            }
            if (slots.Count == 0)
            {
                SelectSlot(-1);
            }

            Debug.Log($"Item {itemDataToDelete.itemName} successfully removed");
            return true;
        }

        public void SelectSlot(int slotIndex)
        {
            OnSlotSelected?.Invoke(slotIndex);
            CurrentSlot = slotIndex;
            if (slots.Count > 0)
            {
                OnInHandItemUpdated?.Invoke(FormatTextForSelectedItem(slots[slotIndex].itemData));
            }
            else
            {
                OnInHandItemCleared?.Invoke();
            }
        }

        public void UseCurrent()
        {
            if (CurrentSlot < 0 || CurrentSlot >= slots.Count) return;

            var slot = slots[CurrentSlot];
            var usageSuccessful = slot.itemData.Use(gameObject);
            if (usageSuccessful && !slot.itemData.reusable)
            {
                Debug.Log($"Item {slot.itemData.itemName} was used successfully and it isn't reusable", this);
                Remove(slot.itemData);
            }
        }

        public List<InventorySlot> GetSlotsByType(ItemType filter)
        {
            return slots
                .Where(s => s.itemData.itemType == filter)
                .ToList();
        }

        public string FormatTextForSelectedItem(ItemData itemData)
        {
            return $"inventory::{itemData.itemName}";
        }
    }
}