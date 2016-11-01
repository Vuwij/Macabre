using UnityEngine;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour
    {
        private MacabreObjectController objectInFront = null;
        
        private int orderInLayer
        {
            get { return spriteRenderer.sortingOrder; }
            set { spriteRenderer.sortingOrder = value; }
        }
        
        /// <summary>
        /// Encounters an object, does a chain algorithm to see which one is in the really front
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Whether or not the object is in front</returns>
        protected void EncounterObjectSortLayer(MacabreObjectController obj)
        {
            if (ObjectIsInFront(obj, this))
            {
                if (objectInFront == null) objectInFront = obj;
                ReorganizeSortingLayer(obj);
                IncrementObjectInFrontIfSortingLayerIsSame();

                if (objectInFront == obj) return;

                if (!ObjectIsInFront(obj, objectInFront)) {
                    // Chain the next object
                    MacabreObjectController objInFrontCopy = objectInFront;
                    objectInFront = obj;
                    obj.EncounterObjectSortLayer(objInFrontCopy);
                }
                else
                {
                    // Chain the next object
                    objectInFront.EncounterObjectSortLayer(obj);
                }
            }
        }

        // Organize the sorting layer
        private void ReorganizeSortingLayer(MacabreObjectController obj)
        {
            if (objectInFront.orderInLayer > orderInLayer) { } // Do nothing
            if (objectInFront.orderInLayer == orderInLayer) objectInFront.orderInLayer++;
            if (objectInFront.orderInLayer < orderInLayer)
            {
                int a = orderInLayer;
                orderInLayer = obj.orderInLayer;
                obj.orderInLayer = a;
            }
        }

        // Increment sorting layer if there is an error
        private void IncrementObjectInFrontIfSortingLayerIsSame()
        {
            if (objectInFront == null) return;
            if (orderInLayer > objectInFront.orderInLayer) return;
            objectInFront.orderInLayer++;
            objectInFront.IncrementObjectInFrontIfSortingLayerIsSame();
        }

        /// <returns>True if Object1 is in front of Object2</returns>
        private static bool ObjectIsInFront(MacabreObjectController obj1, MacabreObjectController obj2)
        {
            float obj1Y = obj1.transform.position.y;
            float obj2Y = obj2.transform.position.y;
            return (obj1Y <= obj2Y);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<MacabreObjectController>() != null)
            {
                MacabreObjectController objController = collision.gameObject.GetComponent<MacabreObjectController>();
                EncounterObjectSortLayer(objController);
            }
        }
    }
}
