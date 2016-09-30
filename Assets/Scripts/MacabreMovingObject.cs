using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MacabreMovingObject : MacabreThing {
	protected Vector3 currentPosition;			// this transform
	protected Vector3 destinationPosition; // The destination Point
	public bool saveLock;

	protected float zAxis;
	protected float duration = 50.0f;
	protected bool isMoving;

	protected bool fadeObject = false;
	protected bool fadeScreen = false;
	
	private BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	private float inverseMoveTime;

	#region Setup

	protected override void Start() {
		base.Start ();

		rb2D = gameObject.GetComponent<Rigidbody2D>();

		currentPosition = transform.position;   // sets myTransform to this GameObject.transform
		destinationPosition = currentPosition;			// prevents myTransform reset
		zAxis = currentPosition.z;
	}

	#endregion

	#region SaveData
	public virtual void saveInfo ()
	{
		var g = GameData.main.Find<MData.MovingObject> (this);

		g.currentPosition = gameObject.transform.position;

	}
	public virtual void loadInfo ()
	{
		var g = GameData.main.Find<MData.MovingObject> (this);
		gameObject.transform.position = g.currentPosition;
		Debug.Log (gameObject.transform.position);
	}

	#endregion

	#region General Methods

	public virtual bool TeleportToLocation(Vector3 location) {
		destinationPosition = location;
		transform.position = location;

		return true;
	}
	
	public virtual bool MoveToLocation() {
		//MOVE GAMEOBJECT TO DESTINATION LOCATION
		if(isMoving && !Mathf.Approximately(transform.position.magnitude, destinationPosition.magnitude)){ 
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destinationPosition, 1/(duration*(Vector3.Distance(gameObject.transform.position, destinationPosition))));
		}
		
		//STOP MOVING IF ARRIVED
		else if(isMoving && Mathf.Approximately(gameObject.transform.position.magnitude, destinationPosition.magnitude)) {
			isMoving = false;
		}
		return false;
	}

	#endregion
}