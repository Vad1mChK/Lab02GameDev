using System;

namespace Inventory
{
    [Flags]
    public enum ItemType
    {
        None = 0,

        Key = 1 << 0,
        TvRemote = 1 << 1,
        PickupItems = Key | TvRemote,

        Spear = 1 << 2,
        Fan = 1 << 3,
        Mask = 1 << 4,
        ArtifactItems = Spear | Fan | Mask
    }
}