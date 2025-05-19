using System.Linq;
using Electronics;
using UnityEngine;

namespace Inventory.ConcreteItems
{
    [CreateAssetMenu(menuName = "Inventory/Item/TV Remote", fileName = "TV Remote", order = 0)]
    public class TVRemoteItem: ItemData
    {
        public string tvId;

        public override bool Use(GameObject user)
        {
            base.Use(user);
            var tv = TvController.Lookup(tvId);
            
            if (tv == null)
            {
                Debug.Log($"tv controller for id {tvId} not found, " +
                          $"try ids {string.Join(", ", TvController._map.Keys.ToArray())}");
                return false;
            }
            
            tv.Toggle();
            return true;
        }
    }
}