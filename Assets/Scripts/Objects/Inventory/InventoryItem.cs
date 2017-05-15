using Objects.Unmovable.Items;
using System;

namespace Objects.Inventory
{
    public abstract class InventoryItem
    {
        public Inventory inventory;

        public InventoryItem(ItemController itemController, Inventory inventory)
        {
            this.inventory = inventory;
        }
    }
}