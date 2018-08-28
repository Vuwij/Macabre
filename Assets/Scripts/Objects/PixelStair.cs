using System;

namespace Objects
{
	public class PixelStair : PixelDoor
    {
		public RampCollider ramp => GetComponentInChildren<RampCollider>();
    }
}
