using UnityEngine;
using System.Collections;
using Objects.Movable;
using Objects.Movable.Characters.Individuals;

namespace Objects.Immovable.Path
{
    public class Door : VirtualPath
    {
		protected override void Start() {
			interactionText = "Press Space To Enter";

			if(GetComponent<Collider2D>() == null)
				gameObject.AddComponent<PolygonCollider2D>();

			if(room != null && room.name != "Exterior")
				enabled = false;
			base.Start();
		}

		public override void InspectionAction(Object controller, RaycastHit2D hit)
		{
			if(room.name == "Exterior" || room.name.Contains("Balcony")) {
				var overworld = GameObject.Find("Overworld");
				var s = overworld.GetComponent<SpriteRenderer>();
				GameObject.Find("Player").GetComponent<Player>().isInsideBuilding = true;
				s.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
			}
			if(destination.name == "Exterior" || destination.name.Contains("Balcony")) {
				var overworld = GameObject.Find("Overworld");
				GameObject.Find("Player").GetComponent<Player>().isInsideBuilding = false;
				Game.main.clock.PeriodicUpdate();
			}

			base.InspectionAction(controller, hit);
		}
    }
}