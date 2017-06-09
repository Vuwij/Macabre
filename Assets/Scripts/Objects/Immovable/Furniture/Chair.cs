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
			base.Start();
		}

		#region IInspectable implementation

		public void InspectionAction (Object controller, UnityEngine.RaycastHit2D hit)
		{
			var character = controller.GetComponent<Character>();
			if(character != null) {
				if(!character.isSittingDown) {
					characterFromPosition = character.transform.position;
					character.orientationX = OrientationX;
					character.orientationY = OrientationY;
					if(OrientationY == 1) {
						characterFoot = new GameObject("Feet", typeof(SpriteRenderer));
						characterFoot.transform.parent = character.transform;
						characterFoot.transform.localPosition = character.childObject.transform.localPosition;
						var s = characterFoot.GetComponent<SpriteRenderer>();
						if(OrientationX == 1)
							s.sprite = character.extraSprites.rightFeet;
						else
							s.sprite = character.extraSprites.leftFeet;
						s.sortingLayerName = "World";
						s.sortingOrder = spriteRenderer.sortingOrder - 1;
					}
					character.isSittingDown = true;
					character.transform.position = characterPosition;
				} else {
					if(characterFoot != null) Destroy(characterFoot);
					character.isSittingDown = false;
					character.transform.position = characterFromPosition;
				}
			}
		}

		#endregion
	}
}

