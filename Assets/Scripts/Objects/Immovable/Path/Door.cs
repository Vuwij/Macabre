using UnityEngine;
using System.Collections;
using Objects.Movable;

namespace Objects.Immovable.Path
{
    public class Door : VirtualPath
    {
		protected override void Start() {
			base.Start();
		}

		public override void InspectionAction(Object controller, RaycastHit2D hit)
		{
			if(room.name == "Exterior" || room.name.Contains("Balcony")) {
				var overworld = GameObject.Find("Overworld");
				var s = overworld.GetComponent<SpriteRenderer>();
				s.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
			}
			if(destination.name == "Exterior" || destination.name.Contains("Balcony")) {
				var overworld = GameObject.Find("Overworld");
				var s = overworld.GetComponent<SpriteRenderer>();
				s.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}

			base.InspectionAction(controller, hit);
		}
    }
}