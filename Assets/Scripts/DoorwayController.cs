using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayController : MonoBehaviour
{
    [SerializeField] private bool mbSideOfDoor = false;

    //Catches collider enters the door
    void OnTriggerEnter2D(Collider2D lcCollision)
    {
        PlayerMovement mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();
        if(mbSideOfDoor)
        {
            mcPlayerCollided.SetDoorOverlap(true, transform.parent.transform, mbSideOfDoor);
        }
        else
        {
            mcPlayerCollided.SetDoorOverlap(true, transform, mbSideOfDoor);
        }
    }

    //Catches collider exits the door
    void OnTriggerExit2D(Collider2D lcCollision)
    {
        PlayerMovement mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();
        mcPlayerCollided.SetDoorOverlap(false, null);
    }

}
