using UnityEngine;

namespace Objects
{
	// This interface tells you an object can be inspected
	public interface IInspectable
    {
		void InspectionAction(Object controller, RaycastHit2D hit = new RaycastHit2D());
    }
}
