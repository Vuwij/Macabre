using UnityEngine;
using System.Collections;

namespace Objects {
	public class PixelLine
    {
		public Vector2 p1;
		public Vector2 p2;

		public PixelLine(Vector2 p1, Vector2 p2) {
			this.p1 = p1;
			this.p2 = p2;
		}

		public bool AboveSegment(PixelBox body)
		{
			return DistanceOrthographic(this, body.center) > 0;
		}

		public static float Distance(PixelLine l1, PixelLine l2) {
			float m = (l1.p2.y - l1.p1.y) / (l1.p2.x - l1.p1.x); // Slope of parallel lines
            float i1 = l1.p1.y - l1.p1.x * m;                    // Intercept 1
            float i2 = l2.p1.y - l2.p1.x * m;                   // Intercept 2
            float dist = (i2 - i1) / Mathf.Sqrt(m * m + 1);
            return dist;
		}

		public static float DistanceOrthographic(PixelLine l1, PixelLine l2) {
			float m = (l1.p2.y - l1.p1.y) / (l1.p2.x - l1.p1.x); // Slope of parallel lines
            float i1 = l1.p1.y - l1.p1.x * m; // Intercept 1
            float i2 = l2.p1.y - l2.p1.x * m; // Intercept 2
            float dist = (i2 - i1) / 2 / Mathf.Abs(m / Mathf.Sqrt(m * m + 1));
            return dist;
        }

		public static float DistanceOrthographic(PixelLine line, Vector2 pt) {
			float m = (line.p2.y - line.p1.y) / (line.p2.x - line.p1.x); // Slope of parallel lines
			float i1 = line.p1.y - line.p1.x * m; // Intercept 1
            float i2 = pt.y - pt.x * m; // Intercept 2
            float dist = (i2 - i1) / 2 / Mathf.Abs(m / Mathf.Sqrt(m * m + 1));
            return dist;
		}

		public void Draw(Color color, float duration) {
			Debug.DrawLine(p1, p2, color, duration);
		}
     }
}