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

namespace Objects.Movable.Characters
{
	public abstract class Character : MovableObject
    {
        // The character associated with the controller, found in the data structure
		new public string name = "Character";
        
        // A simple reference to the player for interaction in conversation
        public static Player player
        {
            get {
				return GameObject.Find("Player").GetComponentInChildren<Player>();
            }
        }

        // The child object is the one that contains the sprite
        private GameObject childObject
        {
            get { return gameObject.transform.Find(gameObject.name + "Sprite").gameObject; }
        }
        
        // What is called when the character gets loaded
        protected override void Start()
        {
            base.Start();
        }

		#region Animation

		private Animator animator
		{
			get
			{
				return GetComponentInChildren<Animator>();
			}
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
					if (movementVelocity.x > 0) xDir = movementSpeed;
					else if (movementVelocity.x < 0) xDir = -movementSpeed;

					if (movementVelocity.y > 0) yDir = movementSpeed;
					else if (movementVelocity.y < 0) yDir = -movementSpeed;
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

		#region Collision

		private SpriteRenderer spriteRenderer
		{
			get { return GetComponentInChildren<SpriteRenderer>(); }
		}

		public override void CreateCollisionCircle()
		{
//			if (collisionCircle == null) collisionCircle = gameObject.AddComponent<EllipseCollider2D>();
//			float width = spriteRenderer.sprite.rect.width;
//			collisionCircle.radiusX = width * 0.25f;
//			collisionCircle.radiusY = width * 0.125f;
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

		private float inspectRadius
		{
			get { return GameSettings.inspectRadius; }
		}
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

		public CharacterInventory inventory = new CharacterInventory();

		public bool AddToInventory(Item i)
		{
			return inventory.Add(i);
		}

		public Transform InventoryFolder
		{
			get
			{
				if(GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory") == null)
				{
					GameObject inventoryFolder = new GameObject("Inventory");
					inventoryFolder.transform.parent = this.transform;
				}

				return GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory");
			}
		}

		#endregion

		#region Movement

		public bool keyboardMovement
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
		public float movementSpeed
		{
			get { return isRunning ? runningSpeed : walkingSpeed; }
		}

		protected bool isRunning
		{
			get { return Input.GetButton("SpeedUp"); }
		}
		private bool isMoving
		{
			get { return true; /*(rb2D.velocity.sqrMagnitude >= float.Epsilon);*/ }
		}

		protected virtual Vector2 movementVelocity
		{
			get { return new Vector2(movementSpeed, movementSpeed * 2.0f); }
		}

		public void ShowMovementAnimation(Vector3 destination)
		{
			if (!isMoving) return;
		}

		public void LockMovement()
		{
//			rb2D.velocity.Set(0, 0);
//			AnimateMovement();
//			lockMovement = true;
		}

		public void UnlockMovement()
		{
//			lockMovement = false;
		}

		#endregion
	}
}