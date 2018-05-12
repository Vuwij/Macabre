using UnityEngine;
using UnityEditor;
using System.Collections;

public class MacabreFootprints : EditorWindow {
    
    static bool Validate () {
        // Check if there are any selected GameObject with more than one path
        foreach (GameObject lObj in Selection.gameObjects) {
            PolygonCollider2D lCollider = lObj.GetComponent<PolygonCollider2D>();
            if (lCollider!=null && lCollider.pathCount > 1) {
                return true;
            }
        }

        return false;
    }

	static void AddPath() {
		foreach (GameObject lObj in Selection.gameObjects) {
			PolygonCollider2D lCollider = lObj.GetComponent<PolygonCollider2D>();
			if (lCollider==null) {
				continue;
			}

			Vector2[] lPath = new Vector2[3];
			lPath[0].Set(0, 5);
			lPath[1].Set(5, 5);
			lPath[2].Set(5, 0);

			lCollider.pathCount++;
			lCollider.SetPath (lCollider.pathCount-1, lPath);

		}
	}

	[MenuItem ("Macabre/Collider/Clean Up Sprite Paths")]
	static void CleanUpSpritePaths() {
		foreach (GameObject lObj in Selection.gameObjects) {
			//Debug.Log (lObj);
			foreach(PolygonCollider2D pCollider2D in lObj.GetComponents<PolygonCollider2D>()) {

				Undo.RecordObject (pCollider2D, "Remove Interior Shapes");

				var points = pCollider2D.points;

				for(int i = 0; i < pCollider2D.points.Length; i++) {
					points[i].Scale(new Vector2(2.0f, 2.0f));
					points[i] = new Vector2(Mathf.Round(points[i].x), Mathf.Round(points[i].y));
					points[i].Scale(new Vector2(0.5f, 0.5f));
				}
				pCollider2D.points = points;
			}
		}
		foreach (GameObject lObj in Selection.gameObjects) {
			//Debug.Log (lObj);
			foreach(EdgeCollider2D eCollider2D in lObj.GetComponents<EdgeCollider2D>()) {
				
				Undo.RecordObject (eCollider2D, "Remove Interior Shapes");
				
				var points = eCollider2D.points;
				
				for(int i = 0; i < eCollider2D.points.Length; i++) {
					points[i].Scale(new Vector2(2.0f, 2.0f));
					points[i] = new Vector2(Mathf.Round(points[i].x), Mathf.Round(points[i].y));
					points[i].Scale(new Vector2(0.5f, 0.5f));
				}
				eCollider2D.points = points;
			}
		}
	}
    
    static void RemoveShapes () {
        foreach (GameObject lObj in Selection.gameObjects) {
            PolygonCollider2D lCollider = lObj.GetComponent<PolygonCollider2D>();
            if (lCollider==null) {
                continue;
            }

            // Allow undo action
            Undo.RecordObject (lCollider, "Remove Interior Shapes");


            // Get the shape that are more to the left than the others to take it as the exterior path
            int   lExteriorShape = 0;
            float lLeftmostPoint = Mathf.Infinity;

            Vector2[] lPath;

            for (int i=0, length=lCollider.pathCount; i<length ; ++i) {
                lPath = lCollider.GetPath(i);

                foreach (Vector2 lPoint in lPath) {
                    if (lPoint.x < lLeftmostPoint) {
                        lExteriorShape = i;
                        lLeftmostPoint = lPoint.x;
                    }
                }
            }

            // Initialize collider with exterior path
            lPath = lCollider.GetPath (lExteriorShape);

            // Set only the exterior path
            lCollider.pathCount = 1;
            lCollider.SetPath (0, lPath);
        }
    }
}