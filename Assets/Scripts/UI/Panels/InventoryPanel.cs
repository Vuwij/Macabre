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

        PixelInventory.ItemSlot itemSelected;
        int itemSelectedNum = 0;
        bool isCombining;

        void RefreshItems()
        {
            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);
            Transform inventoryGUI = transform.Find("Inventory GUI");

            // Small Items
            for (int i = 0; i < 6; ++i) {
                PixelInventory.SmallItemSlot smallItemSlot = pixelInventory.smallItems[i];
                if (smallItemSlot == null) return;
                Transform smallItemParent = inventoryGUI.Find("Small Items");
                Transform slotUI = smallItemParent.Find("Stack " + (i + 1));
                Image img = slotUI.GetComponent<Image>();
                img.color = new Color(255, 255, 255, 0.3f);

                for (int j = 0; j < 4; ++j) {
                    Transform slotObject = slotUI.Find("Background").Find("Object " + (j + 1));
                    Image slotImage = slotObject.GetComponent<Image>();

                    slotImage.color = Color.clear;
                    if (smallItemSlot.items[j] != null) {
                        SpriteRenderer objectUI = smallItemSlot.items[j].GetComponent<SpriteRenderer>();
                        slotImage.sprite = objectUI.sprite;
                        slotImage.color = Color.white;
                    }
                }

                Text t = slotUI.Find("Count").GetComponent<Text>();
                if(pixelInventory.smallItems[i].count >= 2) {
                    t.text = "x" + pixelInventory.smallItems[i].count;
                } else {
                    t.text = "";
                }
            }

            // Large Items
            for (int i = 0; i < 2; ++i) {
                PixelInventory.BigItemSlot bigItemSlot = pixelInventory.bigItems[i];
                Transform bigItemParent = inventoryGUI.Find("Big Items");
                Transform slotUI = bigItemParent.Find("Stack " + (i + 1));

                Image slotImage = slotUI.GetComponent<Image>();
                if (pixelInventory.bigItems[i].item == null)
                    slotImage.color = new Color(255, 255, 255, 0.0f);
                else
                    slotImage.color = new Color(255, 255, 255, 1.0f);

                if (bigItemSlot.item != null) {
                    SpriteRenderer objectUI = bigItemSlot.item.GetComponent<SpriteRenderer>();
                    slotImage.sprite = objectUI.sprite;
                    slotImage.color = Color.white;
                }
            }

            // Property Info
            Text itemname = transform.Find("Item Description Panel").Find("Name").GetComponent<Text>();
            Text itemdescription = transform.Find("Item Description Panel").Find("Description").GetComponent<Text>();
            Text itemproperties = transform.Find("Item Description Panel").Find("Properties").GetComponent<Text>();
            Debug.Assert(itemname != null);
            Debug.Assert(itemdescription != null);
            Debug.Assert(itemproperties != null);
            itemname.text = "";
            itemdescription.text = "";
            itemproperties.text = "";

        }

        public void SelectSmallItem(int itemnum) {
            Debug.Assert(itemnum <= 6 && itemnum > 0);

            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);

            Transform inventoryGUI = transform.Find("Inventory GUI");
            Transform smallItemParent = inventoryGUI.Find("Small Items");

            RefreshItems();

            if (isCombining)
            {
                Debug.Assert(itemSelected != null);
                Debug.Assert(itemSelectedNum > 0 && itemSelectedNum <= 6);
                PixelInventory.SmallItemSlot item1 = (PixelInventory.SmallItemSlot) itemSelected;
                PixelInventory.SmallItemSlot item2 = pixelInventory.smallItems[itemnum - 1];
                pixelInventory.Combine(item1, item2);
                itemSelectedNum = 0;
                itemSelected = null;

                Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
                Image buttonImg = button.GetComponent<Image>();
                buttonImg.color = new Color(255, 255, 255);
                isCombining = false;

                RefreshItems();

                return;
            }

            itemSelectedNum = 0;
            itemSelected = pixelInventory.smallItems[itemnum - 1];
            if (itemSelected.empty)
                return;
            itemSelectedNum = itemnum;

            // Display Item Info
            Text itemname = transform.Find("Item Description Panel").Find("Name").GetComponent<Text>();
            Text itemdescription = transform.Find("Item Description Panel").Find("Description").GetComponent<Text>();
            Text itemproperties = transform.Find("Item Description Panel").Find("Properties").GetComponent<Text>();
            Debug.Assert(itemname != null);
            Debug.Assert(itemdescription != null);
            Debug.Assert(itemproperties != null);

            PixelItem itemToDescribe = null;
            for (int i = 0; i < 4; ++i) {
                if (pixelInventory.smallItems[itemnum - 1].items[i] == null) continue;
                itemToDescribe = pixelInventory.smallItems[itemnum - 1].items[i];
                break;
            }
            Debug.Assert(itemToDescribe != null);

            itemname.text = itemToDescribe.name;
            itemdescription.text = itemToDescribe.description;
            if (itemToDescribe.properties == null)
                itemproperties.text = "";
            else
            {
                string propertyString = "Properties: \n";
                for (int i = 0; i < itemToDescribe.properties.Length; ++i)
                    propertyString = propertyString + itemToDescribe.properties[i] + " ";
                itemproperties.text = propertyString;
            }

            Transform slotUIselected = smallItemParent.Find("Stack " + itemnum);
            Image imgSelected = slotUIselected.GetComponent<Image>();
            imgSelected.color = new Color(255, 255, 255, 0.8f);
        }

        public void SelectLargeItem(int itemnum) {
            Debug.Assert(itemnum <= 2 && itemnum > 0);

            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);

            Transform inventoryGUI = transform.Find("Inventory GUI");
            Transform largeItemParent = inventoryGUI.Find("Big Items");

            RefreshItems();

            if (isCombining) {
                itemSelectedNum = 0;
                itemSelected = null;

                Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
                Image buttonImg = button.GetComponent<Image>();
                buttonImg.color = new Color(255, 255, 255);
                isCombining = false;

                RefreshItems();
            }

            itemSelectedNum = 0;
            itemSelected = pixelInventory.bigItems[itemnum - 1];
            if (itemSelected.empty)
                return;
            itemSelectedNum = itemnum;

            // Display Item Info
            Text itemname = transform.Find("Item Description Panel").Find("Name").GetComponent<Text>();
            Text itemdescription = transform.Find("Item Description Panel").Find("Description").GetComponent<Text>();
            Text itemproperties = transform.Find("Item Description Panel").Find("Properties").GetComponent<Text>();
            Debug.Assert(itemname != null);
            Debug.Assert(itemdescription != null);
            Debug.Assert(itemproperties != null);

            PixelItem itemToDescribe = pixelInventory.bigItems[itemnum - 1].item;
            itemname.text = itemToDescribe.name;
            itemdescription.text = itemToDescribe.description;
            if (itemToDescribe.properties == null)
                itemproperties.text = "";
            else
            {
                string propertyString = "Properties: \n";
                for (int i = 0; i < itemToDescribe.properties.Length; ++i)
             
                    propertyString = propertyString + itemToDescribe.properties[i] + " ";
                itemproperties.text = propertyString;
            }

            Transform slotUIselected = largeItemParent.Find("Stack " + itemnum);
            Image imgSelected = slotUIselected.GetComponent<Image>();
            imgSelected.color = new Color(255, 255, 0, 1.0f);
        }

        public void Combine()
        {
            if (itemSelected == null || itemSelected is PixelInventory.BigItemSlot)
                return;
            
            Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
            Image buttonImg = button.GetComponent<Image>();

            if(isCombining == true) {
                isCombining = false;
                buttonImg.color = new Color(255, 255, 255);
            }
            else {
                isCombining = true;
                buttonImg.color = new Color(255, 0, 0);
            }
        }

        public void Seperate()
        {
            if (itemSelected == null)
                return;

            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);

            pixelInventory.Break(itemSelected);

            // Stop Combining
            Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
            Image buttonImg = button.GetComponent<Image>();
            isCombining = false;
            buttonImg.color = new Color(255, 255, 255);

            itemSelected = null;
            itemSelectedNum = 0;
            RefreshItems();
        }

        public void Inspect()
        {

        }

        public void Drop()
        {
            if (itemSelected == null)
                return;

            GameObject player = GameObject.Find("Player");
            PixelInventory pixelInventory = player.GetComponentInChildren<PixelInventory>();
            Debug.Assert(pixelInventory != null);

            // Stop Combining
            Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
            Image buttonImg = button.GetComponent<Image>();
            isCombining = false;
            buttonImg.color = new Color(255, 255, 255);

            pixelInventory.Drop(itemSelected);

            itemSelected = null;
            itemSelectedNum = 0;
            RefreshItems();
        }

        public void Return()
        {
            // Stop Combining
            Transform button = transform.Find("Inventory GUI").Find("Inventory Buttons").Find("Combine");
            Image buttonImg = button.GetComponent<Image>();
            isCombining = false;
            buttonImg.color = new Color(255, 255, 255);

            itemSelected = null;
            itemSelectedNum = 0;
            gameObject.SetActive(false);
        }
    }
}
