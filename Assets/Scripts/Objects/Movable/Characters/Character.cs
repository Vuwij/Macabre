using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Immovable.Items;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;

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
		protected Animator animator
		{
			get
			{
				return GetComponentInChildren<Animator>();
			}
		}

        protected virtual Vector2 inputVelocity {
            get; set;
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

        [Range(1, 20)]
        public int indoorMovementSpeed = 10;
        [Range(1, 20)]
        public int outdoorMovementSpeed = 30;

		protected override void Start()
        {
            //inventory = new CharacterInventory(gameObject, 6, 1);
			interactionText = "Press T to talk to " + name;

            InvokeRepeating("Movement", 0.0f, 1.0f / indoorMovementSpeed);

			base.Start();
        }

        void Movement()
        {
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

		public void Inspect()
		{
            PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
            var objects = pixelCollider.CheckForInspection();
            foreach(PixelCollider pc in objects) {
                Debug.Log(pc.transform.parent.name);

                // Inspected object is a door
                PixelDoor door = pc.transform.parent.GetComponent<PixelDoor>();
                if(door != null) {
                    PixelRoom room = door.destination;
                    Transform originalroom = transform.parent;
                    Vector2 destinationOffset = door.dropOffLocation + (Vector2) door.transform.position;
                    transform.position = destinationOffset;
                    transform.parent = room.transform;
                    originalroom.gameObject.SetActive(false);
                    room.transform.gameObject.SetActive(true);

                    CancelInvoke("Movement");
                    if(room.name == "Overworld")
                        InvokeRepeating("Movement", 0.0f, 1.0f / outdoorMovementSpeed);
                    else
                        InvokeRepeating("Movement", 0.0f, 1.0f / indoorMovementSpeed);
                    return;
                }

                // Inspected object is an item
                PixelItem item = pc.transform.parent.GetComponent<PixelItem>();
                if(item != null) {
                    PixelInventory inv = GetComponentInChildren<PixelInventory>();
                    Debug.Assert(inv != null);

                    bool succeed = inv.AddItem(item);

                    if (succeed) {
                        animator.SetTrigger(Animator.StringToHash("IsPickup"));
                        item.gameObject.SetActive(false);
                        item.transform.parent = inv.transform;
                    }

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

		public bool AddToInventory(Item i)
		{
            //return inventory.Add(i);
            return false;
		}

		#endregion

	}
}