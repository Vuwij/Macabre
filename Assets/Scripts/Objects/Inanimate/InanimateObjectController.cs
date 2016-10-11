using UnityEngine;

namespace Objects
{
    public abstract class InamimateObjectController : MonoBehaviour {
        public MacabreObject MacabreObject;

        public void TeleportToLocation(Vector3 location)
        {
            destinationPosition = location;
            transform.position = location;
        }

        public void MoveToLocation()
        {
            if (isMoving && !Mathf.Approximately(transform.position.magnitude, destinationPosition.magnitude))
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destinationPosition, 1 / (duration * (Vector3.Distance(gameObject.transform.position, destinationPosition))));
            else if (isMoving && Mathf.Approximately(gameObject.transform.position.magnitude, destinationPosition.magnitude))
                isMoving = false;
        }
    }
}
