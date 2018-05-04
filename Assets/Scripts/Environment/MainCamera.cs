using UnityEngine;
using System.Collections;
using System;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data;

//Changes 
namespace Environment
{
    public sealed class MainCamera : MonoBehaviour
    {
        public static MainCamera main = null;

		Vector2 playerPosition
		{
			get {
				var p = GameObject.Find("Player");
				if (p == null) return Vector2.zero;
				else return p.transform.position;
			}
		}

		Vector2 destination;

        void Start()
        {
            if (main == null) main = this;
            else if (main != null) Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        void LateUpdate()
        {
            destination = playerPosition;
            Vector3 pos = transform.position;
            pos.x = playerPosition.x;
            pos.y = playerPosition.y;
            transform.position = pos;
        }
    }
}
