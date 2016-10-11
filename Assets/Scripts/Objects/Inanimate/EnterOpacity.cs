using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnterOpacity : MacabreThing {

	public class hiddenSpriteList{
		public SpriteRenderer hiddenSprite;
		public hiddenSpriteList(SpriteRenderer hideSprite) {
			hiddenSprite = hideSprite;
		}
	}

	public float Invisibility = 0.0f;
	public bool Unrereveal;
	public bool initialHideOther;
	public bool MakeInactive = false;

	public GameObject[] HideObjects, ShowObjects;
	protected List<hiddenSpriteList> hiddenList, showList;

	public bool changeColliders;
	protected PolygonCollider2D enter, exit;
	protected bool isInside = false;

	protected override void Awake() {
		base.Awake ();

		SetColliders();

		if(!MakeInactive) {
			hiddenList = new List<hiddenSpriteList>();
			showList = new List<hiddenSpriteList>();

			for(int i = 0; i < HideObjects.Length; i++) {
				foreach(SpriteRenderer spriteRenderer in HideObjects[i].GetComponentsInChildren<SpriteRenderer>()) {
					hiddenList.Add (new hiddenSpriteList(spriteRenderer));
				}
			}
			for(int i = 0; i < ShowObjects.Length; i++) {
				foreach(SpriteRenderer spriteRenderer in ShowObjects[i].GetComponentsInChildren<SpriteRenderer>()) {
					showList.Add (new hiddenSpriteList(spriteRenderer));
				}
			}
		}
	}

	void OnEnable() {
		if(Unrereveal) {
			//EventManager.exitbuilding += HideGameObjects;
		}
	}

	protected override void Start() {
		if(initialHideOther) HideGameObjects(true);
		base.Start ();
	}

	//COLLIDERS
	protected void SetColliders() {
		foreach(PolygonCollider2D pcollider in gameObject.GetComponents<PolygonCollider2D>()) {
			if(pcollider.enabled) enter = pcollider;
			if(!pcollider.enabled) exit = pcollider;
		}
	}

	protected void ChangeColliders() {
		if(isInside) {
			enter.enabled = true;
			exit.enabled = false;
		} else if (!isInside) {
			enter.enabled = false;
			exit.enabled = true;
		}
	}

	//ENTER AND EXIT
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "PlayerSprite") {
			isInside = true;
			if(changeColliders) ChangeColliders();
			if(MakeInactive) DeactivateGameObjects(true);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "PlayerSprite") {
			isInside = true;
			if(changeColliders) ChangeColliders();
			if(!MakeInactive) HideGameObjects(false);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "PlayerSprite") {
			isInside = false;
			if(changeColliders) ChangeColliders();
			if(MakeInactive) DeactivateGameObjects(false);
			else if(!Unrereveal) HideGameObjects (true);
		}
	}

	void HideGameObjects(bool enter_) {
		foreach(hiddenSpriteList i in hiddenList) {
			if(enter_) i.hiddenSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			else i.hiddenSprite.color = new Color(1.0f, 1.0f, 1.0f, Invisibility);
		}
		foreach(hiddenSpriteList i in showList) {
			if(enter_) i.hiddenSprite.color = new Color(1.0f, 1.0f, 1.0f, Invisibility);
			else i.hiddenSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	void DeactivateGameObjects(bool enter_) {

		for(int i = 0; i < HideObjects.Length; i++) {
			if(enter_) HideObjects[i].SetActive(false);
			else HideObjects[i].SetActive(true);
		}
		for(int i = 0; i < ShowObjects.Length; i++) {
			if(enter_) ShowObjects[i].SetActive(true);
			else ShowObjects[i].SetActive(false);
		}
	}
}
