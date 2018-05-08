using UnityEngine.UI;
using UnityEngine;
using Objects;

namespace UI.Panels
{
    public sealed class InventoryPanel : UIPanel
    {   
        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshItems();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void RefreshItems()
        {
            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);
            Transform inventoryGUI = transform.Find("Inventory GUI");

            // Small Items
            for (int i = 0; i < 6; ++i) {
                PixelInventory.SmallItemSlot smallItemSlot = pixelInventory.smallItems[i];
                Debug.Assert(smallItemSlot != null);
                Transform smallItemParent = inventoryGUI.Find("Small Items");
                Transform slotUI = smallItemParent.Find("Stack " + (i + 1));

                for (int j = 0; j < 4; ++j) {
                    Transform slotObject = slotUI.Find("Background").Find("Object " + (j + 1));
                    Image slotImage = slotObject.GetComponent<Image>();

                    if (smallItemSlot.items[j] != null) {
                        SpriteRenderer objectUI = smallItemSlot.items[j].GetComponent<SpriteRenderer>();
                        slotImage.sprite = objectUI.sprite;
                    }
                }
            }

            // Large Items
            for (int i = 0; i < 2; ++i) {
                PixelInventory.BigItemSlot bigItemSlot = pixelInventory.bigItems[i];
                Transform bigItemParent = inventoryGUI.Find("Big Items");
                Transform slotUI = bigItemParent.Find("Stack " + (i + 1));

                Image slotImage = slotUI.GetComponent<Image>();
                SpriteRenderer objectUI = bigItemSlot.item.GetComponent<SpriteRenderer>();
                slotImage.sprite = objectUI.sprite;
            }
        }

        public void Combine()
        {
            //if (ItemStackUI.currentlySelected.Count != 2) return;
            
            //ItemStackUI[] toCombine = ItemStackUI.currentlySelected.ToArray();
            //if (toCombine[0].Count + toCombine[1].Count > 4) return;

            //ItemStackUIClassA itemUI1 = null, itemUI2 = null;
            //for (int i = 0; i < 6; i++)
            //{
            //    if (itemStackUIClassAs[i] == toCombine[0])
            //    {
            //        itemUI1 = (toCombine[0] as ItemStackUIClassA);
            //        itemUI2 = (toCombine[1] as ItemStackUIClassA);
            //        break;
            //    }
            //    if(itemStackUIClassAs[i] == toCombine[1])
            //    {
            //        itemUI1 = (toCombine[1] as ItemStackUIClassA);
            //        itemUI2 = (toCombine[0] as ItemStackUIClassA);
            //        break;
            //    }
            //}

            //// Attempt a merge only if the item is single
            ////if (itemUI1.inventoryItem == null || itemUI2.inventoryItem == null) return;
            ////itemUI1.inventoryItem += itemUI2.inventoryItem;
            
            RefreshItems();
        }

        public void Seperate()
        {
            //// Select the currently selected item
            //if (ItemStackUI.currentlySelected.Count != 1) return;
            //var itemStack = ItemStackUI.currentlySelected.Peek();

            //// Check if a seperation is possible and remove one from the bundle
            //if (!(itemStack is ItemStackUIClassA)) return;
            //if (itemStack.Count <= 1) return;

            //// Check if there is available space
            //if (!(itemStackUIClassAs.Any(x => x.Count == 0))) return;

            //ItemStackUIClassA itemStackUIClassARemoveFrom = itemStack as ItemStackUIClassA;

            //// The actual transfer
            ////var itemToBeTransferred = itemStackUIClassARemoveFrom.inventoryItem.items.Last();
            ////inventory.Add(itemToBeTransferred);
            ////itemStackUIClassARemoveFrom.inventoryItem.items.Remove(itemToBeTransferred);
            
            RefreshItems();
        }

        public void Inspect()
        {

        }

        public void Drop()
        {
            //// Select the currently selected item
            //if (ItemStackUI.currentlySelected.Count == 0) return;
            //var itemStack = ItemStackUI.currentlySelected.Dequeue();

            //// Obtain the inventoryItem from the stack
            //InventoryItem inventoryItem;
            //if (itemStack is ItemStackUIClassA)
            //    inventoryItem = (itemStack as ItemStackUIClassA).inventoryItem;
            //else
            //    inventoryItem = (itemStack as ItemStackUIClassB).inventoryItem;

            //// Drop the inventory Item (from PlayerInventory)
            //inventory.Drop(inventoryItem);

            //// Refresh the UI
            //RefreshItems();
        }

        public void Return()
        {
            //TurnOff();
        }
    }
}
