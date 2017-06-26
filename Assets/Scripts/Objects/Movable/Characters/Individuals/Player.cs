using UnityEngine;
using System;
using System.Collections;
using Environment;
using UI;
using UI.Panels;
using UI.Screens;
using UI.Dialogues;
using Objects.Immovable;

namespace Objects.Movable.Characters.Individuals
{
	public sealed class Player : Character
    {
		Vector2 inputVelocity
		{
			get {
				return new Vector2(
					movementSpeed * Input.GetAxisRaw("Horizontal") * 2.0f,
					movementSpeed * Input.GetAxisRaw("Vertical"));
			}
		}
		UIScreens UI {
			get { return Game.main.UI; }
		}

		[HideInInspector]
		public bool isInsideBuilding;
		const float triggerInspectionThreshold = 25.0f;

		protected override void Start()
        {
            base.Start();
            TeleportCameraToPlayer();
			InvokeRepeating("DialogueNearestObject", 0.0f, 0.1f);
        }

		protected override void Update()
		{
			// Movement
			rigidbody2D.velocity = positionLocked ? Vector2.zero : inputVelocity;
			AnimateMovement();

			// Keyboard
			KeyPressed();

			base.Update();
		}

		void KeyPressed() {

			// Key Maps for Inventory
			if (Input.GetButtonDown ("Inventory")) {
				if(!Game.main.UI.currentPanelStack.Contains(UI.Find<DarkScreen>()))
					UI.Find<InventoryPanel>().TurnOn();
				else
					UI.Find<InventoryPanel>().TurnOff();
			}
			if (Input.GetButtonDown ("Inspect")) 
				Inspect();
			if (Input.GetKeyDown(KeyCode.Alpha1))
				KeyPressed(1);
			if (Input.GetKeyDown(KeyCode.Alpha2))
				KeyPressed(2);
			if (Input.GetKeyDown(KeyCode.Alpha3))
				KeyPressed(3);
			if (Input.GetKeyDown(KeyCode.Alpha4))
				KeyPressed(4);
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

		protected override void OnTriggerStay2D (Collider2D collider)
		{
			base.OnTriggerStay2D (collider);
		}

		void DialogueNearestObject() {
			var nearestInspectable = FindNearestObject<IInspectable>();
			if(nearestInspectable != null) {
//				Debug.Log(nearestInspectable.name);
				float distanceToInspectable = triggerInspectionThreshold;
				var imobj = nearestInspectable.GetComponent<ImmovableObject>();
				if(imobj != null)
					distanceToInspectable = Vector2.Distance(imobj.colliderCenter, transform.position);
				else 
					distanceToInspectable = Vector2.Distance(nearestInspectable.transform.position, transform.position);

				if(distanceToInspectable < triggerInspectionThreshold) {
					UI.Find<GameDialogue>().TurnOn();
					UI.Find<GameDialogue>().brightness = (triggerInspectionThreshold - distanceToInspectable) / triggerInspectionThreshold;
					UI.Find<GameDialogue>().text = nearestInspectable.interactionText;
				}
			}
			else {
				UI.Find<GameDialogue>().TurnOff();
			}
		}
    }
}