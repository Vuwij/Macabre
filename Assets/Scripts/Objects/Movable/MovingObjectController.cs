using UnityEngine;

namespace Objects.Movable
{
    public abstract class MovingObjectController : MacabreObjectController {

        protected BoxCollider2D boxCollider
        {
            get { return gameObject.GetComponent<BoxCollider2D>(); }
        }
        protected Rigidbody2D rb2D
        {
            get { return gameObject.GetComponent<Rigidbody2D>(); }
        }

        public abstract void CreateCollisionBox();
    }
}
