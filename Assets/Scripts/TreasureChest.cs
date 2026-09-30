using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    TreasureType meTreasureType = TreasureType.eeCardTreasure;

    //Sprite used to convey the chest has opened
    [SerializeField] private Sprite mcOpenedSprite;

    //Sprite used to convey the chest has opened
    [SerializeField] private Sprite mcBaseTreasureChest;

    //Sprite used to convey the chest has opened
    [SerializeField] private Sprite mcGoldTreasureChest;

    //Sprite used to convey the chest has opened
    [SerializeField] private Sprite mcPurpleTreasureChest;

    //Sprite renderer of the chest
    [SerializeField] private SpriteRenderer mcChestSpriteRenderer;

    //Prefab Card Object used for Playing Card Rewards
    [SerializeField] private GameObject mcCardRewardPrefab;

    //Prefab Coin Object used for Money Rewards
    [SerializeField] private GameObject mcCoinRewardPrefab;

    //Position the card reward is created at
    private Vector2 mcCardRewardPosition;

    //Position the base reward is created at
    private Vector2 mcBaseRewardPosition;

    private bool mbOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        mcCardRewardPosition = new Vector2(transform.position.x, transform.position.y + 1);

        mcBaseRewardPosition = new Vector2(transform.position.x, transform.position.y + 0.5f);
    }

    //Set the treasure type
    public void SetTreasureType(TreasureType peTreasureType)
    {
        meTreasureType = peTreasureType;
    }

    //Catches collider overlaps for the Chest
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        if (!mbOpened)
        {
            //Create reward based on treasure type 
            switch (meTreasureType)
            {
                case TreasureType.eeCoinTreasure:
                    int lnNumCoinRewards = Random.Range(1, 10);

                    for (int i = 0; i < lnNumCoinRewards; i++)
                    {
                        //Instantiate a reward coin object
                        Instantiate(mcCoinRewardPrefab, mcBaseRewardPosition, Quaternion.identity);
                    }

                    break;
                case TreasureType.eeCardTreasure:
                    //Instantiate a reward card object
                    Instantiate(mcCardRewardPrefab, mcCardRewardPosition, Quaternion.identity);
                    break;
            }

            //Open the treasure chest
            mcChestSpriteRenderer.sprite = mcOpenedSprite;

            mbOpened = true;
        }
    }
}
