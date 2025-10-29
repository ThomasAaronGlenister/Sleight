using System;
using UnityEngine;
using Deck;

public class EnemyAttributes
{
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


    //Animation clip string identifiers
    private string mcEnemyAttackAnimationString;
    private string mcEnemyDamagedAnimationString;
    private string mcEnemyMoveAnimationString;
    private string mcEnemyWindupAnimationString;
    private string mcEnemyJumpAnimationString;


    //Constructor
    public EnemyAttributes(
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
        float pfAttackWindupTime = 0f)
    {
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
     }

    public void SetEnemyAnimations(
        string pcEnemyAttackAnimationString,
        string pcEnemyDamagedAnimationString,
        string pcEnemyMoveAnimationString,
        string pcEnemyWindupAnimationString,
        string pcEnemyJumpAnimationString)
    {
         mcEnemyAttackAnimationString = pcEnemyAttackAnimationString;
         mcEnemyDamagedAnimationString = pcEnemyDamagedAnimationString;
         mcEnemyMoveAnimationString = pcEnemyMoveAnimationString;
         mcEnemyWindupAnimationString = pcEnemyWindupAnimationString;
         mcEnemyJumpAnimationString = pcEnemyJumpAnimationString;
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

    //Animation string getters
    public string GetEnemyAttackAnimationString()
    {  return mcEnemyAttackAnimationString; }
    public string GetEnemyDamageAnimationString()
    { return mcEnemyDamagedAnimationString; }
    public string GetEnemyMoveAnimationString()
    { return mcEnemyMoveAnimationString; }
    public string GetWindupAttackAnimationString()
    { return mcEnemyWindupAnimationString; }
    public string GetEnemyJumpAnimationString()
    { return mcEnemyJumpAnimationString; }
}
