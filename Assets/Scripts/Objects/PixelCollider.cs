using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Objects.Movable.Characters;

namespace Objects
{
	[ExecuteInEditMode]
	public class PixelCollider : MonoBehaviour, IComparable<PixelCollider>
	{
		public class MovementRestriction
		{
			public bool restrictNW = false;
			public bool restrictNE = false;
			public bool restrictSW = false;
			public bool restrictSE = false;

			public Direction slopeDirection = Direction.All;
			public float slope = 0.0f;
			public PixelDoor enteredDoor;
		}

		public Vector2 topLeft => (top + left) / 2;
		public Vector2 topRight => (top + right) / 2;
		public Vector2 bottomLeft => (bottom + left) / 2;
		public Vector2 bottomRight => (bottom + right) / 2;
		public Vector2 center => (top + left + right + bottom) / 4;

		public Vector2 topLeftWorld => topLeft + (Vector2)transform.position;
		public Vector2 topRightWorld => topRight + (Vector2)transform.position;
		public Vector2 bottomLeftWorld => bottomLeft + (Vector2)transform.position;
		public Vector2 bottomRightWorld => bottomRight + (Vector2)transform.position;
		public Vector2 centerWorld => center + (Vector2)transform.position;
		public Vector2 topWorld => top + (Vector2)transform.position;
		public Vector2 bottomWorld => bottom + (Vector2)transform.position;
		public Vector2 leftWorld => left + (Vector2)transform.position;
		public Vector2 rightWorld => right + (Vector2)transform.position;
		public Vector2 topPWorld => topP + (Vector2)transform.position;
		public Vector2 bottomPWorld => bottomP + (Vector2)transform.position;
		public Vector2 leftPWorld => leftP + (Vector2)transform.position;
		public Vector2 rightPWorld => rightP + (Vector2)transform.position;

		Vector2 topP => top + new Vector2(0, pixelProximity);
        Vector2 bottomP => bottom + new Vector2(0, -pixelProximity);
        Vector2 leftP => left + new Vector2(-2 * pixelProximity, 0);
        Vector2 rightP => right + new Vector2(2 * pixelProximity, 0);

		public PixelBox collisionBody => new PixelBox(top, left, right, bottom);
		public PixelBox collisionBodyWorld => new PixelBox(topWorld, leftWorld, rightWorld, bottomWorld);

		public float navigationMargin {
			get {
				float topLeftSpace = Vector2.Distance(topLeft, center);
				float topRightSpace = Vector2.Distance(topRight, center);
				float bottomLeftSpace = Vector2.Distance(bottomLeft, center);
				float bottomRightSpace = Vector2.Distance(bottomRight, center);
				List<float> spaces = new List<float> { topLeftSpace, topRightSpace, bottomLeftSpace, bottomRightSpace };
				return spaces.Max();
			}
		}

		protected new PolygonCollider2D collider2D;

		Vector2 top, bottom, left, right;
		Vector2[] colliderPoints;
		Color originalColor;
        Color originalObjColor;

		public bool noSorting;
		public bool noCollision;
        public float visibilityInFront = 0.25f;
		public bool inspectChildObjects = false;
		protected int pixelProximity = 4; // 3 pixels away from the object

		public bool withinProximityBox;
		public bool within;

		public Sprite effectorSprite; // Sprite that shows up when you are within the object
		public Sprite originalSprite;

		protected virtual void Awake()
		{
			if (noCollision) return;

			collider2D = GetComponent<PolygonCollider2D>();

			Debug.Assert(collider2D != null);
			Debug.Assert(collider2D.points.Length == 4);
			Debug.Assert(transform.parent.GetComponent<PixelRoom>() == null);
			Debug.Assert(transform.parent.GetComponent<PolygonCollider2D>() == null);

			colliderPoints = collider2D.points;

			top = colliderPoints[0];
			bottom = colliderPoints[0];
			left = colliderPoints[0];
			right = colliderPoints[0];

			for (int i = 0; i < 4; ++i)
			{
				if (colliderPoints[i].y > top.y)
					top = colliderPoints[i];
				if (colliderPoints[i].y < bottom.y)
					bottom = colliderPoints[i];
				if (colliderPoints[i].x < left.x)
					left = colliderPoints[i];
				if (colliderPoints[i].x > right.x)
					right = colliderPoints[i];
			}

			top += collider2D.offset;
			bottom += collider2D.offset;
			left += collider2D.offset;
			right += collider2D.offset;

			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (sr != null)
			    originalSprite = sr.sprite;
		}

		public void TopologicalSortNearbySortingLayers()
		{
			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 23, Vector2.zero);
			List<PixelCollider> pixelColliders = new List<PixelCollider>();

			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (otherPixelCollider.noSorting) continue;
				if (otherPixelCollider.noCollision) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;
				pixelColliders.Add(otherPixelCollider);
			}

			pixelColliders = TopologicalSort(pixelColliders);
			pixelColliders.Reverse();

			Transform previousTransform = null;

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
				if (sr == null) continue;
				sr.sortingOrder = i * 2;

				// Child objects
				SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer co in childobjects)
				{
					if (co == sr) continue;
					co.sortingOrder = i * 2 + 1;
				}

				//if (previousTransform != null && sr.gameObject.transform != previousTransform) {
				//    Debug.DrawLine(previousTransform.position, sr.gameObject.transform.position, Color.red, 10.0f);
				//}

				previousTransform = sr.gameObject.transform;
				//Debug.Log(sr.gameObject.name + " " + sr.sortingOrder.ToString());
			}

			// Place the player in front of colliding objects
			PixelCollider player = pixelColliders.Find((obj) => obj.transform.parent.tag == "Player");
			if (player != null)
			{
				for (int i = 0; i < pixelColliders.Count; ++i)
				{
					int comparison = player.CompareTo(pixelColliders[i]);
					if (pixelColliders[i] == player) continue;
					if (comparison == -1)
					{
						SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
						if (sr == null) continue;
						Color color = sr.color;
						color.a = pixelColliders[i].visibilityInFront;
						sr.color = color;

						// Child objects
						SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer co in childobjects)
						{
							if (co == sr) continue;
							Color cocolor = co.color;
							if (co.name == "Footprint")
							{
								if (co.transform.parent.GetComponent<PixelExterior>() != null)
									cocolor.a = 0.8f;
							}
							else
								cocolor.a = pixelColliders[i].visibilityInFront;
							co.color = cocolor;
						}

						// If the object is a room, hide the insides of the room
						PixelExterior exterior = pixelColliders[i].transform.parent.GetComponent<PixelExterior>();
						if (exterior != null)
							exterior.HideAllRooms();

					}
					else
					{
						SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
						if (sr == null) continue;
						Color color = sr.color;
						color.a = 1;
						sr.color = color;

						// Child objects
						SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer co in childobjects)
						{
							if (co == sr) continue;
							Color cocolor = co.color;
							if (co.name == "Footprint")
							{
								if (co.transform.parent.GetComponent<PixelExterior>() != null)
									cocolor.a = 0.0f;
								else
									cocolor.a = 0.3f;
							}
							else
								cocolor.a = 1;
							co.color = cocolor;
						}

						// If the object is a room, hide the insides of the room
						PixelExterior exterior = pixelColliders[i].transform.parent.GetComponent<PixelExterior>();
						if (exterior != null)
							exterior.ShowAllRooms();
					}
				}
			}
		}

		public List<PixelCollider> TopologicalSort(List<PixelCollider> pixelColliders)
		{

			// Find adjacency list
			List<KeyValuePair<PixelCollider, PixelCollider>> adjacencyList = new List<KeyValuePair<PixelCollider, PixelCollider>>();

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				for (int j = 0; j < pixelColliders.Count; ++j)
				{
					if (i >= j) continue;
					int comparison = pixelColliders[i].CompareTo(pixelColliders[j]);
					if (comparison > 0)
					{
						adjacencyList.Add(new KeyValuePair<PixelCollider, PixelCollider>(pixelColliders[i], pixelColliders[j]));
					}
					else if (comparison < 0)
					{
						adjacencyList.Add(new KeyValuePair<PixelCollider, PixelCollider>(pixelColliders[j], pixelColliders[i]));
					}
				}
			}

			// Kahn's Algorithm
			List<PixelCollider> sortedPixelColliders = new List<PixelCollider>();
			List<PixelCollider> noIncomingEdgeColliders = new List<PixelCollider>();

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				if (!adjacencyList.Any((x) => x.Value == pixelColliders[i]))
					noIncomingEdgeColliders.Add(pixelColliders[i]);
			}

			while (noIncomingEdgeColliders.Count > 0)
			{
				PixelCollider first = noIncomingEdgeColliders[0];
				noIncomingEdgeColliders.Remove(first);
				sortedPixelColliders.Add(first);

				List<KeyValuePair<PixelCollider, PixelCollider>> objs = adjacencyList.Where((x) => x.Key == first).ToList();
				for (int i = 0; i < objs.Count(); ++i)
				{
					adjacencyList.Remove(objs[i]);
					if (!adjacencyList.Any((x) => x.Value == objs[i].Value))
					{
						noIncomingEdgeColliders.Add(objs[i].Value);
					}
				}
			}

			return sortedPixelColliders;
		}

		// Finds nearest other pixel collider, same as check for collision, but returns a list of objects instead
		public List<PixelCollision> CheckForInspection()
		{
			List<PixelCollision> pixelCollisions = new List<PixelCollision>();

			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.noCollision) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;
				if (otherPixelCollider is MultiBodyPixelCollider) continue;
				if (otherPixelCollider.transform.parent.name == "VirtualObject") continue;

				Transform otherTransform = otherPixelCollider.gameObject.transform;

				Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

				Vector2 othertopWorld = otherPixelCollider.topWorld;
				Vector2 otherbottomWorld = otherPixelCollider.bottomWorld;
				Vector2 otherleftWorld = otherPixelCollider.leftWorld;
				Vector2 otherrightWorld = otherPixelCollider.rightWorld;

				Debug.DrawLine(othertopWorld, otherleftWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherleftWorld, otherbottomWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherbottomWorld, otherrightWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherrightWorld, othertopWorld, Color.yellow, 1.0f);
                
				Direction direction = Direction.All;
				List<PixelCollider> pixelColliders = new List<PixelCollider>();

				if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NW, 5.0f))
				{
					direction = Direction.NW;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NE, 5.0f))
				{
					direction = Direction.NE;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SW, 5.0f))
				{
					direction = Direction.SW;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SE, 5.0f))
				{
					direction = Direction.SE;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}

				foreach (PixelCollider pc in pixelColliders)
				{
					PixelCollision pixelCollision;
					pixelCollision.direction = direction;
					pixelCollision.pixelCollider = pc;
					pixelCollisions.Add(pixelCollision);               
				}
			}

			return pixelCollisions;
		}

		public virtual MovementRestriction CheckForCollision()
		{
			Debug.Assert(!(this is MultiBodyPixelCollider));

			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 10.0f, Vector2.zero);

			MovementRestriction restriction = new MovementRestriction();
            
			// Collided with floor
            PixelRoom floor = transform.parent.parent.GetComponent<PixelRoom>();
            Debug.Assert(floor != null);
            Debug.Assert(floor.colliderPoints.Length == 4);

            PixelBoxComparison cbc = PixelBox.CompareTwoCollisionBodies(collisionBodyWorld, floor.collisionbodyWorld, -2.0f);
			if (!cbc.SEinside) restriction.restrictSE = true;
			if (!cbc.SWinside) restriction.restrictSW = true;
			if (!cbc.NEinside) restriction.restrictNE = true;
			if (!cbc.NWinside) restriction.restrictNW = true;

			// Collided with other object
			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.noCollision) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;

				Transform otherTransform = otherPixelCollider.gameObject.transform;

				if (otherPixelCollider is MultiBodyPixelCollider)
				{
					MultiBodyPixelCollider multi = otherPixelCollider as MultiBodyPixelCollider;
					foreach (PixelBox cbody in multi.collisionBodiesWorld)
                    {
      					cbody.Draw(Color.white, 1.0f);
						if (collisionBodyWorld.WithinRange(cbody, Direction.NW, 0.4f))
							restriction.restrictNW = true;
						if (collisionBodyWorld.WithinRange(cbody, Direction.NE, 0.4f))
							restriction.restrictNE = true;
						if (collisionBodyWorld.WithinRange(cbody, Direction.SW, 0.4f))
							restriction.restrictSW = true;
						if (collisionBodyWorld.WithinRange(cbody, Direction.SE, 0.4f))
							restriction.restrictSE = true;
						
                    }               
				}
				else if (otherPixelCollider is RampCollider)
				{
					Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);
					RampCollider rampCollider = (RampCollider)otherPixelCollider;
					otherPixelCollider.collisionBodyWorld.Draw(Color.white, 1.0f);

					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NW, 0.4f))
                        restriction.restrictNW = true;
                    if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NE, 0.4f))
                        restriction.restrictNE = true;
                    if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SW, 0.4f))
                        restriction.restrictSW = true;
                    if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SE, 0.4f))
                        restriction.restrictSE = true;

					// Comparison with actual box
					PixelBoxComparison bodyComparison = PixelBox.CompareTwoCollisionBodies(collisionBodyWorld, rampCollider.collisionBodyWorld, 0.0f, true);
					PixelBoxComparison proximityComparison = PixelBox.CompareTwoCollisionBodies(collisionBodyWorld, rampCollider.proximityBodyWorld, 0.0f, true);
                    
					// Close to the ramp collision (remove the wall collision) and prevent glitches
					Direction proximityDirection = Direction.All;

					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SE, navigationMargin, navigationMargin) &&
					    rampCollider.rampDirection == Direction.SE) proximityDirection = Direction.SE;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SW, navigationMargin, navigationMargin) &&
					    rampCollider.rampDirection == Direction.SW) proximityDirection = Direction.SW;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NE, navigationMargin, navigationMargin) &&
					    rampCollider.rampDirection == Direction.NE) proximityDirection = Direction.NE;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NW, navigationMargin, navigationMargin) && 
					    rampCollider.rampDirection == Direction.NW) proximityDirection = Direction.NW;

					// Entered the ramp
					bool closeToRamp = proximityDirection != Direction.All;

                    // Free the character to go in that direction
					if (proximityComparison.inside) {
                        if (rampCollider.rampDirection == Direction.NE)
							restriction.restrictNE = false;
						else if (rampCollider.rampDirection == Direction.NW)
                            restriction.restrictNW = false;
						else if (rampCollider.rampDirection == Direction.SE)
                            restriction.restrictSE = false;
						else if (rampCollider.rampDirection == Direction.SW)
                            restriction.restrictSW = false;
					}

					// Check if entered ramp
					withinProximityBox = rampCollider.proximityBodyWorld.WithinCollisionBody(transform.position);
					if (withinProximityBox) {
						within = rampCollider.collisionBodyWorld.WithinCollisionBody(transform.position);

						if (bodyComparison.overlap)
						{
							// Remove glitch caused by going to the side of the walls
							if (rampCollider.rampDirection == Direction.NE || rampCollider.rampDirection == Direction.SW)
							{
								if (!proximityComparison.NWinside)
									restriction.restrictNW = true;
								if (!proximityComparison.SEinside)
									restriction.restrictSE = true;
							}
							else if (rampCollider.rampDirection == Direction.NW || rampCollider.rampDirection == Direction.SE)
							{
								if (!proximityComparison.NEinside)
									restriction.restrictNE = true;
								if (!proximityComparison.SWinside)
									restriction.restrictSW = true;
							}
						}

						SpriteRenderer sr = otherPixelCollider.transform.parent.GetComponent<SpriteRenderer>();
						Debug.Log(sr.name);
						if (otherPixelCollider.effectorSprite != null && sr != null)
                        {
							if (within)
							{
								sr.sprite = otherPixelCollider.effectorSprite;
								rampCollider.OnEffectorEnter();
							}
							else
							{
								sr.sprite = otherPixelCollider.originalSprite;
								rampCollider.OnEffectorExit();
							}
                        }

					}
     
					if (within) {

                        // Draw the ramps
                        rampCollider.collisionBodyRampedWorld.Draw(Color.magenta, 2.0f);
                        PixelBox cramped = rampCollider.MatchCollisionBody(collisionBodyWorld);
                        cramped.Draw(Color.magenta, 2.0f);

                        // Ramp Collision mechanics
                        restriction.restrictNE = false;
                        restriction.restrictNW = false;
                        restriction.restrictSE = false;
                        restriction.restrictSW = false;
                        
                        // Collision with side of ramp
                        PixelBoxComparison cbcRamp = PixelBox.CompareTwoCollisionBodies(cramped, rampCollider.collisionBodyRampedWorld, -2.0f);
						if (!cbcRamp.SEinside && rampCollider.rampDirection != Direction.SE && rampCollider.rampDirection != Direction.NW) restriction.restrictSE = true;
						if (!cbcRamp.SWinside && rampCollider.rampDirection != Direction.SW && rampCollider.rampDirection != Direction.NE) restriction.restrictSW = true;
						if (!cbcRamp.NEinside && rampCollider.rampDirection != Direction.NE && rampCollider.rampDirection != Direction.SW) restriction.restrictNE = true;
						if (!cbcRamp.NWinside && rampCollider.rampDirection != Direction.NW && rampCollider.rampDirection != Direction.SE) restriction.restrictNW = true;

                        restriction.slopeDirection = rampCollider.rampDirection;
                        restriction.slope = rampCollider.slope;

                        // Check for room entry
                        PixelStair pixelStair = otherPixelCollider.transform.parent.GetComponent<PixelStair>();
                        if (pixelStair != null)
                        {
                            bool above = rampCollider.OnFarSide(cramped);
                            if (above)
                            {
                                restriction.enteredDoor = pixelStair;
                            }
                        }
                    }
				}
				else
				{
					otherPixelCollider.collisionBodyWorld.Draw(Color.white, 1.0f);

					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NW, 0.4f))
						restriction.restrictNW = true;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.NE, 0.4f))
						restriction.restrictNE = true;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SW, 0.4f))
						restriction.restrictSW = true;
					if (collisionBodyWorld.WithinRange(otherPixelCollider.collisionBodyWorld, Direction.SE, 0.4f))
						restriction.restrictSE = true;
				}
			}
         
			return restriction;
		}

		public bool CheckForEffector()
		{
			Debug.Assert(!(this is MultiBodyPixelCollider));
			return false;
		}

		public bool CheckForWithinCollider(Vector2 position, float margin = 0.0f)
		{
			if (this is MultiBodyPixelCollider && (this as MultiBodyPixelCollider).collisionBodies.Count() != 0)
			{
				MultiBodyPixelCollider multibody = (this as MultiBodyPixelCollider);

				foreach (PixelBox body in multibody.collisionBodiesWorld)
				{
					bool withinBody = body.WithinCollisionBody(position, margin);
					if (withinBody) return true;
				}
				return false;
			}
			else
			{
				if (this.colliderPoints.Length != 4)
				{
					Debug.Log(this.transform.parent.name);
				}
				Debug.Assert(this.colliderPoints.Length == 4);

				bool withinBody = collisionBodyWorld.WithinCollisionBody(position, margin);
				return withinBody;
			}
		}
              
		// Returns 1 if in front of the other, returns 1 if object is in front of other
		public int CompareTo(PixelCollider other)
		{
			int comparison = 0;

			if (other == this)
				return 0;

			if (other is MultiBodyPixelCollider && this is MultiBodyPixelCollider)
			{
				MultiBodyPixelCollider a = this as MultiBodyPixelCollider;
				MultiBodyPixelCollider b = other as MultiBodyPixelCollider;

				Debug.Assert(a.collisionBodiesWorld.Count() > 0);
				Debug.Assert(b.collisionBodiesWorld.Count() > 0);

				// Find the front most box of all the boxes

				// TODO: Top sort for more complicated buildings
				for (int i = 0; i < a.collisionBodies.Count(); ++i)
				{
					for (int j = 0; j < b.collisionBodies.Count(); ++j)
					{
						int comp = PixelBox.CompareTwoCollisionBodies(a.collisionBodiesWorld[i], b.collisionBodiesWorld[j], 2.0f).inFront;
						if (comp == 1)
							comparison = 1;
						if (comp == -1)
							comparison = -1;
					}
				}
			}
			else if (other is MultiBodyPixelCollider || this is MultiBodyPixelCollider)
			{
				MultiBodyPixelCollider multi;
				PixelCollider single;
				if (other is MultiBodyPixelCollider)
				{
					multi = other as MultiBodyPixelCollider;
					single = this;
				}
				else
				{
					multi = this as MultiBodyPixelCollider;
					single = other;
				}

				PixelBox singleBody = single.collisionBodyWorld;

				// If any of the multi are in front of the single, multi wins
				int multiInFront = 0;
				for (int i = 0; i < multi.collisionBodiesWorld.Count(); ++i)
				{
					int comp = PixelBox.CompareTwoCollisionBodies(multi.collisionBodiesWorld[i], singleBody, 2.0f).inFront;
					if (comp == 1) multiInFront = 1;
					if (comp == -1) multiInFront = -1;
				}

				if (single != this)
				{
					comparison = multiInFront;
				}
				else
				{
					if (multiInFront == 1)
						comparison = -1;
					if (multiInFront == -1)
						comparison = 1;
				}
			}
			else if (other is RampCollider && transform.parent.GetComponent<Character>() != null || this is RampCollider && other.transform.parent.GetComponent<Character>() != null)
			{
				RampCollider rampCollider = (RampCollider)((other is RampCollider) ? other : this);
				PixelCollider characterCollider = (other is RampCollider) ? this : other;
				PixelBoxComparison bodyComparision = PixelBox.CompareTwoCollisionBodies(characterCollider.collisionBodyWorld, rampCollider.collisionBodyWorld, 0.0f);

				if (characterCollider.within || characterCollider.withinProximityBox)
				{
					if (other is RampCollider)
						comparison = 1;
					else
						comparison = -1;
				}
				else {
					Debug.Assert(this.colliderPoints.Length == 4);
                    Debug.Assert(other.colliderPoints.Length == 4);
                    
                    PixelBox a = collisionBodyWorld;
                    PixelBox b = other.collisionBodyWorld;

					comparison = PixelBox.CompareTwoCollisionBodies(a, b, 10.0f).inFront;
				}
			}
			else
			{
				Debug.Assert(this.colliderPoints.Length == 4);
				Debug.Assert(other.colliderPoints.Length == 4);

				PixelBox a = collisionBodyWorld;
				PixelBox b = other.collisionBodyWorld;

				comparison = PixelBox.CompareTwoCollisionBodies(a, b, 10.0f).inFront;
			}
			return comparison;
		}

		public bool ParentIsContainer()
		{
			for (int i = 0; i < transform.parent.parent.childCount; ++i)
			{
				Transform t = transform.parent.parent.GetChild(i);
				if (t.GetComponent<PixelCollider>() == true && !t.GetComponent<PixelCollider>().inspectChildObjects)
					return true;
			}
			return false;
		}

		public List<PixelCollider> GetChildColliders()
		{
			List<PixelCollider> pixelColliders = new List<PixelCollider>();

			for (int i = 0; i < transform.parent.childCount; ++i)
			{
				Transform t = transform.parent.GetChild(i);
				for (int j = 0; j < t.childCount; ++j)
				{
					Transform t2 = t.GetChild(j);
					PixelCollider pc = t2.GetComponent<PixelCollider>();
					if (pc != null)
					{
						pixelColliders.Add(pc);
					}
				}
			}

			return pixelColliders;
		}

		public List<SpriteRenderer> GetChildSpriteRenderers()
		{
			List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

			for (int i = 0; i < transform.parent.childCount; ++i)
			{
				Transform t = transform.parent.GetChild(i);
				for (int j = 0; j < t.childCount; ++j)
				{
					Transform t2 = t.GetChild(j);
					SpriteRenderer pc = t2.GetComponent<SpriteRenderer>();
					if (pc != null)
					{
						spriteRenderers.Add(pc);
					}
				}
			}

			return spriteRenderers;
		}

		bool OtherPixelColliderSameParent(PixelCollider pc)
		{
			return pc.GetPixelRoom() == this.GetPixelRoom();
		}

		public PixelRoom GetPixelRoom()
		{
			PixelRoom pixelRoom = transform.parent.parent.GetComponent<PixelRoom>();
			if (pixelRoom == null)
				pixelRoom = transform.parent.GetComponent<PixelRoom>();
			if (pixelRoom == null)
			{
				Debug.Assert(transform.parent.parent.parent != null);
				pixelRoom = transform.parent.parent.parent.GetComponent<PixelRoom>();
			}
			return pixelRoom;
		}

		public void HighlightObject()
		{
			SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
				Color color = new Color(255f, 255f, 255f, 0.2f);
				spriteRenderer.color = color;
			}
			SpriteRenderer objRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
			if (objRenderer != null)
			{
				originalObjColor = objRenderer.color;
				Color color = new Color(255f, 255f, 0, 1.0f);
				objRenderer.color = color;
			}
		}

		public void UnHighlightObject()
		{
			SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
				spriteRenderer.color = originalColor;

			SpriteRenderer objRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
			if (objRenderer != null)
			{
				objRenderer.color = originalObjColor;
			}
		}

		public WayPoint FindWayPointInDirection(Direction direction)
		{
			PixelRoom pixelRoom = GetPixelRoom();         
			HashSet<WayPoint> navigationMesh = pixelRoom.GetNavigationalMesh();

			Debug.Assert(navigationMesh.Count != 0);
			if (direction == Direction.NE)
			{
				WayPoint closestNE = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, topRightWorld) < Vector2.Distance(arg2.position, topRightWorld) ? arg1 : arg2);
				return closestNE;
			}
			else if (direction == Direction.NW)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, topLeftWorld) < Vector2.Distance(arg2.position, topLeftWorld) ? arg1 : arg2);
				return closestNW;
			}
			else if (direction == Direction.SE)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, bottomRightWorld) < Vector2.Distance(arg2.position, bottomRightWorld) ? arg1 : arg2);
				return closestNW;
			}
			else if (direction == Direction.SW)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, bottomLeftWorld) < Vector2.Distance(arg2.position, bottomLeftWorld) ? arg1 : arg2);
				return closestNW;
			}
			else
			{
				return null;
			}
		}

		public KeyValuePair<PixelPose, float> FindBestWayPoint()
		{
			WayPoint NE = FindWayPointInDirection(Direction.NE);
			WayPoint NW = FindWayPointInDirection(Direction.NW);
			WayPoint SE = FindWayPointInDirection(Direction.SE);
			WayPoint SW = FindWayPointInDirection(Direction.SW);

			Debug.DrawLine(topRightWorld, NE.position, Color.white, 3.0f);
			Debug.DrawLine(topLeftWorld, NW.position, Color.black, 3.0f);
			Debug.DrawLine(bottomRightWorld, SE.position, Color.blue, 3.0f);
			Debug.DrawLine(bottomLeftWorld, SW.position, Color.cyan, 3.0f);

			float deltaNE = Vector2.Distance(NE.position, topRightWorld);
			float deltaNW = Vector2.Distance(NW.position, topLeftWorld);
			float deltaSE = Vector2.Distance(SE.position, bottomRightWorld);
			float deltaSW = Vector2.Distance(SW.position, bottomLeftWorld);

			Dictionary<PixelPose, float> dirDic = new Dictionary<PixelPose, float>();
			PixelPose NEpose = new PixelPose(GetPixelRoom(), Direction.SW, NE.position);
			PixelPose NWpose = new PixelPose(GetPixelRoom(), Direction.SE, NW.position);
			PixelPose SEpose = new PixelPose(GetPixelRoom(), Direction.NW, SE.position);
			PixelPose SWpose = new PixelPose(GetPixelRoom(), Direction.NE, SW.position);
			dirDic.Add(NEpose, deltaNE);
			dirDic.Add(NWpose, deltaNW);
			dirDic.Add(SEpose, deltaSE);
			dirDic.Add(SWpose, deltaSW);

			return dirDic.Aggregate((arg1, arg2) => arg1.Value < arg2.Value ? arg1 : arg2);
		}

		public PixelPose FindBestWayPointPosition(Vector2 starting)
		{
			WayPoint NE = FindWayPointInDirection(Direction.NE);
			WayPoint NW = FindWayPointInDirection(Direction.NW);
			WayPoint SE = FindWayPointInDirection(Direction.SE);
			WayPoint SW = FindWayPointInDirection(Direction.SW);

			List<PixelPose> dirList = new List<PixelPose>();
			PixelPose NEpose = new PixelPose(GetPixelRoom(), Direction.SW, NE.position);
			PixelPose NWpose = new PixelPose(GetPixelRoom(), Direction.SE, NW.position);
			PixelPose SEpose = new PixelPose(GetPixelRoom(), Direction.NW, SE.position);
			PixelPose SWpose = new PixelPose(GetPixelRoom(), Direction.NE, SW.position);
			dirList.Add(NEpose);
			dirList.Add(NWpose);
			dirList.Add(SEpose);
			dirList.Add(SWpose);

			return dirList.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, starting) < Vector2.Distance(arg2.position, starting) ? arg1 : arg2);
		}

		public virtual void OnDrawGizmos()
        {
            Gizmos.DrawSphere(topRightWorld, 0.3f);
            Gizmos.DrawSphere(topLeftWorld, 0.3f);
			Gizmos.DrawSphere(bottomRightWorld, 0.3f);
			Gizmos.DrawSphere(bottomLeftWorld, 0.3f);
			Gizmos.DrawSphere(centerWorld, 0.3f);
			Gizmos.DrawSphere(topWorld, 0.3f);
			Gizmos.DrawSphere(bottomWorld, 0.3f);
			Gizmos.DrawSphere(leftWorld, 0.3f);
			Gizmos.DrawSphere(rightWorld, 0.3f);
        }
	}
}