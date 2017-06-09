using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inventory.Individual
{
	[Serializable]
	public class ObjectInventory : Inventory {

		public ObjectInventory(GameObject gameObject, int classALimit = 4, int classBLimit = 0)
			: base(gameObject, classALimit, classBLimit) {}
	}
}