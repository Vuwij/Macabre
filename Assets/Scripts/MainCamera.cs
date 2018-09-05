using UnityEngine;
using System.Collections;
using System;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data;

public sealed class MainCamera : MonoBehaviour
{
    public static MainCamera main = null;

    Vector3 destinationPosition
    {
        get
        {
			Vector3 position;
			var p = GameObject.Find(followObject);
			if (p == null) position = transform.position;
			else position = p.transform.position;
			position.z = -10;
			return position;
        }
    }

	public string followObject = "Player";

    void Start()
    {
        if (main == null) main = this;
        else if (main != null) Destroy(gameObject);
        DontDestroyOnLoad(this);

		GameObject go = GameObject.Find(followObject);
    }

    void LateUpdate()
    {
		gameObject.transform.position = destinationPosition;
    }
}