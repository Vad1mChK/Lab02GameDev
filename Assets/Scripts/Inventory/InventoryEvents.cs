using System;
using UnityEngine.Events;

namespace Inventory
{
    [Serializable] public class InventoryUpdatedEvent: UnityEvent { }
    [Serializable] public class InventoryItemEvent: UnityEvent<ItemData> { }
    [Serializable] public class InventorySlotEvent: UnityEvent<int> { }
}