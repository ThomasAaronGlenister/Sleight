using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Deck;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class DeckController : MonoBehaviour
{
    //Number of cards in the current deck
    private int mnDeckSize;

    //Set of available cards
    private List<Card> macActiveDeck = new();

    private Card mcReloadCard;

    //Total Number of available Cards
    //private int mnNumberOfCards = 12;

    //Set of available cards
    private List<Card> macPlayerDeck = new();

    //Set of available cards
    private List<Card> macFullDeck = new();

    //Set of fake cards
    private List<GameObject> macFakeCards = new();

    //Position of Fake cards to simulate deck cards
    private Vector3 mcFakeCardOverlapPosition;

    //Rotation of Fake cards to simulate deck cards
    private Quaternion mcFakeCardOverlapRotation;

    //Flag to adjust deck display
    public bool mbCycleDeck = false;

    //Images of cards being displayed to the UI
    private List<GameObject> DisplayCardsImages = new();

    //Number of cards displayed in the cycle
    private int DisplayCardImageNumber = 12;

    //Offset from the front card in the display card cycle
    private int mnSwapCardOffset;

    //Base size of display cards
    public float lfDisplayCardSize = 250f;

    //Base size of display cards
    public float lfHandCardSize = 220f;

    //card Template prefab assigned in the editor
    public GameObject mcCardTemplate;

    //rotation point for cards
    private Vector2 mcCardCenterPoint = new Vector2(0, 0);

    //index into the display card cycle that dictates the front card
    private Vector2 mcFrontCardPosition;

    //radius of the circle of the display card cycle
    private float mnCardRotationRadius = 2f;

    //Start angle of first card in rotation, will signify front card position
    private int mnCardRotationAngleDeg = 0;

    private float mnCardXRotationValue = 0;
    private float mnCardYRotationValue = 0;

    //Rotation speed multipliers for the reload and cycle animations
    public int mnRotationSpeedMultiplier = 1;
    public int mnReloadRotationSpeedMultiplier = 1;

    //pointer to the Card on top of the deck or the card that may be used or added to sleight
    public int mcFrontCardIndex = 11;
    //pointer to the Next Card in the deck
    private int mcNextCardIndex;

    //Display card cycle center position on the UI canvas
    private int DeckPositionX = 980;
    private int DeckPositionY = -473;

    //Deck Cycle direction (true = counter clockwise) (false = clockwise)
    public bool DeckCycle = true;

    public int mnActiveDeckTopCardPointer;

    public bool mbReload = false;
    public bool CardsShiftReload = false;
    public bool mbReloadCharging = false;

    public bool mbActivate = false;
    public bool CardsShiftActivate = false;

    public bool mbAddHandActivate = false;
    public bool mbAddCardShiftActivate = false;

    public float mfCardDissolve = 0f;

    public float mrCardDissolveRate = 6f;

    public float mfCycleDeckStop = 0;
    public float mfCycleDeckStopActivate = 0;
    public float mfRotationIncrement = 10f;

    public bool start = true;

    private int mnReloadCount = 1;
    private int mnTimesReloaded = 1;
    private int mnMaxReloadCount = 3;

    //Set of cards added to the hand
    List<Card> macSleightCards = new();

    public float mfHandCardPositionY = 1f;
    public float mfHandCardPositionX = -8.5f;
    public float mfHandCardSlideRate = 5f;
    public float mfHandCardMultipleOffset = 20f;

    public int mnHandSize = 0;

    public Transform HandCardHoldPosition;
    public Transform HandCardHoldEndPosition;

    public float CardShiftToHandTime = 0.2f;
    public float CardSnapBackInHandTime = 0.02f;
    private float CardShiftElapsedTime = 0f;

    public float HandCardsRaiseOffset = 0.1f;

    public AnimationCurve mcCardToHandRotationAnimationCurve;

    public AnimationCurve mcCardToHandMovementAnimationCurve;

    // Start is called before the first frame update
    void Start()
    {
        //Swap card is the offset of the front card display index
        mnSwapCardOffset = (DisplayCardImageNumber / 2);

        //Set up collection of all playing cards
        InitializeFullDeck();

        //Set up Deck cycle visual interface
        InitializeDeckCycleUserInterface();

        //Set up collection of fake display cards 
        InitializeFakeCards();

        //Set up Reload card
        InitializeReloadCard();

        mnActiveDeckTopCardPointer = 0;

        /**
         *  TODO : Temperary code to set up basic deck
         */
        for (int i = 0; i < 13; i++)
        {
            macPlayerDeck.Add(macFullDeck[UnityEngine.Random.Range(0, (macFullDeck.Count))]);
        }

        Time.fixedDeltaTime = 1.0f / 60f;

    } //End Start

    // Update is called once per frame
    void Update()
    {
        //if Cycle is requested and Front card is not last or first of deck
        if (mbCycleDeck && !mbReload && 
            ((mnActiveDeckTopCardPointer < macActiveDeck.Count - 1 && DeckCycle)
            || (mnActiveDeckTopCardPointer > 0 && !DeckCycle)))
        {
            mbCycleDeck = CycleDeck(DeckCycle, mnRotationSpeedMultiplier);
        }

        if(mbActivate)
        {
            ActivateCardDisplayAdjust(macSleightCards.Count != 0);
        }

        if(mbAddHandActivate)
        {
            AddCardToHandDisplayAdjust();
        }

        if(start)
        {
            CardsShiftReload = true;
            mbReload = true;
            start = false;
        }

        //If the player is charging the shuffle card
        if (mbReloadCharging)
        {
            UpdateReloadCard();
        }

        if (mbReload)
        {
            ReloadDeck();
        }

        if(mnActiveDeckTopCardPointer != (macActiveDeck.Count - 1))
        {
            CardTilt();
        }

    } //End Update

    public bool ChargingReloadCard()
    {
        return mbReloadCharging;
    }

    private void CardTilt()
    {
        float sine = Mathf.Sin(Time.time);
        float cosine = Mathf.Cos(Time.time);

        float lerpX = Mathf.LerpAngle(DisplayCardsImages[mcFrontCardIndex].transform.eulerAngles.x, sine * 10, 10 * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(DisplayCardsImages[mcFrontCardIndex].transform.eulerAngles.y, cosine * 10, 10 * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(DisplayCardsImages[mcFrontCardIndex].transform.eulerAngles.z, DisplayCardsImages[mcFrontCardIndex].transform.eulerAngles.z, 10 * Time.deltaTime);

        DisplayCardsImages[mcFrontCardIndex].transform.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    public void ShiftDeckAction(bool pbDirection)
    {
        if(!(macActiveDeck[mnActiveDeckTopCardPointer] == mcReloadCard && pbDirection))
        {
            mbCycleDeck = true;
            DeckCycle = pbDirection;  
        }  
    }

    public void ReloadDeck()
    {

        if(CardsShiftReload)
        {
            //Clear the active deck
            macActiveDeck.Clear();

            //Clear all DisplayCards
            for(int lnDisCard = 0; lnDisCard < 12; lnDisCard++)
            {
                ClearCardToDisplay(DisplayCardsImages[lnDisCard]);
            }

            //Reset Active deck with each card in player deck
            for (int i = 0; i < macPlayerDeck.Count; i++)
            {
                AddCard(macPlayerDeck[i]);
            }

            AddCard(mcReloadCard);

            //Start cycle at bottom card in the active deck
            mnActiveDeckTopCardPointer = macActiveDeck.Count + mnSwapCardOffset - 1;
            CardsShiftReload = false;
        }

        
        CycleDeck(false, mnReloadRotationSpeedMultiplier);

        if(mnActiveDeckTopCardPointer == 0)
        {
            mbReload = false;
        }
        
    } //End ReloadDeck

    /**
     * METHOD:: ActivateCardDisplayAdjust 
     * Updates the display deck to convey a card being used  
     *  
     */
    private void ActivateCardDisplayAdjust(bool pbCardInHand)
    {
        bool lbCycleDirection = true;
        int lnCardsToCycle = 7;
        int lnActiveTopCardAdjustment = -1;

        if(mnActiveDeckTopCardPointer == 0)
        {
            lbCycleDirection = false;
            lnActiveTopCardAdjustment = 1;
        }
        else if(mnActiveDeckTopCardPointer == (macActiveDeck.Count - 1))
        {
            lbCycleDirection = true;
            lnActiveTopCardAdjustment = -1;
        }

        //Shift all cards up to swap card offset downwards
        if (CardsShiftActivate && !pbCardInHand)
        {
            //Shift Fake card on top of current front card. it will either be added to hand or spent
            AssignCardToDisplay(macFakeCards[0], macActiveDeck[mnActiveDeckTopCardPointer]);

            for (int i = 0; i < lnCardsToCycle; i++)
            {
                int lnFrontOffset = GetDisplayCardOffsetIndex(mcFrontCardIndex, i, lbCycleDirection);
                ShiftDisplayCard(lnFrontOffset,
                    GetDisplayCardOffsetIndex(lnFrontOffset, 1, lbCycleDirection));

                if(!lbCycleDirection)
                {
                    if (mnActiveDeckTopCardPointer + (i + 1) < macActiveDeck.Count)
                    {
                        AssignCardToDisplay(DisplayCardsImages[lnFrontOffset],
                            macActiveDeck[mnActiveDeckTopCardPointer + (i + 1)]);
                    }
                    else
                    {
                        ClearCardToDisplay(DisplayCardsImages[lnFrontOffset]);
                    }
                }
                else
                {
                    if (mnActiveDeckTopCardPointer - (i + 1) >= 0)
                    {
                        AssignCardToDisplay(DisplayCardsImages[lnFrontOffset],
                            macActiveDeck[mnActiveDeckTopCardPointer - (i + 1)]);
                    }
                    else
                    {
                        ClearCardToDisplay(DisplayCardsImages[lnFrontOffset]);
                    }
                }
            }


            //Flag prevents cards from being re-adjusted via update method
            CardsShiftActivate = false;
        }

        //First Dissolve Fake Card to indicate card being used up
        if(mfCardDissolve < 1f)
        {
            //Dissolve deck card
            if(!pbCardInHand)
            {
                mfCardDissolve += (Time.deltaTime * mrCardDissolveRate);
                macFakeCards[0].GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", mfCardDissolve);
            }
            //Dissolve all cards in hand
            else
            {
                mfCardDissolve += (Time.deltaTime * 2);

                for (int lnFake = 0; lnFake < macSleightCards.Count; lnFake++)
                {
                    macFakeCards[lnFake].GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", mfCardDissolve);
                    macFakeCards[lnFake].transform.position = 
                        new Vector3(macFakeCards[lnFake].transform.position.x, macFakeCards[lnFake].transform.position.y + HandCardsRaiseOffset, 0);
                }
            }
        }
        //Rotate deck cards if hand is empty
        else if(!pbCardInHand)
        {
            //Loop over all display card game objects
            for (int i = 0; i < lnCardsToCycle; i++)
            {
                int CardOffset = GetDisplayCardOffsetIndex(mcFrontCardIndex, i, lbCycleDirection);

                //Maintain card orientation as revolution occurs
                Quaternion lcOriginalRotation = DisplayCardsImages[CardOffset].transform.rotation;

                //rotate stack of cards
                //if pbDirection is set to false rotate in the opposite direction 
                DisplayCardsImages[CardOffset].transform.RotateAround(this.transform.position, Vector3.forward,
                    ((lbCycleDirection) ? -1 : 1) * mfRotationIncrement * mnRotationSpeedMultiplier);

                //Return card to original orientation
                DisplayCardsImages[CardOffset].transform.rotation = lcOriginalRotation;
            }

            mfCycleDeckStopActivate += (mfRotationIncrement);
        }

        //Reset Hand cards if any in hand
        if(pbCardInHand && mfCardDissolve >= 1f)
        {
            ResetFakeCards();
            macSleightCards.Clear();
            mnHandSize = 0;
            mbActivate = false;
            mfCardDissolve = 0f;
        }
        //Check if Next card on the stack has moved into front card position
        else if (mfCycleDeckStopActivate >= 30f)
        {
            //Clear Fake card 
            macFakeCards[0].GetComponent<SpriteRenderer>().sprite = null;
            mfCardDissolve = 0f;
            macFakeCards[0].GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", mfCardDissolve);

            //Remove the Active card from the deck
            macActiveDeck.Remove(macActiveDeck[mnActiveDeckTopCardPointer]);

            if(mnActiveDeckTopCardPointer != 0)
            {
                mnActiveDeckTopCardPointer += lnActiveTopCardAdjustment;
            }

            mfCycleDeckStopActivate = 0f;

            //Finally set Card Activate to false to allow next action
            mbActivate = false;
        }
    } //End ActivateCardDisplayAdjust

    /**
     * METHOD:: AddCardToHandDisplayAdjust 
     * Updates the display deck to convey a card being added to hand  
     *  
     */
    private void AddCardToHandDisplayAdjust()
    {
        bool lbCycleDirection = true;
        int lnCardsToCycle = 7;
        int lnActiveTopCardAdjustment = -1;

        if (mnActiveDeckTopCardPointer == 0)
        {
            lbCycleDirection = false;
            lnActiveTopCardAdjustment = 1;
        }
        else if (mnActiveDeckTopCardPointer == (macActiveDeck.Count - 1))
        {
            lbCycleDirection = true;
            lnActiveTopCardAdjustment = -1;
        }

        //Shift all cards up to swap card offset downwards
        if (mbAddCardShiftActivate)
        {
            //Shift Fake card on top of current front card. it will either be added to hand or spent
            AssignCardToDisplay(macFakeCards[mnHandSize], macActiveDeck[mnActiveDeckTopCardPointer]);

            macFakeCards[mnHandSize].transform.localScale = new Vector2(lfHandCardSize, lfHandCardSize);

            for (int i = 0; i < lnCardsToCycle; i++)
            {
                int lnFrontOffset = GetDisplayCardOffsetIndex(mcFrontCardIndex, i, lbCycleDirection);
                ShiftDisplayCard(lnFrontOffset,
                    GetDisplayCardOffsetIndex(lnFrontOffset, 1, lbCycleDirection));

                if (!lbCycleDirection)
                {
                    if (mnActiveDeckTopCardPointer + (i + 1) < macActiveDeck.Count)
                    {
                        AssignCardToDisplay(DisplayCardsImages[lnFrontOffset],
                            macActiveDeck[mnActiveDeckTopCardPointer + (i + 1)]);
                    }
                    else
                    {
                        ClearCardToDisplay(DisplayCardsImages[lnFrontOffset]);
                    }
                }
                else
                {
                    if (mnActiveDeckTopCardPointer - (i + 1) >= 0)
                    {
                        AssignCardToDisplay(DisplayCardsImages[lnFrontOffset],
                            macActiveDeck[mnActiveDeckTopCardPointer - (i + 1)]);
                    }
                    else
                    {
                        ClearCardToDisplay(DisplayCardsImages[lnFrontOffset]);
                    }
                }
            }


            //Flag prevents cards from being re-adjusted via update method
            mbAddCardShiftActivate = false;
        }

        mcFakeCardOverlapPosition = DisplayCardsImages[mcFrontCardIndex].transform.position;

        //First Shift Fake card to Hand
        if (CardShiftElapsedTime < CardShiftToHandTime)
        {
            CardShiftElapsedTime += Time.deltaTime;

            float HandCardStartAngle = (mnHandSize * 5);
            float HardCardAngleIncrement = 10;

            for(int lnHandCard = 0; lnHandCard <= mnHandSize; lnHandCard++)
            {
                if (CardShiftElapsedTime < (CardShiftToHandTime - CardSnapBackInHandTime))
                {
                    //Card being shifted from deck to hand
                    if(lnHandCard == mnHandSize)
                    {
                        //Move fake card to top of hand over time
                        macFakeCards[lnHandCard].transform.position = Vector3.Lerp(mcFakeCardOverlapPosition, HandCardHoldPosition.transform.position,
                            CardShiftElapsedTime / (CardShiftToHandTime - CardSnapBackInHandTime));
                    }
                    //This card is already in the hand
                    else
                    {
                        //Move fake card to top of hand over time
                        macFakeCards[lnHandCard].transform.position = Vector3.Lerp(HandCardHoldEndPosition.transform.position, HandCardHoldPosition.transform.position,
                            CardShiftElapsedTime / (CardShiftToHandTime - CardSnapBackInHandTime));
                    }

                    //Rotate card as it moves to hand based on animation curve
                    float lfCardRotateCurveValue = mcCardToHandRotationAnimationCurve.Evaluate(CardShiftElapsedTime / (CardShiftToHandTime - CardSnapBackInHandTime));
                    macFakeCards[lnHandCard].transform.rotation = Quaternion.Lerp(Quaternion.identity, HandCardHoldPosition.transform.rotation, lfCardRotateCurveValue);
                }
                else
                {
                    Vector3 lcEndPosition = new Vector3(HandCardHoldEndPosition.transform.position.x + (lnHandCard * 0.15f),
                        HandCardHoldEndPosition.transform.position.y - (lnHandCard * 0.05f), HandCardHoldEndPosition.transform.position.z);

                    //Move fake card to rest position
                    macFakeCards[lnHandCard].transform.position = Vector3.Lerp(HandCardHoldPosition.transform.position, lcEndPosition,
                        (CardShiftElapsedTime - (CardShiftToHandTime - CardSnapBackInHandTime)) / CardSnapBackInHandTime);

                    //Rotate card as it moves to hand based on animation curve
                    float lfCardRotateCurveValue = mcCardToHandRotationAnimationCurve.Evaluate((CardShiftElapsedTime - (CardShiftToHandTime - CardSnapBackInHandTime)) / CardSnapBackInHandTime);
                    macFakeCards[lnHandCard].transform.rotation = Quaternion.Lerp(HandCardHoldPosition.transform.rotation, 
                        Quaternion.Euler(0, 0, HandCardStartAngle - (HardCardAngleIncrement * lnHandCard)), lfCardRotateCurveValue);
                }
            }
        }
        else
        {
            //Loop over all display card game objects
            for (int i = 0; i < lnCardsToCycle; i++)
            {
                int CardOffset = GetDisplayCardOffsetIndex(mcFrontCardIndex, i, lbCycleDirection);

                //Maintain card orientation as revolution occurs
                Quaternion lcOriginalRotation = DisplayCardsImages[CardOffset].transform.rotation;

                //rotate stack of cards
                //if pbDirection is set to false rotate in the opposite direction 
                DisplayCardsImages[CardOffset].transform.RotateAround(this.transform.position, Vector3.forward,
                    ((lbCycleDirection) ? -1 : 1) * mfRotationIncrement * mnRotationSpeedMultiplier);

                //Return card to original orientation
                DisplayCardsImages[CardOffset].transform.rotation = lcOriginalRotation;
            }

            mfCycleDeckStopActivate += (mfRotationIncrement);
        }

        //Check if Next card on the stack has moved into front card position
        if (mfCycleDeckStopActivate >= 30f)
        {
            CardShiftElapsedTime = 0;

            //Add the card to the hand
            macSleightCards.Add(macActiveDeck[mnActiveDeckTopCardPointer]);

            //Remove the Active card from the deck
            macActiveDeck.Remove(macActiveDeck[mnActiveDeckTopCardPointer]);

            if (mnActiveDeckTopCardPointer != 0)
            {
                mnActiveDeckTopCardPointer += lnActiveTopCardAdjustment;
            }

            mfCycleDeckStopActivate = 0f;

            //Finally set Add Card shift Activate to false to allow next action
            mbAddHandActivate = false;

            mnHandSize++;
        }
    }

    /**
     * METHOD:: Moves a Display Card to the position of another display card.
     */
    private void ShiftDisplayCard(int pnDisplayCardIndexToShift, int pnDisplayCardIndexTarget)
    {
        DisplayCardsImages[pnDisplayCardIndexToShift].transform.position
            = DisplayCardsImages[pnDisplayCardIndexTarget].transform.position;
    } //End ShiftDisplayCard

    /**
     * METHOD:: GetDisplayCardOffsetIndex
     *  Gets the clockwise or counter clockwise rotation offset of the provided display index.
     */
    private int GetDisplayCardOffsetIndex(int DisplayIndex, int DisplayIndexOffset, bool abCycleDirection)
    {
        int AdjustedOffset = DisplayIndex + (((abCycleDirection) ? -1 : 1) * DisplayIndexOffset);

        //if Adjustment is not in range of Display Card indices 
        if (AdjustedOffset < 0)
        {
            AdjustedOffset = DisplayCardImageNumber + AdjustedOffset;
        }
        else if(AdjustedOffset > (DisplayCardImageNumber - 1))
        {
            AdjustedOffset = AdjustedOffset - DisplayCardImageNumber;
        }

        return AdjustedOffset;
    } //End GetDisplayCardOffsetIndex

    public List<Card> GetAttackCards()
    {
        List<Card> lacAttackcards = new List<Card>();

        //If no cards in hand
        if (macActiveDeck[mnActiveDeckTopCardPointer] != mcReloadCard && macSleightCards.Count == 0)
        {
            lacAttackcards.Add(macActiveDeck[mnActiveDeckTopCardPointer]);
        }
        else if(macActiveDeck.Count != 0)
        {
            lacAttackcards = macSleightCards;
        }

        return lacAttackcards;
    }

    public bool CardsInHand()
    {
        return (macSleightCards.Count != 0);
    }

    /**
     * METHOD:: ActivateCard
     * 
     */
    public bool ActivateCard(bool pbCharge)
    {
        bool lbCardActivated = false;

        //If front card is the ReloadCard
        if (mnActiveDeckTopCardPointer == (macActiveDeck.Count - 1) && macSleightCards.Count == 0)
        {
            Animator lcReloadAnimation = DisplayCardsImages[mcFrontCardIndex].GetComponent<Animator>();

            lcReloadAnimation.enabled = true;
            if(pbCharge)
            {
                lcReloadAnimation.Play("ReloadAnimation");
                lcReloadAnimation.speed = 1;
                mbReloadCharging = true;
            }
            else
            {
                lcReloadAnimation.speed = 0;
                mbReloadCharging = false;
                macActiveDeck[mnActiveDeckTopCardPointer].SetCardSprite(DisplayCardsImages[mcFrontCardIndex].GetComponent<SpriteRenderer>().sprite);
            }
        }
        //Ensure that card not currently being activated and deck features at least one card and not currently reloading
        else if (macActiveDeck.Count != 0 && !mbActivate && pbCharge && !mbReload)
        {
            mbActivate = true;
            CardsShiftActivate = true;
            lbCardActivated = true;
        }

        return lbCardActivated;
    } //End ActivateReload

    /**
     * METHOD:: IncrementHand - Adds the top card to the hand 
     */
    public bool IncrementHand()
    {
        bool lbAddtoHandSuccussful = false;
        //If front card is not the ReloadCard
        if (mnActiveDeckTopCardPointer != (macActiveDeck.Count - 1) &&
            macSleightCards.Count < 5)
        {
            mbAddHandActivate = true;
            mbAddCardShiftActivate = true;

            lbAddtoHandSuccussful = true;
        }

        return lbAddtoHandSuccussful;
    }

    /**
     * METHOD: Returns the flag that commences adding a card to the hand
     */
    public bool GetAddHandActivate()
    {
        return mbAddHandActivate;
    }

    /**
     * METHOD:: ActivateReload
     * 
     */
    public void ActivateReload()
    {
        mbReload = true;
        CardsShiftReload = true;
    } //End ActivateReload

    /**
     * METHOD:: AddCard 
     * 
     */
    private void AddCard(Card pcCard)
    {
        macActiveDeck.Add(pcCard);
    } // End AddCard

    /**
     * METHOD:: AssignCardToDisplay 
     * Sets the Display card sprite to the provided card
     */
    private void AssignCardToDisplay(GameObject pcDisplayCard, Card pcCard)
    {
        if (pcDisplayCard.GetComponent<Animator>().isActiveAndEnabled)
        {
            pcDisplayCard.GetComponent<Animator>().enabled = false;
        }

        GameObject Overlay = pcDisplayCard.transform.GetChild(0).gameObject;

        SpriteRenderer lcOverlaySprite = Overlay.GetComponent<SpriteRenderer>();

        lcOverlaySprite.sprite = pcCard.GetCardOverlaySprite();

        SpriteRenderer lcDisplayCardSprite = pcDisplayCard.GetComponent<SpriteRenderer>();
        lcDisplayCardSprite.sprite = pcCard.GetCardSprite();
    } //End AssignCardToDisplay

    /**
     * METHOD:: ClearCardToDisplay 
     * Sets the Display card sprite to NULL
     */
    private void ClearCardToDisplay(GameObject pcDisplayCard)
    {
        if (pcDisplayCard.GetComponent<Animator>().isActiveAndEnabled)
        {
            pcDisplayCard.GetComponent<Animator>().enabled = false;
        }

        GameObject Overlay = pcDisplayCard.transform.GetChild(0).gameObject;

        SpriteRenderer lcOverlaySprite = Overlay.GetComponent<SpriteRenderer>();

        lcOverlaySprite.sprite = null;

        SpriteRenderer lcDisplayCardSprite = pcDisplayCard.GetComponent<SpriteRenderer>();
        lcDisplayCardSprite.sprite = null;
    } //End ClearCardToDisplay

    /**
     * METHOD:: InitializeFakeCards 
     *  Creates set of fake card displays that replicate activated cards for display purposes.
     */
    private void InitializeFakeCards()
    {
        for(int lnFakeCardIndex = 0; lnFakeCardIndex < 5; lnFakeCardIndex++)
        {
            mcFakeCardOverlapPosition = new Vector3(DisplayCardsImages[mcFrontCardIndex].transform.position.x,
                 DisplayCardsImages[mcFrontCardIndex].transform.position.y);
            GameObject lcCardObject = Instantiate(
                 mcCardTemplate, mcFakeCardOverlapPosition, Quaternion.identity);
            lcCardObject.name = "FakeCard: " + (lnFakeCardIndex + 1);
            lcCardObject.transform.SetParent(this.transform);
            lcCardObject.transform.localScale = new Vector2(lfDisplayCardSize, lfDisplayCardSize);

            lcCardObject.GetComponent<Animator>().enabled = false;

            lcCardObject.GetComponent<SpriteRenderer>().sortingLayerName = ("FakeCard" + (lnFakeCardIndex + 1));

            macFakeCards.Add(lcCardObject);
        }

    }

    private void ResetFakeCards()
    {
        for (int lnFakeCardIndex = 0; lnFakeCardIndex < 5; lnFakeCardIndex++)
        {
            macFakeCards[lnFakeCardIndex].transform.localScale = new Vector2(lfDisplayCardSize, lfDisplayCardSize);

            mcFakeCardOverlapPosition = DisplayCardsImages[mcFrontCardIndex].transform.position;
            macFakeCards[lnFakeCardIndex].transform.position = DisplayCardsImages[mcFrontCardIndex].transform.position;
            macFakeCards[lnFakeCardIndex].transform.rotation = Quaternion.identity;
            SpriteRenderer lcDisplayCardSprite = macFakeCards[lnFakeCardIndex].GetComponent<SpriteRenderer>();
            lcDisplayCardSprite.sprite = null;
            macFakeCards[lnFakeCardIndex].GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", 0f);
        }
    }
    private void InitializeReloadCard()
    {

        //Get all Sprites for a Reload animation
        Sprite[] lacCardSprites = Resources.LoadAll<Sprite>("CardSprites/ReloadCard/ChargeCards");

        //Get all Sprites for a Reload animation
        Sprite[] lacCardOverlaySprites = Resources.LoadAll<Sprite>("CardSprites/ReloadCard/NumberOverlays");

        //Initialize Reload Card
        mcReloadCard = new Card("Reload Card", CardSuit.eeCardSuitEnd, CardAttackType.eePhysicalAttack,
            SuitEffect.eeKnock, CardRank.eeCardRankEnd, RankEffect.eeNone, lacCardSprites[0]);

        mcReloadCard.SetCardOverlaySprite(lacCardOverlaySprites[0]);
    }

    private void UpdateReloadCard()
    {
        Animator lcReloadCardAnimator = DisplayCardsImages[mcFrontCardIndex].GetComponent<Animator>();

        AnimatorStateInfo stateInfo =
             lcReloadCardAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1.0f)
        {
            //Get all Sprites for a Reload animation
            Sprite[] lacCardSprites = Resources.LoadAll<Sprite>("CardSprites/ReloadCard/ChargeCards");

            //Reset to first frame
            lcReloadCardAnimator.Play("ReloadAnimation", 0, 0f);
            lcReloadCardAnimator.Update(0f);
            macActiveDeck[mnActiveDeckTopCardPointer].SetCardSprite(lacCardSprites[0]);

            //Get all Sprites for a Reload animation
            Sprite[] lacCardOverlaySprites = Resources.LoadAll<Sprite>("CardSprites/ReloadCard/NumberOverlays");

            if (mnReloadCount == 1)
            {
                CardsShiftReload = true;
                mbReload = true;

                mbReloadCharging = false;

                lcReloadCardAnimator.speed = 0;

                mnTimesReloaded++;
                //Check if Reload count max should be updated
                if(mnTimesReloaded > mnMaxReloadCount)
                {
                    mnReloadCount = mnMaxReloadCount;
                }
                else
                {
                    mnReloadCount = mnTimesReloaded;
                }

                mcReloadCard.SetCardOverlaySprite(lacCardOverlaySprites[mnReloadCount - 1]);
            }
            else
            {
                mnReloadCount--;

                //Update overlay on shuffle card
                mcReloadCard.SetCardOverlaySprite(lacCardOverlaySprites[mnReloadCount - 1]);
                DisplayCardsImages[mcFrontCardIndex].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = lacCardOverlaySprites[mnReloadCount - 1];
            }
        }
    }


    /**
     * METHOD:: CycleDeck 
     * Refreshes deck displayed on the canvas.
     * PARAMETER:: pbDirection - flag to indicate which direction to cycle the deck
     *      true - counterclockwise / false - clockwise
     */
    private bool CycleDeck(bool pbDirection, int pnRotationSpeedMultiplier)
    {
        bool lbCyclededDeck = true;

        //Loop over all display card game objects
        for (int i = 0; i < DisplayCardsImages.Count; i++)
        {
            //Maintain card orientation as revolution occurs
            Quaternion lcOriginalRotation = DisplayCardsImages[i].transform.rotation;

            //rotate stack of cards
            //if pbDirection is set to false rotate in the opposite direction 
            DisplayCardsImages[i].transform.RotateAround(this.transform.position, Vector3.forward,
                ((pbDirection) ? 1 : -1) * (mfRotationIncrement * pnRotationSpeedMultiplier));

            //Return card to original orientation
            DisplayCardsImages[i].transform.rotation = lcOriginalRotation;
        }

        //Next card index equals index higher or lower based on cycle direction
            
        //CounterClockwise rotation
        if (pbDirection)
        {
            mcNextCardIndex = (mcFrontCardIndex != 11) ? mcFrontCardIndex + 1 : 0;
        }
        //Clockwise Rotation
        else
        {
            mcNextCardIndex = (mcFrontCardIndex != 0) ? mcFrontCardIndex - 1 : 11;
        }

        mfCycleDeckStop += (mfRotationIncrement);

        //Check if Next card on the stack has moved into front card position
        if (mfCycleDeckStop >= 30f)
        {
            //Deck opposite of front card index should show up
            int lnOppositeCardIndex =
            (mcFrontCardIndex >= mnSwapCardOffset) ? mcFrontCardIndex - mnSwapCardOffset : mcFrontCardIndex + mnSwapCardOffset;

            //Cycle Counter clockwise
            if (pbDirection)
            {
                //Assign opposite card index sprite
                if (macActiveDeck.Count > mnActiveDeckTopCardPointer + mnSwapCardOffset)
                {
                    AssignCardToDisplay(DisplayCardsImages[lnOppositeCardIndex], macActiveDeck[mnActiveDeckTopCardPointer + mnSwapCardOffset]);
                }
                //Else assign blank card to opposite index
                else
                {
                    ClearCardToDisplay(DisplayCardsImages[lnOppositeCardIndex]);
                }

                mnActiveDeckTopCardPointer += (mnActiveDeckTopCardPointer != macActiveDeck.Count) ? 1 : 0;
            }
            //Cycle Clockwise
            else
            {
                //Assign opposite card index sprite
                if (mnActiveDeckTopCardPointer - mnSwapCardOffset >= 0)
                {
                    AssignCardToDisplay(DisplayCardsImages[lnOppositeCardIndex], macActiveDeck[mnActiveDeckTopCardPointer - mnSwapCardOffset]);
                }
                //Else assign blank card to opposite index
                else
                {
                    ClearCardToDisplay(DisplayCardsImages[lnOppositeCardIndex]);
                }

                mnActiveDeckTopCardPointer -= (mnActiveDeckTopCardPointer != 0) ? 1 : 0;
            }

            lbCyclededDeck = false;
            DisplayCardsImages[mcFrontCardIndex].GetComponent<SpriteRenderer>().sortingLayerName = "DeckCards";
            mcFrontCardIndex = mcNextCardIndex;
            DisplayCardsImages[mcFrontCardIndex].GetComponent<SpriteRenderer>().sortingLayerName = "FrontCard";

            mfCycleDeckStop = 0;
        }

        return lbCyclededDeck;
    } //End CycleDeck

    /**
     * METHOD:: InitializeDeckCycleUserInterface
     *  Sets up visual indicator of deck on canvas
     * */
    private void InitializeDeckCycleUserInterface()
    {
        this.transform.position = new Vector3(0, 0, 0);

        //TODO: Magic numbers
        mcFrontCardPosition = new Vector2(-90, 155);

        //Set up displayed cards parameters
        for (int lnDisplayCardImageCounter = 0; lnDisplayCardImageCounter < DisplayCardImageNumber; lnDisplayCardImageCounter++)
        {
            mnCardXRotationValue = (mnCardRotationRadius * (Mathf.Sin(mnCardRotationAngleDeg * (Mathf.PI / 180))));
            mnCardYRotationValue = (mnCardRotationRadius * (Mathf.Cos(mnCardRotationAngleDeg * (Mathf.PI / 180))));

            //Create set of rotational display cards 
            GameObject lcCardObject = Instantiate(
                mcCardTemplate, new Vector3(0 + mnCardXRotationValue, 0 + mnCardYRotationValue), Quaternion.identity);
            lcCardObject.name = "Display Card: " + lnDisplayCardImageCounter;
            lcCardObject.transform.SetParent(this.transform);
            lcCardObject.transform.localScale = new Vector2(lfDisplayCardSize, lfDisplayCardSize);

            lcCardObject.GetComponent<Animator>().enabled = false;

            //Set front card position, this will be used during cycle to check stop position
            if (lnDisplayCardImageCounter == DisplayCardImageNumber - 1)
            {
                mcFrontCardPosition = lcCardObject.transform.localPosition;
            }

            //lcDisplayCard.sprite = mySprite;
            DisplayCardsImages.Add(lcCardObject);

            mnCardRotationAngleDeg += 30;
        } //End InitializeDeckCycleUserInterface

        //Set up local position of cycling deck to corner of Canvas
        this.transform.localPosition = new Vector3(DeckPositionX, DeckPositionY, 0);
    } //End InitializeDeckCycleUserInterface

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
                if (lacCardSprites.Length != 0)
                {
                    //Create card description - "Ace of Hearts" etc.
                    string lcCardDescription = GetRankString((CardRank)lnRankCount) + " of " + lcSuitString + "s";

                    Card lcCard = new Card(lcCardDescription, (CardSuit)lnSuitCount, CardAttackType.eePhysicalAttack, (SuitEffect)lnSuitCount,
                        (CardRank)lnRankCount, RankEffect.eeNone, lacCardSprites[lnRankCount]);

                    macFullDeck.Add(lcCard);
                }
            }
        }
    } //End InitializeFullDeck

    /**
     * METHOD::  GetSuitString: Returns a string identifier for the card suit provided.
     *  Used only by InitializeFullDeck
     * **/
    private string GetSuitString(CardSuit peCardSuit)
    {
        //Get Suit String from index
        string CardSuitString;
        switch (peCardSuit)
        {
            case CardSuit.eeSword:
                CardSuitString = "Sword";
                break;
            case CardSuit.eeAxe:
                CardSuitString = "Axe";
                break;
            case CardSuit.eeArcher:
                CardSuitString = "Archer";
                break;
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
    } //End GetSuitString

    /**
     * METHOD::  GetRankString: Returns a string identifier for the card rank provided.
     *  Used only by InitializeFullDeck
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
    } //End GetRankString

}
