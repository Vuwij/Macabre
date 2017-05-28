using UnityEngine;
using System;
using System.Collections;
using Environment;

namespace Objects.Movable.Characters.Individuals
{
	public sealed class Player : Character
    {
		new bool isRunning
		{
			get { return Input.GetButton("SpeedUp"); }
		}

		protected override void Start()
        {
            base.Start();
            TeleportCameraToPlayer();
        }

		void Update()
		{
			// Set the movement
			rigidbody2D.velocity = movementLocked ? Vector2.zero : movementVelocity;

			AnimateMovement();
		}

		void TeleportCameraToPlayer()
		{
			var main = Camera.main;
			if(!main) return;

			Vector3 newPosition = new Vector3(
				transform.position.x,
				transform.position.y,
				-10);
			main.transform.position = newPosition;
		}
		protected Vector2 movementVelocity
		{
			get {
				return new Vector2(
					movementSpeed * Input.GetAxisRaw("Horizontal") * 2.0f,
					movementSpeed * Input.GetAxisRaw("Vertical"));
			}
		}
    }
}