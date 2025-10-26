using System;

public class Enemy
{
    private string mcEnemyName; 

    //String references to Enemy animations
	private string mcIdleAnimation;
    private string mcMoveAnimation;
    private string mcJumpAnimation;
    private string mcAttackAnimation;
    private string mcWindupAnimation;

    //Enemy Movement Speed
    private float mfMovementSpeed;

    //Enemy Jump power
    private float mfJumpPower;

    //If enemy is a flying enemy
    private bool mbFlyingEnemy = false;

    //Wind up time for attack in seconds
    private float mfWindUpTime;

    //Distance in path units that causes enemy aggression
    private int mnAgroDistance;

    //Hit points assigned to enemy
    private int mnEnemyHitPoints;

    /*
     * METHOD: Enemy constructor 
     */
    public Enemy(string pnEnemyName, float pfMoveSpeed = 10, float pfJumpPower = 10,
        bool pbFlying = false, float pfWindUpTimeSec = 0, int pnAgroDistance = 5, int pnEnemyHP = 30)
	{
        mcEnemyName = pnEnemyName;
        mfMovementSpeed = pfMoveSpeed;
        mfJumpPower = pfJumpPower;
        mbFlyingEnemy = pbFlying;
        mfWindUpTime = pfWindUpTimeSec;
        mnAgroDistance = pnAgroDistance;
        mnEnemyHitPoints = pnEnemyHP;
    }

    /*
     * METHOD: Move Speed Setter
     */
    public void SetMovementSpeed(float pfMoveSpeed)
    {
        mfMovementSpeed = pfMoveSpeed;
    }

    /*
     * METHOD: Jump Power Setter
     */
    public void SetMovementSpeed(float pfJumpPower)
    {
        mfJumpPower = pfJumpPower;
    }

    /*
     * METHOD: Flying enemy Setter
     */
    public void IsFlyingEnemy(bool pbFlying)
    {
        mbFlyingEnemy = pbFlying;
    }

    /*
     * METHOD: Wind up Time Setter
     */
    public void SetWindUpTime(float pfWindUpTimeSec)
    {
        mfWindUpTime = pfWindUpTimeSec;
    }

    /*
     * METHOD: Agro Distance Setter
     */
    public void SetAgroDistance(int pnAgroDistance)
    {
        mnAgroDistance = pnAgroDistance;
    }

    /*
     * METHOD: Hit point setter
     */
    public void SetHitPoints(int pnEnemyHitPoints)
    {
        mnEnemyHitPoints = pnEnemyHitPoints;
    }

    /*
     * METHOD: Take Hit point damage 
     * RETURN: if HP drops below zero
     */
    public bool TakeDamage(int pnDamage)
    {
        mnEnemyHitPoints -= pnDamage;

        bool lbDefeated = false; 

        if(mnEnemyHitPoints <= 0)
        {
            lbDefeated = true
        }

        return lbDefeated; 
    }
}
