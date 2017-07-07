using System;
using UnityEngine;
using Objects.Movable.Characters;

namespace Objects.Immovable.Furniture
{
	public class Chair : CharacterFurniture, IInspectable
	{
		public Vector2 characterPosition {
			get {
				return this.colliderCenter + characterOffset;
			}
		}

		public Vector2 characterOffset = new Vector2(0.0f, 25.0f);
		Vector2 characterFromPosition;
		GameObject characterFoot;

		protected override void Start() {
			interactionText = "Press T to Sit Down";
			base.Start();
		}

		#region IInspectable implementation

		public void InspectionAction (Object controller, UnityEngine.RaycastHit2D hit)
		{
			var character = controller.GetComponent<Character>();
			if(character != null) {
				if(!character.isSittingDown) {
					interactionText = "Press T to Stand Up";
					characterFromPosition = character.transform.position;
					character.orientationX = OrientationX;
					character.orientationY = OrientationY;
					character.isSittingDown = true;
					character.transform.position = characterPosition;
					character.UpdateSortingLayer(0);
				} else {
					interactionText = "Press T to Sit Down";
					character.isSittingDown = false;
					character.transform.position = characterFromPosition;
				}
			}
		}
		#endregion
	}
}

