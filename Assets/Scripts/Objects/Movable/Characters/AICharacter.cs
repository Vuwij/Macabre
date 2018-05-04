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
			randomGlobal,
		}

		const float maxDistNextPath = 100.0f;
		public Vector2[] predefinedMovementLocations;
		protected static Vector2[] predefinedMovementLocationsGlobal {
			get {
				List<Vector2> allLocations = new List<Vector2>();
				var allAI = GameObject.FindObjectsOfType<AICharacter>();
				foreach(var ai in allAI) {
					allLocations.AddRange(ai.predefinedMovementLocations.ToList());
				}
				return allLocations.ToArray();
			}
		}
		protected Queue<Vector2> movementPath = new Queue<Vector2>();
		public List<Vector2> setMovementPath = new List<Vector2>();
		int setMovementPathIndex = 0;
		public MovementState movementState = MovementState.idle;
		public IdleMovementType movementType = IdleMovementType.randomGlobal;
		public float idleMovementDelay = 1.0f;
		float? waitTime;

        protected override void Start() {
			base.Start();
			InvokeRepeating("DecideMovement", 0.0f, 1.0f);
			movementPath.Enqueue(transform.position);
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
				else if(movementType == IdleMovementType.random || movementType == IdleMovementType.randomGlobal) {
					if(movementPath.Count < 5) {
						Vector2 lastPath;
						if(movementPath.Count == 0)
							lastPath = transform.position;
						else
							lastPath = movementPath.Last();


						IEnumerable<Vector2> vPaths;
						if(movementType == IdleMovementType.random)
							vPaths = predefinedMovementLocations.Where(x => Vector2.Distance(lastPath, x) < maxDistNextPath);
						else if(movementType == IdleMovementType.randomGlobal)
							vPaths = predefinedMovementLocationsGlobal.Where(x => Vector2.Distance(lastPath, x) < maxDistNextPath);
						else throw new Exception("Error, invalid combination");

						int c = vPaths.Count();

						if(c != 0) {
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
							movementPath.Enqueue(limitedPath);
						}
					}
				}
			}
			else if (movementState == MovementState.quest) {

			}
		}
	}
}

