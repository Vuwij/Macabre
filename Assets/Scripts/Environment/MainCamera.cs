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

        private void Start()
        {
            if (main == null) main = this;
            else if (main != null) Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        private void Update() { FollowPlayer(); }

        private Vector2 playerPosition
        {
            get {
                if (Characters.playerController != null)
                    return Characters.playerController.transform.position;
                return Vector2.zero;
            }
        }
        private Vector2 cameraPosition
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        private Vector2 destination;

        private float CameraSpeed
        {
            get
            {
                if (Characters.playerController != null)
                    return Characters.playerController.movementSpeed;
                else
                    return GameSettings.cameraSpeed;
            }
        }

        public static void TeleportToPlayer()
        {
            Vector3 newPosition = new Vector3(
                main.playerPosition.x,
                main.playerPosition.y,
                -10);
            main.transform.position = newPosition;
        }

        private void FollowPlayer()
        {
            // Set the destination accordingly to the player according to the current object floor
            destination = playerPosition;
            
            // Slowly move towards the player
            if (Vector2.Distance(cameraPosition, destination) >= 0.01f)
                StartCoroutine(MoveCameraToPlayerPosition());
        }
        
        private IEnumerator MoveCameraToPlayerPosition()
        {
            while ((destination - cameraPosition).sqrMagnitude > 0.01f)
            {
                // Stop if game is paused
                if (GameManager.gamePaused) yield break;

                // Move the camera to the desntiatoin
                Vector2 delta = Vector2.MoveTowards(cameraPosition, destination, UnityEngine.Time.deltaTime * CameraSpeed * 0.005f);
                transform.position = new Vector3(delta.x, delta.y, -10);
                
                yield return null;
            }
            yield break;
        }
    }
}
