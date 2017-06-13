﻿using System;
using Objects.Inventory;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Objects.Immovable.Items;
using System.Collections;

namespace Objects.Immovable.Furniture
{
	[ExecuteInEditMode]
	public class Table : StorageFurniture
	{
		public Vector2 verticalOffset = new Vector2(0, 8.0f);

		protected override void Start() {
			base.Start();
		}

		protected override void reloadItems() {
			base.reloadItems();

			foreach(InventoryItem ii in storage.classAItems) {
				for(int i = 0; i < ii.items.Count; i++) {
					if(ii.items[i] == null) return;
					ii.items[i].gameObject.transform.localPosition = ii.tableOffset + verticalOffset;
				}
			}
		}

		void OnValidate() {
			reloadItems();
		}
	}
}

