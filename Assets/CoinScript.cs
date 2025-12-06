using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    //velocity of the coin as it spawns
    public Vector2 mcInitialVelocity;
    public Rigidbody2D mcRigidbody;

    //Sprites for different coin sizes
    [SerializeField] private Sprite mcSmallCoin;
    [SerializeField] private Sprite mcMediumCoin;
    [SerializeField] private Sprite mcLargeCoin;

    //Reference to the Money counter UI element
    [SerializeField] private GameObject mcMoneyCount;

    //Size multiplier of the coin object
    [SerializeField] private float mfSizeMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale *= mfSizeMultiplier;
    }

    //Catches collider overlaps for the Coin
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        //Increment the Money Count UI element
        
    }
}
