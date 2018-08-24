using UnityEngine;

namespace Objects
{
	public class PixelPoint
    {
		public static float DistanceOrthographic(Vector2 p1, Vector2 p2, Direction direction)
        {
            float m = 0.0f;
            Debug.Assert(direction != Direction.All);
            if (direction == Direction.NE)
                m = 0.5f;
            else if (direction == Direction.NW)
                m = -0.5f;
            else if (direction == Direction.SE)
                m = -0.5f;
            else if (direction == Direction.SW)
                m = 0.5f;

			float i1 = p1.y - p1.x * m; // Intercept 1
            float i2 = p2.y - p2.x * m; // Intercept 2
            float dist = (i2 - i1) / 2 / Mathf.Abs(m / Mathf.Sqrt(m * m + 1));
            return dist;
        }

		public static float DistanceOrthographicAbs(Vector2 p1, Vector2 p2, Direction direction) => Mathf.Abs(DistanceOrthographic(p1, p2, direction));

		public static Vector2 Shift(Vector2 p, Direction direction, float distance) {
			Debug.Assert(direction != Direction.All && direction != Direction.NEandSW && direction != Direction.NWandSE);

			Vector2 pp = new Vector2(p.x, p.y);

			if (direction == Direction.NE)
			{
				pp.x = p.x + distance / Mathf.Sqrt(5);
				pp.y = p.y +  distance / Mathf.Sqrt(5) / 2;
			}
			else if (direction == Direction.NW)
			{
				pp.x = p.x + distance / Mathf.Sqrt(5);
                pp.y = p.y - distance / Mathf.Sqrt(5) / 2;
			}
			else if (direction == Direction.SE)
            {
                pp.x = p.x - distance / Mathf.Sqrt(5);
                pp.y = p.y + distance / Mathf.Sqrt(5) / 2;
            }
			else if (direction == Direction.SW)
            {
                pp.x = p.x - distance / Mathf.Sqrt(5);
                pp.y = p.y - distance / Mathf.Sqrt(5) / 2;
            }
			return pp;
		}
    }
}
