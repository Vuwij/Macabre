using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController, IInspectable
    {
        private float inspectRadius
        {
            get { return GameSettings.inspectRadius; }
        }
        private RaycastHit2D hit;
        
        public void InspectionAction(RaycastHit2D raycastHit)
        {
            StartConversation();
        }

        public void Inspect()
        {
            RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, inspectRadius, Vector2.zero);
            foreach (RaycastHit2D raycastHit in castStar)
            {
                if (InspectionIsInvalid(raycastHit)) continue;

                IInspectable[] mObj = raycastHit.collider.GetComponentsInChildren<IInspectable>();
                if (mObj.Length != 0)
                {
                    foreach (IInspectable obj in mObj)
                        obj.InspectionAction(raycastHit);
                    return;
                }
            }
        }

        public bool InspectionIsInvalid(RaycastHit2D raycastHit)
        {
            if (raycastHit.collider.GetComponent<Character>() != null)
                if (!(raycastHit.collider is CircleCollider2D)) return false;
            return true;
        }
    }
}