using UnityEngine;
using System;
using System.Collections;
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

		protected override void Start()
        {
            base.Start();
        }

		protected override void Update()
		{
			KeyPressed();

			base.Update();
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
                int selection = -1;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    selection = 0;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    selection = 1;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    selection = 2;
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    selection = 3;

                if (selection != -1)
                    if (conversationState.InputIsValid(selection))
                        conversationState.character.InvokeDialogue(selection);
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