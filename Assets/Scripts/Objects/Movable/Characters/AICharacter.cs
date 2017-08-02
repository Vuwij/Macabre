using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Objects.Movable.Characters
{
	public class AICharacter : Character
	{
		public enum MovementState {
			frozen, // Not moving, staying in a spot
			idle, 	// Moving according to Random MovementType
			quest	// Navigation to a destination
		}

		public enum IdleMovementType {
			multiplePoints,
			random,
		}

		public const float maxDistNextPath = 100.0f;
		public Vector2[] predefinedMovementLocations;
		public Queue<Vector2> movementPath = new Queue<Vector2>();
		public List<Vector2> setMovementPath = new List<Vector2>();
		int setMovementPathIndex = 0;
		MovementState movementState = MovementState.idle;
		IdleMovementType movementType = IdleMovementType.random;


		protected virtual void Start() {
			base.Start();
			InvokeRepeating("DecideMovement", 0.0f, 1.0f);
			movementPath.Enqueue(transform.position);
		}

		void Update() {
			base.Update();

			if(destinationPosition != null) {
//				Debug.Log(collisionbox);
				float distanceToStop = collisionbox.radius;
//				if(pendingInspection is Item) 
//					distanceToStop = 1.0f;
//
				if(Vector2.Distance((Vector2) destinationPosition, (Vector2) transform.position) < distanceToStop) {

					// Inspectable object
//					if(pendingInspection is IInspectable) {
//						inspectedObject = pendingInspection as IInspectable;
//						(pendingInspection as IInspectable).InspectionAction(this);
//						pendingInspection = null;
//					}
					destinationPosition = null;
				}
				//Debug.DrawLine(transform.position, (Vector3) destinationPosition, Color.red, 10.0f);
				var direction = (Vector3) destinationPosition - transform.position;
				var directionN = Vector3.Normalize(direction);
				rigidbody2D.velocity = positionLocked ? Vector2.zero : (Vector2) directionN * walkingSpeed;
			} else {
				if(movementState == MovementState.idle) {
					if(movementPath.Count != 0) {
						destinationPosition = movementPath.Dequeue();
//						Debug.Log(destinationPosition);
					}
				}
			}

			AnimateMovement();
		}

		void DecideMovement() {
			if(movementState == MovementState.frozen) {
				return;
			}
			else if (movementState == MovementState.idle) {
				if(movementType == IdleMovementType.multiplePoints) {
					if(movementPath.Count < 5) {
						movementPath.Enqueue(setMovementPath[setMovementPathIndex]);
						if(setMovementPathIndex == movementPath.Count -1)
							setMovementPathIndex = 0;
						else
							setMovementPathIndex++;
					}
				}
				else if(movementType == IdleMovementType.random) {
					if(movementPath.Count < 5) {
						Vector2 lastPath;
						if(movementPath.Count == 0)
							lastPath = transform.position;
						else
							lastPath = movementPath.Last();

						var vPaths = predefinedMovementLocations.Where(x => Vector2.Distance(lastPath, x) < maxDistNextPath);
						int c = vPaths.Count();
						Debug.Log(c);
						var randirection = UnityEngine.Random.insideUnitCircle;
						Vector2 decidedPath = new Vector2();
						float closestAngle = float.PositiveInfinity;
						foreach(var path in vPaths) {
							var angle = Vector2.Angle(path,randirection); 
							if(closestAngle >= Math.Abs(angle)) {
								closestAngle = Math.Abs(angle);
								decidedPath = path;
							}
						}
						// Walk to however far it can get
						var limitedPath = FindHitFromRaycast(decidedPath);
						if(decidedPath != Vector2.zero)
							movementPath.Enqueue(limitedPath);
					}
				}
			}
			else if (movementState == MovementState.quest) {

			}
		}
	}
}

