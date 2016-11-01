using UnityEngine;

namespace Objects.Movable
{
    public abstract partial class MovingObjectController : MacabreObjectController {
        protected Rigidbody2D rb2D
        {
            get { return gameObject.GetComponent<Rigidbody2D>(); }
        }

        protected override void Start()
        {
            base.Start();
            CreateCollisionCircle();
            CreateProximityCircle();
        }
    }
}
