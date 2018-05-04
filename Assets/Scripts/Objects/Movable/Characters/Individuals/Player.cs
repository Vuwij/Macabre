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
                PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
                PixelCollider.MovementRestriction mr;
                mr.restrictNE = false;
                mr.restrictNW = false;
                mr.restrictSE = false;
                mr.restrictSW = false;

                if (pixelCollider != null)
                    mr = pixelCollider.CheckForCollision();

                //Debug.Log("NE: " + mr.restrictNE + " NW: " + mr.restrictNW + " SE: " + mr.restrictSE + " SW: " + mr.restrictSW);

                if (Input.GetAxisRaw("Horizontal") > 0 && !mr.restrictNE)
                    return new Vector2(2, 1);
                else if (Input.GetAxisRaw("Horizontal") < 0 && !mr.restrictSW)
                    return new Vector2(-2, -1);
                else if (Input.GetAxisRaw("Vertical") > 0 && !mr.restrictNW)
                    return new Vector2(-2, 1);
                else if (Input.GetAxisRaw("Vertical") < -0 && !mr.restrictSE)
                    return new Vector2(2, -1);
                return Vector2.zero;
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
            InvokeRepeating("Movement", 0.0f, 1.0f/movementSpeed);
        }

        void Movement() {

            if (inputVelocity != Vector2.zero && !positionLocked)
            {
                Vector2 pos = transform.position;
                pos.x = pos.x + inputVelocity.x;
                pos.y = pos.y + inputVelocity.y;
                transform.position = pos;
                UpdateSortingLayer();
            }
            characterVelocity = inputVelocity;
            AnimateMovement();
        }

		protected override void Update()
		{
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
			//if (Input.GetMouseButtonDown(0)) {
			//	// TODO ignore UI clicks
			//	if(isSittingDown) {
			//		isSittingDown = false;
			//	}

			//	// Detect if object is nearby
			//	var obj = FindInspectablePixelAroundPosition(mousePosition);
			//	Vector2 hitposition = FindHitFromRaycast(mousePosition);

			//	if(!positionLocked) {
			//		if(hitposition != Vector2.zero) {
			//			// TODO Slow down character movement
			//			destinationPosition = hitposition;
			//		}
			//		else destinationPosition = mousePosition;

			//		if(obj != null) { // Walk to the object and then interact
			//			pendingInspection = obj;
			//		}
			//		else { // Simply walk to the destination
			//			pendingInspection = null;
			//		}
			//	}
			//	else { // Direct interaction with the object
			//		if(obj != null)
			//			(obj as IInspectable).InspectionAction(this);
			//	}
			//}
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