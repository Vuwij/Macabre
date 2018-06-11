using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layer
{
    Front,
    World,
    Back
}

[System.Serializable]
public struct OtherVisibleRoom
{
    public PixelRoom room;
    public Layer layer;
}

public class PixelRoom : MonoBehaviour {

    new PolygonCollider2D collider2D;

    [HideInInspector]
    public Vector2 top, bottom, left, right;
    [HideInInspector]
    public Vector2[] colliderPoints;
    
	public OtherVisibleRoom[] otherVisibleRooms;

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

	public void OnEnable()
	{
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
		foreach(SpriteRenderer sr in spriteRenderers) {
			SetSortingLayer(Layer.World, sr);
		}

		foreach(OtherVisibleRoom room in otherVisibleRooms) {
			room.room.gameObject.SetActive(true);
			SpriteRenderer[] srs = room.room.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in srs) {
				SetSortingLayer(room.layer, sr);
            }
		}
	}

	public void OnDisable()
	{
		foreach (OtherVisibleRoom room in otherVisibleRooms) {
			room.room.gameObject.SetActive(false);
        }
	}


	void SetSortingLayer(Layer layer, SpriteRenderer sr) {
		if (layer == Layer.World)
		{
			if (sr.sortingLayerName == "Front - World")
				sr.sortingLayerName = "World";
			else if (sr.sortingLayerName == "Back - World")
				sr.sortingLayerName = "World";
			else if (sr.sortingLayerName == "Front - Foreground")
				sr.sortingLayerName = "Foreground";
			else if (sr.sortingLayerName == "Back - Foreground")
				sr.sortingLayerName = "Foreground";
			else if (sr.sortingLayerName == "Front - Background")
				sr.sortingLayerName = "Background";
			else if (sr.sortingLayerName == "Back - Background")
				sr.sortingLayerName = "Background";
		}
		else if (layer == Layer.Back) {
			if (sr.sortingLayerName == "Front - World")
				sr.sortingLayerName = "Back - World";
            else if (sr.sortingLayerName == "World")
				sr.sortingLayerName = "Back - World";
            else if (sr.sortingLayerName == "Foreground")
				sr.sortingLayerName = "Back - Foreground";
            else if (sr.sortingLayerName == "Back - Foreground")
				sr.sortingLayerName = "Back - Foreground";
            else if (sr.sortingLayerName == "Front - Background")
				sr.sortingLayerName = "Back - Background";
            else if (sr.sortingLayerName == "Background")
				sr.sortingLayerName = "Back - Background";
		}
		else if (layer == Layer.Front) {
			if (sr.sortingLayerName == "World")
				sr.sortingLayerName = "Front - World";
            else if (sr.sortingLayerName == "Back - World")
				sr.sortingLayerName = "Front - World";
            else if (sr.sortingLayerName == "Foreground")
				sr.sortingLayerName = "Front - Foreground";
            else if (sr.sortingLayerName == "Back - Foreground")
				sr.sortingLayerName = "Front - Foreground";
            else if (sr.sortingLayerName == "Background")
				sr.sortingLayerName = "Front - Background";
            else if (sr.sortingLayerName == "Back - Background")
				sr.sortingLayerName = "Front - Background";
		}
	}
}
