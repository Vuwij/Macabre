using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using Objects;

public class MacabreFootprint : EditorWindow {
    
	[MenuItem ("Macabre/Object/Find Object Footprints")]
    static void Update () {

        foreach (GameObject obj in Selection.gameObjects) {
            MacabreObjectController[] objControllers = obj.GetComponentsInChildren<MacabreObjectController>();

            foreach (MacabreObjectController objController in objControllers)
            {
                if (objController == null) return;

                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer.sprite == null) return;

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
            footprint.Apply();
        }
    }
}