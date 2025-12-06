using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //velocity of the coin as it spawns
    public Rigidbody2D mcRigidbody;

    //SpriteRenderer reference
    [SerializeField] private SpriteRenderer mcSpriteRenderer;

    //Sprites for different coin sizes
    [SerializeField] private Sprite mcSmallCoin;
    [SerializeField] private Sprite mcMediumCoin;
    [SerializeField] private Sprite mcLargeCoin;

    //Size multiplier of the coin object
    [SerializeField] private float mfSizeMultiplier = 1;

    //Time the coin exists before it can be picked up
    [SerializeField] private float mfTimeBeforeCollection = 0;

    //Amount this coin is worth
    private int mnCoinValue = 1;

    //Particles to play on pickup
    public ParticleSystem mcBurstFX;

    //Flag indicating that this coin can be picked up
    bool mbCanBeCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale *= mfSizeMultiplier;

        //Initial velocity to pop up out of what instantiates this coin
        mcRigidbody.velocity = new Vector2(Random.Range(-1.0f, 1.0f), 5);

        //Hold pickup time 
        StartCoroutine(EnablePickupCoroutine(1f));
    }

    //Stalls pickup time for a moment
    IEnumerator EnablePickupCoroutine(float pfPickupTime)
    {
        yield return new WaitForSeconds(pfPickupTime);

        mbCanBeCollected = true;
    }

    public void SetCoinType(CoinType peCoinSize)
    {
        switch(peCoinSize)
        {
            case CoinType.eeMediumCoin:
                mnCoinValue = 5;
                break;
            case CoinType.eeLargeCoin:
                mnCoinValue = 10;
                break;
        }
    }

    //Catches collider overlaps for the Coin
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        if(mbCanBeCollected)
        {
            //Increment the Money Count UI element
            PlayerMovement lcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();

            if (lcPlayerCollided)
            {
                lcPlayerCollided.AdjustCoinCount(mnCoinValue);
                mcBurstFX.Play();

                mcBurstFX.transform.SetParent(null);

                Destroy(gameObject);
            }
        }
    }
}

