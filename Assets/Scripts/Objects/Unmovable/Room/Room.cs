using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Unmovable.Building;
using Objects.Unmovable.Furniture;
using Objects.Unmovable.Path;
using UnityEngine;
using Extensions;

namespace Objects.Unmovable
{
	public class Room : UnmovableObject
    {
		public Building.Building buildingController
        {
            get
            {
				var room = GetComponentInParent<Building.Building>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }
		public VirtualPath[] paths
		{
			get { return GetComponentsInChildren<VirtualPath>(); }
		}
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
		protected PolygonCollider2D playerShadow; // The top part of the room where the player its located
		Material shadowMaterial;

		protected override void Start() {
			shadowMaterial = Resources.Load("Materials/Shadow", typeof(Material)) as Material;
			createShadows();

			base.Start();
		}

		protected virtual void createShadows() {
			
		}

		void OnTriggerEnter2D(Collider2D collider) {
			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				var c = spriteRenderer.color;
				c.a = 0.3f;
				spriteRenderer.color = c;
			}

			// Creates the child shadow
			var childShadow = new GameObject();
			childShadow.name = this.name + " Shadow";
			childShadow.transform.parent = transform;
			childShadow.transform.position = transform.position + new Vector3(0, 0, -1);
			var polygon = childShadow.AddComponent<Polygon>();
			polygon.pc2.points = wallPoints;
			polygon.mr.material = shadowMaterial;
			polygon.Refresh();
		}

		void OnTriggerExit2D(Collider2D collider) {
			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				var c = spriteRenderer.color;
				c.a = 1.0f;
				spriteRenderer.color = c;
			}
			GameObject.Destroy(GameObject.Find(this.name + " Shadow"));
		}
    }
}
