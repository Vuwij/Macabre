using Objects.Immovable.Items;
using System;

namespace Objects.Inventory
{
    public abstract class InventoryItem
    {
        public Inventory inventory;

        public InventoryItem(Item itemController, Inventory inventory)
        {
            this.inventory = inventory;
        }
    }
}