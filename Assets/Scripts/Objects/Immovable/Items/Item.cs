﻿using Data.Databases;
using System;
using UnityEngine;
using System.Collections.Generic;
using Objects.Inventory;
using System.Collections;

namespace Objects.Immovable.Items
{
    [Serializable]
	public class Item : ImmovableObject, IInspectable {
		public override Vector2 colliderCenter {
			get {
				return gameObject.transform.position;
			}
		}

		public int ID;
		public string description;
		public List<string> attributes = new List<string>();
		public Dictionary<string, object> properties = new Dictionary<string, object>();
        public ItemType type = ItemType.InventoryItemClassA;
        
		CollisionCircle collisionCircle;

		protected override void Start() {
			collisionCircle = new CollisionCircle(gameObject, 2);
			interactionText = "Press T to pick up " + name;
			base.Start();
		}

		public void InspectionAction(Object obj, RaycastHit2D hit)
		{
			var character = obj as Movable.Characters.Character;
			character.isPickingUp = true;
			StartCoroutine(AddItemIntoInventory(character));
		}

		IEnumerator AddItemIntoInventory(Movable.Characters.Character character)
		{
			yield return new WaitForSeconds(1.0f);
			// Now do your thing here
			if (character != null)
			{
				bool addedToInventory = character.AddToInventory(this);
				if (addedToInventory)
				{
					gameObject.SetActive(false);
					gameObject.transform.parent = character.inventory.folder;
				}
			}
		}

		public override void UpdateSortingLayer (int? offset = null)
		{
			if(transform.parent != null) {
				if(transform.parent.GetComponent<IItemContainer>() != null) {
					spriteRenderer.sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
					spriteRenderer.sortingLayerID = transform.parent.GetComponent<SpriteRenderer>().sortingLayerID;
				}
			}
			else base.UpdateSortingLayer ();
		}
    }
}
