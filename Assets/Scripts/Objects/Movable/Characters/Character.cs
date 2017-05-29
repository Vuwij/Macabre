using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Conversation;
using System.Linq;
using Objects.Unmovable.Items;
using Objects.Inventory;
using Objects.Inventory.Individual;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;

namespace Objects.Movable.Characters
{
	public abstract class Character : MovableObject
    {
		Player player
        {
            get {
				return GameObject.Find("Player").GetComponentInChildren<Player>();
            }
        }
		GameObject childObject
        {
            get { return gameObject.transform.Find(gameObject.name + "Sprite").gameObject; }
        }
		Animator animator
		{
			get
			{
				return GetComponentInChildren<Animator>();
			}
		}
		protected bool keyboardMovement
		{
			get { return GameSettings.useKeyboardMovement; }
		}
		protected float walkingSpeed
		{
			get { return GameSettings.characterWalkingSpeed; }
		}
		protected float runningSpeed
		{
			get { return GameSettings.characterRunningSpeed; }
		}
		protected bool isRunning
		{
			get { return Input.GetButton("SpeedUp"); }
		}
		public float movementSpeed
		{
			get { return isRunning ? runningSpeed : walkingSpeed; }
		}
		bool isMoving
		{
			get { return (rigidbody2D.velocity.sqrMagnitude >= float.Epsilon); }
		}
		float inspectRadius
		{
			get { return GameSettings.inspectRadius; }
		}

		protected override void Start()
        {
			inventory = new CharacterInventory(gameObject, 1, 6);
			base.Start();
        }

		void Add(Character s) {}

		#region Movement and Animation

		public void LockMovement() {
			movementLocked = true;
		}

		public void UnlockMovement() {
			movementLocked = false;
		}

		public void AnimateDeath()
		{
			animator.SetBool(Animator.StringToHash("Die"), true);
		}

		protected void AnimateMovement()
		{
			float xDir = 0, yDir = 0;

			if (isMoving)
			{
				if (keyboardMovement)
				{
					if (rigidbody2D.velocity.x > 0) xDir = movementSpeed;
					else if (rigidbody2D.velocity.x < 0) xDir = -movementSpeed;

					if (rigidbody2D.velocity.y > 0) yDir = movementSpeed;
					else if (rigidbody2D.velocity.y < 0) yDir = -movementSpeed;
				}

				animator.SetBool(Animator.StringToHash("IsActive"), false);
				animator.SetBool(Animator.StringToHash("IsMoving"), true);

				if (xDir != 0) animator.SetFloat(Animator.StringToHash("MoveSpeed-x"), xDir);
				if (yDir != 0) animator.SetFloat(Animator.StringToHash("MoveSpeed-y"), yDir);
			}
			else
			{
				animator.SetBool(Animator.StringToHash("IsActive"), true);
				animator.SetBool(Animator.StringToHash("IsMoving"), false);
			}
		}

		#endregion

		#region Conversation

		public static ConversationState conversationState;

		// Invoked everytime when the spacebar is pressed or an decision is made
		public ConversationState Dialogue(int decision = 0)
		{
			// Check if the character exists in database
			if (conversationState == null) conversationState = new ConversationState(this);
			else conversationState = conversationState.GetNextState(decision);

			ConversationState.DisplayState(conversationState);

			return conversationState;
		}

		#endregion

		#region Inspection

		private RaycastHit2D hit;

		public void InspectionAction(Object obj, RaycastHit2D raycastHit)
		{
			if(conversationState != null && conversationState.conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse) return;
			Dialogue(0);
		}

		public void KeyPressed
		(int keyPressed = 0)
		{
			if (conversationState.InputIsValid(keyPressed))
				conversationState.character.Dialogue(keyPressed - 1);
		}

		public void Inspect()
		{
			RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, inspectRadius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				if (InspectionIsInvalid(raycastHit)) continue;

				IInspectable mObj = raycastHit.collider.GetComponent<IInspectable>();
				if (mObj != null)
				{
					Debug.Log(raycastHit.collider.name);
					mObj.InspectionAction(this, raycastHit);
					return;
				}
			}
		}

		// Cannot Raycast Hit anything on this opject
		private bool InspectionIsInvalid(RaycastHit2D raycastHit)
		{
			// No triggers
			if (raycastHit.collider.isTrigger) return true;

			// If hit a character
			if (raycastHit.collider.GetComponent<Character>() != null)
			{
				// Cannot hit itself
				if (raycastHit.collider.GetComponent<Character>() == this) return true;

				// Can only get circle collider
				if (raycastHit.collider is PolygonCollider2D) return false;
			}   

			return false;
		}

		#endregion

		#region Inventory

		public CharacterInventory inventory;

		public bool AddToInventory(Item i)
		{
			return inventory.Add(i);
		}

		#endregion

	}
}