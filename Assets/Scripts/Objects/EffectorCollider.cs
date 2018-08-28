using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
	public class EffectorCollider : PixelCollider
    {
		public List<GameObject> enabledWhenInside = new List<GameObject>();
		public List<GameObject> disabledWhenInside = new List<GameObject>();

		public void OnEffectorEnter()
		{
			foreach (GameObject g in enabledWhenInside)
				g.SetActive(true);
			
			foreach (GameObject g in disabledWhenInside)
				g.SetActive(false);
		}

        public void OnEffectorExit()
		{
			foreach (GameObject g in enabledWhenInside)
				g.SetActive(false);

            foreach (GameObject g in disabledWhenInside)
				g.SetActive(true);
		}
	}
}
