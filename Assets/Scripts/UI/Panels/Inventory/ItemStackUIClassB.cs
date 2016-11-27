using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Objects.Inventory;
using System;

namespace UI.Panels.Inventory
{
    public class ItemStackUIClassB : ItemStackUI
    {
        public InventoryItemClassB inventoryItem;
        public ItemStackUIClassB(GameObject imageParent) : base(imageParent) { }

        public override int Count
        {
            get
            {
                if (inventoryItem == null) return 0;
                return inventoryItem.item == null ? 0 : 1;
            }
            set { }
        }

        public Image image
        {
            get
            {
                return imageParent.GetComponentsInChildren<Image>()
                    .FirstOrDefault();
            }
        }

        public void Update(InventoryItemClassB item)
        {
            inventoryItem = item;

            if (item == null)
            {
                image.sprite = null;
                image.color = Color.black;
            }
            else
            {
                var renderer = item.item.GetComponent<SpriteRenderer>();
                Sprite s = renderer.sprite;
                image.sprite = s;
            }
        }
    }
}
