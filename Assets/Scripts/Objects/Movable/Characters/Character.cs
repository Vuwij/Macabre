using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Immovable.Items;
using Objects.Inventory;
using Objects.Inventory.Individual;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;
using Objects.Immovable;
using Objects.Immovable.Furniture;

namespace Objects.Movable.Characters
{
	public abstract class Character : MovableObject, IInspectable
    {
		Player player
        {
            get {
				return GameObject.Find("Player").GetComponentInChildren<Player>();
            }
        }
		public GameObject childObject
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
		protected int walkingSpeed
		{
			get { return GameSettings.characterWalkingSpeed; }
		}
		protected int runningSpeed
		{
			get { return GameSettings.characterRunningSpeed; }
		}
		protected bool isRunning
		{
			get { return Input.GetButton("SpeedUp"); }
		}
		public int movementSpeed
		{
			get { return isRunning ? runningSpeed : walkingSpeed; }
		}
		float inspectRadius
		{
			get { return GameSettings.inspectRadius; }
		}
		public int orientationX
		{
			get {
				return animator.GetInteger("OrientationX");
			}
			set {
				animator.SetInteger("OrientationX", value);
			}
		}
		public int orientationY
		{
			get {
				return animator.GetInteger("OrientationY");
			}
			set {
				animator.SetInteger("OrientationY", value);
			}
		}
		public bool isSittingDown
		{
			get {
				return animator.GetBool("IsSitting");
			}
			set {
				if(value) {
					if(orientationY == 1) {
						characterFoot = new GameObject("Feet", typeof(SpriteRenderer));
						characterFoot.transform.parent = transform;
						characterFoot.transform.localPosition = childObject.transform.localPosition;
						var s = characterFoot.GetComponent<SpriteRenderer>();
						if(orientationX == 1)
							s.sprite = extraSprites.rightFeet;
						else
							s.sprite = extraSprites.leftFeet;
						s.sortingLayerName = "World";
						s.sortingOrder = spriteRenderer.sortingOrder - 1;
					}
				} else {
					if(characterFoot != null)
						Destroy(characterFoot);
				}
				animator.SetBool("IsSitting", value);
			}
		}
		public bool isPickingUp
		{
			set {
				animator.SetTrigger("IsPickup");
			}
		}
		protected bool positionLocked
		{
			get {
				return isSittingDown || isTalking;
			}
		}

		protected override void Start()
        {
			inventory = new CharacterInventory(gameObject, 6, 1);
			interactionText = "Press T to talk to " + name;
			base.Start();
        }

		protected override void Update()
		{
			base.Update();
		}

		#region Movement and Animation

		[Serializable]
		public struct ExtraSprites {
			public Sprite leftFeet;
			public Sprite rightFeet;
		}
		public ExtraSprites extraSprites;
		GameObject characterFoot;

        protected Vector2 characterVelocity;

		protected void AnimateMovement()
		{
            if (characterVelocity != Vector2.zero)
			{
				animator.SetBool(Animator.StringToHash("IsActive"), false);
				animator.SetBool(Animator.StringToHash("IsMoving"), true);
                animator.SetFloat(Animator.StringToHash("MoveSpeed-x"), characterVelocity.x);
                animator.SetFloat(Animator.StringToHash("MoveSpeed-y"), characterVelocity.y);
			}
			else
			{
				animator.SetBool(Animator.StringToHash("IsActive"), true);
				animator.SetBool(Animator.StringToHash("IsMoving"), false);
			}
		}

		#endregion

		#region Conversation

		public bool isTalking;

		public static ConversationState conversationState; // Null if no conversation is happening

		// Invoked everytime when the spacebar is pressed or an decision is made
		public ConversationState InvokeDialogue(int decision = 0)
		{
			if (conversationState == null)
				conversationState = new ConversationState(this);
			else
				conversationState = conversationState.GetNextState(decision);

			if(conversationState != null) {
				conversationState.DisplayState();
			}
			else {
				ConversationState.TurnOff();
			}

			return conversationState;
		}

		public void InspectionAction(Object obj, RaycastHit2D raycastHit)
		{
			if(conversationState != null && conversationState.conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse) return;
			InvokeDialogue(0);
		}

		#endregion

		#region Inspection

		RaycastHit2D hit;
		protected IInspectable inspectedObject;

		public virtual void KeyPressed (int selection = -1)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
				selection = 0;
			if (Input.GetKeyDown(KeyCode.Alpha2))
				selection = 1;
			if (Input.GetKeyDown(KeyCode.Alpha3))
				selection = 2;
			if (Input.GetKeyDown(KeyCode.Alpha4))
				selection = 3;

			if (selection != -1)
				if (conversationState.InputIsValid(selection))
					conversationState.character.InvokeDialogue(selection);
		}

		public void Inspect()
		{
			RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, inspectRadius, Vector2.zero);
			foreach (RaycastHit2D raycastHit in castStar)
			{
				if (InspectionIsInvalid(raycastHit)) continue;
				if(raycastHit.collider.GetComponent<Objects.Object>().enabled == false) continue;
				inspectedObject = raycastHit.collider.GetComponent<IInspectable>();
				if (inspectedObject != null)
				{
					Debug.Log("Inpecting " + raycastHit.collider.name);
					inspectedObject.InspectionAction(this, raycastHit);
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