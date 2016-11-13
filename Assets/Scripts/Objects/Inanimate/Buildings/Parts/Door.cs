using UnityEngine;
using System.Collections;

namespace Objects.Inanimate.Buildings.Parts
{
    // TODO fix door script
    public class Door
    {
        //[Header("Door Settings")]
        //public bool hasDoor = true;
        //public bool isMainDoor = false;
        //private bool doorOpened = false;

        ////DOOR STATES
        //protected bool hasDoorOpenState = false;
        //private SpriteRenderer door;
        //public bool locked = false;
        //public Sprite doorOpenState;
        //protected Sprite doorClosedState;
        //private string doorSortingLayer;

        ////DOOR'S ACTUAL LOCATION AND PIVOT
        //public Vector3 doorPivot;
        //private Vector3 doorLocation;

        ////DOOR DIRECTION
        //public enum Direction : int { NW = 1, NE = 2, SE = 3, SW = 4 };
        //public Direction dir;

        //protected override void InitialSettings()
        //{
        //    if (hasDoor)
        //    {
        //        door = GetComponent<SpriteRenderer>();
        //        doorClosedState = GetComponent<SpriteRenderer>().sprite;
        //    }

        //    if (doorOpenState == null) hasDoorOpenState = false;
        //    else hasDoorOpenState = true;

        //    doorLocation = doorPivot + transform.position;
        //    if (hasDoor) doorSortingLayer = door.sortingLayerName;
        //}

        //protected override IEnumerator Entry()
        //{

        //    //WAIT FOR PLAYER TO STOP
        //    yield return new WaitForSeconds(0.1f); //INITIALLY WAIT

        //    if (IsMoving)
        //    { //IF PLAYER IS MOVING THEN CONTINUE TO WAIT
        //        yield return new WaitForSeconds(0.5f);
        //        while (IsMoving) yield return null;
        //    }
        //    else yield return null;

        //    //CHECK DISTANCE
        //    if (CheckDistance()) yield break;

        //    if (CheckLock()) yield break;

        //    //FREEZES PLAYER
        //    playerScript.frozen = true;

        //    //OPEN DOOR
        //    if (hasDoorOpenState) door.sprite = doorOpenState;

        //    //FADE SCREEN INITIALLY
        //    while (fadeOut.alpha < 1)
        //    {
        //        fadeOut.alpha += Time.deltaTime;
        //        yield return null;
        //    }

        //    //HIDE OTHER COMPONENTS
        //    HideOtherComponents();

        //    //MOVE PLAYER
        //    MovePlayer();

        //    //CHANGE DOOR APPEARANCE
        //    ChangeDoorAppearance();

        //    //CHANGE COLLIDERS
        //    if (useColliders) ChangeColliders();

        //    //SET PLAYER STATE
        //    SetPlayerState();

        //    //RESET DOOR COLOR
        //    if (hasDoor) door.color = new Color(255, 255, 255, 255);

        //    //UNFADE SCREEN
        //    while (fadeOut.alpha >= 0)
        //    {
        //        fadeOut.alpha -= Time.smoothDeltaTime;
        //        yield return null;
        //    }
        //    //ClOSE DOOR
        //    if (hasDoor) door.sprite = doorClosedState;

        //    //END THE COROUTINE
        //    coroutineStarted = false;
        //    yield break;
        //}

        ///****************** UNIQUE DOOR FUNCTIONS ******************/

        //protected override bool CheckDistance()
        //{
        //    Debug.Log("Checking for Door... DoorPivot = " + doorLocation + " PlayerPosition = " + playerLocation +
        //               "Distance = " + Vector3.Distance(doorLocation, playerLocation));
        //    Debug.DrawLine(doorLocation, playerLocation, Color.red, 5.0f);
        //    targetLocation = doorLocation;
        //    return base.CheckDistance();
        //}

        //protected override bool CheckLock()
        //{
        //    if (locked)
        //    {
        //        Debug.Log("Door Locked");
        //        return true;
        //    }
        //    else return false;
        //}

        //internal void ChangeDoorDirections()
        //{
        //    switch ((int)dir)
        //    {
        //        case 1: //NW
        //            dir = Direction.SE;
        //            break;
        //        case 2: //NE
        //            dir = Direction.SW;
        //            break;
        //        case 3: //SE
        //            dir = Direction.NW;
        //            break;
        //        case 4: //SW
        //            dir = Direction.NE;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //protected void ChangeDoorAppearance()
        //{
        //    if (!IsInside)
        //    {
        //        if (isMainDoor) Camera.main.backgroundColor = new Color(0, 0, 0);
        //    }
        //    else
        //    {
        //        if (isMainDoor) Camera.main.backgroundColor = new Color(255, 255, 255);
        //    }
        //    if (hasDoor)
        //    {
        //        if (door.sortingLayerName == doorSortingLayer)
        //        {
        //            if (!IsReverse) door.sortingLayerName = "Objects - Front";
        //            else door.sortingLayerName = "Objects - back";
        //        }
        //        else door.sortingLayerName = doorSortingLayer;
        //    }
        //}

        //protected override void MovePlayer()
        //{
        //    switch ((int)dir)
        //    {
        //        case 1: //NW
        //        case 3: //SE
        //            targetLocation = (doorLocation - player.transform.position) + doorLocation;
        //            break;
        //        case 2: //NE
        //        case 4: //SW
        //            targetLocation = (doorLocation - player.transform.position) + doorLocation;
        //            break;

        //    }

        //    base.MovePlayer();

        //    //DOOR OPENING SETTINGS
        //    if (!doorOpened)
        //    {
        //        fromLocation = playerScript.location;
        //        doorOpened = true;
        //    }

        //    //RELOCATE PLAYER
        //    if (!IsInside)
        //    {
        //        fromLayer = player.layer;
        //        playerScript.ChangeLocation((int)entryfloor);
        //        playerScript.location = toLocation;
        //    }
        //    else
        //    {
        //        playerScript.ChangeLocation(fromLayer);
        //        playerScript.location = fromLocation;
        //    }
        //}

        //protected void DoorFade()
        //{
        //    if (hasDoor)
        //    {
        //        float fadeAmount = Mathf.Clamp(0.1f / Vector3.Distance(player.transform.position, doorLocation), 0.0f, 1.0f);
        //        door.color = new Color(1.0f, 1.0f, 1.0f, fadeAmount);
        //    }
        //}

        ///****************** GIZMOS ******************/
        //void OnDrawGizmos()
        //{
        //    DoorGizmos();
        //}
        //void OnDrawGizmosSelected()
        //{
        //    DoorGizmos();
        //}

        //void DoorGizmos()
        //{
        //    if (Application.isPlaying)
        //    {
        //        Gizmos.DrawIcon(doorLocation, "Light Gizmo.tiff", true);
        //        Gizmos.DrawLine(doorLocation, transform.position);
        //    }
        //    else
        //    {
        //        Gizmos.DrawIcon(doorPivot + transform.position, "Light Gizmo.tiff", true);
        //        Gizmos.DrawLine(doorPivot + transform.position, transform.position);
        //    }
        //}

    }
}