using System;
using UnityEngine;
using Deck;
using static UnityEditor.ShaderData;
using Unity.VisualScripting;

public class AttackAttributes
{
    //Name of the attack
    private String mcAttackName = "";

    //Attack Damage and additions
    private int mnAttackDamage = 0;
    private int mnDamageAddition = 0;
    private int mnDamageMulitplier = 1;

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

    //Force applied by the attack
    private float mfKnockBack = 0f;

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

    private Color mcEffectColor = Color.white;

    //Sets whether this attacks hitbox is active
    private bool mbHasHitbox = true;

    //Sets whether this attacks hitbox is purely used as a trigger
    private bool mbHitBoxIsTrigger = false;

    //Force multiplier applied to Attack every update
    private float mfForceAmplifier = 0f;

    /********************************ENEMY ATTACK************************************/
    //Attack values used for Enemy AI to use certain attacks

    //Flag whether this attack is an enemy attack
    private bool mbEnemyAttack = false;

    //Space Enemy has between ground to perform attack
    private float mfGroundSpace;

    //Space Enemy has between walls to perform attack
    private float mfWallSpace;

    //Horizontal area the target must be within to perform attack
    private float mfTargetMinRangeX;
    private float mfTargetMaxRangeX;

    //Vertical area the target must be within to perform attack
    private float mfTargetMinRangeY;
    private float mfTargetMaxRangeY;

    //Cool down time taken after attack is performed
    private float mfCoolDownPeriod;

    //Floating point time frame for next time this attack can be performed
    private float mfCoolDownTime;

    //Maintains time from each consecutive bound check
    private float mfCoolDownTimeDifference = 0;

    /** 
     * Integer value determining chance for this attack to be performed over others.
     * If multiple attacks fall within a trigger then priority values will be added
     * and random integer will be generated within the range to determine which attack will play
    **/
    private int mnChanceValue;

    //Id value set to this attack when it is added to list of attacks
    private int mnEnemyAttackId = 1;

    /*************************************************************************/


    //List of sub attacks this attack can create
    private AttackAttributes lcSubAttack;

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
        string pcAttackName,
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
        bool pbAnimateAttacker = true,
        float pfKnockBack = 0,
        bool pbHasHitbox = true,
        bool pbHitBoxIsTrigger = false,
        float pfForceAmplifier = 0
        )
    {
        mcAttackName = pcAttackName;
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
        mfKnockBack = pfKnockBack;
        mbHasHitbox = pbHasHitbox;
        mbHitBoxIsTrigger = pbHitBoxIsTrigger;
        mfForceAmplifier = pfForceAmplifier;
    }

    //Sets the attack range and chance values for enemy attacks
    public void SetEnemyAttackValues(
        float pfGroundSpace = 0,
        float pfWallSpace = 0,
        float pfTargetMinRangeX = -20,
        float pfTargetMaxRangeX = 20,
        float pfTargetMinRangeY = -20,
        float pfTargetMaxRangeY = 20,
        float pfCoolDownPeriod = 1,
        int pnChanceValue = 1,
        float pfAttackCooldown = 0)
    {
        mbEnemyAttack = true;
        mfGroundSpace = pfGroundSpace;
        mfWallSpace = pfWallSpace;
        mfTargetMinRangeX = pfTargetMinRangeX;
        mfTargetMaxRangeX = pfTargetMaxRangeX;
        mfTargetMinRangeY = pfTargetMinRangeY;
        mfTargetMaxRangeY = pfTargetMaxRangeY;
        mfCoolDownPeriod = pfCoolDownPeriod;   
        mnChanceValue = pnChanceValue;
        mfCoolDownTime = pfAttackCooldown;
    }

    //Adds a sub attack to this attack
    public void AddSubAttack(AttackAttributes lcSubAttackAttributes)
    {
        lcSubAttack = lcSubAttackAttributes;
    }

    public AttackAttributes GetSubAttack()
    {
        return lcSubAttack;
    }

    //Enemy attack ID setter
    public void SetEnemyAttackId(int pnAttackId)
    {
        mnEnemyAttackId = pnAttackId;
    }

    //Enemy attack ID getter
    public int GetEnemyAttackId()
    {
        return mnEnemyAttackId;
    }

    public bool WithinAttackBounds(float pfGroundDistance, float pfWallDistance, 
        float pfTargetPositionX, float pfTargetPositionY)
    {

        bool lbWithinBounds = false;
        if (pfGroundDistance >= mfGroundSpace && pfWallDistance >= mfWallSpace &&
            pfTargetPositionX >= mfTargetMinRangeX && pfTargetPositionX <= mfTargetMaxRangeX &&
            pfTargetPositionY >= mfTargetMinRangeY && pfTargetPositionY <= mfTargetMaxRangeY)
        {
            //If Time difference 
            if(Time.time - mfCoolDownTimeDifference > mfCoolDownTime)
            {
                lbWithinBounds = true;

                mfCoolDownTimeDifference = Time.time;
            }
        }
        return lbWithinBounds;
    }

    //Method to assign Effect chances to attack attributes
    public void SetEffectChances(int[] panEffectChances)
    {
        for(int lnEffect = 0; lnEffect < (int)SuitEffect.eeSuitEffectEnd; lnEffect++)
        {
            manSuitEffectPercentages[lnEffect] = panEffectChances[lnEffect];
        }
    }

    //Method to assign Effect chances to attack attributes
    public void AddEffectSkillAdjustments(int pnEffectId, int pnEffectChanceAddition = 0, int pnEffectChanceMultiplier = 1)
    {
        manSuitEffectPercentages[pnEffectId] = (manSuitEffectPercentages[pnEffectId] + pnEffectChanceAddition)
            * pnEffectChanceMultiplier;

        //Debug.Log((SuitEffect) pnEffectId + " Chance " + manSuitEffectPercentages[pnEffectId] + " % ");
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

    // Attack Name getter
    public String GetAttackName() 
    { return mcAttackName; } 

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

    //Attack Addition setter
    public void SetAttackAddition(int pnAttackAddition)
    {
        mnDamageAddition = pnAttackAddition;
    }

    //Attack Mulitplier setter
    public void SetAttackMuliplier(int pnAttackMultiplier)
    {
        mnDamageMulitplier = pnAttackMultiplier;
    }

    //Attack Damage getter
    public int GetAttackDamage()
    { return (mnAttackDamage + mnDamageAddition) * mnDamageMulitplier; }

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

    public bool GetHasHitbox()
        { return mbHasHitbox; }

    public bool GetHitBoxIsTrigger()
    {
        return mbHitBoxIsTrigger;
    }

    public float GetForceAmplifier()
    {
        return mfForceAmplifier;
    }

    public bool GetMoveTowardsTarget()
    { return mbMoveTowardsTarget; }

    //Attack Direction Setter/Getter
    public void SetDirection(AttackDirection peAttackDirection)
    { meAttackDirection = peAttackDirection; }
    public AttackDirection GetDirection()
    { return meAttackDirection; }

    //Ground Space getter
    public float GetGroundSpace()
    { return mfGroundSpace; }

    //Wall Space getter
    public float GetWallSpace()
    { return mfWallSpace; }

    //Target Min range x getter
    public float GetTargetMinRangeX()
    { return mfTargetMinRangeX; }

    //Target Max range x getter
    public float GetTargetMaxRangeX()
    { return mfTargetMaxRangeX; }

    //Target Min range Y getter
    public float GetTargetMinRangeY()
    { return mfTargetMinRangeY; }

    //Target Max range Y getter
    public float GetTargetMaxRangeY()
    { return mfTargetMaxRangeY; }

    //Chance value getter
    public int GetChanceValue()
    { return mnChanceValue; }

    //Attack cooldown getter
    public float GetAttackCooldownTime()
    {
        return mfCoolDownTime;
    }

    //Knock back getter
    public float GetKnockBack()
    { return mfKnockBack; }

    public string GetString()
    {
        string lbReturn = mcAttackName + "," +
                mfHorizontalOffset + "," +
                mfVerticalOffset + "," +
                mfSideAttackOffset + "," +
                mfUpDownAttackOffset + "," +
                mfAttackBaseSizeMultiplier + "," +
                (mbDisjointed ? 1 : 0) + "," +
                (mbRevolveAround ? 1 : 0) + "," +
                mfTravelDistance + "," +
                mfLifeTime + "," +
                mfMovementForce + "," +
                (int)meAttackMovement + "," +
                (mbHasAnimationFlip ? 1 : 0) + "," +
                mnAttackerAnimation + "," +
                (mbSingleAnimationAttack ? 1 : 0) + "," +
                mcAttackAnimationString + "," +
                (int)meAttackCapsuleColliderDirection + "," +
                (mbParticleTrailEnabled ? 1 : 0) + "," +
                mcParticleTrailColor.ToHexString() + "," +
                mfRotationRate + "," +
                (mbGroundOrigination ? 1 : 0) + "," +
                (mbWallOrigination ? 1 : 0) + "," +
                (mbHasSubAttack ? 1 : 0) + "," +
                mnSubAttackIndex + "," +
                (mbTangible ? 1 : 0) + "," +
                mfAdjustedAngle + "," +
                mnNumberOfInstances + "," +
                mfMass + "," +
                mfGravityScale + "," +
                mfAttackDelay + "," +
                (mbCreateSubAttackOnEnd ? 1 : 0) + "," +
                (int)mcSubAttackDirectionOnRight + "," +
                (int)mcSubAttackDirectionOnLeft + "," +
                (int)mcSubAttackDirectionOnUp + "," +
                (int)mcSubAttackDirectionOnDown + "," +
                (mbMoveTowardsTarget ? 1 : 0) + "," +
                (mbAnimateAttacker ? 1 : 0) + "," +
                mfKnockBack + "," +
                (mbHasHitbox ? 1 : 0) + "," +
                (mbHitBoxIsTrigger ? 1 : 0) + "," +
                mfForceAmplifier 
                ;

        return lbReturn;
    }
}
