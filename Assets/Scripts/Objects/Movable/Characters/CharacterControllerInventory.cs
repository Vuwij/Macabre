using UnityEngine;
using System.Linq;
using Objects.Inanimate.Items;
using Objects.Inventory;
using Objects.Inventory.Individual;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        public CharacterInventory inventory
        {
            get { return character.inventory; }
        }

        public bool AddToInventory(ItemController i)
        {
            return inventory.Add(i);
        }

        public Transform InventoryFolder
        {
            get
            {
                if(GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory") == null)
                {
                    GameObject inventoryFolder = new GameObject("Inventory");
                    inventoryFolder.transform.parent = this.transform;
                }

                return GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory");
            }
        }

    }
}