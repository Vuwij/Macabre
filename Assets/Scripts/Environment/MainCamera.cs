using UnityEngine;
using System.Collections;
using System;
using Objects.Movable.Characters;

namespace Environment
{
    public class MainCamera : MonoBehaviour
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
                if(Objects.Movable.Characters.CharacterController.playerController != null)
                    return Objects.Movable.Characters.CharacterController.playerController.transform.position;
                return Vector2.zero;
            }
        }
        private Vector2 cameraPosition
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        private Vector2 destination;

        private void FollowPlayer()
        {
            destination = playerPosition;
            
            if (Vector2.Distance(playerPosition, destination) >= 0.5f)
                StartCoroutine(MoveCameraToPlayerPosition());
        }
        
        private IEnumerator MoveCameraToPlayerPosition()
        {
            while ((destination - cameraPosition).sqrMagnitude > 0.01f)
            {
                // Constrain the camera position

                // Move the camera to the desntiatoin
                cameraPosition = Vector2.MoveTowards(cameraPosition, destination, UnityEngine.Time.deltaTime * GameSettings.characterRunningSpeed);

                yield return null;
            }
            yield break;
        }
    }
}
