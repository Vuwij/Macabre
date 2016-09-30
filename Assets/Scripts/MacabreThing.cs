using UnityEngine;
using System.Collections;

/// <summary>
/// Anything related to the macabre object
/// </summary>
public abstract class MacabreThing : MonoBehaviour {

	public bool initialHide = false;

	protected virtual void Awake () {
		if(initialHide) gameObject.SetActive(false);
	}

	protected virtual void Start() {
	}

}
