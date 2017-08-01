using System;
using UnityEngine;
using System.Collections.Generic;

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
			backAndForth,
			multiplePoints,
			random
		}
		public Vector2[] predefinedMovementLocations;
		public Queue<Vector2> movementPath = new Queue<Vector2>();
		MovementState movementState = MovementState.idle;
		IdleMovementType movementType = IdleMovementType.random;

		void Start() {
			base.Update();
			InvokeRepeating("DecideMovement", 0.0f, 1.0f);
		}

		void Update() {
			base.Update();
		}

		void DecideMovement() {
			if(movementState == MovementState.frozen) {
				return;
			}
			else if (movementState == MovementState.idle) {
				if(movementType == IdleMovementType.backAndForth) {

				}
				else if(movementType == IdleMovementType.multiplePoints) {

				}
				else if(movementType == IdleMovementType.random) {
					if(movementPath.Count < 5) {

					}
				}
			}
			else if (movementState == MovementState.quest) {

			}
		}
	}
}

