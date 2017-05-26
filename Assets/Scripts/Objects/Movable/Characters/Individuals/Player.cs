using UnityEngine;
using System;
using System.Collections;
using Environment;

namespace Objects.Movable.Characters.Individuals
{
	public sealed class Player : Character
    {
        protected override void Start()
        {
            base.Start();
            TeleportCameraToPlayer();
        }

		private void TeleportCameraToPlayer()
		{
			var main = Camera.main;
			if(!main) return;

			Vector3 newPosition = new Vector3(
				transform.position.x,
				transform.position.y,
				-10);
			main.transform.position = newPosition;
		}

        void Update()
        {
            if (keyboardMovement) moveUsingKeyboard();

            AnimateMovement();
        }

		#region Inventory

		new bool isRunning
		{
			get { return Input.GetButton("SpeedUp"); }
		}

		protected override Vector2 movementVelocity
		{
			get {
				return new Vector2(
					movementSpeed * Input.GetAxisRaw("Horizontal") * 2.0f,
					movementSpeed * Input.GetAxisRaw("Vertical"));
			}
		}

		void moveUsingKeyboard()
		{
//			rb2D.velocity = lockMovement ? Vector2.zero : movementVelocity;
		}

		#endregion

    }
}