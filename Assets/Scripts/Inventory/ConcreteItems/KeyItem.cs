using System.Linq;
using Misc;
using UnityEngine;

namespace Inventory.ConcreteItems
{
    [CreateAssetMenu(menuName = "Inventory/Item/Key", fileName = "Key", order = 0)]
    public class KeyItem : ItemData
    {
        [Header("Key-specific properties")]
        public string doorId;
        public override bool Use(GameObject user)
        {
            base.Use(user);
            var doorOpener = DoorOpener.Lookup(doorId);
            
            if (doorOpener == null)
            {
                Debug.Log($"door opener for id {doorId} not found, try keys {string.Join(',', DoorOpener._map.Keys.ToList())}");
                return false;
            }

            if (!doorOpener.Usable)
            {
                Debug.Log($"door opener for id {doorId} not usable");
                return false;
            }

            doorOpener.ToggleOpen();
            return true;
        }
    }
}