using UnityEngine;
using System.Collections;
using System;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data;

namespace Environment
{
    public sealed class MainCamera : MonoBehaviour
    {
        public static MainCamera main = null;

		Player player {
			get {
				var p = GameObject.Find("Player");
				if (p == null) return null;
				else return p.GetComponent<Player>();
			}
		}
		Vector2 playerPosition
		{
			get {
				var p = GameObject.Find("Player");
				if (p == null) return Vector2.zero;
				else return p.transform.position;
			}
		}
		float speed
		{
			get
			{
				var p = GameObject.Find("Player");
				if (p == null) return GameSettings.cameraSpeed;
				return Vector2.Distance((Vector2) p.transform.position, (Vector2) transform.position);
			}
		}

		Vector2 destination;

        void Start()
        {
            if (main == null) main = this;
            else if (main != null) Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        void Update() { 
			destination = playerPosition;

			if (Vector2.Distance(transform.position, destination) >= 0.01f)
				StartCoroutine(MoveCameraToPlayerPosition());
		}

        IEnumerator MoveCameraToPlayerPosition()
        {
			while ((destination - (Vector2) transform.position).sqrMagnitude > 0.01f)
            {
                // Stop if game is paused
				if (Game.main.gamePaused) yield break;

                // Move the camera to the desntiatoin
                Vector2 delta = Vector2.MoveTowards(transform.position, destination, UnityEngine.Time.deltaTime * speed * 0.005f);
                transform.position = new Vector3(delta.x, delta.y, -10);
                
                yield return null;
            }
            yield break;
        }
    }
}
