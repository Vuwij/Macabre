using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Objects;
using System.Linq;
using Objects.Immovable.Rooms;

public class MacabreFootprint : EditorWindow {
    
	[MenuItem ("Macabre/Object/Find Object Footprints")]
    static void Update () {

        foreach (GameObject obj in Selection.gameObjects) {
			Objects.Object[] objControllers = obj.GetComponentsInChildren<Objects.Object>(true);

			foreach (Objects.Object objController in objControllers)
            {
                if (objController == null) continue;

                SpriteRenderer spriteRenderer = objController.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                if (spriteRenderer.sprite == null) continue;

                Undo.RecordObject(objController, "Set Object Footprint");

				// Find the footprint
				if (objController.footprint == null)
				{
					string textureName = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);
					string textureNameWithFootprint = textureName.Replace(".png", "");
					string footprintTextureName = textureNameWithFootprint + "Footprint.png";
					if (!File.Exists(footprintTextureName)) return;

					// Create the footprint image
					objController.footprint = (Texture2D)AssetDatabase.LoadAssetAtPath(footprintTextureName, typeof(Texture2D));
				}
            }
        }
    }
    
    [MenuItem("Macabre/Object/Find Footprint Collider")]
    static void FindFootprintCollider()
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
			var mobj = obj.GetComponent<Objects.Object>();
            if (mobj == null) continue;


			if(mobj is Room) {
				if (obj.GetComponent<EdgeCollider2D>() != null) continue;
				var polygonCollider = obj.gameObject.AddComponent<EdgeCollider2D>();
				CalculateFootprintEdgeCollider(obj.GetComponent<SpriteRenderer>(), mobj.footprint, polygonCollider);
			}
			else {
				if (obj.GetComponent<PolygonCollider2D>() != null) continue;
				var polygonCollider = obj.gameObject.AddComponent<PolygonCollider2D>();
				CalculateFootprintPolygonCollider(obj.GetComponent<SpriteRenderer>(), mobj.footprint, polygonCollider);
			}
        }
    }

	static Color sharedColor = Color.black;

	static void CalculateFootprintPolygonCollider(SpriteRenderer spriteRenderer, Texture2D footprint, PolygonCollider2D collider)
    {
        Sprite sprite = spriteRenderer.sprite;
        Rect rect = sprite.rect;
		int x = Mathf.FloorToInt(rect.x);
        int y = Mathf.FloorToInt(rect.y);
        int width = Mathf.FloorToInt(rect.width);
        int height = Mathf.FloorToInt(rect.height);
		Color[] colorMap = footprint.GetPixels(x, y, width, height);

		// Colors we are finding
		Dictionary<Color, Vector2[]> colorPoints = new Dictionary<Color, Vector2[]>();

		for (int p = 0; p < width * height; p++) {
			if(colorMap[p] == sharedColor) continue;
			if(colorMap[p].a == 0) continue;
			if(colorPoints.ContainsKey(colorMap[p])) continue;
			colorPoints.Add(colorMap[p], null);
		}

		for(int c = 0; c < colorPoints.Count; c++) {
			int topIndex = 0;
			int bottomIndex = 0;
			int leftIndex = 0;
			int rightIndex = 0;

			int xMax = 0;
			int xMin = 0;
			int yMax = 0;
			int yMin = 0;

			// Find a first valid pixel
			for (int p = 0; p < width * height; p++)
			{
				if (colorMap[p] == colorPoints.ElementAt(c).Key)
				{
					int i = p % width;
					int j = p / width;

					rightIndex = p;
					leftIndex = p;
					bottomIndex = p;
					topIndex = p;

					xMax = i;
					xMin = i;
					yMax = j;
					yMin = j;

					break;
				}
			}

			// Loop through all the pixels and get the 4 directional indices
			for (int p = 0; p < width * height; p++)
			{
				if (colorMap[p] == colorPoints.ElementAt(c).Key)
				{
					int i = p % width;
					int j = p / width;

					if (i > xMax)
					{
						rightIndex = p;
						xMax = i;
					}
					else if (i < xMin)
					{
						leftIndex = p;
						xMin = i;
					}
					if (j > yMax)
					{
						bottomIndex = p;
						yMax = j;
					}
					else if (j < yMin)
					{
						topIndex = p;
						yMin = j;
					}
				}
			}

			// Create the collider2D based on the index, since its one pixel to one index
			Vector2 topVector = new Vector2(topIndex % width, topIndex / width) - sprite.pivot;
			Vector2 bottomVector = new Vector2(bottomIndex % width, bottomIndex / width) - sprite.pivot;
			Vector2 leftVector = new Vector2(leftIndex % width, leftIndex / width) - sprite.pivot;
			Vector2 rightVector = new Vector2(rightIndex % width, rightIndex / width) - sprite.pivot;

			Vector2[] points = new Vector2[5]
			{
				topVector,
				rightVector,
				bottomVector,
				leftVector,
				topVector
			};

			colorPoints[colorPoints.ElementAt(c).Key] = points;
		}

		collider.pathCount = colorPoints.Count;

		int index = 0;
		foreach(var c in colorPoints) {
			collider.SetPath(index, c.Value);
			index++;
		}
    }

	static void CalculateFootprintEdgeCollider(SpriteRenderer spriteRenderer, Texture2D footprint, EdgeCollider2D collider) {
		Sprite sprite = spriteRenderer.sprite;
		Rect rect = sprite.rect;
		int x = Mathf.FloorToInt(rect.x);
		int y = Mathf.FloorToInt(rect.y);
		int width = Mathf.FloorToInt(rect.width);
		int height = Mathf.FloorToInt(rect.height);
		Color[] colorMap = footprint.GetPixels(x, y, width, height);

		// Colors we are finding
		Dictionary<Color, Vector2[]> colorPoints = new Dictionary<Color, Vector2[]>();

		for (int p = 0; p < width * height; p++) {
			if(colorMap[p] == sharedColor) continue;
			if(colorMap[p].a == 0) continue;
			if(colorPoints.ContainsKey(colorMap[p])) continue;
			colorPoints.Add(colorMap[p], null);
		}

		for(int c = 0; c < colorPoints.Count; c++) {
			int topIndex = 0;
			int bottomIndex = 0;
			int leftIndex = 0;
			int rightIndex = 0;

			int xMax = 0;
			int xMin = 0;
			int yMax = 0;
			int yMin = 0;

			// Find a first valid pixel
			for (int p = 0; p < width * height; p++)
			{
				if (colorMap[p] == colorPoints.ElementAt(c).Key)
				{
					int i = p % width;
					int j = p / width;

					rightIndex = p;
					leftIndex = p;
					bottomIndex = p;
					topIndex = p;

					xMax = i;
					xMin = i;
					yMax = j;
					yMin = j;

					break;
				}
			}

			// Loop through all the pixels and get the 4 directional indices
			for (int p = 0; p < width * height; p++)
			{
				if (colorMap[p] == colorPoints.ElementAt(c).Key)
				{
					int i = p % width;
					int j = p / width;

					if (i > xMax)
					{
						rightIndex = p;
						xMax = i;
					}
					else if (i < xMin)
					{
						leftIndex = p;
						xMin = i;
					}
					if (j > yMax)
					{
						bottomIndex = p;
						yMax = j;
					}
					else if (j < yMin)
					{
						topIndex = p;
						yMin = j;
					}
				}
			}

			// Create the collider2D based on the index, since its one pixel to one index
			Vector2 topVector = new Vector2(topIndex % width, topIndex / width) - sprite.pivot;
			Vector2 bottomVector = new Vector2(bottomIndex % width, bottomIndex / width) - sprite.pivot;
			Vector2 leftVector = new Vector2(leftIndex % width, leftIndex / width) - sprite.pivot;
			Vector2 rightVector = new Vector2(rightIndex % width, rightIndex / width) - sprite.pivot;

			Vector2[] points = new Vector2[5]
			{
				topVector,
				rightVector,
				bottomVector,
				leftVector,
				topVector
			};

			colorPoints[colorPoints.ElementAt(c).Key] = points;
		}

		collider.points = colorPoints.ElementAt(0).Value;
	}
}