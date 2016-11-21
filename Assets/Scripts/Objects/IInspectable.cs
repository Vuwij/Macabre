using UnityEngine;

namespace Objects
{
    interface IInspectable
    {
        void InspectionAction(MacabreObjectController controller, RaycastHit2D hit);
    }
}
