using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Deck;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    //Total current health points maintained by the enemy
    private int mnHealthPoints = 100;

    //Total current Armor points maintained by the enemy
    private int mnArmorPoints = 100;

    //Particle Smoke
    public ParticleSystem smokeFX;

    //Target component for the path finding check
    public Transform mcTarget;

    //Animation controller
    public Animator mcAnimator;

    //Value for distance to target, updated every frame
    public float mfDistanceToTarget = 10;

    //Facing right flag
    bool isFacingRight = false;

    //Layer check for touching ground
    public LayerMask mcGroundLayer;
    public bool mbIsGrounded;

    //A* Path variables
    Path mcPath;
    int mnCurrentWaypoint = 0;
    Seeker mcSeeker;

    //Enemy Hurt Box
    Rigidbody2D mcRigidBody;

    //Enemy Sprite Renderer
    SpriteRenderer mcSpriteRenderer;

    //Base color
    private Color mcOriginalColor;

    //Member to maintain time between attacks
    private float mfAttackCoolDownElapsedTime = 0;

    //State maintainer
    public EnemyState meUpdateEnemyState;

    //Enemy Attributes driving behavoir
    EnemyAttributes mcEnemyAttributes;

    //Reference to attack generator child
    public AttackGenerator mcAttackGenerator;

    // Start is called before the first frame update
    void Start()
    {
        mcTarget = GameObject.Find("Player").transform;
        mcSeeker = GetComponent<Seeker>();
        mcRigidBody = GetComponent<Rigidbody2D>();
        mcAnimator = GetComponent<Animator>();
        mcSpriteRenderer = GetComponent<SpriteRenderer>();
        mcOriginalColor = mcSpriteRenderer.color;

        meUpdateEnemyState = EnemyState.eeEnemyIdle;

        InvokeRepeating("UpdatePath", 0f, 0.5f);

        Time.fixedDeltaTime = 1.0f / 60f;

    }

    //Assigns a set of Attributes
    public void SetEnemyAttributes(EnemyAttributes pcEnemyAttributes)
    {
        mcEnemyAttributes = pcEnemyAttributes;

        mnHealthPoints = mcEnemyAttributes.GetHealthPoints();
        mnArmorPoints = mcEnemyAttributes.GetArmorPoints();
    }

    public bool WithinAttackRange()
    {
        bool lbWithinRange = false;
        if (mcPath != null && mcPath.vectorPath.Count <= mcEnemyAttributes.GetAttackDistance())
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
    }

    //METHOD:: Receives Damage input to the enemy
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection)
    {
        mnHealthPoints -= pnDamageAmount;

        StartCoroutine(TakeDamage());

        if (mnHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Attack()
    {
        meUpdateEnemyState = EnemyState.eeEnemyKnockback;

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyWindup);

        mcRigidBody.velocity = Vector3.zero;

        yield return new WaitForSeconds(mcEnemyAttributes.GetEnemyAttackWindupTime());

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyAttack);

        yield return null;

        //Pass attack card set along with direction to attack generator
        mcAttackGenerator.GenerateAttack(EnemyAttacks.eeChaseFreakAttack_1, 5, (isFacingRight) ? AttackDirection.eeRightward : AttackDirection.eeLeftward);

        //Direction is Waypoint plus the current enemy position
        Vector2 lcDirection = (mcTarget.position - transform.position).normalized;
        lcDirection.y += 0.3f;

        mcRigidBody.AddForce(lcDirection * 4, ForceMode2D.Impulse);

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyIdle);

        yield return new WaitForSeconds(1);

        meUpdateEnemyState = EnemyState.eeEnemyIdle;
    }

    /*
     * METHOD: Coroutine to Take Damage function called to update enemy health and apply knockback
     */
    private IEnumerator TakeDamage()
    {
        meUpdateEnemyState = EnemyState.eeEnemyKnockback;

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyKnockback);
        mcRigidBody.velocity = Vector3.zero;
        yield return null;

        //Direction is Waypoint minus the current enemy position
        Vector2 lcDirection = (transform.position - mcTarget.position).normalized;

        //TODO: Add KnockBack to attack attributes
        lcDirection.y += 0.5f; 

        mcRigidBody.AddForce(lcDirection * 4, ForceMode2D.Impulse);

        mcSpriteRenderer.color = Color.red;

        //TODO: Set to attacks stun time
        yield return new WaitForSeconds(0.5f);
        mcSpriteRenderer.color = mcOriginalColor;

        meUpdateEnemyState = EnemyState.eeEnemyIdle;
        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyIdle);
    }

    /*
     * METHOD: Continuously called and resets path 
     */
    private void UpdatePath()
    {
        if(mcSeeker.IsDone())
        {
            mcSeeker.StartPath(mcRigidBody.position, mcTarget.position, OnPathComplete);
        }
    }

    private void Update()
    {
        mfAttackCoolDownElapsedTime += Time.deltaTime;

        //Handle Animation updates
        mbIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, mcGroundLayer);
        mcAnimator.SetBool("Grounded", mbIsGrounded);

        mfDistanceToTarget = Vector2.Distance(mcRigidBody.position, mcTarget.position);

        //Check if enemy is facing correct way
        Flip();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Check if path points exist, if not do not move
        if (mcPath == null || mnCurrentWaypoint >= mcPath.vectorPath.Count)
        {
            return;
        }

        //If target is within range of Attack
        if (WithinAttackRange() && (mfAttackCoolDownElapsedTime > mcEnemyAttributes.GetAttackCooldown()) && meUpdateEnemyState != EnemyState.eeEnemyKnockback)
        {
            meUpdateEnemyState = EnemyState.eeEnemyAttack;
            StartCoroutine(Attack());
            mfAttackCoolDownElapsedTime = 0;
        }
        else if(mfDistanceToTarget < 1f)
        {

        }
        //If target is within range of Follow
        else if (meUpdateEnemyState != EnemyState.eeEnemyKnockback)
        {
            //Direction is Waypoint minus the current enemy position
            Vector2 lcDirection = ((Vector2)mcPath.vectorPath[mnCurrentWaypoint] - mcRigidBody.position).normalized;
            Vector2 lcForce = lcDirection * mcEnemyAttributes.GetMovementSpeed() * Time.deltaTime;

            //if Enemy is flying ground check need not occur
            if (!mcEnemyAttributes.GetFlyingEnemy())
            {
                float lfJumpPower = mcRigidBody.velocity.y;

                lcForce.x = ((lcForce.x > 0f) ? 1 : -1) * mcEnemyAttributes.GetMovementSpeed();
                if (lcDirection.y > 0.4 && mbIsGrounded)
                {
                    lfJumpPower = mcEnemyAttributes.GetJumpPower();
                }

                mcRigidBody.velocity = new Vector2(((lcForce.x > 0f) ? 1 : -1) * mcEnemyAttributes.GetMovementSpeed(), lfJumpPower);
            }
            else
            {
                mcRigidBody.AddForce(lcForce);
                meUpdateEnemyState = EnemyState.eeEnemyMove;
            }
        }

        //Update Path finding 
        float lfDistance = Vector2.Distance(mcRigidBody.position, mcPath.vectorPath[mnCurrentWaypoint]);
        if (lfDistance < mcEnemyAttributes.GetWaypointDistance())
        {
            mnCurrentWaypoint++;
        }


    }

    /*
     * METHOD: Resets path based on target and enemy position
     */
    void OnPathComplete(Path lcPath)
    {
        if(!lcPath.error)
        {
            mcPath = lcPath;
            mnCurrentWaypoint = 0;
        }
    }


    //Method to reverse direction the enemy is facing
    private void Flip()
    {
        if (isFacingRight && mcTarget.position.x < transform.position.x || !isFacingRight && mcTarget.position.x > transform.position.x)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    /*
     * METHOD: checks if enemy is within path point range to go aggro
     */
    bool WithinAggroRange()
    {
        bool lbWithinRange = false;
        if(mcPath != null && mcPath.vectorPath.Count <= mcEnemyAttributes.GetFollowDistance())
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
    }
}
