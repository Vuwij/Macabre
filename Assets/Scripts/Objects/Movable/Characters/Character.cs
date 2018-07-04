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

		public Queue<GameTask> characterTasks = new Queue<GameTask>();

		List<WayPoint> wayPoints;
		public Vector2 wayPointVelocity;

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

		private void OnEnable()
		{
			StartCoroutine("UpdateCharacterAction");
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
			if(!positionLocked) {
				if (inputVelocity != Vector2.zero)
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
                EnterDoor(door);
                return;
            }

            // Inspected object is an item
            PixelItem item = pc.pixelCollider.transform.parent.GetComponent<PixelItem>();
            if (item != null)
            {
                PixelInventory inv = GetComponentInChildren<PixelInventory>();
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
            currrentlySpeakingTo = pc.pixelCollider.transform.parent.GetComponent<Character>();
            if (currrentlySpeakingTo != null)
            {
                currrentlySpeakingTo.currentConversationState.Display();
                if (currrentlySpeakingTo.currentConversationState.nextStates.Count <= 1)
                {
                    currrentlySpeakingTo.currentConversationState = currrentlySpeakingTo.currentConversationState.NextState();
                    currrentlySpeakingTo.currentConversationState.UpdateConversationConditions();
                }
            }
		}

		public void EnterDoor(PixelDoor door) {
			PixelRoom room = door.destination;
            Transform originalroom = transform.parent;

			transform.parent = null; // Prevents disabling the player

			originalroom.gameObject.SetActive(false);
            room.transform.gameObject.SetActive(true);

			Vector2 destinationOffset = door.dropOffWorldLocation;
            facingDirection = destinationOffset - (Vector2)transform.position;
            transform.position = destinationOffset;
            transform.parent = room.transform;
                     
            room.OnEnable();

            UpdateSortingLayer();

            CancelInvoke("Movement");
            AnimateMovement();
            InvokeRepeating("Movement", 0.0f, 1.0f / room.RoomWalkingSpeed);

            return;
		}

        public void Talk(int selection) {
			if (currrentlySpeakingTo == null)
				return;            

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
			HashSet<WayPoint> Q = pixelRoom.GetNavigationalMesh(transform.position, stepSize);
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

		public bool Navigate(PixelRoom room, PixelCollider pixelCollider) {

			// Find a list of doors to navigate to
			List<PixelDoor> path = FindPathToRoom(room);
			if(path != null) {
				foreach(PixelDoor door in path) {
					Debug.Log("Take " + door.name + " to " + door.destination);
				}
			}

            // Enqueue walkto tasks
			foreach(PixelDoor door in path) {
				CharacterTask walkToDoorTask = new CharacterTask(GameTask.TaskType.WALKTO, door.dropInWorldLocation);
				CharacterTask enterDoorTask = new CharacterTask(GameTask.TaskType.ENTERDOOR, door);
				characterTasks.Enqueue(walkToDoorTask);
				characterTasks.Enqueue(enterDoorTask);
			}

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

		public void WalkAndInspectObject(PixelCollider pixelCollider, Vector2 walkToPosition = default(Vector2))
		{
			if (walkToPosition == default(Vector2))
				walkToPosition = pixelCollider.transform.position;
			WayPoint position = pixelCollider.FindWalkToPosition(walkToPosition);
			CharacterTask characterTask = new CharacterTask(GameTask.TaskType.WALKTO, position.position);
            characterTasks.Enqueue(characterTask);
			PixelCollision pc = new PixelCollision();
			pc.pixelCollider = pixelCollider;
			pc.direction = Direction.All;
			CharacterTask inspectTask = new CharacterTask(GameTask.TaskType.INSPECT, pc);
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
					Debug.Assert(t.arguments.Count() == 2);
					bool completed = Navigate((PixelRoom)t.arguments[0], (PixelCollider)t.arguments[1]);
                    if (completed)
						characterTasks.Dequeue();
				}
				else if (t.taskType == GameTask.TaskType.WALKTO)
                {
                    Debug.Assert(t.arguments.Count() == 1);
					bool completed = WalkTo((Vector2)t.arguments[0]);
                    if (completed)
						characterTasks.Dequeue();
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
				yield return new WaitForFixedUpdate();
			}
		}
	}
}