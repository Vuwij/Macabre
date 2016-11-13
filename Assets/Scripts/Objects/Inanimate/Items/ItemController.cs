using UnityEngine;

namespace Objects.Inanimate.Items
{
    public abstract partial class ItemController : InanimateObjectController {
        // The character associated with the controller, found in the data structure
        public Item item
        {
            get
            {
                return Items.ItemDictionary[name];
            }
        }
        
        // This is the object for the character controller
        protected override MacabreObject model
        {
            get
            {
                return item;
            }
        }
    }
}
