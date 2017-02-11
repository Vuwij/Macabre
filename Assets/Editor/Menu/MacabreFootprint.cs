using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using Objects;
using Objects.Inanimate;

public class MacabreFootprint : EditorWindow {
    
	[MenuItem ("Macabre/Object/Find Object Footprints")]
    static void Update () {

        foreach (GameObject obj in Selection.gameObjects) {
            MacabreObjectController[] objControllers = obj.GetComponentsInChildren<MacabreObjectController>(true);

            foreach (MacabreObjectController objController in objControllers)
            {
                if (objController == null) continue;

                SpriteRenderer spriteRenderer = objController.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                if (spriteRenderer.sprite == null) continue;

                Undo.RecordObject(objController, "Set Object Footprint");
                FindFootprint(ref objController.footprint, spriteRenderer);
            }
        }
    }

    static void FindFootprint(ref Texture2D footprint, SpriteRenderer spriteRenderer)
    {
        if (footprint == null)
        {
            string textureName = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);
            string textureNameWithFootprint = textureName.Replace(".png", "");
            string footprintTextureName = textureNameWithFootprint + "Footprint.png";
            if (!File.Exists(footprintTextureName)) return;

            // Create the footprint image
            footprint = (Texture2D)AssetDatabase.LoadAssetAtPath(footprintTextureName, typeof(Texture2D));
        }
    }
    
    [MenuItem("Macabre/Object/Find Footprint Collider")]
    static void FindFootprintCollider()
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
            var mobj = obj.GetComponent<MacabreObjectController>();
            if (mobj == null) continue;

            if (obj.GetComponent<PolygonCollider2D>() != null) continue;
            var polygonCollider = obj.gameObject.AddComponent<PolygonCollider2D>();
            polygonCollider.points = GetVector2EdgesFromTexture(obj.GetComponent<SpriteRenderer>(), mobj.footprint);
        }
    }


    // Find the vector 2 from edges in collider
    static Vector2[] GetVector2EdgesFromTexture(SpriteRenderer spriteRenderer, Texture2D footprint)
    {
        // In here we create the collider circle by finding the points on the sprite
        Sprite sprite = spriteRenderer.sprite;
        Rect rect = sprite.rect;

        int x = Mathf.FloorToInt(rect.x);
        int y = Mathf.FloorToInt(rect.y);
        int width = Mathf.FloorToInt(rect.width);
        int height = Mathf.FloorToInt(rect.height);

        // Get the color map from the sprite Collider Shape
        Color[] colorMap = footprint.GetPixels(x, y, width, height);

        int topIndex = 0;
        int bottomIndex = 0;
        int leftIndex = 0;
        int rightIndex = 0;

        int xMax = 0;
        int xMin = 0;
        int yMax = 0;
        int yMin = 0;

        // Find a valid pixel
        for (int p = 0; p < width * height; p++)
        {
            if (colorMap[p].a != 0.0f)
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
            if (colorMap[p].a != 0.0f)
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

        Debug.DrawLine(topVector, leftVector);
        Debug.DrawLine(leftVector, bottomVector);
        Debug.DrawLine(bottomVector, rightVector);
        Debug.DrawLine(rightVector, topVector);

        return points;
    }

}