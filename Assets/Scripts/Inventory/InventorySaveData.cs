using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Inventory
{
    [Serializable]
    public class InventorySaveData
    {
        public List<InventorySaveItem> items;
    }
}