using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReward : MonoBehaviour
{
    //Sprite renderer of the card reward
    [SerializeField] private SpriteRenderer mcCardRewardSpriteRenderer;

    //DeckController used for getting Card Reward and adding it to deck
    [SerializeField] private DeckController DeckControllerAccess;

    private Card mcRewardCard;

    // Start is called before the first frame update
    void Start()
    {
        DeckControllerAccess = GameObject.Find("DeckController").GetComponent<DeckController>();

        if(DeckControllerAccess)
        {
            mcRewardCard = DeckControllerAccess.GetCard();

            mcCardRewardSpriteRenderer.sprite = mcRewardCard.GetCardSprite();
        }
    }

    //Catches collider overlaps for the Chest
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        if (DeckControllerAccess)
        {
            DeckControllerAccess.InsertNewCard(mcRewardCard);
        }

        Destroy(gameObject);
    }
}
