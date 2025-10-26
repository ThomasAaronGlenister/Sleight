using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Deck;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    private int HealthPoints = 500;

    //Particle Smoke
    public ParticleSystem smokeFX;

    //Target component for the path finding check
    public Transform mcTarget;

    //Animation controller
    public Animator mcAnimator;

    //specifies if enemy is flying or not
    private bool mbFlyingEnemy = false;

    //Move speed of the enemy
    public float mfMovementSpeed = 30f;

    //Distance between path points for seeker
    public float mfNextWaypointDistance = 3f;

    bool isFacingRight = false;

    public LayerMask mcGroundLayer;

    //Jump power of the enemy
    public float mfJumpForce = 200f;

    Path mcPath;
    int mnCurrentWaypoint = 0;

    Seeker mcSeeker;
    Rigidbody2D mcRigidBody;
    SpriteRenderer mcSpriteRenderer;

    private Color mcOriginalColor;

    public bool mbIsGrounded;

    //Specifies distance to target for enemy to Aggro
    //Value is referenced to path units
    public int mnFollowDistance = 7;

    //Specifies distance to target for enemy to Attack
    //Value is referenced to path units
    public int mnAttackDistance = 2;

    public float mfStunTime = 0.5f;

    private float mfAttackCoolDown = 2f;

    private float mfAttackCoolDownElapsedTime = 0;

    //Force applied with the attack
    private float mfAttackLungeHorizontal = 5;

    //Force applied with the attack
    private float mfAttackLungeVertical = 0.4f;

    //Force applied on hit
    private float mfDamageKnockBackVertical = 0.2f;

    //Reference to attack generator child
    public AttackGenerator mcAttackGenerator;

    // Start is called before the first frame update
    void Start()
    {
        mcSeeker = GetComponent<Seeker>();
        mcRigidBody = GetComponent<Rigidbody2D>();
        mcAnimator = GetComponent<Animator>();
        mcSpriteRenderer = GetComponent<SpriteRenderer>();
        mcOriginalColor = mcSpriteRenderer.color;   

        InvokeRepeating("UpdatePath", 0f, 0.5f);

        Time.fixedDeltaTime = 1.0f / 60f;
    }

    public bool WithinAttackRange()
    {
        bool lbWithinRange = false;
        if (mcPath != null && mcPath.vectorPath.Count <= mnAttackDistance)
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
    }

    private IEnumerator Attack()
    {
        mcAnimator.SetTrigger("Attack");

        mcRigidBody.velocity = Vector3.zero;

        //Pass attack card set along with direction to attack generator
        mcAttackGenerator.GenerateAttack(EnemyAttacks.eeChaseFreakAttack_1, (isFacingRight) ? AttackDirection.eeRightward : AttackDirection.eeLeftward);

        //Direction is Waypoint plus the current enemy position
        Vector2 lcDirection = ((Vector2)mcPath.vectorPath[mnCurrentWaypoint] - mcRigidBody.position).normalized;

        if (lcDirection.y < 0.1f)
        {
            lcDirection.y += mfAttackLungeVertical;
        }

        mcRigidBody.AddForce(lcDirection * mfAttackLungeHorizontal, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        mcAnimator.SetTrigger("Reset");
    }

    //METHOD:: Receives Damage input to the enemy
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection)
    {
        HealthPoints -= pnDamageAmount;

        StartCoroutine(TakeDamage());

        if (HealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator TakeDamage()
    {
        mcAnimator.SetTrigger("Damaged");
        mcRigidBody.velocity = Vector3.zero;

        smokeFX.Play();

        //Direction is Waypoint minus the current enemy position
        Vector2 lcDirection = (transform.position - mcTarget.position).normalized;
        if(lcDirection.y < 0.1f)
        {
            lcDirection.y += mfDamageKnockBackVertical; 
        }

        mcRigidBody.AddForce(lcDirection * 20, ForceMode2D.Impulse);

        mcSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(mfStunTime);
        mcSpriteRenderer.color = mcOriginalColor;

        mcAnimator.SetTrigger("Reset");
    }

    /*
     * METHOD: Continuously called and resets path 
     */
    void UpdatePath()
    {
        if(mcSeeker.IsDone())
        {
            mcSeeker.StartPath(mcRigidBody.position, mcTarget.position, OnPathComplete);
        }
    }

    private void Update()
    {
        mfAttackCoolDownElapsedTime += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mcAnimator.SetFloat("magnitude", mcRigidBody.velocity.magnitude);
        mcAnimator.SetFloat("yVelocity", mcRigidBody.velocity.y);

        if (mcPath == null)
        {
            return;
        }

        //Check if path points exist, if not do not move
        if(mnCurrentWaypoint >= mcPath.vectorPath.Count)
        {
            return;
        }

        //If target is within range of Attack
        if (WithinAttackRange() && (mfAttackCoolDownElapsedTime > mfAttackCoolDown))
        {
            StartCoroutine(Attack());
            mfAttackCoolDownElapsedTime = 0;
        }
        //If target is within range of Follow
        else if (WithinAggroRange())
        {
            //Direction is Waypoint minus the current enemy position
            Vector2 lcDirection = ((Vector2)mcPath.vectorPath[mnCurrentWaypoint] - mcRigidBody.position).normalized;
            Vector2 lcForce = lcDirection * mfMovementSpeed * Time.deltaTime;

            mbIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, mcGroundLayer);

            //if Enemy is flying ground check need not occur
            if (!mbFlyingEnemy)
            {
                float lfJumpPower = mcRigidBody.velocity.y;

                lcForce.x = ((lcForce.x > 0f) ? 1 : -1) * mfMovementSpeed;
                if (lcForce.y > 0.01 && mbIsGrounded)
                {
                    lfJumpPower = mfJumpForce;
                }

                mcRigidBody.velocity = new Vector2(((lcForce.x > 0f) ? 1 : -1) * mfMovementSpeed, lfJumpPower);

            }
            else
            {
                mcRigidBody.AddForce(lcForce);
            }

        }

        float lfDistance = Vector2.Distance(mcRigidBody.position, mcPath.vectorPath[mnCurrentWaypoint]);

        if (lfDistance < mfNextWaypointDistance)
        {
            mnCurrentWaypoint++;
        }
        else
        {
            Flip();
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

    private void Flip()
    {
        if (isFacingRight && mcRigidBody.velocity.x < -0.1f || !isFacingRight && mcRigidBody.velocity.x > 0.1f)
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
        if(mcPath != null && mcPath.vectorPath.Count <= mnFollowDistance)
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
    }
}
