#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Extensions
{
    [AddComponentMenu("Physics 2D/Ellipse Collider 2D")]
    public sealed class EllipseCollider2D : Collider2D
    {
        public float radiusX = 1, radiusY = 2;

        [Range(10, 90)]
        public int smoothness = 30;
        
        private Vector2 center { get { return (Vector2)transform.position + offset; } }

        public new bool IsTouching(Collider2D collider2D)
        {
            collider2D.
            return true;
        }

        private Vector2[] points
        {
            get {
                List<Vector2> points = new List<Vector2>();

                float ang = 0;

                for (int i = 0; i <= smoothness; i++)
                {
                    float a = ang * Mathf.Deg2Rad;

                    // https://www.uwgb.edu/dutchs/Geometry/HTMLCanvas/ObliqueEllipses5a.HTM
                    float x = center.x + radiusX * Mathf.Cos(a) - radiusY * Mathf.Sin(a);
                    float y = center.y - radiusX * Mathf.Cos(a) - radiusY * Mathf.Sin(a);

                    points.Add(new Vector2(x, y));
                    ang += 360f / smoothness;
                }
                return points.ToArray();
            }
        }
    }
}
#endif