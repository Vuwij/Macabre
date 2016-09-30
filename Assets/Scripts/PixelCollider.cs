using UnityEngine;
using System.Collections;

public class PixelCollider : MonoBehaviour {

	private PolygonCollider2D polygonCollider2D;
	public Texture2D bitMap;

	public bool resetbitMap = false;

	void Awake () {
		if(!polygonCollider2D) {
			polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
		}

		if(!resetbitMap) {
			detectSeperateObjects();
		}

	}

	void detectSeperateObjects() {
		detectCircles();
		detectQuads();
		detectTriangles();
	}

	void detectCircles() {

	}

	void detectQuads() {
		for(int i = 0; i < bitMap.width; i++) {
			for(int j = 0; j < bitMap.height; j++) {

			}
		}
	}

	void detectTriangles() {

	}

	void addCollider() {

	}
}