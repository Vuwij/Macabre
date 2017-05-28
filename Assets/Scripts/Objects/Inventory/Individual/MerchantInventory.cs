using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inventory.Individual
{
    public class MerchantInventory : Inventory {

		public MerchantInventory(GameObject gameObject, int classALimit, int classBLimit)
			: base(gameObject, classALimit, classBLimit) {}
		
	}
}