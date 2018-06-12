using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{   
	public class PixelExterior : MonoBehaviour
	{
		public OtherVisibleRoom[] otherVisibleRooms;

		public void ShowAllRooms()
        {
			foreach (OtherVisibleRoom room in otherVisibleRooms)
			{
				// Show all objects
                foreach (SpriteRenderer sr in room.room.GetComponentsInChildren<SpriteRenderer>())
                {
                    Color c = sr.color;
					if (sr.name != "Footprint" && !sr.name.Contains("Shadow"))
						c.a = 1.0f;
					else
						continue;
                    sr.color = c;
                }
                // Except the room
                SpriteRenderer spriteRenderer = room.room.GetComponent<SpriteRenderer>();
                Color cc = spriteRenderer.color;
                cc.a = 1.0f;
                spriteRenderer.color = cc;
			}
        }

		public void HideAllRooms() {
			foreach (OtherVisibleRoom room in otherVisibleRooms)
            {
				// Hide all objects
                foreach (SpriteRenderer sr in room.room.GetComponentsInChildren<SpriteRenderer>())
                {
					if (sr.name == "Footprint" || sr.name.Contains("Shadow"))
                        continue; 
					Color c = sr.color;
                    c.a = 0;
                    sr.color = c;
                }
                // Except the room
                SpriteRenderer spriteRenderer = room.room.GetComponent<SpriteRenderer>();
                Color cc = spriteRenderer.color;
                cc.a = 0.3f;
                spriteRenderer.color = cc;
            }
		}      
	}
}