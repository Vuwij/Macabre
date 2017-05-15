using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Conversation;
using System.Linq;
using Objects.Unmovable.Items;

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
            // Sort all the objects with offset
            ResortWithOffset(ref castStar);

            foreach (RaycastHit2D raycastHit in castStar)
            {
                if (InspectionIsInvalid(raycastHit)) continue;

                IInspectable mObj = raycastHit.collider.GetComponent<IInspectable>();
                if (mObj != null)
                {
                    Debug.Log(raycastHit.collider.name);
                    mObj.InspectionAction(this, raycastHit);
                    return;
                }
            }
        }
        
        // TODO: Validate this
        private void ResortWithOffset(ref RaycastHit2D[] castStar)
        {
            List<RaycastHit2D> offsetables = new List<RaycastHit2D>();
            List<int> originalOrder = new List<int>();
            for (int i = 0; i < castStar.Length; i++)
            {
                var offsetPosition = castStar[i].transform.gameObject.GetComponent<IOffsetable>();
                if (offsetPosition != null)
                {
                    originalOrder.Add(i);
                    offsetables.Add(castStar[i]);
                }
            }
            offsetables.OrderBy(x => Vector2.Distance(x.transform.gameObject.GetComponent<IOffsetable>().newPosition, transform.position));

            // Resort the list
            for(int i = 0; i < originalOrder.Count; i++)
            {
                castStar[originalOrder[i]] = offsetables[i];
            }
        }

        // Cannot Raycast Hit anything on this opject
        private bool InspectionIsInvalid(RaycastHit2D raycastHit)
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
            
            return false;
        }
    }
}