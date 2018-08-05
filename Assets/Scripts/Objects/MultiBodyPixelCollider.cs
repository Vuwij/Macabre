using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
	public class MultiBodyPixelCollider : PixelCollider
	{   
		public CollisionBody[] collisionBodies;
		public CollisionBody[] collisionBodiesP;

		public CollisionBody[] collisionBodiesWorld {
			get {
				CollisionBody[] bodies = collisionBodies;
				for (int i = 0; i < bodies.Length; ++i) {
					bodies[i].top += (Vector2) transform.position;
					bodies[i].bottom += (Vector2) transform.position;
					bodies[i].left += (Vector2) transform.position;
					bodies[i].right += (Vector2) transform.position;
				}
				return bodies;
			}
		}

		protected override void Awake()
		{
			if (noCollision) return;

            collider2D = GetComponent<PolygonCollider2D>();

            Debug.Assert(collider2D != null);
			for (int i = 0; i < collider2D.pathCount; ++i) {
				Debug.Assert(collider2D.GetPath(i).Length == 4);
			}
            Debug.Assert(transform.parent.GetComponent<PixelRoom>() == null);
            Debug.Assert(transform.parent.GetComponent<PolygonCollider2D>() == null);

			collisionBodies = new CollisionBody[collider2D.pathCount];
			for (int i = 0; i < collider2D.pathCount; ++i)
			{
				collisionBodies[i].top = collider2D.GetPath(i)[0];
				collisionBodies[i].bottom = collider2D.GetPath(i)[0];
				collisionBodies[i].left = collider2D.GetPath(i)[0];
				collisionBodies[i].right = collider2D.GetPath(i)[0];
			}

			for (int j = 0; j < collider2D.pathCount; ++j)
			{
				for (int i = 0; i < 4; ++i)
                {
					if (collider2D.GetPath(j)[i].y > collisionBodies[j].top.y)
						collisionBodies[j].top = collider2D.GetPath(j)[i];
					if (collider2D.GetPath(j)[i].y < collisionBodies[j].bottom.y)
						collisionBodies[j].bottom = collider2D.GetPath(j)[i];
					if (collider2D.GetPath(j)[i].x < collisionBodies[j].left.x)
						collisionBodies[j].left = collider2D.GetPath(j)[i];
					if (collider2D.GetPath(j)[i].x > collisionBodies[j].right.x)
						collisionBodies[j].right = collider2D.GetPath(j)[i];
                }
			}

			for (int i = 0; i < collider2D.pathCount; ++i) {
				collisionBodies[i].top += collider2D.offset;
				collisionBodies[i].bottom += collider2D.offset;
				collisionBodies[i].left += collider2D.offset;
				collisionBodies[i].right += collider2D.offset;
			}

			collisionBodiesP = new CollisionBody[collider2D.pathCount];
			for (int i = 0; i < collider2D.pathCount; ++i) {
				collisionBodiesP[i].top = collisionBodies[i].top + new Vector2(0, pixelProximity);
				collisionBodiesP[i].bottom = collisionBodies[i].bottom + new Vector2(0, -pixelProximity);
				collisionBodiesP[i].left = collisionBodies[i].left + new Vector2(-2 * pixelProximity, 0);
				collisionBodiesP[i].right = collisionBodies[i].right + new Vector2(2 * pixelProximity, 0);
			}
		}
	}
}