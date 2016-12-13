using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Internal;

namespace UnityEngine
{
    [AddComponentMenu("Physics 2D/Ellipse Collider 2D")]
    public sealed class EllipseCollider2D : MonoBehaviour
    {
        private PolygonCollider2D polygonCollider = null;
        
        public Vector2[] points
        {
            get { return getPoints(); }
        }

        [Range(0, 25)]
        public float radiusX = 1, radiusY = 2;

        [Range(10, 90)]
        public int smoothness = 30;

        [Range(-180, 180)]
        public int rotation = 0;

        public bool isTrigger
        {
            get { return polygonCollider.isTrigger; }
            set { polygonCollider.isTrigger = value; }
        }

        public Vector2 offset = new Vector2(0.0f, 0.0f);

        public void Awake()
        {
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            polygonCollider.points = new Vector2[]{ };
            //polygonCollider.hideFlags = HideFlags.HideInInspector;
        }

        public void Start()
        {
            polygonCollider.points = getPoints();
        }

        public Vector2[] getPoints()
        {
            List<Vector2> points = new List<Vector2>();

            float ang = 0;
            float o = rotation * Mathf.Deg2Rad;

            for (int i = 0; i <= smoothness; i++)
            {
                float a = ang * Mathf.Deg2Rad;

                // https://www.uwgb.edu/dutchs/Geometry/HTMLCanvas/ObliqueEllipses5a.HTM
                float x = offset.x + radiusX * Mathf.Cos(a) * Mathf.Cos(o) - radiusY * Mathf.Sin(a) * Mathf.Sin(o);
                float y = offset.y - radiusX * Mathf.Cos(a) * Mathf.Sin(o) - radiusY * Mathf.Sin(a) * Mathf.Cos(o);

                points.Add(new Vector2(x, y));
                ang += 360f / smoothness;
            }

            return points.ToArray();
        }
        
        public static implicit operator Collider2D(EllipseCollider2D collider)
        {
            if (collider.polygonCollider.points.Length == 0) collider.polygonCollider.points = collider.getPoints();
            return collider.polygonCollider;
        }
    }
}