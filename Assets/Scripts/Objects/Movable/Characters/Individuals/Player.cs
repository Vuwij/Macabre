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
        protected override Vector2 inputVelocity
		{
			get {
                PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
                PixelCollider.MovementRestriction mr = new PixelCollider.MovementRestriction();
                mr.restrictNE = false;
                mr.restrictNW = false;
                mr.restrictSE = false;
                mr.restrictSW = false;

                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (!state.IsName("Idle") && !state.IsName("Move") && !state.IsName("Look"))
                    return Vector2.zero;

                if ((int) Input.GetAxisRaw("Horizontal") == 0 && (int) Input.GetAxisRaw("Vertical") == 0)
                    return Vector2.zero;

                if (pixelCollider != null)
                    mr = pixelCollider.CheckForCollision();

				if (Input.GetAxisRaw("Horizontal") > 0 && mr.restrictNE)
					facingDirection = new Vector2(2, 1);
                else if (Input.GetAxisRaw("Horizontal") < 0 && mr.restrictSW)
					facingDirection = new Vector2(-2, -1);
                else if (Input.GetAxisRaw("Vertical") > 0 && mr.restrictNW)
					facingDirection = new Vector2(-2, 1);
                else if (Input.GetAxisRaw("Vertical") < -0 && mr.restrictSE)
					facingDirection = new Vector2(2, -1);


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
              
		protected override void Start()
        {
            base.Start();
        }

		protected override void FixedUpdate()
		{
			MouseClicked();
			KeyPressed();
			base.FixedUpdate();
		}

		void KeyPressed() {

			if(Input.anyKey) {

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

                // Conversation
                int selection = 0;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    selection = 1;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    selection = 2;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    selection = 3;
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    selection = 4;
                if(selection != 0)
                    Talk(selection);
			}
            
		}

		void MouseClicked() {
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
						bool withinCollider = pixelCollider.CheckForWithinCollider(mousePosition);
						if (withinCollider)
						{
							Debug.Log(pixelCollider.transform.parent.name);
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
                        CharacterTask characterTask = new CharacterTask(GameTask.TaskType.NAVIGATE, mousePosition);
                        characterTasks.Enqueue(characterTask);
                    }               
				}
			}
		}
        
		void HoverOverObject() {

		}
    }
}