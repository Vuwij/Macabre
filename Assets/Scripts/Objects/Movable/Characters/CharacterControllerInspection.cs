using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Conversations;
using Objects.Inanimate.Items;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController, IInspectable
    {
        private float inspectRadius
        {
            get { return GameSettings.inspectRadius; }
        }
        private RaycastHit2D hit;
        
        public void InspectionAction(MacabreObjectController obj, RaycastHit2D raycastHit)
        {
            if(conversationState != null && conversationState.conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse) return;
            Dialogue(0);
        }

        public void KeyPressed
            (int keyPressed = 0)
        {
            if (conversationState.InputIsValid(keyPressed))
                conversationState.characterController.Dialogue(keyPressed - 1);
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
                        obj.InspectionAction(this, raycastHit);
                    return;
                }
            }
        }
        
        // Cannot Raycast Hit anything on this opject
        public bool InspectionIsInvalid(RaycastHit2D raycastHit)
        {
            // No triggers
            if (raycastHit.collider.isTrigger) return true;
            
            // If hit a character
            if (raycastHit.collider.GetComponent<CharacterController>() != null)
            {
                // Cannot hit itself
                if (raycastHit.collider.GetComponent<CharacterController>() == this) return true;

                // Can only get circle collider
                if (raycastHit.collider is PolygonCollider2D) return false;
            }   
            
            // If hit an object
            if (raycastHit.collider.GetComponent<ItemController>() != null)
            {
                return false;
            }

            return true;
        }
    }
}