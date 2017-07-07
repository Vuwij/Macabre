using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Immovable.Buildings;
using Objects.Immovable.Furniture;
using Objects.Immovable.Path;
using UnityEngine;
using Extensions;

namespace Objects.Immovable.Rooms
{
	public class Room : ImmovableObject
    {
		public Buildings.Building buildingController
        {
            get
            {
				var room = GetComponentInParent<Buildings.Building>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }
		public VirtualPath[] paths
		{
			get { return GetComponentsInChildren<VirtualPath>(); }
		}
		// The rooms that share the same activeness
		public Room[] sharedRooms;
		protected List<AbstractFurniture> furniture
		{
			get
			{
				return gameObject.GetComponentsInChildren<AbstractFurniture>().ToList();
			}
		}
		EdgeCollider2D wall {
			get {
				return this.GetComponent<EdgeCollider2D>();
			}
		}
		protected virtual Vector2[] wallPoints {
			get {
				return wall.points;
			}
		}

		protected override void Start() {
			if(name != "Exterior") {
				foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
					sr.sortingLayerName = "Background";
				}
			}
			foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
				sr.sortingLayerName = "World";
			}
			base.Start();
		}

		public void SetOpacity(float opacity) {
			foreach(var obj in GetComponentsInChildren<SpriteRenderer>()) {
				var c = obj.color;
				c.a = opacity;
				obj.color = c;
			}
		}

		void OnEnable() {
			// Doors
			foreach (var door in gameObject.GetComponentsInChildren<Door>(true)) {
				door.enabled = true;
			}
			// Sorting Layers
			foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
				sr.sortingLayerName = "World";
			}
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
			// Shared Rooms
			foreach(var room in sharedRooms) {
				room.gameObject.SetActive(true);
			}
		}

		void OnDisable() {
			// Doors
			foreach (var door in gameObject.GetComponentsInChildren<Door>(true)) {
				door.enabled = false;
			}
			// Sorting Layers
			foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
				sr.sortingLayerName = "Background";
			}
			foreach(var room in sharedRooms) {
				room.gameObject.SetActive(false);
			}
		}
    }
}
