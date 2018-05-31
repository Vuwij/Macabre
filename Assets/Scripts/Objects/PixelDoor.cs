using UnityEngine;

namespace Objects
{
	public class PixelDoor : MonoBehaviour
    {
        public PixelRoom destination;
        public Vector2 dropOffLocation; // Specified in pixels

		public Direction interactionDirection = Direction.All;

		private void OnDrawGizmos()
		{
#if UNITY_EDITOR
			Gizmos.color = Color.white;
			Gizmos.DrawIcon(transform.position + (Vector3)dropOffLocation, "Destination");
#endif
		}
	}
}