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

        public SmallItemSlot[] smallItems = new SmallItemSlot[6];
        public BigItemSlot[] bigItems = new BigItemSlot[2];

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
