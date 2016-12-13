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
            ICustomCollider[] objContollers = obj.GetComponentsInChildren<ICustomCollider>();

            foreach (ICustomCollider objController in objContollers)
            {
                objController.CreateCollisionCircle();
            }
        }
    }
}