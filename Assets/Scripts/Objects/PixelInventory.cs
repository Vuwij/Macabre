using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PixelInventory : MonoBehaviour
    {
        public class SmallItemSlot {
            public PixelItem[] items = new PixelItem[4];
        }
        public class BigItemSlot {
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

		public void Start()
		{
            smallItems = new SmallItemSlot[6];
            bigItems = new BigItemSlot[2];
            Debug.Assert(smallItems[0] != null);
		}

        public SmallItemSlot[] smallItems;
        public BigItemSlot[] bigItems;

        public bool AddItem(PixelItem pixelItem) {

            if(pixelItem.isLargeItem) {
                foreach(BigItemSlot bi in bigItems)
                {
                    if(bi.item == null) {
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
                    bool emptySlot = true;
                    foreach (PixelItem pi in si.items)
                    {
                        if (pi != null)
                            emptySlot = false;
                    }
                    if (emptySlot) {
                        smallItemSlot = si;
                        break;
                    }
                }
                if (smallItemSlot == null) 
                    return false; // No spots left to fill

                smallItemSlot.items[0] = pixelItem;
            }
            return true;
        }
    }
}
