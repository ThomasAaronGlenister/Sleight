using System;
using UnityEngine;
using Deck;

public class AttackAttributes
{
	// Horizontal space offset for the Attack
	private float mfHorizontalOffset = 0;

    // Vertical space offset for the Attack
    private float mfVerticalOffset = 0;

	//base attack transform size multiplier
	private float mfAttackBaseSizeMultiplier = 1;

    public AttackAttributes(float pfHorzOffset, float pfVertOffset, float pfSizeMult)
	{
        mfHorizontalOffset = pfHorzOffset;
        mfVerticalOffset = pfVertOffset;
        mfAttackBaseSizeMultiplier = pfSizeMult;

    }

    //Horizontal Offset Getter
    public float GetHorizontalOffset()
    {
        return mfHorizontalOffset;
    }

    //Vertial offset getter
    public float GetVerticalOffset()
    {
        return mfVerticalOffset;
    }

    //Size multiplier getter
    public float GetAttackBaseSizeMultiplier()
    {
        return mfAttackBaseSizeMultiplier;
    }

}
