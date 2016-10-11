using UnityEngine;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {
        protected MacabreObject mObject;

        protected T GetNearestMacabreObject<T>()
            where T : MacabreObjectController
        {
            RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, GameSettings.inspectRadius, Vector2.zero);

            foreach (RaycastHit2D raycastHit in castStar)
            {
                T hit = raycastHit.collider.GetComponentInChildren<T>();
                if (hit != null) return hit;
            }

            Debug.Log("No objects within radius");
            return null;
        }
    }
}
