using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Deck;

//Represents the Values of a single attack or Item Card
public class Card
{
    //The Sprite of the card
    private Sprite mcCardSprite;

    //The Sprite of the Overlay
    private Sprite mcCardOverlaySprite;

    //String description of rank and suit of card
    private string maCardDescription;

    //Attack Type of the Card
    private CardAttackType meCardAttackType;

    //Suit of the Card
    private CardSuit meSuit;

    //secondary effect of suit
    private SuitEffect meSuitEffect;

    //Rank of Card
    private CardRank meCardRank;

    //Base Damage of Card derived from rank
    private int mnBaseDamage;

    //Percentage chance of secondary effect derived from suit
    int mnEffectChancePercentage;

    //Constructor :: 
    public Card(string paCardDescription, CardSuit peCardSuit, CardAttackType peAttackType,
        SuitEffect peSuitEffect, CardRank peCardRank, RankEffect peRankEffect, Sprite pcCardSprite, 
        int pnEffectChancePercentage = 0)
    {
        //Assign Base Card information
        maCardDescription = paCardDescription;
        meCardAttackType = peAttackType;
        meSuit = peCardSuit;
        meSuitEffect = peSuitEffect;
        meCardRank = peCardRank;
        mcCardSprite = pcCardSprite;
        mcCardOverlaySprite = null;
        mnEffectChancePercentage = pnEffectChancePercentage;

        mnBaseDamage = GetBaseDamage(meCardRank);
    }

    //Card base damage getter
    public int GetCardDamage()
    {
        return mnBaseDamage;
    }

    public Sprite GetCardSprite()
    {
        return mcCardSprite;
    }

    public CardSuit GetCardSuit()
    {
        return meSuit;
    }

    public SuitEffect GetCardSuitEffect()
    { 
        return meSuitEffect; 
    }

    public void SetCardSprite(Sprite pcCardSprite)
    {
        mcCardSprite = pcCardSprite;
    }

    public Sprite GetCardOverlaySprite()
    {
        return mcCardOverlaySprite;
    }

    public void SetCardOverlaySprite(Sprite pcCardSprite)
    {
        mcCardOverlaySprite = pcCardSprite;
    }

    public int GetCardEffectChance()
    {
        return mnEffectChancePercentage;
    }

    //Determines Base damage based on card rank
    private int GetBaseDamage(CardRank leCardRank)
    {
        int lnBaseDamage = 0;

        if(leCardRank == CardRank.eeAce)
        {
            lnBaseDamage = 30;
        }
        else if(leCardRank == CardRank.eeJack || leCardRank == CardRank.eeQueen || leCardRank == CardRank.eeKing)
        {
            lnBaseDamage = 10;
        }
        else
        {
            lnBaseDamage = ((int)leCardRank) + 1;
        }

        return lnBaseDamage;
    }
}
