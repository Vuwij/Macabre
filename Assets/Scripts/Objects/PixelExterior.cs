using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{   
	public class PixelExterior : MonoBehaviour
	{
		public OtherVisibleRoom[] otherVisibleRooms;

		public List<PixelDoor> pixelDoors
        {
            get
            {
                List<PixelDoor> doors = new List<PixelDoor>();

                for (int i = 0; i < transform.childCount; ++i)
                {
                    Transform t = transform.GetChild(i);
                    PixelDoor door = t.GetComponent<PixelDoor>();
                    if (door != null)
                    {
                        doors.Add(door);
                    }
                }

                return doors;
            }
        }

		public void ShowAllRooms()
        {
			foreach (OtherVisibleRoom room in otherVisibleRooms)
			{
				// Show all objects
				for (int i = 0; i < room.room.transform.childCount; ++i)
				{
					Transform obj = room.room.transform.GetChild(i);

					SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
					if (sr != null && sr.name != "Footprint")
					{
						Color c = sr.color;
						c.a = 1.0f;
						sr.color = c;

						foreach (SpriteRenderer srchild in sr.GetComponentsInChildren<SpriteRenderer>())
						{
							if (srchild != null && srchild.name != "Footprint")
							{
								Color ccc = srchild.color;
								ccc.a = 0.8f;
								srchild.color = ccc;
							}
						}
					}
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
				for (int i = 0; i < room.room.transform.childCount; ++i) {
					Transform obj = room.room.transform.GetChild(i);
                    
					SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
					if (sr != null) {
						Color c = sr.color;
                        c.a = 0;
                        sr.color = c;

						foreach (SpriteRenderer srchild in sr.GetComponentsInChildren<SpriteRenderer>())
                        {
                            if (srchild != null)
                            {
                                Color ccc = srchild.color;
                                ccc.a = 0;
                                srchild.color = ccc;
                            }
                        }
					}               
				}

                // Except the room
                SpriteRenderer spriteRenderer = room.room.GetComponent<SpriteRenderer>();
                Color cc = spriteRenderer.color;
				cc.a = GetVisibilityInFront();
                spriteRenderer.color = cc;
            }
		} 
       
		float GetVisibilityInFront() {
			for (int i = 0; i < transform.childCount; ++i)
			{
				Transform t = transform.GetChild(i);
				MultiBodyPixelCollider multiBodyPixelCollider = t.GetComponent<MultiBodyPixelCollider>();
				if(multiBodyPixelCollider != null)
				{
					return multiBodyPixelCollider.visibilityInFront;
				}
			}
			throw new System.Exception("No Multibody Pixel Collider for " + this.name);
		}
	}
}