using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Objects.Inventory;

namespace UI.Panels.Inventory
{
    public class ItemStackUIClassA : ItemStackUI
    {
        public InventoryItemClassA inventoryItem;
        public ItemStackUIClassA(GameObject imageParent) : base(imageParent) { }

        public override int Count
        {
            get
            {
                if (inventoryItem == null) return 0;
                return inventoryItem.items.Count();
            }
            set {
                Text t = imageParent.GetComponentInChildren<Text>();

                if (value >= 4 || value < 0) throw new UnityException("Inventory stack invalid value");
                if (value == 0)
                    t.text = "";
                else
                    t.text = "x" + value;
            }
        }

        public List<Image> imageStack
        {
            get
            {
                return imageParent.GetComponentsInChildren<Image>()
                    .Where(x => x.transform.parent.name == "Background")
                    .ToList();
            }
        }

        public void Update(InventoryItemClassA item)
        {
            inventoryItem = item;

            if (item == null)
            {
                foreach (Image image in imageStack)
                {
                    image.sprite = null;
                    image.color = Color.black;
                }
                Count = 0;
            }
            else
            {
                foreach (Image image in imageStack)
                {
                    image.sprite = null;
                    image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
                for(int i = 0; i < item.items.Count; i++)
                {
                    var itemController = item[i];
                    var renderer = itemController.GetComponent<SpriteRenderer>();
                    Sprite s = renderer.sprite;
                    imageStack[i].sprite = s;
                    imageStack[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                Count = item.items.Count;
            }
        }
    }
}
