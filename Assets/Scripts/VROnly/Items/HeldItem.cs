using Inventory;
using UnityEngine;

namespace VROnly.Items
{
    public class HeldItem: MonoBehaviour
    {
        [SerializeField] private ItemData data;
        public ItemType Type => data.itemType;
        public ItemData Data => data;
    }
}