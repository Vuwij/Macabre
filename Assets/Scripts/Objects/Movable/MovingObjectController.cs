using UnityEngine;

namespace Objects.Movable
{
    public abstract class MovingObjectController : MacabreObjectController {
        new protected MovingObject mObject;

        protected BoxCollider2D boxCollider
        {
            get { return gameObject.GetComponent<BoxCollider2D>(); }
        }
        protected Rigidbody2D rb2D
        {
            get { return gameObject.GetComponent<Rigidbody2D>(); }
        }

        private float duration
        {
            get
            {
                return GameSettings.moveMovementSpeed;
            }
        }
        
        public void TeleportToLocation(Vector2 location)
        {
            mObject.destinationPosition = location;
            mObject.currentPosition = location;
            transform.position = location;
        }

        
        public void MoveToLocation()
        {
            if (!Mathf.Approximately(transform.position.magnitude, mObject.destinationPosition.magnitude))
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, mObject.destinationPosition, 1 / (duration * (Vector3.Distance(gameObject.transform.position, destinationPosition))));
        }
    }
}
