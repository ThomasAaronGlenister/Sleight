using System;
using UnityEngine;
using Deck;
using System.Collections.Generic;

public class EnemyAttributes
{
    //Enemy Name
    private string mcEnemyName;

    //Enemy HP
    private int mnHealthPoints;

    //Enemy Armor
    private int mnArmorPoints;

    //Enemy Movement Speed
    private float mfMovementSpeed;

    //Enemy jump power
    private float mfJumpPower;

    //Enemy Mass
    private float mfEnemyMass;

    //Enemy Rigid Body Gravity scale
    private float mfGravityScale;

    //Enemy Rigid Body Angular Drag
    private float mfAngularDrag;

    //If this enemy flies
    private bool mbFlyingEnemy;

    //Distance from target to start following
    private float mfFollowDistance;

    //Distance from target to start attacking
    private float mfAttackDistance;

    //Time between attacks
    private float mfAttackCooldown;

    //Force applied on enemy during attack
    private float mfAttackLungeMultiplier;
    private float mfAttackLungeVertical;

    //Distance between tracking points for the enemy
    private float mfWaypointDistance;

    //Enemy attack Damage
    private int mnAttackDamage;

    //Enemy attack wind up time
    private float mnAttackWindupTime;

    //Size adjustment for enemy
    private float mfSizeMultiplier = 1;

    //Animation Clips Path
    private string mcEnemyAnimationClipsPath;

    //Animation clip string identifiers
    private AnimationClip mcEnemyAttackAnimationClip = null;
    private AnimationClip mcEnemyAttack2AnimationClip = null;
    private AnimationClip mcEnemyAttack3AnimationClip = null;
    private AnimationClip mcEnemyDamageAnimationClip;
    private AnimationClip mcEnemyMoveAnimationClip;
    private AnimationClip mcEnemyWaitAnimationClip;
    private AnimationClip mcEnemyJumpAnimationClip;
    private AnimationClip mcEnemyIdleAnimationClip;

    private List<AnimationClip> macEnemyAttackAnimations = new List<AnimationClip>();

    //Enemy hitbox size and offsets
    private float mfBoxColliderWidthX = 1;
    private float mfBoxColliderHeightY = 1;
    private float mfBoxColliderXOffset = 0;
    private float mfBoxColliderYOffset = 0;

    private float mnMovementTargetOffsetX = 0;
    private float mnMovementTargetOffsetY = 0;

    private bool mbForceMovement = false;

    private float mfIdleMovementRange = 0;
    private float mfIdleMovementSpeed = 1;

    //List of attacks this enemy can perform
    List<AttackAttributes> macEnemyAttacks = new List<AttackAttributes>();

    //Constructor
    public EnemyAttributes(
        String name,
        int pnHealthPoints = 100,
        int pnArmorPoints = 0,
        float pfMovementSpeed = 1,
        float pfJumpPower = 1,
        float pfEnemyMass = 1,
        float pfGravityScale = 1,
        bool pbFlyingEnemy = false,
        float pfFollowDistance = 7,
        float pfAttackDistance = 2,
        float pfAttackCooldown = 1,
        float pfAttackLungeVertical = 0,
        float pfAttackLungeMultiplier = 1,
        float pfWaypointDistance = 3,
        int pnAttackDamage = 0,
        float pfAttackWindupTime = 0f,
        float pfSizeMultiplier = 1,
        float pfBoxColliderWidthX = 1,
        float pfBoxColliderHeightY = 1,
        float pfBoxColliderXOffset = 0,
        float pfBoxColliderYOffset = 0,
        float pnMovementTargetOffsetX = 0,
        float pnMovementTargetOffsetY = 0,
        bool pbForceMovement = false,
        float pfIdleMovementRange = 3,
        float pfIdleMovementSpeed = 1)
    {
        mcEnemyName = name;
        mnHealthPoints = pnHealthPoints;
        mnArmorPoints = pnArmorPoints;
        mfMovementSpeed = pfMovementSpeed;
        mfJumpPower = pfJumpPower;
        mfEnemyMass = pfEnemyMass;
        mfGravityScale = pfGravityScale;
        mbFlyingEnemy = pbFlyingEnemy;
        mfFollowDistance = pfFollowDistance;
        mfAttackDistance = pfAttackDistance;
        mfAttackCooldown = pfAttackCooldown;
        mfAttackLungeVertical = pfAttackLungeVertical;
        mfAttackLungeMultiplier = pfAttackLungeMultiplier;
        mfWaypointDistance = pfWaypointDistance;
        mnAttackDamage = pnAttackDamage;
        mnAttackWindupTime = pfAttackWindupTime;
        mfSizeMultiplier = pfSizeMultiplier;
        mfBoxColliderWidthX = pfBoxColliderWidthX;
        mfBoxColliderHeightY = pfBoxColliderHeightY;
        mfBoxColliderXOffset = pfBoxColliderXOffset;
        mfBoxColliderYOffset = pfBoxColliderYOffset;
        mnMovementTargetOffsetX = pnMovementTargetOffsetX;
        mnMovementTargetOffsetY = pnMovementTargetOffsetY;
        mbForceMovement = pbForceMovement;
        mfIdleMovementRange = pfIdleMovementRange;
        mfIdleMovementSpeed = pfIdleMovementSpeed;
        SetEnemyAnimations("Animations/EnemyAnimations/" + mcEnemyName + "Animations");
    }

    //Adds a attack the enemy may perform
    public void AddEnemyAttack(AttackAttributes pcNewAttack)
    {
        macEnemyAttacks.Add(pcNewAttack);
    }

    public void SetEnemyAnimations(
        string pcEnemyAttackAnimationClipsPath)
    {
        mcEnemyAnimationClipsPath = pcEnemyAttackAnimationClipsPath;

        AnimationClip[] EnemyClips = Resources.LoadAll<AnimationClip>(mcEnemyAnimationClipsPath);

        foreach (AnimationClip lcClip in EnemyClips)
        {
            if(lcClip.name == mcEnemyName + "Move")
            {
                mcEnemyMoveAnimationClip = lcClip;
            }
            else if(lcClip.name == mcEnemyName + "Attack")
            {
                mcEnemyAttackAnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Attack2")
            {
                mcEnemyAttack2AnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Attack3")
            {
                mcEnemyAttack3AnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Damage")
            {
                mcEnemyDamageAnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Wait")
            {
                mcEnemyWaitAnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Jump")
            {
                mcEnemyJumpAnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Idle")
            {
                mcEnemyIdleAnimationClip = lcClip;
            }
        }
    }

    /**
     * Gets the attack to be performed based on enemy position and attributes
     * pfGroundDistance - distance enemy is from groundq
     * pfWallDistance - distance enemy is from wall
     * pfTargetPositionX - Horizontal distance from enemy to target
     * pfTargetPositionY - Vertical distance from enemy to target
     * 
     * Returns Null if no attack is within range
     */
    public AttackAttributes GetAttack(float pfGroundDistance, float pfWallDistance, float pfTargetPositionX, float pfTargetPositionY)
    {
        AttackAttributes lcReturnAttack = null;

        //Create list of possible attacks that can be performed
        List<AttackAttributes> lacPossibleAttacks = new List<AttackAttributes>();

        foreach(AttackAttributes lcAttackAttributes in macEnemyAttacks)
        {
            if(lcAttackAttributes.WithinAttackBounds(pfGroundDistance, pfWallDistance, pfTargetPositionX, pfTargetPositionY))
            {
                //add same attack mulitple times to list depending on chance value
                for(int lnAttAtt = 0; lnAttAtt < lcAttackAttributes.GetChanceValue(); lnAttAtt++)
                {
                    lacPossibleAttacks.Add(lcAttackAttributes);
                }
            }
        }

        //Get random attack from list 
        if(lacPossibleAttacks.Count != 0)
        {
            lcReturnAttack = lacPossibleAttacks[UnityEngine.Random.Range(0, lacPossibleAttacks.Count)];
        }

        return lcReturnAttack;
    }

    //Attribute Getters
    public int GetHealthPoints()
        { return mnHealthPoints; }
    public int GetArmorPoints()
        { return mnArmorPoints; }
    public float GetMovementSpeed()
        { return mfMovementSpeed; }
    public float GetJumpPower()
        { return mfJumpPower; }
    public float GetEnemyMass()
    { return mfEnemyMass; }
    public float GetGravityScale()
        { return mfGravityScale; }
    public bool GetFlyingEnemy()
        { return mbFlyingEnemy; }
    public float GetFollowDistance()
        { return mfFollowDistance; }
    public float GetAttackDistance()
        { return mfAttackDistance; }
    public float GetAttackCooldown()
        { return mfAttackCooldown; }
    public float GetAttackLungeMultiplier()
        { return mfAttackLungeMultiplier; }
    public float GetAttackLungeVertical()
        { return mfAttackLungeVertical; }
    public float GetWaypointDistance()
        { return mfWaypointDistance; }
    public int GetEnemyAttackDamage()
        { return mnAttackDamage; }
    public float GetEnemyAttackWindupTime()
        { return mnAttackWindupTime; }
    public float GetSizeMultiplier()
    { return mfSizeMultiplier; }

    //BoxCollider Hitbox settings Getters
    public Vector2 GetBoxColliderSize()
    { return new Vector2(mfBoxColliderWidthX, mfBoxColliderHeightY); }
    public Vector2 GetBoxColliderOffset()
    { return new Vector2(mfBoxColliderXOffset, mfBoxColliderYOffset); }

    //Animation string getters
    public string GetEnemyAnimationClips()
    {  return mcEnemyAnimationClipsPath; }

    public AnimationClip GetEnemyMoveAnimationClip()
    { return mcEnemyMoveAnimationClip; }

    //Attack animation clips
    public AnimationClip GetEnemyAttackAnimationClip()
    { return mcEnemyAttackAnimationClip; }
    public AnimationClip GetEnemyAttack2AnimationClip()
    { return mcEnemyAttack2AnimationClip; }
    public AnimationClip GetEnemyAttack3AnimationClip()
    { return mcEnemyAttack3AnimationClip; }

    public AnimationClip GetEnemyJumpAnimationClip()
    { return mcEnemyJumpAnimationClip; }
    public AnimationClip GetEnemyWaitAnimationClip()
    { return mcEnemyWaitAnimationClip; }
    public AnimationClip GetEnemyDamageAnimationClip()
    { return mcEnemyDamageAnimationClip; }
    public AnimationClip GetEnemyIdleAnimationClip()
    { return mcEnemyIdleAnimationClip; }

    public float GetMovementTargetOffsetX()
    { return mnMovementTargetOffsetX; }

    public float GetMovementTargetOffsetY()
    { return mnMovementTargetOffsetY; }

    public bool GetForceMovement()
    { return mbForceMovement; }

    public float GetIdleMovementRange()
    { return mfIdleMovementRange; }

    public float GetIdleMovementSpeed()
    { return mfIdleMovementSpeed; }
}
