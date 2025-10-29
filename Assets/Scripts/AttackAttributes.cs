using System;
using UnityEngine;
using Deck;

public class AttackAttributes
{
    //Attack Damage
    private int mnAttackDamage = 0;

    // Horizontal space offset for the Attack
    private float mfHorizontalOffset = 0;

    // Vertical space offset for the Attack
    private float mfVerticalOffset = 0;

    // Horizontal space offset for the Attack
    private float mfSideAttackOffset = 0;

    // Vertical space offset for the Attack
    private float mfUpDownAttackOffset = 0;

    //base attack transform size multiplier
    private float mfAttackBaseSizeMultiplier = 1;

    //Flag indicating if this attack sticks to parent object
    private bool mbDisjointed = false;

    //Flag indicating this attack should revolve around a point during its lifetime
    private bool mbRevolveAround = false;

    //Revolution point for the attack
    private Vector3 mcRevolutionPoint = Vector3.zero;

    //Value indicating movement distance of attack
    private float mfTravelDistance = 0f;

    //Value indicating the time it takes attack to travel to end point
    private float mfTravelTime = 1f;

    //Flag indicating that this attack has reverse animation
    private bool mbHasAnimationFlip = false;

    //Player/Enemy attack animation to be played
    int mnAttackerAnimation = 0;

    //flag that indicates this attacks lifetime is linked to the animation 
    private bool mbSingleAnimationAttack;

    //String identifier for attack animation
    private string mcAttackAnimationString;

    //Set of forces that may be applied to the attacker during the attack
    AttackForces msAttackForces;

    //Direction this attacks hit box capsule is set to
    private CapsuleDirection2D meAttackCapsuleColliderDirection;

    //Direction this attack is set to go in
    AttackDirection meAttackDirection;

    /**
     * METHOD: Constructor for Attack Settings.
     * Horizontal Offset, 
     * Vertical Offset, 
     * Size Multiplier, 
     * Is Disjointed, 
     * Revolve Around
     * Travel Distance, 
     * Travel Time, 
     * Single Animation Lifetime,
     * Attack Animation String
     */
    public AttackAttributes(
        float pfHorzOffset,
        float pfVertOffset,
        float pfSideAttackOffset,
        float pfUpDownAttackOffset,
        float pfSizeMult,
        bool pbDisjointed,
        bool pbRevolveAround,
        float pfTravelDistance,
        float pfTravelTime,
        bool pbHasAnimationFlip,
        int pnAttackerAnimation,
        bool pbSingleAnimationLifetime,
        string pcAttackAnimationString,
        CapsuleDirection2D peAttackCapsuleColliderDirection)
    {
        mfHorizontalOffset = pfHorzOffset;
        mfVerticalOffset = pfVertOffset;
        mfSideAttackOffset = pfSideAttackOffset;
        mfUpDownAttackOffset = pfUpDownAttackOffset;
        mfAttackBaseSizeMultiplier = pfSizeMult;
        mbDisjointed = pbDisjointed;
        mbRevolveAround = pbRevolveAround;
        mfTravelDistance = pfTravelDistance;
        mfTravelTime = pfTravelTime;
        mbHasAnimationFlip = pbHasAnimationFlip;
        mnAttackerAnimation = pnAttackerAnimation;
        mbSingleAnimationAttack = pbSingleAnimationLifetime;
        mcAttackAnimationString = pcAttackAnimationString;
        meAttackCapsuleColliderDirection = peAttackCapsuleColliderDirection;
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

    //Side attack offset getter
    public float GetSideAttackOffset()
    {
        return mfSideAttackOffset;
    }

    //Up down attack offset getter
    public float GetUpDownAttackOffset()
    {
        return mfUpDownAttackOffset;
    }

    //Size multiplier getter
    public float GetAttackBaseSizeMultiplier()
    {
        return mfAttackBaseSizeMultiplier;
    }

    //Disjointed Getter
    public bool GetDisjointed()
    {
        return mbDisjointed;
    }

    //Revolution Getter
    public bool GetRevolveAround()
    {
        return mbRevolveAround;
    }

    //Travel Distance Getter
    public float GetTravelDistance()
    {
        return mfTravelDistance;
    }

    //Travel Time Getter
    public float GetTravelTime()
    {
        return mfTravelTime;
    }

    //Has animation flip getter
    public bool GetHasAnimationFlip()
    {
        return mbHasAnimationFlip;
    }

    //Player Attack Animation Getter
    public int GetPlayerAttackAnimation()
    {
        return mnAttackerAnimation;
    }

    //Single animation lifetime getter
    public bool GetSingleAnimationLifetime()
    {
        return mbSingleAnimationAttack;
    }

    //Attack Animation string getter
    public string GetAttackAnimationString()
    {
        return mcAttackAnimationString;
    }

    //Revolution point getter
    public Vector2 GetRevolutionPoint()
    {
        return mcRevolutionPoint;
    }

    //Attack Damage Setter
    public void SetAttackDamage(int pnDamage)
    {
        mnAttackDamage = pnDamage;
    }

    //Attack Damage getter
    public int GetAttackDamage()
    {
        return mnAttackDamage;
    }

    //Capsule collider direction getter
    public CapsuleDirection2D GetAttackCapsuleColliderDirection()
    {
        return meAttackCapsuleColliderDirection;
    }


    //Attack Direction Setter/Getter
    public void SetDirection(AttackDirection peAttackDirection)
    {
        meAttackDirection = peAttackDirection;
    }

    public AttackDirection GetDirection()
    {
        return meAttackDirection;
    }
}
