using UnityEngine;
using System.Collections;

public class DisableAtStart : MonoBehaviour {

	void Awake () {
		gameObject.SetActive (false);
	}
}
