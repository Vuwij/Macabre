using UnityEngine;
using System;
using System.Collections;
using Environment;
using UI;
using UI.Panels;
using UI.Screens;
using UI.Dialogues;
using Objects.Immovable;
using Objects.Immovable.Items;

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
		Objects.Object pendingInspection;
		const float triggerInspectionThreshold = 15.0f;

		protected override void Start()
        {
            base.Start();
            TeleportCameraToPlayer();
			InvokeRepeating("DialogueNearestObject", 0.0f, 0.1f);
			InvokeRepeating("HoverOverObject", 0.0f, 0.1f);
        }

		protected override void Update()
		{
			// Movement
			if(destinationPosition != null) {
				float distanceToStop = collisionbox.radius;
				if(pendingInspection is Item) 
					distanceToStop = 1.0f;
				
				if(Vector2.Distance((Vector2) destinationPosition, (Vector2) transform.position) < distanceToStop) {

					// Inspectable object
					if(pendingInspection is IInspectable) {
						inspectedObject = pendingInspection as IInspectable;
						(pendingInspection as IInspectable).InspectionAction(this);
						pendingInspection = null;
					}
					destinationPosition = null;
				}
				//Debug.DrawLine(transform.position, (Vector3) destinationPosition, Color.red, 10.0f);
				var direction = (Vector3) destinationPosition - transform.position;
				var directionN = Vector3.Normalize(direction);
				rigidbody2D.velocity = positionLocked ? Vector2.zero : (Vector2) directionN * walkingSpeed;
			}
			else rigidbody2D.velocity = positionLocked ? Vector2.zero : inputVelocity;

			AnimateMovement();

			// Keyboard
			KeyPressed();

			// Mouse Click
			MouseClicked();

			base.Update();
		}

		public override void KeyPressed(int selection = -1) {

			if(Input.anyKey) {
				//destinationPosition = null;

				// Key Maps for Inventory
				if (Input.GetButtonDown ("Inventory")) {
					if(!Game.main.UI.currentPanelStack.Contains(UI.Find<DarkScreen>()))
						UI.Find<InventoryPanel>().TurnOn();
					else
						UI.Find<InventoryPanel>().TurnOff();
				}
				if (Input.GetButtonDown ("Inspect")) 
					Inspect();
			}
			base.KeyPressed(selection);
		}

		void MouseClicked() {
			if (Input.GetMouseButtonDown(0)) {
				// TODO ignore UI clicks
				if(isSittingDown) {
					isSittingDown = false;
				}

				// Detect if object is nearby
				var obj = FindInspectablePixelAroundPosition(mousePosition);
				Vector2 hitposition = FindHitFromRaycast(mousePosition);

				if(!positionLocked) {
					if(hitposition != Vector2.zero) {
						// TODO Slow down character movement
						destinationPosition = hitposition;
					}
					else destinationPosition = mousePosition;

					if(obj != null) { // Walk to the object and then interact
						pendingInspection = obj;
					}
					else { // Simply walk to the destination
						pendingInspection = null;
					}
				}
				else { // Direct interaction with the object
					if(obj != null)
						(obj as IInspectable).InspectionAction(this);
				}
			}
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
			var nearestInspectable = FindNearestObject<IInspectable>(triggerInspectionThreshold);
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

		void HoverOverObject() {
			var obj = FindInspectablePixelAroundPosition(mousePosition);
			if(obj != null) {
				if(obj is IInspectable) {
					obj.ShowHoverText();
				}
			}
		}
    }
}