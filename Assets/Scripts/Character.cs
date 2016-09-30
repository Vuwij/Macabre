using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ExitAnimation
{
	Teleport,
	Fade,
	Smooth,
}

public class Character : MacabreMovingObject, ISavable {

	CanvasGroup fadeOut;
	protected Animator characterAnimator;
	private GameObject childObject;
	private Character player;

	public bool keyboardMovement = true;
	public bool mouseMovement;
	protected Vector2 moveVelocity;

	// The data to be saved
	public string location;
	new public string name;
	public CharacterStatList characterStat = new CharacterStatList (new List<string> () {
		"health",
		"sanity"
	});

	#region Initialize

	protected override void Start() {
		base.Start ();
		characterAnimator = GetComponentInChildren<Animator> ();

		if (GameObject.Find ("Fade Screen")) {
			fadeOut = GameObject.Find ("Fade Screen").GetComponent<CanvasGroup> ();
		}
		childObject = gameObject.transform.FindChild(gameObject.name + "Sprite").gameObject;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Character> ();

		SetupConversation ();
	}

	#endregion

	#region SaveData
	public override void saveInfo ()
	{
		base.saveInfo ();
		var g = GameData.main.Find<MData.Character> (this);

		g.location = location;
		g.name = name;

	}
	public override void loadInfo ()
	{
		base.loadInfo ();
		var g = GameData.main.Find<MData.Character> (this);
		Debug.Log (g.pathName);
		location = g.location;
		name = g.name;
	}

	#endregion

	#region CharacterLocation

	/// <summary>
	/// Locks the character's movement, prevents him from moving
	/// Can invoke using LockMovement() and UnlockMovement()
	/// </summary>
	protected bool lockMovement = false;

	public void ChangeLocation() {
		gameObject.layer = LayerMask.NameToLayer("Outside");
		childObject.layer = LayerMask.NameToLayer("Outside");
		ChangeSortingLayer(null);
	}

	public void ChangeLocation(string Destination) {
		gameObject.layer = LayerMask.NameToLayer(Destination);
		childObject.layer = LayerMask.NameToLayer(Destination);
		ChangeSortingLayer(Destination);
	}

	public void ChangeLocation(int DestinationIndex) {
		gameObject.layer = DestinationIndex;
		childObject.layer = DestinationIndex;
		ChangeSortingLayer (LayerMask.LayerToName(DestinationIndex));
	}

	private void ChangeSortingLayer(string SortingDestination) {
		var sortingLayer = gameObject.GetComponentInChildren<SpriteRenderer>();
		if(SortingDestination == null) sortingLayer.sortingLayerName = "Character - Front";
		else if(SortingDestination == "Floor 1" || SortingDestination == "Floor 2" ||
		        SortingDestination == "Floor 3" || SortingDestination == "Floor 4" ||
		        SortingDestination == "Underground 1" || SortingDestination == "Underground 2" ||
		        SortingDestination == "Underground 3") {
			sortingLayer.sortingLayerName = "Character - Middle 2";
		}
		else if(SortingDestination == "Outside") sortingLayer.sortingLayerName = "Character - Front 2";
		gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = sortingLayer.sortingLayerName;
	}

	#endregion

	#region Movement

	// Checks if the charater has moved
	private bool CoroutineStarted = false;

	public void Movement (Vector3 destination, ExitAnimation exitAnimation)
	{
		if (CoroutineStarted) return;
		switch (exitAnimation) {
		case ExitAnimation.Fade:
			StartCoroutine (FadeAndMove (destination));
			break;
		case ExitAnimation.Smooth:
			StartCoroutine (Smooth (destination));
			break;
		case ExitAnimation.Teleport:
			StartCoroutine (Teleport (destination));
			break;
		default:
			throw new UnityException ("The Movement Function doesn't accept the parameter " + exitAnimation);
		}
	}

	public void FadeAndMoveSet(Vector3 destination) {
		if(!CoroutineStarted) StartCoroutine (FadeAndMove (destination));
	}

	public void LockMovement() {
		rb2D.velocity.Set (0, 0);
		this.AnimateMovement(false);
		this.lockMovement = true;
	}

	public void UnlockMovement() {
		this.lockMovement = false;
	}

	protected void AnimateMovement(bool isMoving) {
		int xDir = 0, yDir  = 0;
		
		if (isMoving) {
			if(mouseMovement) {
				if (destinationPosition.x > transform.position.x) xDir = 1;
				else if (destinationPosition.x < transform.position.x) xDir = -1;
				
				if (destinationPosition.y > transform.position.y) yDir = 1;
				else if (destinationPosition.y < transform.position.y) yDir = -1;
			}
			if(keyboardMovement) {
				if(moveVelocity.x > 0) xDir = 1;
				else if(moveVelocity.x < 0) xDir = -1;

				if(moveVelocity.y > 0) yDir = 1;
				else if(moveVelocity.y < 0) yDir = -1;
			}

			characterAnimator.SetBool (Animator.StringToHash ("IsActive"), false);
			characterAnimator.SetBool (Animator.StringToHash ("IsMoving"), true);
			
			if(xDir != 0) characterAnimator.SetFloat (Animator.StringToHash ("MoveSpeed-x"), xDir);
			if(yDir != 0) characterAnimator.SetFloat (Animator.StringToHash ("MoveSpeed-y"), yDir);
			
		} else {
			characterAnimator.SetBool (Animator.StringToHash ("IsActive"), true);
			characterAnimator.SetBool (Animator.StringToHash ("IsMoving"), false);
		}
	}

	private IEnumerator FadeAndMove (Vector3 destination)
	{
		CoroutineStarted = true;

		//FADE SCREEN INITIALLY
		while (fadeOut.alpha < 1) {
			fadeOut.alpha += Time.deltaTime;
			yield return null;
		}

		//MOVE PLAYER TO LOCATION
		TeleportToLocation (destination);

		//UNFADE SCREEN
		while (fadeOut.alpha > 0) {
			fadeOut.alpha -= Time.smoothDeltaTime;
			yield return null;
		}

		CoroutineStarted = false;
		yield break;
	}

	private IEnumerator Teleport (Vector3 destination)
	{
		yield break;
	}
	
	private IEnumerator Smooth (Vector3 destination)
	{
		yield break;
	}

	#endregion

	#region Inspection

	[HideInInspector]
	public bool frozen = false;

	[HideInInspector]
	public bool isInsideBuilding = false;

	[HideInInspector]
	public bool returnToGame = false;

	private float inspectRadius;
	private RaycastHit2D hit;
	private RaycastHit2D[] castStar;

	private Character dialogueCharacter;

	private void EntryDetection(RaycastHit2D hit) {
		EnterScript Entry;
		if(hit.collider.GetComponent<MacabreDoor>() != null) {
			Debug.Log ("Door Detected");
			Entry = hit.collider.GetComponent<MacabreDoor>();
			Entry.SendMessage("ObjectAction");
		} else if(hit.collider.GetComponent<MacabreStairs>() != null) {
			Debug.Log ("Stairs Detected");
			Entry = hit.collider.GetComponent<MacabreStairs>();
			Entry.SendMessage("ObjectAction");
		} else if(hit.collider.GetComponent<MacabrePortal>() != null) {
			Debug.Log ("Portal Detected");
			Entry = hit.collider.GetComponent<MacabrePortal>();
			Entry.SendMessage("ObjectAction");
		}
	}

	public void FindCharacterAndContinueConversation(int choice) {
		dialogueCharacter.SendMessage ("DialogueDecision", choice);
	}

	public T GetNearest <T> ()
		where T: Component {
		inspectRadius = Settings.searchRadius;
		castStar = Physics2D.CircleCastAll(transform.position, inspectRadius, Vector2.zero);

		if (castStar.Length == 0) {
			Debug.Log ("No objects within radius");
			return null;
		}
		for (int i = 0; i < castStar.Length; i++) {
			if (castStar [i].collider.tag != "Inspectable" &&
				castStar [i].collider.tag != "Entrance" &&
				castStar [i].collider.tag != "Character")
				continue;
			if (castStar [i].collider.gameObject.tag == "Player")
				continue;
			if (castStar [i].collider.GetComponentInChildren<T> () != null)
				return castStar [i].collider.GetComponentInChildren<T> ();
		}

		Debug.Log ("No objects within radius");
		return null;
	}

	public void Inspect() {
		if(Settings.debugINSPECT) Debug.Log ("1. Inspecting...");
		inspectRadius = Settings.searchRadius;
		castStar = Physics2D.CircleCastAll(transform.position, inspectRadius, Vector2.zero);

		for(int i = 0; i < castStar.Length; i++) {
			switch(castStar[i].collider.tag) {
			case "Inspectable":
				Debug.Log ("2. Object Detected -> " + castStar[i].collider.name);

				if(!returnToGame) {
					castStar[i].collider.GetComponent<MacabreObject>().SendMessage("ObjectAction", false);
					returnToGame = true;
				} else {
					castStar[i].collider.GetComponent<MacabreObject>().SendMessage("ObjectAction", true);
					returnToGame = false;
				}

				return;
			case "Entrance":
				Debug.Log("2. Entrance Detected");
				EntryDetection (castStar[i]);
				return;
			case "Character":
				if(!(castStar [i].collider is CircleCollider2D)) continue;
				Debug.Log ("2. Character Detected");
				dialogueCharacter = castStar [i].collider.GetComponent<Character> ();
				dialogueCharacter.DialogueAction();
				return;
			case "Pickable":
				Debug.Log ("2. Pickable Detected");
				var obj = castStar [i].collider.gameObject.GetComponent<MItem.Item> ();
				if(obj == null) throw new UnityException("GameObject"  + castStar[i].collider.gameObject.name + " is marked pickable, but doesn't contain MItem.Item class");
				AddToInventory (obj);
				return;
			}
		}
		castStar = null;
	}

	#endregion

	#region Conversation

	enum ConversationStatus {
		Start = 'S', CharacterResponse = 'R', PlayerClosedResponse = 'C', 
		PlayerOpenResponse = 'O', EndConversation = 'E' };
	//S = Start, R = Character Response, C = Player Closed Response, O = Player Open Response, E = End Conversation
	ConversationStatus CONVO_status_next = ConversationStatus.Start;
	ConversationStatus CONVO_status_current;

	singleConversation convo;
	string selectedMultipleReponseString;

	private void DialogueClear() {
		convo.conversation = "";
		convo.speaker = "";
		convo.spoken = "";
//		for (int i = 0; i < 4; i++)
//			convo.response [i] = "";
	}

	public void DialogueAction() {
		if(Settings.debugCONVO) Debug.Log ("3. Conversation between PLAYER & " + name);
		if (!DatabaseManager.main.CONVO_TestCharacterConversation (this.name)) {
			return;
		}
		player.LockMovement ();
		LockMovement ();

		Dialogue(0);
	}

	/// <summary>
	/// Chooses a decision, can be 1, 2, 3, 4 and outcome is dependent
	/// </summary>
	/// <param name="decision">Can only be 1, 2, 3, 4</param>
	public void DialogueDecision(int decision) {
		if (decision >= 5 || decision <= 0) {
			Debug.LogWarning("Decision out of scope, valid decisions 1, 2, 3, 4");
			return;
		}
		Debug.Log ("3. Player decision is " + decision);

		dialogueCharacter = GetNearest<Character> ();

		if (dialogueCharacter == null) {
			Debug.LogError ("Error: Character to reply not found");
			return;
		} else {
			if (dialogueCharacter.convo.response [decision - 1] == "" || dialogueCharacter.convo.response [decision - 1] == null) {
				Debug.LogWarning ((decision).ToString() + " is not an option");
			} else {
				InputManager.UnlockPendingDecision ();
				dialogueCharacter.Dialogue (decision);
			}
		}
	}

	public void Dialogue(int decision) {

		/* Decide the next conversation status */
		if (Settings.debugCONVO) {
			Debug.Log ("4. " + CONVO_status_current + " -> " + CONVO_status_next); 
			DatabaseManager.main.CONVO_printAllPrerequisites ();
		}

		// If current & next = END, then next = start
		if (CONVO_status_current == ConversationStatus.EndConversation &&
			CONVO_status_next == ConversationStatus.EndConversation) {
			CONVO_status_next = ConversationStatus.Start;

			UIManager.main.CloseDialogueScreen ();
			player.UnlockMovement ();
			UnlockMovement ();
			Debug.Log ("5. Conversation Ended");
			return;
		}

		// If it is a character reponse then close dialogue screen and continue
		if ((CONVO_status_current == ConversationStatus.CharacterResponse && CONVO_status_next != ConversationStatus.EndConversation) ||
			(CONVO_status_current == ConversationStatus.PlayerClosedResponse && CONVO_status_next != ConversationStatus.EndConversation)
		) {
			if (DatabaseManager.main.CONVO_checkEndConversations (name)) {
				CONVO_status_next = ConversationStatus.EndConversation;

			}
		}

		/* Clears the dialogue */
		DialogueClear ();

		/* Retrieves the correct message */
		if (CONVO_status_next == ConversationStatus.Start) {
			convo = DatabaseManager.main.CONVO_start (name);
		}
		if (CONVO_status_next == ConversationStatus.PlayerOpenResponse) {
			convo = DatabaseManager.main.CONVO_continue (name, decision, true);	// Decision goes here
		}
		if (CONVO_status_next == ConversationStatus.PlayerClosedResponse) {
			if (CONVO_status_current == ConversationStatus.PlayerOpenResponse) {
				convo = DatabaseManager.main.CONVO_finishresponse (convo, name, decision);
				if (Settings.debugCONVO)
					Debug.Log ("5. Last Decision is " + decision);
			} else {
				convo = DatabaseManager.main.CONVO_continue (name, decision, true);	// Decision goes here
			}
		}
		DatabaseManager.main.CONVO_printAllPrerequisites ();
		if (CONVO_status_next == ConversationStatus.CharacterResponse) {
			convo = DatabaseManager.main.CONVO_continue (name, decision, true);	// Decision goes here
		}
		if (CONVO_status_next == ConversationStatus.EndConversation) {
			convo = DatabaseManager.main.CONVO_continue (name, decision, true);	// Decision goes here
		}

		/* Displays the correct message */
		convo.print ();
		if (CONVO_status_next == ConversationStatus.PlayerOpenResponse) {
			InputManager.LockInspection ();
			InputManager.LockPendingDecision();
			UIManager.main.DisplayMultipleResponse (
				GameObject.FindGameObjectWithTag ("PlayerSprite").GetComponentInChildren<SpriteRenderer> ().sprite,
				convo.speaker,					// string characterName,
				convo.response,					// string textEntry,
				true 							// bool continueText
			);
		}
		if (CONVO_status_next == ConversationStatus.PlayerClosedResponse) {
			InputManager.UnlockInspection ();
			InputManager.UnlockPendingDecision();
			UIManager.main.DisplaySingleResponse (
				GameObject.FindGameObjectWithTag ("PlayerSprite").GetComponentInChildren<SpriteRenderer> ().sprite,
				convo.speaker,					// string characterName,
				convo.conversation,				// string textEntry,
				true 							// bool continueText
			);
		}
		if (CONVO_status_next == ConversationStatus.Start) {
			UIManager.main.DisplaySingleResponse (
				(convo.speaker == "Player") ?	// Sprite mainImageEntry,
				GameObject.FindGameObjectWithTag ("PlayerSprite").GetComponentInChildren<SpriteRenderer> ().sprite :
				this.GetComponentInChildren<SpriteRenderer> ().sprite,
				convo.speaker,					// string characterName,
				convo.conversation,				// string textEntry,
				true 							// bool continueText
			);
		}
		if (CONVO_status_next == ConversationStatus.CharacterResponse) {
			InputManager.UnlockInspection ();
			InputManager.UnlockPendingDecision ();
			UIManager.main.DisplayCharacterText (
				this.GetComponentInChildren<SpriteRenderer> ().sprite,
				convo.speaker,					// string characterName,
				convo.conversation,				// string textEntry,
				true 							// bool continueText
			);
		}
		if (CONVO_status_next == ConversationStatus.EndConversation) {
			UIManager.main.DisplaySingleResponse (
				(convo.speaker == "Player") ?	// Sprite mainImageEntry,
				GameObject.FindGameObjectWithTag ("PlayerSprite").GetComponentInChildren<SpriteRenderer> ().sprite :
				this.GetComponentInChildren<SpriteRenderer> ().sprite,
				convo.speaker,					// string characterName,
				convo.conversation,				// string textEntry,
				true 							// bool continueText
			);
		}

		CONVO_status_current = CONVO_status_next;

		/* Checks the next conversation */
		if (CONVO_status_current == ConversationStatus.Start) {
			if (convo.speaker == "Player") {
				CONVO_status_next = ConversationStatus.CharacterResponse;
			} else {
				if (convo.options == true) {
					CONVO_status_next = ConversationStatus.PlayerOpenResponse;
				} else {
					CONVO_status_next = ConversationStatus.PlayerClosedResponse;
				}
			}
		}
		if (CONVO_status_current == ConversationStatus.CharacterResponse) {
			if (convo.speaker == "Player") {
				CONVO_status_next = ConversationStatus.CharacterResponse;
			} else {
				if (convo.options == true) {
					CONVO_status_next = ConversationStatus.PlayerOpenResponse;
				} else {
					CONVO_status_next = ConversationStatus.PlayerClosedResponse;
				}
			}
		}
		if (CONVO_status_current == ConversationStatus.EndConversation) {
			CONVO_status_next = ConversationStatus.EndConversation;
		}
		if (CONVO_status_current == ConversationStatus.PlayerClosedResponse) {
			if (convo.speaker == "Player") {
				CONVO_status_next = ConversationStatus.CharacterResponse;
			} else {
				if (convo.options == true) {
					CONVO_status_next = ConversationStatus.PlayerOpenResponse;
				} else {
					CONVO_status_next = ConversationStatus.PlayerClosedResponse;
				}
			}
		}
		if (CONVO_status_current == ConversationStatus.PlayerOpenResponse) {
			CONVO_status_next = ConversationStatus.PlayerClosedResponse;
		}

		DatabaseManager.main.CONVO_printAllPrerequisites ();

	}

	private void SetupConversation() {
		convo = new singleConversation (false);
	}

	#endregion

	#region Inventory

	public virtual bool AddToInventory(MItem.Item m) {
		return false;
	}
	public bool CanPickUpObject = true;

	#endregion

	#region Death

	void deathAnimation() {
		characterAnimator.SetBool (Animator.StringToHash ("Die"), true);
	}

	void reviveAnimation() {
		characterAnimator.SetBool (Animator.StringToHash ("Die"), false);
	}

	#endregion

	#region State Actions

	void OnTriggerEnter2D(Collider2D other) {
		if(other.GetComponent<SpriteRenderer>() == null) return;
		var spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
		var otherSpriteRenderer = other.GetComponent<SpriteRenderer>();

		if(other.tag == "PlayerSprite" || other.tag == "CharacterSprite") {
			switch(gameObject.layer) {
			case 16:
				spriteRenderer.sortingLayerName = "Character - Front 2";
				otherSpriteRenderer.sortingLayerName = "Character - Front 1";
				break;
			default:
				spriteRenderer.sortingLayerName = "Character - Middle 2";
				otherSpriteRenderer.sortingLayerName = "Character - Middle 1";
				break;
			}
		}

		if(other.GetType() == typeof(PolygonCollider2D)) return;

		if(other.tag == "Object" || other.tag == "Inspectable" || other.tag == "Entrance") {
			if(spriteRenderer.sortingLayerName == "Character - Back 1") otherSpriteRenderer.sortingLayerName = "Objects - Back 1";
			else if(spriteRenderer.sortingLayerName == "Character - Back 2") otherSpriteRenderer.sortingLayerName = "Objects - Back 2";
			else if(spriteRenderer.sortingLayerName == "Character - Middle 1") otherSpriteRenderer.sortingLayerName = "Objects - Middle 1";
			else if(spriteRenderer.sortingLayerName == "Character - Middle 2") otherSpriteRenderer.sortingLayerName = "Objects - Middle 2";
			else if(spriteRenderer.sortingLayerName == "Character - Front 1") otherSpriteRenderer.sortingLayerName = "Objects - Front 1";
			else if(spriteRenderer.sortingLayerName == "Character - Front 2") otherSpriteRenderer.sortingLayerName = "Objects - Front 2";
		}
	}

	//Collision with things

	void OnCollisionEnter2D (Collision2D other) {
		isMoving = false;
		destinationPosition = transform.position;
		AnimateMovement(false);
	}

	void OnCollisionStay2D (Collision2D other) {
		isMoving = false;
		AnimateMovement(false);
	}

	#endregion

}

	