using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    //Player Controller used to handle collisions
    PlayerMovement mcPlayerCollided;

    //Damage the Obstacle applies
    public int mnObstacleDamage = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Catches collider overlaps for the Obstacle
    private void OnTriggerStay2D(Collider2D lcCollision)
    {
        Debug.Log("Obstacle collided!!!");

        mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();

        if (mcPlayerCollided)
        {
            mcPlayerCollided.Damage(mnObstacleDamage, 5f, AttackDirection.eeUpwards);
        }
        else
        {
            Debug.Log("Obstacle couldnt find player collider");
        }
    }
}
