using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Deck;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;

public class DeckControls : MonoBehaviour
{
    //Number of cards in the current deck
    private int mnDeckSize;

    //Set of available cards
    private List<Card> ActiveDeck = new();

    //Total Number of available Cards
    //private int mnNumberOfCards = 13;

    //Set of available cards
    private List<Card> FullDeck = new();

    //Flag to adjust deck display
    public bool mbRefreshDeck = true;

    //Images of cards being displayed to the UI
    private List<GameObject> DisplayCardsImages = new();

    private int DisplayCardImageNumber = 12;

    //Base size of display cards
    public Vector2 lcDisplayCardSizeDelta = new Vector2(1, 1);

    //card Template prefab assigned in the editor
    public GameObject mcCardTemplate;

    //rotation point for cards
    private Vector3 mcCardCenterPoint = new Vector3(10, 0,0);

    private Vector3 mcFrontCardPosition;

    private int mnCardRotationRadius = 175;

    //Start angle of first card in rotation, will signify front card position
    private int mnCardRotationAngleDeg = 120;

    private int mnCardXRotationValue = 0;
    private int mnCardYRotationValue = 0;
    private float mrCardRevolution = 45;

    public int mnRotationSpeedMultiplier = 3;

    //pointer to the Card on top of the deck or the card that may be used or added to sleight
    private int mcFrontCardIndex;
    //pointer to the Next Card in the deck
    private int mcNextCardIndex;


    // Start is called before the first frame update
    void Start()
    {
        //Set up collection of all playing cards
        InitializeFullDeck();

        Sprite mySprite = Resources.Load<Sprite>("CardSprites/HeartCards/a_AceOfHearts");

        //Set up displayed cards parameters
        for (int lnDisplayCardImageCounter = 0; lnDisplayCardImageCounter < DisplayCardImageNumber; lnDisplayCardImageCounter++)
        {
            mnCardXRotationValue = (int)(mnCardRotationRadius * (Mathf.Sin(mnCardRotationAngleDeg * (Mathf.PI / 180))));
            mnCardYRotationValue = (int)(mnCardRotationRadius * (Mathf.Cos(mnCardRotationAngleDeg * (Mathf.PI / 180))));

            //Get front card position from initial card added
            if(lnDisplayCardImageCounter == 0)
            {
                mcFrontCardPosition = new Vector3(10 + mnCardXRotationValue, 0 + mnCardYRotationValue);
            }

            GameObject lcCardObject = Instantiate(
                mcCardTemplate, new Vector3(10 + mnCardXRotationValue, 0 + mnCardYRotationValue), Quaternion.identity);
            lcCardObject.transform.SetParent(this.transform);
            Image lcDisplayCard = lcCardObject.GetComponent<Image>();
            lcDisplayCard.rectTransform.sizeDelta = lcDisplayCardSizeDelta;

            lcDisplayCard.sprite = mySprite;
            DisplayCardsImages.Add(lcCardObject);

            mnCardRotationAngleDeg += 30;
        }

        //Index of the Card on top of the deck
        mcFrontCardIndex = 0;
        mcNextCardIndex = 1;

        /**
         *  Temperary code to set up basic deck
         */

    }

    // Update is called once per frame
    void Update()
    {
        CycleDeck(true);
    }

    public void RefreshDeckControl()
    {
        mbRefreshDeck = true;
    }

    /**
     * METHOD:: CycleDeck 
     * Refreshes deck displayed on the canvas.
     * PARAMETER:: pbDirection - flag to indicate which direction to cycle the deck
     *      true - counterclockwise / false - clockwise
     */
    private void CycleDeck(bool pbDirection)
    {
        if (mbRefreshDeck)
        {
            //Loop over all display card game objects
            for (int i = 0; i < DisplayCardsImages.Count; i++)
            {
                //Maintain card orientation as revolution occurs
                Quaternion lcOriginalRotation = DisplayCardsImages[i].transform.rotation;

                //rotate stack of cards
                //if pbDirection is set to false rotate in the opposite direction 
                DisplayCardsImages[i].transform.RotateAround(mcCardCenterPoint, Vector3.forward,
                    ((pbDirection) ? 1 : -1) * mrCardRevolution * (Time.deltaTime * mnRotationSpeedMultiplier));

                //Return card to original orientation
                DisplayCardsImages[i].transform.rotation = lcOriginalRotation;
            }

            //Next card index equals index higher or lower based on cycle direction
            if(pbDirection)
            {
                mcNextCardIndex = (mcFrontCardIndex != 11) ? mcFrontCardIndex + 1 : 0;
            }
            else
            {
                mcNextCardIndex = (mcFrontCardIndex != 0) ? mcFrontCardIndex - 1 : 11;
            }

            //Check if Next card on the stack has moved into front card position
            if (Vector3.Distance(DisplayCardsImages[mcNextCardIndex].transform.position, mcFrontCardPosition) <= 10f)
            {
                mbRefreshDeck = false;
                mcFrontCardIndex = mcNextCardIndex;
            }
        }
    }

    /**
     * METHOD::  InitializeFullDeck
     *   Loads all Card objects available 
     *   Index of card value equals (Suit of card enum) * eeCardRankEnd + (rank of card)
     *   Rank of card matches value (Ace = 1, Jack = 11, Queen = 12, King = 13)
     *   Check CommonTypesAppendix.cs for enumerations
    **/
    private void InitializeFullDeck()
    {
        //Loop over all Card Suits
        for (int lnSuitCount = 0; lnSuitCount < (int)CardSuit.eeCardSuitEnd; lnSuitCount++)
        {
            string lcSuitString = GetSuitString((CardSuit)lnSuitCount);

            //Get all Sprites for a single suit
            Sprite[] lacCardSprites = Resources.LoadAll<Sprite>("CardSprites/" + lcSuitString + "Cards");

            //For each Card Rank add a Card object to list of full Deck
            for (int lnRankCount = 0; lnRankCount < (int)CardRank.eeCardRankEnd; lnRankCount++)
            {
                if(lacCardSprites.Length != 0)
                {
                    //Create card description - "Ace of Hearts" etc.
                    string lcCardDescription = GetRankString((CardRank)lnRankCount) + " of " + lcSuitString + "s";

                    //TODO: Adjust effect chance based on suit
                    Card lcCard = new Card(lcCardDescription, (CardSuit)lnSuitCount, CardAttackType.eePhysicalAttack, (SuitEffect)lnSuitCount,
                        (CardRank)lnRankCount, RankEffect.eeNone, lacCardSprites[lnRankCount], 5);

                    FullDeck.Add(lcCard);
                }
            }
        }
    }

    /**
     * METHOD::  GetSuitString: Returns a string identifier for the card suit provided.
     * 
     * **/
    private string GetSuitString(CardSuit peCardSuit)
    {
        //Get Suit String from index
        string CardSuitString;
        switch (peCardSuit)
        {
            case CardSuit.eeHeart:
                CardSuitString = "Heart";
                break;
            case CardSuit.eeSpade:
                CardSuitString = "Spade";
                break;
            case CardSuit.eeClover:
                CardSuitString = "Clover";
                break;
            case CardSuit.eeDiamond:
                CardSuitString = "Diamond";
                break;
            default:
                CardSuitString = "Null";
                break;
        }

        return CardSuitString;
    }

    /**
     * METHOD::  GetRankString: Returns a string identifier for the card rank provided.
     */
    private string GetRankString(CardRank peCardRank)
    {
        //Get Suit String from index
        string CardRankString;
        switch (peCardRank)
        {
            case CardRank.eeAce:
                CardRankString = "Ace";
                break;
            case CardRank.eeTwo:
                CardRankString = "Two";
                break;
            case CardRank.eeThree:
                CardRankString = "Three";
                break;
            case CardRank.eeFour:
                CardRankString = "Four";
                break;
            case CardRank.eeFive:
                CardRankString = "Five";
                break;
            case CardRank.eeSix:
                CardRankString = "Six";
                break;
            case CardRank.eeSeven:
                CardRankString = "Seven";
                break;
            case CardRank.eeEight:
                CardRankString = "Eight";
                break;
            case CardRank.eeNine:
                CardRankString = "Nine";
                break;
            case CardRank.eeTen:
                CardRankString = "Ten";
                break;
            case CardRank.eeJack:
                CardRankString = "Jeck";
                break;
            case CardRank.eeQueen:
                CardRankString = "Queen";
                break;
            case CardRank.eeKing:
                CardRankString = "King";
                break;
            default:
                CardRankString = "Null";
                break;
        }

        return CardRankString;
    }

}
