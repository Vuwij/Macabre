using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using UI;
using UI.Panels;
using UI.Screens;
using UnityEngine.Animations;
using UI.Dialogues;
using Objects.Immovable;

namespace Objects.Movable.Characters.Individuals
{
	public sealed class Player : Character
    {
        bool DebugWindowOpen
		{
			get {
				GameObject debugWindow = GameObject.Find("DebugLogPopup");
                CanvasGroup canvasGroup = debugWindow.GetComponent<CanvasGroup>();
                if (!canvasGroup.interactable)
					return true;
				return false;
			}
		}

		bool controllable => !positionLocked && currentlySpeakingTo == null;

		protected override Vector2 inputVelocity
		{
			get {
				// Check if movable
				if (!controllable) return Vector2.zero;
                
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (!state.IsName("Idle") && !state.IsName("Move") && !state.IsName("Look"))
                    return Vector2.zero;

                if ((int) Input.GetAxisRaw("Horizontal") == 0 && (int) Input.GetAxisRaw("Vertical") == 0)
                    return Vector2.zero;

				if (DebugWindowOpen)
					return Vector2.zero;

                // Check Collisions
				PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
                PixelCollider.MovementRestriction mr = new PixelCollider.MovementRestriction();

                if (pixelCollider != null)
                    mr = pixelCollider.CheckForCollision();

				float lr = 2;
				float ud = 1;

				if (mr.slopeDirection != Direction.All) {
					Debug.Log("hi");
					if (mr.slopeDirection == Direction.NE || mr.slopeDirection == Direction.SW)
					{
						ud = ud * (1 + mr.slope);
						lr = lr / (1 + mr.slope);
					}
				}

				if (Input.GetAxisRaw("Horizontal") > 0 && mr.restrictNE)
					facingDirection = new Vector2(lr, ud);
                else if (Input.GetAxisRaw("Horizontal") < 0 && mr.restrictSW)
					facingDirection = new Vector2(-lr, -ud);
                else if (Input.GetAxisRaw("Vertical") > 0 && mr.restrictNW)
					facingDirection = new Vector2(-lr, ud);
                else if (Input.GetAxisRaw("Vertical") < -0 && mr.restrictSE)
					facingDirection = new Vector2(lr, -ud);
                
                if (Input.GetAxisRaw("Horizontal") > 0 && !mr.restrictNE)
                    return new Vector2(lr, ud);
                else if (Input.GetAxisRaw("Horizontal") < 0 && !mr.restrictSW)
                    return new Vector2(-lr, -ud);
                else if (Input.GetAxisRaw("Vertical") > 0 && !mr.restrictNW)
                    return new Vector2(-lr, ud);
                else if (Input.GetAxisRaw("Vertical") < -0 && !mr.restrictSE)
                    return new Vector2(lr, -ud);
                return Vector2.zero;
			}
		}
        
		Vector2 mousePosition
        {
            get {
                Vector2 click = Input.mousePosition;
                var offset = click - new Vector2(320.0f, 180.0f);
                offset.Scale(new Vector2(0.5f, 0.5f));
                click = offset + new Vector2(320.0f, 180.0f);
                return Camera.main.ScreenToWorldPoint(click);
            }
        }

		PixelCollider currentHighlighted;

		protected override void Start()
        {
            base.Start();
        }
        
		void Update()
		{
			if (controllable)
				MouseClicked();
			
			KeyPressed();
            HoverOverObject();
		}

		void KeyPressed() {
            
			if(Input.anyKey) {
				// Console
				if (DebugWindowOpen)
					return;

				if (controllable)
				{
					// Inventory
					if (Input.GetButtonDown("Inventory"))
					{
						UIScreenManager screenManager = FindObjectOfType<UIScreenManager>();
						InventoryPanel panel = screenManager.GetComponentInChildren<InventoryPanel>(true);
						Debug.Assert(panel != null);
						if (!panel.gameObject.activeInHierarchy)
							panel.gameObject.SetActive(true);
						else
							panel.gameObject.SetActive(false);
					}

					// Inspection
					else if (Input.GetButtonDown("Inspect"))
					{
						Inspect();
					}
				}
				else
				{
					// Conversation
					if (Input.GetButtonDown("Inspect"))
						Talk();
					else {
						int selection = 0;
						if (Input.GetKeyDown(KeyCode.Alpha1))
							selection = 1;
						else if (Input.GetKeyDown(KeyCode.Alpha2))
							selection = 2;
						else if (Input.GetKeyDown(KeyCode.Alpha3))
							selection = 3;
						else if (Input.GetKeyDown(KeyCode.Alpha4))
							selection = 4;

						if (selection != 0)
							Talk(selection);
					}
				}
			}
            
		}

		void MouseClicked() {
			if (DebugWindowOpen)
				return;

			if(Input.GetMouseButtonDown(0)) {
				Vector3 castStart = mousePosition;
				castStart.z = -10.0f;

				RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(mousePosition, 30.0f, Vector2.zero);
                                
                // Detected inspected objects
				foreach (var hit in raycastHits)
                {
					GameObject obj = hit.collider.gameObject;
					if (obj == this) continue;

					PixelCollider pixelCollider = obj.GetComponent<PixelCollider>();
					if (pixelCollider != null)
					{
						if (pixelCollider.inspectChildObjects) continue;
						PixelCollider characterCollider = this.GetComponentInChildren<PixelCollider>();
						if (pixelCollider.GetPixelRoom() != characterCollider.GetPixelRoom()) continue;
						if (pixelCollider.transform.parent.name == "VirtualObject") continue;
						if (pixelCollider.transform.parent.name == "Player") continue;

						bool withinCollider = pixelCollider.CheckForWithinCollider(mousePosition);                  
						if (withinCollider)
						{
							Debug.Log(pixelCollider.transform.parent.name);
                            
							NavigateObject(pixelCollider.GetPixelRoom(), pixelCollider);

                            // Inspect Object
							PixelCollision pc = new PixelCollision();
                            pc.pixelCollider = pixelCollider;
                            pc.direction = Direction.All;
                            CharacterTask inspectTask = new CharacterTask(GameTask.TaskType.INSPECT, pc);
                            characterTasks.Enqueue(inspectTask);

							return;
						}
					}
                }

				// Detected inspected rooms (for navigation)
				foreach (var hit in raycastHits)
				{
					GameObject obj = hit.collider.gameObject;
                    if (obj == this) continue;
                                   
                    PixelRoom pixelRoom = obj.GetComponent<PixelRoom>();
                    if (pixelRoom != null)
                    {
						PixelCollider pixelCollider = gameObject.GetComponentInChildren<PixelCollider>();
						if (pixelCollider == null) continue;
						if (pixelCollider.GetPixelRoom() != pixelRoom) continue;

						Debug.Log(pixelRoom.name);

						// Navigate Maze Room
						CharacterTask characterTask = new CharacterTask(GameTask.TaskType.WALKTO, mousePosition);
                        characterTasks.Enqueue(characterTask);
                    }
				}
			}
		}
        
        // Just highlight the object if your mouse is over it
		void HoverOverObject() {
			Vector3 castStart = mousePosition;
            castStart.z = -10.0f;

            RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(mousePosition, 30.0f, Vector2.zero);

            // Detected inspected objects
            foreach (var hit in raycastHits)
            {
                GameObject obj = hit.collider.gameObject;
                if (obj == this) continue;

                PixelCollider pixelCollider = obj.GetComponent<PixelCollider>();
                if (pixelCollider != null)
                {
					if (pixelCollider.inspectChildObjects) continue;
					if (pixelCollider.transform.parent.name == "Player") continue;
					PixelCollider characterCollider = this.GetComponentInChildren<PixelCollider>();
                    if (pixelCollider.GetPixelRoom() != characterCollider.GetPixelRoom()) continue;

					bool withinCollider = pixelCollider.CheckForWithinCollider(mousePosition);
                    if (withinCollider)
                    {
                        //Debug.Log(pixelCollider.transform.parent.name);

						if (currentHighlighted != pixelCollider) {
							if (currentHighlighted != null)
                                currentHighlighted.UnHighlightObject();
							currentHighlighted = pixelCollider;
                            currentHighlighted.HighlightObject();
						}

                        return;
                    }
                }
            }

			if (currentHighlighted != null) {
				currentHighlighted.UnHighlightObject();
				currentHighlighted = null;
			}
		}      
    }
}