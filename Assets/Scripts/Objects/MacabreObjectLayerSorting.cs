using UnityEngine;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {

        //TODO Fix sorting layer issue
        /*
        public void ChangeLocation()
        {
            gameObject.layer = LayerMask.NameToLayer("Outside");
            childObject.layer = LayerMask.NameToLayer("Outside");
            ChangeSortingLayer(null);
        }

        public void ChangeLocation(string Destination)
        {
            gameObject.layer = LayerMask.NameToLayer(Destination);
            childObject.layer = LayerMask.NameToLayer(Destination);
            ChangeSortingLayer(Destination);
        }

        public void ChangeLocation(int DestinationIndex)
        {
            gameObject.layer = DestinationIndex;
            childObject.layer = DestinationIndex;
            ChangeSortingLayer(LayerMask.LayerToName(DestinationIndex));
        }

        private void ChangeSortingLayer(string SortingDestination)
        {
            var sortingLayer = gameObject.GetComponentInChildren<SpriteRenderer>();
            if (SortingDestination == null) sortingLayer.sortingLayerName = "Character - Front";
            else if (SortingDestination == "Floor 1" || SortingDestination == "Floor 2" ||
                    SortingDestination == "Floor 3" || SortingDestination == "Floor 4" ||
                    SortingDestination == "Underground 1" || SortingDestination == "Underground 2" ||
                    SortingDestination == "Underground 3")
            {
                sortingLayer.sortingLayerName = "Character - Middle 2";
            }
            else if (SortingDestination == "Outside") sortingLayer.sortingLayerName = "Character - Front 2";
            gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = sortingLayer.sortingLayerName;
        }
        */
    }
}
