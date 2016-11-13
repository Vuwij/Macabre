using UnityEngine;

namespace Objects
{
    interface IInspectable
    {
        void InspectionAction(RaycastHit2D hit, int keyPressed);
    }
}
