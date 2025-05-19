using System;

namespace Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemData itemData;
        public int count;

        public InventorySlot(ItemData itemData, int count = 1)
        {
            this.itemData = itemData;
            this.count = count;
        }
    }
}