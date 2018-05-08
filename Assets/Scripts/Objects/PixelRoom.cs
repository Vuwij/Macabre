using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelRoom : MonoBehaviour {

    new PolygonCollider2D collider2D;

    [HideInInspector]
    public Vector2 top, bottom, left, right;
    [HideInInspector]
    public Vector2[] colliderPoints;

    void Awake()
    {
        collider2D = GetComponent<PolygonCollider2D>();

        Debug.Assert(collider2D != null);
        Debug.Assert(collider2D.points.Length == 4);
        colliderPoints = collider2D.points;

        top = colliderPoints[0];
        bottom = colliderPoints[0];
        left = colliderPoints[0];
        right = colliderPoints[0];

        for (int i = 0; i < 4; ++i)
        {
            if (colliderPoints[i].y > top.y)
                top = colliderPoints[i];
            if (colliderPoints[i].y < bottom.y)
                bottom = colliderPoints[i];
            if (colliderPoints[i].x < left.x)
                left = colliderPoints[i];
            if (colliderPoints[i].x > right.x)
                right = colliderPoints[i];
        }
    }
}
