using UnityEngine;
using System.Collections;

public class MacabreStairs : EnterScript {

	[Header("Stair Settings")]
	public Vector3 upLocation;
	public Vector3 downLocation;
	
	protected override void Start () {
		base.Start ();
		InitialSettings ();
	}

	protected override void Update() {
		base.Update ();
	}

	protected override void InitialSettings() {
		downLocation += transform.position;
		upLocation += transform.position;
		
		targetLocation = upLocation;
		//Debug.DrawLine (upLocation, downLocation, Color.red, 10.0f);
	}

	protected override IEnumerator Entry() {
		
		//WAIT FOR PLAYER TO STOP
		yield return new WaitForSeconds(0.1f); //INITIALLY WAIT
		
		if(IsMoving) { //IF PLAYER IS MOVING THEN CONTINUE TO WAIT
			yield return new WaitForSeconds(0.5f);
			while(IsMoving) yield return null;
		} else yield return null;
		
		//CHECK DISTANCE
		if(CheckDistance()) yield break;
		
		if(CheckLock()) yield break;
		
		//FADE SCREEN INITIALLY
		while (fadeOut.alpha < 1) {
			fadeOut.alpha += Time.deltaTime;
			yield return null;
		}
		
		//HIDE OTHER COMPONENTS
		HideOtherComponents();
		
		//MOVE PLAYER
		MovePlayer();
		
		//CHANGE COLLIDERS
		if(useColliders) ChangeColliders ();
		Debug.Log ("waiting");
		//UNFADE SCREEN
		while (fadeOut.alpha > 0) {
			fadeOut.alpha -= Time.smoothDeltaTime;
			yield return null;
		}

		//SET PLAYER STATE
		SetPlayerState();

		//END THE COROUTINE
		coroutineStarted = false;
		yield break;
	}

	/****************** UNIQUE STAIR FUNCTIONS ******************/

	protected override bool CheckDistance() {
		Debug.Log ("Checking for Stairs... DownLocation = " + downLocation + " UpLocation = " + upLocation + 
		           " PlayerPosition = " + playerLocation);
		if(IsInside) targetLocation = upLocation;
		else targetLocation = downLocation;
		return base.CheckDistance();
	}

	protected override void MovePlayer() {
		if(IsInside) {
			targetLocation = downLocation;
			playerScript.ChangeLocation (player.layer - 1);
		} else {
			targetLocation = upLocation;
			playerScript.ChangeLocation (player.layer + 1);
		}

		base.MovePlayer ();
	}

	/****************** GIZMOS ******************/
	void OnDrawGizmos() {
		StairGizmos();
	}
	void OnDrawGizmosSelected() {
		StairGizmos();
	}

	void StairGizmos() {
		if(Application.isPlaying) {
			Gizmos.DrawIcon(downLocation, "Light Gizmo.tiff", true);
			Gizmos.DrawIcon(upLocation, "Light Gizmo.tiff", true);
			Gizmos.DrawLine (downLocation,upLocation);
		} else {
			Gizmos.DrawIcon(downLocation + transform.position, "Light Gizmo.tiff", true);
			Gizmos.DrawIcon(upLocation + transform.position, "Light Gizmo.tiff", true);
			Gizmos.DrawLine (downLocation + transform.position,upLocation + transform.position);
		}
	}
}
