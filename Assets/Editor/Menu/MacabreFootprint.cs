using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Objects;
using System.Linq;

public class MacabreFootprint : EditorWindow {
    
	[MenuItem ("Macabre/Object/Find Object Footprints")]
    static void FindObjectFootprints () {

        foreach (GameObject objs in Selection.gameObjects) {
			Objects.Object[] objss = objs.GetComponentsInChildren<Objects.Object>(true);

			foreach (Objects.Object o in objss)
            {
                if (o == null) continue;

                SpriteRenderer spriteRenderer = o.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                if (spriteRenderer.sprite == null) continue;

                Undo.RecordObject(o, "Set Object Footprint");

				// Find the footprint
				if (o.footprint == null)
				{
					string textureName = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);
					string textureNameWithFootprint = textureName.Replace(".png", "");
					string footprintTextureName = textureNameWithFootprint + "Footprint.png";
					if (!File.Exists(footprintTextureName)) return;

					// Create the footprint image
					o.footprint = (Texture2D)AssetDatabase.LoadAssetAtPath(footprintTextureName, typeof(Texture2D));
				}
            }
        }
    }

	static Color sharedColor = Color.black;

	static void CalculateFootprintPolygonCollider(SpriteRenderer spriteRenderer, Texture2D footprint)
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
			Color validColor = colorPoints.ElementAt(c).Key;
			for (int p = 0; p < width * height; p++)
			{
				bool valid = (colorMap[p] == validColor);
				if(colorMap[p] == Color.black) {
					// Right
					if (p + 2 < width * height && p + 2 >= 0 && colorMap[p + 2] == validColor)
						valid = true;
					else if (p + 1 < width * height && p + 1 >= 0 && colorMap[p + 1] == validColor)
						valid = true;
					else if (p - 2 < width * height && p - 2 >= 0 && colorMap[p - 2] == validColor)
						valid = true;
					else if (p - 1 < width * height && p - 1 >= 0 && colorMap[p - 1] == validColor)
						valid = true;
					else if (p + width < width * height && p + width >= 0 && colorMap[p + width] == validColor)
						valid = true;
					else if (p - width < width * height && p - width >= 0 && colorMap[p - width] == validColor)
						valid = true;
				}
				if (valid)
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

			topVector = topVector + new Vector2(1.0f, 0.5f);
			bottomVector = bottomVector + new Vector2(1.0f, 0.5f);
			leftVector = leftVector + new Vector2(1.0f, 0.5f);
			rightVector = rightVector + new Vector2(0.0f, 0.5f);

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

		if(colorPoints.Count == 0) return;
		var collider = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
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
		Dictionary<Color, Vector2> colorPoints = new Dictionary<Color, Vector2>();

		// Find the map
		for (int p = 0; p < width * height; p++) {
			if(colorMap[p] == Color.black) continue;
			if(colorMap[p].a == 0) continue;
			if(colorPoints.ContainsKey(colorMap[p])) continue;

			Vector2 point = new Vector2(p % width, p / width) - sprite.pivot + new Vector2(1.0f, 0.5f);
			colorPoints.Add(colorMap[p], point);
		}

		// Sort
		var orderedPoints = colorPoints.OrderBy(p => p.Key.grayscale).ToList();

		List<Vector2> orderedPoint2 = new List<Vector2>();
		foreach(var p in orderedPoints) {
			orderedPoint2.Add(p.Value);
		}
		orderedPoint2.Add(orderedPoint2.First());

		collider.points = orderedPoint2.ToArray();
	}
}