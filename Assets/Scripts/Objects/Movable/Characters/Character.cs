using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;

namespace Objects.Movable.Characters
{
	public abstract class Character : MovableObject
    {
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

        bool positionLocked;

        [HideInInspector]
        public string description;

        [Range(1, 20)]
        public int indoorMovementSpeed = 10;

        [Range(1, 20)]
        public int outdoorMovementSpeed = 30;


		[Serializable]
		public struct ExtraSprites {
			public Sprite leftFeet;
			public Sprite rightFeet;
		}
		public ExtraSprites extraSprites;
		GameObject characterFoot;

        protected Vector2 characterVelocity;
		protected Vector2 facingDirection;      

        protected override void Start()
        {
            UpdateFromPrefab();

			PixelRoom room = transform.parent.GetComponent<PixelRoom>();
			InvokeRepeating("Movement", 0.0f, 1.0f / room.RoomWalkingSpeed);

            base.Start();
        }

        void UpdateFromPrefab() {
            Character prefab = Resources.Load<Character>("Characters/" + name);
			if(prefab == null) {
				Debug.LogError(name + " not found in prefab");
				Debug.Assert(prefab != null);
			}
            conversationStates = prefab.conversationStates;
            currentConversationState = prefab.currentConversationState;
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

			if(characterVelocity != Vector2.zero)
			    facingDirection = characterVelocity;

            AnimateMovement();
        }

        protected override void Update()
        {
            base.Update();
        }

		protected void AnimateMovement()
		{
			if (characterVelocity != Vector2.zero)
			{
				animator.SetBool(Animator.StringToHash("IsActive"), false);
				animator.SetBool(Animator.StringToHash("IsMoving"), true);
			}
			else
			{
				animator.SetBool(Animator.StringToHash("IsActive"), true);
				animator.SetBool(Animator.StringToHash("IsMoving"), false);
			}

			animator.SetFloat(Animator.StringToHash("MoveSpeed-x"), facingDirection.x);
			animator.SetFloat(Animator.StringToHash("MoveSpeed-y"), facingDirection.y);
		}

		public Character currrentlySpeakingTo;
		public ConversationState currentConversationState;
        public Dictionary<string, ConversationState> conversationStates = new Dictionary<string, ConversationState>();
		public Dictionary<string, string> characterEvents = new Dictionary<string, string>(); // Temporary, per conversation

		public void Inspect()
		{
            PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
            var objects = pixelCollider.CheckForInspection();
			foreach(PixelCollision pc in objects) {
				Debug.Log(pc.pixelCollider.transform.parent.name);

                // Inspected object is a door
				PixelDoor door = pc.pixelCollider.transform.parent.GetComponent<PixelDoor>();
                if(door != null) {
					if(door.interactionDirection != Direction.All) {
						if (door.interactionDirection != pc.direction) continue;
						if (facingDirection.x > 0 && facingDirection.y > 0 && door.interactionDirection != Direction.NE) continue;
						if (facingDirection.x > 0 && facingDirection.y < 0 && door.interactionDirection != Direction.SE) continue;
						if (facingDirection.x < 0 && facingDirection.y > 0 && door.interactionDirection != Direction.NW) continue;
						if (facingDirection.x < 0 && facingDirection.y < 0 && door.interactionDirection != Direction.SW) continue;
					}
                    
                    PixelRoom room = door.destination;
                    Transform originalroom = transform.parent;
                    Vector2 destinationOffset = door.dropOffLocation + (Vector2) door.transform.position;
					facingDirection = destinationOffset - (Vector2)transform.position;
                    transform.position = destinationOffset;
                    transform.parent = room.transform;
                    
					originalroom.gameObject.SetActive(false);
                    room.transform.gameObject.SetActive(true);
					room.OnEnable();
                    
					UpdateSortingLayer();

                    CancelInvoke("Movement");
					AnimateMovement();

					InvokeRepeating("Movement", 0.0f, 1.0f / room.RoomWalkingSpeed);
                    
                    return;
                }

                // Inspected object is an item
				PixelItem item = pc.pixelCollider.transform.parent.GetComponent<PixelItem>();
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

                // Inspected object is a character
				currrentlySpeakingTo = pc.pixelCollider.transform.parent.GetComponent<Character>();
				if(currrentlySpeakingTo != null) {
					currrentlySpeakingTo.currentConversationState.Display();
					if (currrentlySpeakingTo.currentConversationState.nextStates.Count <= 1){
						currrentlySpeakingTo.currentConversationState = currrentlySpeakingTo.currentConversationState.NextState();
						currrentlySpeakingTo.currentConversationState.UpdateConversationConditions();
					}
                }
            }
		}

        public void Talk(int selection) {
			if (currrentlySpeakingTo.currentConversationState.nextStates.Count == 1)
				return;

			ConversationState nextState = currrentlySpeakingTo.currentConversationState.NextState(selection);
			if (nextState == null) 
				return;
			else 
				currrentlySpeakingTo.currentConversationState = nextState;
			
			currrentlySpeakingTo.currentConversationState.DisplayCurrent();     
			currrentlySpeakingTo.currentConversationState.UpdateConversationConditions();
        }
	}
}