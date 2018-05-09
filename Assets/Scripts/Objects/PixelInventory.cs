using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Objects.Movable.Characters;

namespace Objects
{
    public class PixelInventory : MonoBehaviour
    {
        public abstract class ItemSlot {
            public abstract bool empty { get; }
        }
        public class SmallItemSlot : ItemSlot {
            public int count {
                get {
                    int total = 0;
                    for (int i = 0; i < 4; ++i)
                        if (items[i] != null) total++;
                    return total;
                }
            }
            public override bool empty {
                get {
                    for (int i = 0; i < 4; ++i)
                        if (items[i] != null) return false;
                    return true;
                }
            }
            public PixelItem[] items = new PixelItem[4];

            public void Clear() {
                for (int i = 0; i < 4; ++i)
                {
                    if(items[i] != null)
                        Destroy(items[i].gameObject);
                    items[i] = null;
                }
            }

        }
        public class BigItemSlot : ItemSlot {
            public override bool empty {
                get {
                    if (item != null) return false;
                    return true;
                }
            }
            public PixelItem item;
        }

        public int smallItemCount {
            get {
                int count = 0;
                foreach(SmallItemSlot si in smallItems) {
                    foreach(PixelItem pi in si.items) {
                        if (pi != null)
                            count++;
                    }
                }
                return count;
            }
        }

        public int bigItemCount
        {
            get
            {
                int count = 0;
                foreach (BigItemSlot bi in bigItems)
                {
                    if (bi.item != null)
                        count++;
                }
                return count;
            }
        }

		public void Awake()
		{
            for (int i = 0; i < 6; ++i) {
                smallItems[i] = new SmallItemSlot();
            }
            for (int i = 0; i < 2; ++i) {
                bigItems[i] = new BigItemSlot();
            }
		}

        public SmallItemSlot[] smallItems = new SmallItemSlot[6];
        public BigItemSlot[] bigItems = new BigItemSlot[2];

        public bool AddItem(PixelItem pixelItem) {

            if (pixelItem.isLargeItem) {
                foreach(BigItemSlot bi in bigItems)
                {
                    if(bi.empty) {
                        bi.item = pixelItem;
                        return true;
                    }
                }
                return false;
            }
            else {
                SmallItemSlot smallItemSlot = null;

                foreach (SmallItemSlot si in smallItems)
                {
                    if (!si.empty) continue;
                    smallItemSlot = si;
                    break;
                }
                if (smallItemSlot == null) 
                    return false; // No spots left to fill

                smallItemSlot.items[0] = pixelItem;
            }
            return true;
        }

        bool SameItem(SmallItemSlot a, SmallItemSlot b) {
            Debug.Assert(!a.empty);
            Debug.Assert(!b.empty);
            PixelItem combineItem = null;

            foreach (PixelItem item in a.items)
            {
                if (item == null) continue;

                if (combineItem == null)
                    combineItem = item;
                else
                {
                    if (item != null)
                    {
                        Debug.Assert(item.name == combineItem.name);
                    }
                }
            }
            foreach (PixelItem item in b.items)
            {
                if (item == null) continue;

                if (combineItem.name != item.name)
                    return false;
            }
            return true;
        }

        public bool Combine(SmallItemSlot a, SmallItemSlot b) {
            Debug.Assert(!a.empty);
            Debug.Assert(!b.empty);
            if (a.count + b.count > 4) return false;

            if(SameItem(a, b)) {
                // Passes tests, b goes into a
                for (int i = 0; i < 4; ++i)
                {
                    if (b.items[i] != null)
                    {
                        for (int j = 0; j < 4; ++j)
                        {
                            if (a.items[j] == null)
                            {
                                a.items[j] = b.items[i];
                                b.items[i] = null;
                            }
                        }
                    }
                }
                return true;
            }

            PixelItem itema = null, itemb = null;
            for (int i = 0; i < 4; ++i) {
                if(a.items[i] != null) {
                    itema = a.items[i];
                    break;
                }
            }
            for (int i = 0; i < 4; ++i) {
                if (b.items[i] != null) {
                    itemb = b.items[i];
                    break;
                }
            }
            Debug.Assert(itema != null);
            Debug.Assert(itemb != null);
            PixelItem itemc = PixelItem.Combine(itema, itemb);
            if (itemc == null) return false;

            GameObject itemcObj = (GameObject) Resources.Load("Items/" + itemc.name);
            Debug.Assert(itemcObj != null);
            a.Clear();
            b.Clear();
            GameObject obj = Instantiate(itemcObj, gameObject.transform);
            obj.gameObject.name = itemcObj.name;
            obj.gameObject.SetActive(false);
            PixelItem pixelItem = obj.GetComponent<PixelItem>();
            Debug.Assert(pixelItem != null);
            AddItem(pixelItem);

            return true;
        }

        public void Break(ItemSlot itemSlot) {
            if (itemSlot is SmallItemSlot) {
                SmallItemSlot smallItem = (SmallItemSlot) itemSlot;

                // Find an empty spot to fill
                SmallItemSlot emptyItemSlot = null;
                foreach (SmallItemSlot si in smallItems)
                {
                    if (!si.empty) continue;
                    emptyItemSlot = si;
                    break;
                }
                if (emptyItemSlot == null)
                    return; // No spots left to fill

                if (smallItem.count >= 2) {
                    if(smallItem.count == 2 || smallItem.count == 3) {
                        for (int i = 0; i < 4; ++i) {
                            if (smallItem.items[i] != null) {
                                emptyItemSlot.items[0] = smallItem.items[i];
                                smallItem.items[i] = null;
                                return;
                            }
                        }
                    }
                    else if(smallItem.count == 4) {
                        bool foundone = false;
                        for (int i = 0; i < 4; ++i){
                            if (smallItem.items[i] != null) {
                                if (!foundone) {
                                    emptyItemSlot.items[0] = smallItem.items[i];
                                    smallItem.items[i] = null;
                                    foundone = true;
                                }
                                else {
                                    emptyItemSlot.items[1] = smallItem.items[i];
                                    smallItem.items[i] = null;
                                    return;
                                }
                            }
                        }
                    }
                }
                // Break item into constituent parts
                PixelItem pixelItem = null;
                for (int i = 0; i < 4; ++i) {
                    if (smallItem.items[i] != null)
                        pixelItem = smallItem.items[i];
                }
                if(pixelItem.breakable) {
                    if (pixelItem.breakapart.Count != 2) return;

                    PixelItem item1 = pixelItem.breakapart[0];
                    PixelItem item2 = pixelItem.breakapart[1];

                    Debug.Assert(item1 != null);
                    Debug.Assert(item2 != null);

                    GameObject item1Obj = (GameObject)Resources.Load("Items/" + item1.name);
                    GameObject item2Obj = (GameObject)Resources.Load("Items/" + item2.name);

                    Debug.Assert(item1Obj != null);
                    Debug.Assert(item2Obj != null);

                    smallItem.Clear();

                    GameObject item1Instance = Instantiate(item1Obj, gameObject.transform);
                    GameObject item2Instance = Instantiate(item2Obj, gameObject.transform);
                    item1Instance.gameObject.SetActive(false);
                    item2Instance.gameObject.SetActive(false);
                    item1Instance.name = item1Obj.name;
                    item2Instance.name = item2Obj.name;

                    AddItem(item1Instance.GetComponent<PixelItem>());
                    AddItem(item2Instance.GetComponent<PixelItem>());
                }
            }
        }

        // Drops all the items straight at the characters foot
        public void Drop(ItemSlot itemSlot) {
            if (itemSlot is SmallItemSlot)
            {
                SmallItemSlot smallItemSlot = (SmallItemSlot)itemSlot;
                Debug.Assert(smallItemSlot.count != 0);

                foreach (PixelItem item in smallItemSlot.items)
                {
                    if (item == null) continue;
                    item.transform.parent = transform.parent.parent;
                    Vector2 position = (Vector2) transform.parent.position;
                    position += Random.insideUnitCircle * 2;
                    item.transform.position = position;
                    item.gameObject.SetActive(true);
                }

                for (int i = 0; i < 4; ++i) {
                    smallItemSlot.items[i] = null;
                }

            } else {
                BigItemSlot bigItemSlot = (BigItemSlot)itemSlot;
                Debug.Assert(bigItemSlot.item != null);

                PixelItem item = bigItemSlot.item;
                item.transform.parent = transform.parent.parent;
                Vector2 position = transform.parent.position;
                position += Random.insideUnitCircle * 2;
                item.transform.position = position;
                item.gameObject.SetActive(true);
                bigItemSlot.item = null;
            }

            Character character = transform.parent.GetComponent<Character>();
            if(character != null) {
                character.UpdateSortingLayer();
            }
        }
    }
}
