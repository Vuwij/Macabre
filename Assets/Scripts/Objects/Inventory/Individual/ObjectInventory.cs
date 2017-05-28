using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inventory.Individual
{
	public class ObjectInventory : Inventory {

		public ObjectInventory(GameObject gameObject, int classALimit, int classBLimit)
			: base(gameObject, classALimit, classBLimit) {}
	}
}