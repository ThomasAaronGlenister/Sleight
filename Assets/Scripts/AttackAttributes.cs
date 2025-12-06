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

    //Rate that the attack spins
    private float mfRotationRate = 0;

    //Flag indicating this attack should revolve around a point during its lifetime
    private bool mbRevolveAround = false;

    //Revolution point for the attack
    private Vector3 mcRevolutionPoint = Vector3.zero;

    //Value indicating movement distance of attack
    private float mfTravelDistance = 0f;

    //Value indicating the Life Time of this attack
    private float mfLifeTime = 0f;

    //Impulse force applied if attack movement type is Force
    private float mfMovementForce = 0f;

    //Movement type of the attack
    private AttackMovementType meAttackMovement = AttackMovementType.eeNoMovement;

    //Flag indicating that this attack has reverse animation
    private bool mbHasAnimationFlip = false;

    //Player/Enemy attack animation to be played
    int mnAttackerAnimation = 0;

    //flag that indicates this attacks lifetime is linked to the animation 
    private bool mbSingleAnimationAttack;

    //String identifier for attack animation
    private string mcAttackAnimationString;

    //Set of forces that may be applied to the attacker during the attack
    private AttackForces msAttackForces;

    //Direction this attacks hit box capsule is set to
    private CapsuleDirection2D meAttackCapsuleColliderDirection;

    //Direction this attack is set to go in
    AttackDirection meAttackDirection;

    //Partical Trail Values
    private bool mbParticleTrailEnabled;
    private Color mcParticleTrailColor;

    //Flags indicating this attack originates from the ground/wall
    private bool mbGroundOrigination = false;
    private bool mbWallOrigination = false;

    //Flag indicating this attack has a sub attack
    private bool mbHasSubAttack = false;

    //Index of the attack produced by the attack
    private int mnSubAttackIndex = 0;

    //Flag indicating this attack can be used as a wall/platform
    private bool mbTangible = false;

    //Angle adjustment of disjointed attack
    private float mfAdjustedAngle = 0;

    //Number of duplications of this attack
    private int mnNumberOfInstances = 0;

    private float mfMass = 0f;
    private float mfGravityScale = 0f;

    //Time before Attack begins
    private float mfAttackDelay = 0f;

    bool mbCreateSubAttackOnEnd = false;
    AttackDirection mcSubAttackDirectionOnRight = AttackDirection.eeRightward;
    AttackDirection mcSubAttackDirectionOnLeft = AttackDirection.eeLeftward;
    AttackDirection mcSubAttackDirectionOnUp = AttackDirection.eeUpwards;
    AttackDirection mcSubAttackDirectionOnDown = AttackDirection.eeDownwards;

    //Flag indicating this attack is directed towards closest target
    private bool mbMoveTowardsTarget = false;

    AttackDirection[] macSubAttackDirections = new AttackDirection[4];

    //Array where index equals Suit Effect type and Value is percentage chance to proc
    int[] manSuitEffectPercentages = new int[(int)SuitEffect.eeSuitEffectEnd];

    //Base Suit Effect Durations
    float[] mafSuitEffectDurations = new float[(int)SuitEffect.eeSuitEffectEnd] { 0.1f,0.1f,0.1f,5,5,5,5 };

    //Flag indicating this attack should cause an animation on the attacker to play
    private bool mbAnimateAttacker = true;

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
        float pfLifeTime,
        float pfMovementForce,
        AttackMovementType peAttackMovement,
        bool pbHasAnimationFlip,
        int pnAttackerAnimation,
        bool pbSingleAnimationLifetime,
        string pcAttackAnimationString,
        CapsuleDirection2D peAttackCapsuleColliderDirection,
        bool pbParticleTrail,
        Color pcParticleColor,
        float pfRotationRate = 0,
        bool pbGroundOrigination = false,
        bool pbWallOrigination = false,
        bool pbHasSubAttack = false,
        int pnSubAttackIndex = 0,
        bool pbTangible = false,
        float pfAdjustedAngle = 0,
        int pnNumberOfAttackInstances = 1,
        float pfMass = 1,
        float pfGravityScale = 1,
        float pfAttackDelay = 0,
        bool pbCreateSubAttackOnEnd = false,
        AttackDirection pcSubAttackDirectionOnRight = AttackDirection.eeRightward,
        AttackDirection pcSubAttackDirectionOnLeft = AttackDirection.eeLeftward,
        AttackDirection pcSubAttackDirectionOnUp = AttackDirection.eeUpwards,
        AttackDirection pcSubAttackDirectionOnDown = AttackDirection.eeDownwards,
        bool pbMoveTowardsTarget = false,
        bool pbAnimateAttacker = true
        )
    {
        mfHorizontalOffset = pfHorzOffset;
        mfVerticalOffset = pfVertOffset;
        mfSideAttackOffset = pfSideAttackOffset;
        mfUpDownAttackOffset = pfUpDownAttackOffset;
        mfAttackBaseSizeMultiplier = pfSizeMult;
        mbDisjointed = pbDisjointed;
        mbRevolveAround = pbRevolveAround;
        mfTravelDistance = pfTravelDistance;
        mfLifeTime = pfLifeTime;
        mfMovementForce = pfMovementForce;  
        meAttackMovement = peAttackMovement;
        mbHasAnimationFlip = pbHasAnimationFlip;
        mnAttackerAnimation = pnAttackerAnimation;
        mbSingleAnimationAttack = pbSingleAnimationLifetime;
        mcAttackAnimationString = pcAttackAnimationString;
        meAttackCapsuleColliderDirection = peAttackCapsuleColliderDirection;
        mbParticleTrailEnabled = pbParticleTrail;
        mcParticleTrailColor = pcParticleColor;
        mfRotationRate = pfRotationRate;

        mbGroundOrigination = pbGroundOrigination;
        mbWallOrigination = pbWallOrigination;

        mbHasSubAttack = pbHasSubAttack;
        mnSubAttackIndex = pnSubAttackIndex;

        mbTangible = pbTangible;

        mfAdjustedAngle = pfAdjustedAngle;
        mnNumberOfInstances = pnNumberOfAttackInstances;

        mfMass = pfMass;
        mfGravityScale = pfGravityScale;

        mfAttackDelay = pfAttackDelay;

        mbCreateSubAttackOnEnd = pbCreateSubAttackOnEnd;

        mcSubAttackDirectionOnRight = pcSubAttackDirectionOnRight;
        mcSubAttackDirectionOnLeft = pcSubAttackDirectionOnLeft;
        mcSubAttackDirectionOnUp = pcSubAttackDirectionOnUp;
        mcSubAttackDirectionOnDown = pcSubAttackDirectionOnDown;

        mbMoveTowardsTarget = pbMoveTowardsTarget;

        macSubAttackDirections[(int)AttackDirection.eeRightward] = mcSubAttackDirectionOnRight;
        macSubAttackDirections[(int)AttackDirection.eeLeftward] = mcSubAttackDirectionOnLeft;
        macSubAttackDirections[(int)AttackDirection.eeUpwards] = mcSubAttackDirectionOnUp;
        macSubAttackDirections[(int)AttackDirection.eeDownwards] = mcSubAttackDirectionOnDown;

        mbAnimateAttacker = pbAnimateAttacker;
    }

    //Method to assign Effect chances to attack attributes
    public void SetEffectChances(int[] panEffectChances)
    {
        for(int lnEffect = 0; lnEffect < (int)SuitEffect.eeSuitEffectEnd; lnEffect++)
        {
            manSuitEffectPercentages[lnEffect] = panEffectChances[lnEffect];
        }
    }

    //Effect chances getter
    public int[] GetEffectChances()
    {
        return manSuitEffectPercentages;
    }

    public float GetEffectDuration(SuitEffect peSuitEffect)
    {
        return mafSuitEffectDurations[(int)peSuitEffect];
    }

    //Horizontal Offset Getter
    public float GetHorizontalOffset()
    { return mfHorizontalOffset; }

    //Vertial offset getter
    public float GetVerticalOffset()
    { return mfVerticalOffset; }

    //Side attack offset getter
    public float GetSideAttackOffset()
    { return mfSideAttackOffset; }

    //Up down attack offset getter
    public float GetUpDownAttackOffset()
    { return mfUpDownAttackOffset; }

    //Size multiplier getter
    public float GetAttackBaseSizeMultiplier()
    { return mfAttackBaseSizeMultiplier; }

    //Disjointed Getter
    public bool GetDisjointed()
    { return mbDisjointed; }

    //Revolution Getter
    public bool GetRevolveAround()
    { return mbRevolveAround; }

    //Travel Distance Getter
    public float GetTravelDistance()
    { return mfTravelDistance; }

    //Travel Time Getter
    public float GetLifeTime()
    { return mfLifeTime; }

    public float GetMovementForce()
    { return mfMovementForce; }

    public AttackMovementType GetAttackMovementType()
    { return meAttackMovement; }

    //Has animation flip getter
    public bool GetHasAnimationFlip()
    { return mbHasAnimationFlip; }

    //Player Attack Animation Getter
    public int GetPlayerAttackAnimation()
    { return mnAttackerAnimation; }

    //Single animation lifetime getter
    public bool GetSingleAnimationLifetime()
    { return mbSingleAnimationAttack; }

    //Attack Animation string getter
    public string GetAttackAnimationString()
    { return mcAttackAnimationString; }

    //Revolution point getter
    public Vector2 GetRevolutionPoint()
    { return mcRevolutionPoint; }

    //Attack Damage Setter
    public void SetAttackDamage(int pnDamage)
    { mnAttackDamage = pnDamage; }

    //Attack Damage getter
    public int GetAttackDamage()
    { return mnAttackDamage; }

    //Capsule collider direction getter
    public CapsuleDirection2D GetAttackCapsuleColliderDirection()
    { return meAttackCapsuleColliderDirection; }

    //Particle Trail Flag getter
    public bool GetParticleTrailEnabled()
    { return mbParticleTrailEnabled; }

    //Particle trail Color getter
    public Color GetParticleTrailColor()
    { return mcParticleTrailColor; }

    //Rotation rate getter
    public float GetRotationRate()
    { return mfRotationRate; }

    //Ground Origination getter
    public bool GetGroundOrigination()
    { return mbGroundOrigination; }

    //Wall Origination Getter
    public bool GetWallOrigination()
    { return mbWallOrigination; }

    //Has sub attack getter
    public bool GetHasSubAttack()
    { return mbHasSubAttack; }

    //Sub Attack index getter
    public int GetSubAttackIndex()
    { return mnSubAttackIndex; }

    //Is tangible getter
    public bool GetTangible()
    { return mbTangible; }

    //Adjusted angle getter
    public float GetAdjustedAngle()
    { return mfAdjustedAngle; }
    public void SetAdjustedAngle(float pfAdjustedAngle)
    { mfAdjustedAngle = pfAdjustedAngle; }

    //Number of duplicate attacks getter
    public int GetNumInstances()
    { return mnNumberOfInstances; }

    //Mass Getter
    public float GetMass()
    { return mfMass; }

    //Gravity Scale getter
    public float GetGravityScale()
    { return mfGravityScale; }

    public float GetAttackDelay()
    { return mfAttackDelay; }

    //Sub Attack on end Getter
    public bool GetCreateSubAttackOnEnd()
    { return mbCreateSubAttackOnEnd; }

    //Sub attack direction getter
    public AttackDirection GetSubAttackDirection(AttackDirection leParentAttackDirection)
    { return macSubAttackDirections[(int)leParentAttackDirection]; }

    public bool GetAnimateAttacker()
    { return mbAnimateAttacker; }

    public bool GetMoveTowardsTarget()
    { return mbMoveTowardsTarget; }

    //Attack Direction Setter/Getter
    public void SetDirection(AttackDirection peAttackDirection)
    { meAttackDirection = peAttackDirection; }
    public AttackDirection GetDirection()
    { return meAttackDirection; }
}
