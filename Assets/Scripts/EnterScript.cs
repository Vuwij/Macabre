using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class EnterScript: MonoBehaviour {

	//GAMEOBJECTS AND COMPONENTS
	protected CanvasGroup fadeOut;
	protected GameObject player;
	protected CharacterPlayer playerScript;

	//CHANGE LAYER AND LOCATION SETTINGS
	[Header("Change Physical Locations")]
	public bool changeLocations = false;
	public string fromLocation, toLocation;
	
	public enum EntryFloor : int {
		Underground_3 = 8, Underground_2 = 9, Underground_1 = 10,
		Floor_1 = 11, Floor_2 = 12, Floor_3 = 13, Floor_4 = 14, Outside = 16,
	};
	public EntryFloor entryfloor;
	protected int fromLayer;

	//DETECTION DISTANCE
	public float detectionDistance = 1.0f;
	protected Vector3 playerLocation, targetLocation;

	//CHANGE HIDDEN COMPONENTS
	[Header("Hidden Components")]
	public GameObject[] HideComponents;
	public GameObject[] ShowComponents;

	//IF USE COLLIDERS
	public bool useColliders = false;
	protected PolygonCollider2D enter, exit;

	//TRIGGERS
	protected bool coroutineStarted;	//If the Coroutine has started
	protected bool IsInside = false;	//If the player has moved inside
	public bool IsReverse = false;		//If the door or stairs is reversed
	protected bool IsMoving;
	/**
	 * LIST OF FUNCTIONS
	 * 
	 * Start() - Get plyaerScript, fadeOut and playerScript
	 * SetColliders() - Set Colliders if useColliders is true
	 * InitialSettings() - Initial Settings for each type of object
	 * Update() - PlayerLocation, IsMoving and DoorFade Updates
	 * ClickAction() - If the Object is clicked
	 */

	protected virtual void Start() {
		if(useColliders) SetColliders ();

		player = GameObject.FindWithTag("Player");
		playerScript = player.GetComponent<CharacterPlayer>();
		fadeOut = GameObject.Find ("Fade Screen").GetComponent<CanvasGroup>();
	}

	protected abstract void InitialSettings();

	protected void SetColliders() {
		foreach(PolygonCollider2D pcollider in gameObject.GetComponents<PolygonCollider2D>()) {
			if(pcollider.enabled) enter = pcollider;
			if(!pcollider.enabled) exit = pcollider;
		}
	}

	protected virtual void Update () {
		playerLocation = player.transform.position;
		try { IsMoving = player.GetComponentInChildren<Animator>().GetBool ("IsMoving"); } catch {}
	}

	protected void ObjectAction() {
		if(!coroutineStarted) {
			StartCoroutine (Entry());
			coroutineStarted = true;
		}
	}

	/***** List of ENTRY FUNCTIONS *****/

	protected abstract IEnumerator Entry();

	protected virtual bool CheckDistance() {
		if(Vector3.Distance(playerLocation, targetLocation) > detectionDistance) {
			coroutineStarted = false;
			return true;
		}

		Debug.Log ("Entry Success");
		return false;
	}

	protected virtual bool CheckLock() {
		return false;
	}
	
	protected virtual void HideOtherComponents() {
		if(!IsInside) {
			for(int i = 0; i < HideComponents.Length; i++) HideComponents[i].SetActive(false); //Hide Exterior
			for(int i = 0; i < ShowComponents.Length; i++) ShowComponents[i].SetActive(true); //Show Interior
		} else if (IsInside) {
			for(int i = 0; i < HideComponents.Length; i++) HideComponents[i].SetActive(true); //Hide Exterior
			for(int i = 0; i < ShowComponents.Length; i++) ShowComponents[i].SetActive(false); //Show Interior
		}
	}

	protected virtual void MovePlayer() {
		playerScript.TeleportToLocation(targetLocation);
		playerScript.frozen = false; //UNFREEZES THE PLAYER
	}
	
	internal void ChangeColliders() {
		if(!useColliders) return;
		if(IsInside) {
			enter.enabled = true;
			exit.enabled = false;
		} else if (!IsInside) {
			enter.enabled = false;
			exit.enabled = true;
		}
	}

	protected void SetPlayerState() {
		if(IsInside) IsInside = false;
		else IsInside = true;
	}
}

#if UNITY_EDITOR
#endif
