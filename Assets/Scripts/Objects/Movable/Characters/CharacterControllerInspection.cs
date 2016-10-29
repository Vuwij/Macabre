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
            Dialogue();
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
                    Debug.Log(raycastHit.collider.name);
                    foreach (IInspectable obj in mObj)
                        obj.InspectionAction(raycastHit);
                    return;
                }
            }
        }

        // Cannot Raycast Hit anything on this opject
        public bool InspectionIsInvalid(RaycastHit2D raycastHit)
        {
            // If hit a character
            if (raycastHit.collider.GetComponent<CharacterController>() != null)
            {
                // Cannot hit itself
                if (raycastHit.collider.GetComponent<CharacterController>() == this) return true;
                
                // Can only get circle collider
                if (!(raycastHit.collider is CircleCollider2D)) return true;
            }   
            
            return false;
        }
    }
}