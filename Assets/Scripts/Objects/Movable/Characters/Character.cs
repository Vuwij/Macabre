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
		protected Animator animator { get { return GetComponentInChildren<Animator>(); }}

		protected virtual Vector2 inputVelocity { get; set; }

		public PixelPose pose {
			get {
				PixelPose p = new PixelPose();
				PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
				p.pixelRoom = pixelCollider.GetPixelRoom();
				p.position = pixelCollider.transform.position;

				if (facingDirection.x > 0 && facingDirection.y > 0)
					p.direction = Direction.NE;
				else if (facingDirection.x > 0 && facingDirection.y < 0)
					p.direction = Direction.SE;
				else if (facingDirection.x < 0 && facingDirection.y > 0)
                    p.direction = Direction.NW;
				else if (facingDirection.x < 0 && facingDirection.y < 0)
                    p.direction = Direction.SW;

				return p;
			}
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

		public Queue<GameTask> characterTasks = new Queue<GameTask>();

		List<WayPoint> wayPoints;
		public Vector2 wayPointVelocity;

		public bool positionLocked = false;

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

		public Character currentlySpeakingTo;
        public ConversationState currentConversationState;
        public Dictionary<string, ConversationState> conversationStates = new Dictionary<string, ConversationState>();
        public Dictionary<string, string> characterEvents = new Dictionary<string, string>(); // Temporary, per conversation

		public CharacterStatistics statistics = new CharacterStatistics();

        protected override void Start()
        {
            UpdateFromPrefab();
			UpdateSortingLayer();
            base.Start();         
        }
        
		private void OnEnable()
		{
			StartCoroutine("UpdateCharacterAction");
			PixelRoom room = transform.parent.GetComponent<PixelRoom>();
			InvokeRepeating("Movement", 0.0f, 1.0f / room.RoomWalkingSpeed);
		}

		private void OnDisable()
		{
			CancelInvoke("Movement");
		}

		void UpdateFromPrefab() {
            Character prefab = Resources.Load<Character>("Characters/" + name);
			if(prefab == null) {
				Debug.LogError(name + " not found in prefab");
				Debug.Assert(prefab != null);
			}
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
                characterVelocity = inputVelocity;
                if (wayPoints != null)
                {
                    wayPoints.Clear();
                    wayPointVelocity = Vector2.zero;
                }
            }
			else if (wayPointVelocity != Vector2.zero)
			{
				Vector2 pos = transform.position;
				pos.x = pos.x + wayPointVelocity.x;
				pos.y = pos.y + wayPointVelocity.y;
				transform.position = pos;
				UpdateSortingLayer();
				characterVelocity = wayPointVelocity;
			}
			else
			{
				characterVelocity = Vector2.zero;
			}
                     
			if(characterVelocity != Vector2.zero)
			    facingDirection = characterVelocity;

            AnimateMovement();
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
        
		public void Inspect()
		{
            PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
            var objects = pixelCollider.CheckForInspection();
			foreach(PixelCollision pc in objects) {
				Debug.Log(pc.pixelCollider.transform.parent.name);
 				InspectObject(pc);
            }
		}

		public void InspectObject(PixelCollision pc)
		{
            // Inspected object is a door
            PixelDoor door = pc.pixelCollider.transform.parent.GetComponent<PixelDoor>();
            if (door != null)
            {
				if (door.interactionDirection != Direction.All && pc.direction != Direction.All)
                {
                    if (door.interactionDirection != pc.direction) return;
                    if (facingDirection.x > 0 && facingDirection.y > 0 && door.interactionDirection != Direction.NE) return;
                    if (facingDirection.x > 0 && facingDirection.y < 0 && door.interactionDirection != Direction.SE) return;
                    if (facingDirection.x < 0 && facingDirection.y > 0 && door.interactionDirection != Direction.NW) return;
                    if (facingDirection.x < 0 && facingDirection.y < 0 && door.interactionDirection != Direction.SW) return;
                }
				animator.SetTrigger(Animator.StringToHash("IsInteract"));
                EnterDoor(door);
                return;
            }

            // Inspected object is an item
            PixelItem item = pc.pixelCollider.transform.parent.GetComponent<PixelItem>();
            if (item != null)
            {
                CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
                Debug.Assert(inv != null);

                bool succeed = inv.AddItem(item);

                if (succeed)
                {
                    animator.SetTrigger(Animator.StringToHash("IsPickup"));
                    item.gameObject.SetActive(false);
                    item.transform.parent = inv.transform;
                }

                return;
            }

            // Inspected object is a character
			currentlySpeakingTo = pc.pixelCollider.transform.parent.GetComponent<Character>();
			if (currentlySpeakingTo == this) currentlySpeakingTo = null; // Cannot talk to one self
			if (currentlySpeakingTo != null)
				Talk();
		}

		public void EnterDoor(PixelDoor door) {
			PixelRoom room = door.destination;
            Transform originalroom = transform.parent;

			Vector2 originalposition = transform.position;
			transform.parent = null; // Prevents disabling the player

			originalroom.gameObject.SetActive(false);
            room.transform.gameObject.SetActive(true);

			Vector2 destinationOffset = door.dropOffWorldLocation;
            facingDirection = destinationOffset - (Vector2)transform.position;
			if (!(door is PixelStair))
				transform.position = destinationOffset;

            transform.parent = room.transform;

            room.OnEnable();

            UpdateSortingLayer();

            CancelInvoke("Movement");
            AnimateMovement();
            InvokeRepeating("Movement", 0.0f, 1.0f / room.RoomWalkingSpeed);

			if (door is PixelStair)
                transform.position = originalposition;

            return;
		}

         public void Talk(int selection = 0) {
			if (selection == 0)
			{
				// Do the talk
				currentlySpeakingTo.currentConversationState.Display();
				if (currentlySpeakingTo != null && currentlySpeakingTo.currentConversationState.nextStates.Count <= 1)
				{
					currentlySpeakingTo.currentConversationState = currentlySpeakingTo.currentConversationState.NextState();
					currentlySpeakingTo.currentConversationState.UpdateConversationConditions();
					currentlySpeakingTo.currentConversationState.AnimateConversationActions();
					if (currentlySpeakingTo.currentConversationState.stateName == "Silent")
						currentlySpeakingTo = null;
				}
			}
			else
			{
				if (currentlySpeakingTo == null)
					return;

				if (currentlySpeakingTo.currentConversationState.nextStates.Count == 1)
					return;

				ConversationState nextState = currentlySpeakingTo.currentConversationState.NextState(selection);

				if (nextState == null)
					return;
				else
					currentlySpeakingTo.currentConversationState = nextState;

				currentlySpeakingTo.currentConversationState.DisplayCurrent();
				currentlySpeakingTo.currentConversationState.UpdateConversationConditions();
				currentlySpeakingTo.currentConversationState.AnimateConversationActions();
			}
        }
        
        // Walking in a room
		public void WalkInRoom(PixelRoom room, Vector2 walkFromPosition, Vector2 walkToPosition = default(Vector2))
        {
			if (walkToPosition == default(Vector2))
                walkToPosition = room.center;

            PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();

            if (room != pixelCollider.GetPixelRoom())
                room.gameObject.SetActive(true);
            
			HashSet<WayPoint> navigationMesh = room.GetNavigationalMesh(pixelCollider, walkFromPosition);
			WayPoint closest = navigationMesh.Aggregate((i1, i2) => Vector2.Distance(i1.position, walkToPosition) < Vector2.Distance(i2.position, walkToPosition) ? i1 : i2);

            if (room != pixelCollider.GetPixelRoom())
                room.gameObject.SetActive(false);

			Debug.DrawLine(transform.position, closest.position, Color.magenta, 10.0f);

            CharacterTask characterTask = new CharacterTask(GameTask.TaskType.WALKTO, closest.position);
            characterTasks.Enqueue(characterTask);
        }

        // Navigates in the current room only
		public bool WalkTo(Vector2 destination)
		{
			// Find and draw the navigational path
			if(wayPoints == null) {
				wayPoints = FindPathToLocation(destination);
				if (wayPoints.Count != 0)
                {               
                    WayPoint point = wayPoints.First();
                    foreach (WayPoint w in wayPoints)
                    {
                        Debug.DrawLine(point.position, w.position, Color.red, 10.0f);
                        point = w;
                    }
					transform.position = wayPoints.First().position;
                }            
				return false;
			}

			if (wayPoints.Count == 0) {
				wayPoints = null;
				return true;
			}

            // Direction in terms of the direction walked
			Vector2 direction = wayPoints.First().position - (Vector2)transform.position;

			PixelCollider pixelCollider = transform.GetComponentInChildren<PixelCollider>();
            Debug.Assert(pixelCollider != null);
            PixelRoom pixelRoom = pixelCollider.GetPixelRoom();
			int stepSize = pixelRoom.stepSize;
            
			if (direction.sqrMagnitude < 2.5f) {
				wayPointVelocity = Vector2.zero;
				wayPoints.RemoveAt(0);
				return false;
			}

			if (direction.x > 0 && direction.y > 0)
				wayPointVelocity = new Vector2(2, 1);
			else if (direction.x < 0 && direction.y > 0)
				wayPointVelocity = new Vector2(-2, 1);
			else if (direction.x > 0 && direction.y < 0)
				wayPointVelocity = new Vector2(2, -1);
			else if (direction.x < 0 && direction.y < 0)
				wayPointVelocity = new Vector2(-2, -1);

			return false;
		}

		List<WayPoint> FindPathToLocation(Vector2 destination)
		{
			PixelCollider pixelCollider = transform.GetComponentInChildren<PixelCollider>();
			Debug.Assert(pixelCollider != null);
			PixelRoom pixelRoom = pixelCollider.GetPixelRoom();
			Debug.Assert(pixelRoom != null);

			// Dijkstra's Algorithm Parameters
			int stepSize = pixelRoom.stepSize;
			float minDistanceToTarget = stepSize * 2;

			// Initialization
			HashSet<WayPoint> Q = pixelRoom.GetNavigationalMesh(pixelCollider);
			WayPoint target = new WayPoint
            {
				position = destination,
				distance = float.MaxValue,
                previous = null
            };
			WayPoint current = Q.Aggregate((i1, i2) => (i1.position - (Vector2)transform.position).sqrMagnitude < (i2.position - (Vector2)transform.position).sqrMagnitude ? i1 : i2);
			current.distance = 0;
            
			// Propogation
			while(Q.Count > 0) {
				WayPoint u = Q.Aggregate((i1, i2) => i1.distance < i2.distance ? i1 : i2);
				Q.Remove(u);
                
				if (WayPoint.Distance(u, target) < minDistanceToTarget) {
					target.previous = u;

					List<WayPoint> path = new List<WayPoint>();
					if (u.previous == null)
						return path;
					while(u.previous != null) {
						path.Add(u);
						u = u.previous;
					}
					path.Add(u);
					path.Reverse();
                    return path;
				}

				foreach(WayPoint v in u.neighbours) {
					float alt = u.distance + WayPoint.Distance(u, v);
					if (alt < v.distance) {
						v.distance = alt;
						v.previous = u;
					}
				}            
			}

			return new List<WayPoint>();
		}

		public bool NavigateObject(PixelRoom room, PixelCollider pixelCollider, Direction direction = Direction.All) {
            
			PixelRoom pixelRoom = pixelCollider.GetPixelRoom();
            PixelCollider playerCollider = GetComponentInChildren<PixelCollider>();

			// Find last location
			// Closest from the door or the players position
            List<PixelDoor> path = FindPathToRoom(room);

            Vector2 startPosition;
            if (path.Count != 0)
            {
                room.gameObject.SetActive(true);                
                room.GetNavigationalMesh(playerCollider, path.Last().dropOffWorldLocation);
				room.gameObject.SetActive(false);
                startPosition = path.Last().dropOffWorldLocation;
            }
            else
            {
                startPosition = transform.position;
            }

            // Navigate from the last location to find the pixel pose
			PixelPose pixelPose;
			if (pixelCollider != null)
            {
				pixelRoom.GetNavigationalMesh(playerCollider, startPosition);
    
				// Object is movable
                if (pixelCollider.transform.parent.GetComponent<MovableObject>())
                {
                    if (direction != Direction.All)
					{
						WayPoint wayPoint = pixelCollider.FindWayPointInDirection(direction);
						pixelPose = new PixelPose(pixelRoom, direction, wayPoint.position);
					}
					else
					{
                        // Player moves to the characters position first                  
						KeyValuePair<PixelPose, float> bestPlayerMovementWayPoint = pixelCollider.FindBestWayPoint();
						pixelPose = bestPlayerMovementWayPoint.Key;

						Debug.DrawLine(transform.position, pixelPose.position, Color.red, 10.0f);                  

						// Character enqueues a movement to the best place for that position
						Character character = pixelCollider.GetComponentInParent<Character>();
						if(character != null) {
							PixelPose translatedPose = pixelPose.TranslatePose(2*(pixelCollider.navigationMargin + playerCollider.navigationMargin));
							PixelPose flippedPose = translatedPose.Flip();

							GameTask characterNavTask = new GameTask(GameTask.TaskType.NAVIGATE);
                            characterNavTask.character = character;
							characterNavTask.arguments.Add(flippedPose);
							character.characterTasks.Enqueue(characterNavTask);                   
						}
					}
                }
                else
                {
					if (direction != Direction.All)
                    {
                        WayPoint wayPoint = pixelCollider.FindWayPointInDirection(direction);
						pixelPose = new PixelPose(pixelRoom, direction, wayPoint.position);
                    }
                    else
                    {
                        PixelCollider characterCollider = GetComponentInChildren<PixelCollider>();
						pixelPose = pixelCollider.FindBestWayPointPosition(startPosition);
                    }
                }
				Navigate(pixelPose);            
            }
            return true;         
		}

		public bool Navigate(PixelPose pose) {
			// Find a list of doors to navigate to
			List<PixelDoor> path = FindPathToRoom(pose.pixelRoom);
            if (path != null)
            {
                foreach (PixelDoor door in path)
                {
                    Debug.Log("Take " + door.name + " to " + door.destination);
                }
            }

            // Enqueue walkto tasks
            foreach (PixelDoor door in path)
            {
                CharacterTask walkToDoorTask = new CharacterTask(GameTask.TaskType.WALKTO, door.dropInWorldLocation);
                CharacterTask enterDoorTask = new CharacterTask(GameTask.TaskType.ENTERDOOR, door);
                characterTasks.Enqueue(walkToDoorTask);
                characterTasks.Enqueue(enterDoorTask);
            }

            Vector2 lastPosition;
            if (path.Count == 0)
                lastPosition = transform.position;
            else
                lastPosition = path.Last().dropOffWorldLocation;

			WalkInRoom(pose.pixelRoom, lastPosition, pose.position);

			CharacterTask faceTask = new CharacterTask(GameTask.TaskType.FACEDIRECTION, pose.direction);
			faceTask.character = this;
			characterTasks.Enqueue(faceTask);

			return true;
		}

		public bool FaceDirection(Direction direction) {
			if (direction == Direction.All) return true;
			characterVelocity = Vector2.zero;
			if (direction == Direction.NE)
				facingDirection = new Vector2(2, 1);
			else if (direction == Direction.NW)
                facingDirection = new Vector2(-2, 1);
			else if (direction == Direction.SE)
                facingDirection = new Vector2(2, -1);
			else if (direction == Direction.SW)
                facingDirection = new Vector2(-2, -1);
			return true;
		}

        // Navigate the entire world. Uses breath first search
		List<PixelDoor> FindPathToRoom(PixelRoom destination) {
            // Variables
			Queue<PixelRoom> OpenSet = new Queue<PixelRoom>();
			HashSet<PixelRoom> ClosedSet = new HashSet<PixelRoom>();
			Dictionary<PixelRoom, List<PixelDoor>> Meta = new Dictionary<PixelRoom, List<PixelDoor>>();

			// Initalization
			PixelCollider pixelCollider = transform.GetComponentInChildren<PixelCollider>();
            PixelRoom start = pixelCollider.GetPixelRoom();
			Debug.Assert(start != null);
			OpenSet.Enqueue(start);

			while(OpenSet.Count > 0) {
				PixelRoom subtreeRoot = OpenSet.Dequeue();
				if(subtreeRoot == destination) {
					List<PixelDoor> actionList = new List<PixelDoor>();
					PixelRoom state = subtreeRoot;

					while (Meta.ContainsKey(state) && Meta[state].Count() >= 0)
                    {
						actionList.Add(Meta[state].First());
						state = Meta[state].First().source;
                    }
                    actionList.Reverse();
                    return actionList;
				}

				foreach(PixelDoor pixeldoor in subtreeRoot.pixelDoors) {
					if (ClosedSet.Contains(pixeldoor.destination))
						continue;

					if (!OpenSet.Contains(pixeldoor.destination)) {
						if (!Meta.ContainsKey(pixeldoor.destination))
						    Meta.Add(pixeldoor.destination, new List<PixelDoor>());
						Meta[pixeldoor.destination].Add(pixeldoor);
						OpenSet.Enqueue(pixeldoor.destination);
					}
				}

				ClosedSet.Add(subtreeRoot);
			}

			return null;
		}

		public void CreateItem(GameObject item, int quantity = 1)
		{
			Debug.Assert(quantity >= 1 && quantity <= 4);

			for (int i = 0; i < quantity; ++i)
			{
				GameObject newObj = Instantiate(item);
				newObj.gameObject.name = item.name;

				CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
				Debug.Assert(inv != null);

				PixelItem pixelItem = newObj.GetComponent<PixelItem>();

				bool succeed = inv.AddItem(pixelItem);
				if (succeed)
				{
					newObj.gameObject.SetActive(false);
					newObj.transform.parent = inv.transform;
				}
				else
				{
					Destroy(newObj);
					Debug.LogWarning("Inventory Full");
				}
			}
		}

		public void Puts(int number, string item, PixelStorage pixelStorage) {
			CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
            Debug.Assert(inv != null);

			bool hasItem = inv.HasItem(item, number);
			if(!hasItem) {
				Debug.LogWarning(this.gameObject.name + " does not have " + number + " " + item);
				return;
			}

			Debug.Assert(number >= 1 && number <= 24);
			for (int i = 0; i < number; ++i) {
				GameObject obj = inv.GetItem(item);
				pixelStorage.AddObject(obj);
			}

			animator.SetTrigger(Animator.StringToHash("IsInteract"));
		}

		public void Takes(int number, string item, PixelStorage pixelStorage)
        {
            CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
            Debug.Assert(inv != null);

			bool hasItem = pixelStorage.HasObject(item, number);
            if (!hasItem)
            {
				Debug.LogWarning(pixelStorage.name + " does not have " + number + " " + item);
                return;
            }

            Debug.Assert(number >= 1 && number <= 24);
            for (int i = 0; i < number; ++i)
            {
				GameObject obj = pixelStorage.TakeObject(item);
				if (obj == null)
					break;
				PixelItem pixelItem = obj.GetComponent<PixelItem>();
				Debug.Assert(pixelItem != null);

				bool succeed = inv.AddItem(pixelItem);

                if (succeed)
                {
                    //animator.SetTrigger(Animator.StringToHash("IsPickup"));
					pixelItem.gameObject.SetActive(false);
					pixelItem.transform.parent = inv.transform;
                }            
            }

			animator.SetTrigger(Animator.StringToHash("IsInteract"));
        }

		public void Gives(int number, string item, Character character)
		{
			CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
            Debug.Assert(inv != null);

            bool hasItem = inv.HasItem(item, number);
            if (!hasItem)
            {
                Debug.LogWarning(this.gameObject.name + " does not have " + number + " " + item);
                return;
            }

            Debug.Assert(number >= 1 && number <= 24);

			CharacterInventory toInv = character.GetComponentInChildren<CharacterInventory>();
			Debug.Assert(toInv != null);

			animator.SetTrigger(Animator.StringToHash("IsInteract"));

            for (int i = 0; i < number; ++i)
            {
                GameObject obj = inv.GetItem(item);
				PixelItem pixelItem = obj.GetComponent<PixelItem>();
				toInv.AddItem(pixelItem);
				obj.transform.parent = toInv.transform;
            }
		}

		public void Steals(int number, string item, Character character)
        {
            CharacterInventory inv = GetComponentInChildren<CharacterInventory>();
            Debug.Assert(inv != null);

            bool hasItem = inv.HasItem(item, number);
            if (!hasItem)
            {
                Debug.LogWarning(this.gameObject.name + " does not have " + number + " " + item);
                return;
            }

            Debug.Assert(number >= 1 && number <= 24);

			CharacterInventory toInv = character.GetComponentInChildren<CharacterInventory>();
            Debug.Assert(toInv != null);

			animator.SetTrigger(Animator.StringToHash("IsInteract"));

            for (int i = 0; i < number; ++i)
            {
				GameObject obj = toInv.GetItem(item);
				Debug.Assert(obj != null);
                PixelItem pixelItem = obj.GetComponent<PixelItem>();
				toInv.AddItem(pixelItem);
				obj.transform.parent = toInv.transform;
            }
        }

        // TODO 3. There is no mount function yet. Create the mount function to mount onto chairs and beds. Determine if something is mountable or not
		public bool Mount()
		{
			// The mount function must have arguments that include the object to be mounted, and the destination object to be mounted. 
			// It should call PixelMount.Mount() and PixelMount.Dismount() and it should change the characters sprite to a sitting position via an animation
			// The foot of the character is a seperate sprite and should be created and located to the precise location of the character's position when mounting and dismounting
			// The seperate sprite gameobject should be deleted when it is finished dismounting from the object to mount
			// The function should return true if the object to mount is unsuccesful
			return true;
		}

		IEnumerator UpdateCharacterAction()
		{
			while (true)
			{
				if (characterTasks.Count == 0) {
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				            
				GameTask t = characterTasks.Peek();
				Debug.Assert(t != null);
                
				if (t.taskType == GameTask.TaskType.NAVIGATE)
				{
					Debug.Assert(t.arguments.Count() >= 1 && t.arguments.Count() <= 3);

					bool completed;
					positionLocked = true;
					if (t.arguments.Count() == 1)
					{
						PixelPose pixelPose = (PixelPose)t.arguments[0];
						completed = Navigate(pixelPose);
					}
					else
					{
						Direction direction = Direction.All;
						if (t.arguments.Count() == 3)
							direction = (Direction)t.arguments[2];
						completed = NavigateObject((PixelRoom)t.arguments[0], (PixelCollider)t.arguments[1], direction);
					}
					if (completed)
					{
						positionLocked = false;
						characterTasks.Dequeue();
					}
				}
				else if (t.taskType == GameTask.TaskType.WALKTO)
                {
                    Debug.Assert(t.arguments.Count() == 1);
					positionLocked = true;
					bool completed = WalkTo((Vector2)t.arguments[0]);
					if (completed)
					{
						positionLocked = false;
						characterTasks.Dequeue();
					}
                }
				else if (t.taskType == GameTask.TaskType.ENTERDOOR)
				{
					Debug.Assert(t.arguments.Count() == 1);
					EnterDoor((PixelDoor)t.arguments[0]);
                    characterTasks.Dequeue();
				}
				else if (t.taskType == GameTask.TaskType.INSPECT)
                {
                    Debug.Assert(t.arguments.Count() == 1);
					InspectObject((PixelCollision)t.arguments[0]);
                    characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.CREATE)
				{
					Debug.Assert(t.arguments.Count() == 2);
					CreateItem((GameObject)t.arguments[1], (int)t.arguments[0]);
					characterTasks.Dequeue();
				}
				else if (t.taskType == GameTask.TaskType.PUTS)
                {
                    Debug.Assert(t.arguments.Count() == 3);
					Puts((int)t.arguments[0], (string)t.arguments[1], (PixelStorage) t.arguments[2]);
                    characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.TAKES)
                {
                    Debug.Assert(t.arguments.Count() == 3);
                    Takes((int)t.arguments[0], (string)t.arguments[1], (PixelStorage)t.arguments[2]);
                    characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.GIVES)
                {
                    Debug.Assert(t.arguments.Count() == 3);
					Gives((int)t.arguments[0], (string)t.arguments[1], (Character) t.arguments[2]);
                    characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.STEALS)
                {
                    Debug.Assert(t.arguments.Count() == 3);
					Steals((int)t.arguments[0], (string)t.arguments[1], (Character) t.arguments[2]);
                    characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.FACEDIRECTION)
                {
                    Debug.Assert(t.arguments.Count() == 1);
					FaceDirection((Direction)t.arguments[0]);
					characterTasks.Dequeue();
                }
				else if (t.taskType == GameTask.TaskType.MOUNT)
				{
					// TODO 2. Create a function here that calls the mount function
				}
				yield return new WaitForFixedUpdate();
			}
		}
	}
}