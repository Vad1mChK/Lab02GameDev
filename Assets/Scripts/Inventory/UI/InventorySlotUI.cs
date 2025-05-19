using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventorySlotUI: MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Button selectButton;
        [SerializeField] private Image selectedOverlay;

        public void Setup(int index, InventorySlot slot, Action<int> onSelect)
        {
            iconImage.sprite = slot.itemData.icon;
            countText.text = slot.count > 1 ? slot.count.ToString() : "";
            
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(index));
        }

        public void SetSelected(bool isSelected)
        {
            selectedOverlay.gameObject.SetActive(isSelected);
        }
    }
}