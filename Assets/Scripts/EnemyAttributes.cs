using System;
using UnityEngine;
using Deck;

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

    //Enemy Attack
    EnemyAttacks meEnemyAttack;

    //Enemy attack Damage
    private int mnAttackDamage;

    //Enemy attack wind up time
    private float mnAttackWindupTime;

    //Size adjustment for enemy
    private float mfSizeMultiplier = 1;

    //Animation Clips Path
    private string mcEnemyAnimationClipsPath;

    //Animation clip string identifiers
    private AnimationClip mcEnemyAttackAnimationClip;
    private AnimationClip mcEnemyDamageAnimationClip;
    private AnimationClip mcEnemyMoveAnimationClip;
    private AnimationClip mcEnemyWindupAnimationClip;
    private AnimationClip mcEnemyJumpAnimationClip;
    private AnimationClip mcEnemyIdleAnimationClip;

    //Enemy hitbox size and offsets
    private float mfBoxColliderWidthX = 1;
    private float mfBoxColliderHeightY = 1;
    private float mfBoxColliderXOffset = 0;
    private float mfBoxColliderYOffset = 0;


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
        EnemyAttacks peEnemyAttack = EnemyAttacks.eeNone,
        int pnAttackDamage = 0,
        float pfAttackWindupTime = 0f,
        float pfSizeMultiplier = 1,
        float pfBoxColliderWidthX = 1,
        float pfBoxColliderHeightY = 1,
        float pfBoxColliderXOffset = 0,
        float pfBoxColliderYOffset = 0)
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
        meEnemyAttack = peEnemyAttack;
        mnAttackDamage = pnAttackDamage;
        mnAttackWindupTime = pfAttackWindupTime;
        mfSizeMultiplier = pfSizeMultiplier;
        mfBoxColliderWidthX = pfBoxColliderWidthX;
        mfBoxColliderHeightY = pfBoxColliderHeightY;
        mfBoxColliderXOffset = pfBoxColliderXOffset;
        mfBoxColliderYOffset = pfBoxColliderYOffset;
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
            else if (lcClip.name == mcEnemyName + "Damage")
            {
                mcEnemyDamageAnimationClip = lcClip;
            }
            else if (lcClip.name == mcEnemyName + "Windup")
            {
                mcEnemyWindupAnimationClip = lcClip;
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
    public EnemyAttacks GetEnemyAttack()
        { return meEnemyAttack;  }
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
    public AnimationClip GetEnemyAttackAnimationClip()
        { return mcEnemyAttackAnimationClip; }
    public AnimationClip GetEnemyJumpAnimationClip()
    { return mcEnemyJumpAnimationClip; }
    public AnimationClip GetEnemyWindupAnimationClip()
    { return mcEnemyWindupAnimationClip; }
    public AnimationClip GetEnemyDamageAnimationClip()
    { return mcEnemyDamageAnimationClip; }
    public AnimationClip GetEnemyIdleAnimationClip()
    { return mcEnemyIdleAnimationClip; }

}
