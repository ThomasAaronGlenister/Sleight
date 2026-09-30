using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Deck;
using TMPro;
using System;

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

    //offset to the target this enemy should be moving towards
    public Vector3 mcTargetPositionOffset = Vector2.zero;

    //Animation controller
    public Animator mcAnimator;

    //Value for distance to target, updated every frame
    public float mfDistanceToTarget = 10;
    public float mfDistanceToTarget_X = 10;
    public float mfDistanceToTarget_Y = 10;

    //Value for distance to ground and walls 
    public float mfDistanceToGround = 10;
    public float mfDistanceToWall = 10;

    //Facing right flag
    bool isFacingRight = false;

    //Flag Determining if enemy is faced away from target
    bool mbFacingAwayFromTarget = false;

    //Flag indicating the enemy 
    bool mbAggroed = false;

    //Layer check for distance to wall
    public LayerMask mcWallLayer;

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

    //Enemy Box Collider
    BoxCollider2D mcEnemyBoxCollider;

    //Base color
    private Color mcOriginalColor;

    //Member to maintain time between attacks
    private float mfAttackCoolDownElapsedTime = 0;

    //Member to maintain time between Jumps
    private float mfJumpCoolDownElapsedTime = 0;

    //Member to maintain time between Idle Movements
    private float mfIdleCoolDownElapsedTime = 0;

    //State maintainer
    public EnemyState meUpdateEnemyState;

    //Enemy Attributes driving behavoir
    EnemyAttributes mcEnemyAttributes;

    //Reference to attack generator child
    public AttackGenerator mcAttackGenerator;

    // Animation overrides
    private AnimatorOverrideController mcAnimationOverrideController;

    //Set of durations suit effects are applied for
    float[] mafEffectAppliedTime = new float[(int)SuitEffect.eeSuitEffectEnd]
    {0, 0, 0, 0, 0, 0, 0};

    List<GameObject> macEffectIcons = new List<GameObject>();

    bool mbDebug = true;

    //Flag dictating that enemy may move 
    public bool mbLockMovement = false;

    public float mfHorizontalMovementMultiplier = 1;

    //Defines the next Direction the enemy should move to while idling
    private Vector2 mcIdleDirection = new Vector2(1,1);

    // Start is called before the first frame update
    void Start()
    {
        mcTarget = GameObject.Find("Player").transform;
        mcSeeker = GetComponent<Seeker>();
        mcRigidBody = GetComponent<Rigidbody2D>();
        mcAnimator = GetComponent<Animator>();
        mcSpriteRenderer = GetComponent<SpriteRenderer>();
        mcSpriteRenderer.material.SetFloat("_DissolveAmount", 0);
        mcEnemyBoxCollider = GetComponent<BoxCollider2D>();
        mcOriginalColor = mcSpriteRenderer.color;

        meUpdateEnemyState = EnemyState.eeEnemyIdle;

        //Get access to effect icons
        if (GameObject.Find("EffectArc"))
        {
            foreach (Transform child in GameObject.Find("EffectArc").transform)
            {
                macEffectIcons.Add(child.gameObject);
            }
        }

        Time.fixedDeltaTime = 1.0f / 60f;

    }

    //Collects root motion data to be applied 
    private void OnAnimatorMove()
    {
        mcRigidBody.position = mcRigidBody.position + (Vector2)mcAnimator.deltaPosition;
    }

    //Assigns a set of Attributes
    public void SetEnemyAttributes(EnemyAttributes pcEnemyAttributes)
    {
        mcEnemyAttributes = pcEnemyAttributes;

        transform.localScale *= mcEnemyAttributes.GetSizeMultiplier();

        mcTargetPositionOffset.x = pcEnemyAttributes.GetMovementTargetOffsetX();
        mcTargetPositionOffset.y = pcEnemyAttributes.GetMovementTargetOffsetY();

        //Adjust box collider size based on sprite size
        GetComponent<CapsuleCollider2D>().size = pcEnemyAttributes.GetBoxColliderSize();
        GetComponent<CapsuleCollider2D>().offset = pcEnemyAttributes.GetBoxColliderOffset();

        mnHealthPoints = mcEnemyAttributes.GetHealthPoints();
        mnArmorPoints = mcEnemyAttributes.GetArmorPoints();

        mcRigidBody = GetComponent<Rigidbody2D>();
        mcRigidBody.gravityScale = mcEnemyAttributes.GetGravityScale();
        mcRigidBody.mass = mcEnemyAttributes.GetEnemyMass();

        //Override base enemy animations with attribute assigned assets
        ReplaceEnemyAnimations(mcEnemyAttributes);

        InvokeRepeating("UpdatePath", 0f, 0.5f);

    }

    //Assigns a set of Attributes
    public void ReplaceEnemyAnimations(EnemyAttributes pcEnemyAttributes)
    {
        //Generate override controller from runtime controller
        mcAnimationOverrideController = new AnimatorOverrideController(mcAnimator.runtimeAnimatorController);

        mcAnimator.runtimeAnimatorController = mcAnimationOverrideController;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(mcAnimationOverrideController.overridesCount);
        mcAnimationOverrideController.GetOverrides(overrides);

        //Reassign animation clips 
        for (int i = 0; i < overrides.Count; i++)
        {
            switch (overrides[i].Key.name)
            {
                case "EnemyAttack":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyAttackAnimationClip());
                    break;
                case "EnemyAttack2":
                    if (pcEnemyAttributes.GetEnemyAttack2AnimationClip() != null)
                    {
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyAttack2AnimationClip());
                    }
                    break;
                case "EnemyAttack3":
                    if (pcEnemyAttributes.GetEnemyAttack3AnimationClip() != null)
                    {
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyAttack3AnimationClip());
                    }
                    break;
                case "EnemyMove":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyMoveAnimationClip());
                    break;
                case "EnemyDamage":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyDamageAnimationClip());
                    break;
                case "EnemyWait":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyWaitAnimationClip());
                    break;
                case "EnemyJump":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyJumpAnimationClip());
                    break;
                case "EnemyIdle":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, pcEnemyAttributes.GetEnemyIdleAnimationClip());
                    break;
            }

        }

        //Apply the new animation clips
        mcAnimationOverrideController.ApplyOverrides(overrides);
    }

    //Checks if enemy is in attack range
    public bool WithinAttackRange()
    {
        bool lbWithinRange = false;
        if (mfDistanceToTarget <= mcEnemyAttributes.GetAttackDistance())
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
    }

    //METHOD:: Receives Damage input to the enemy
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection, float[] pafEffects)
    {
        SetSuitEffects(pafEffects);

        StartCoroutine(TakeDamage(pnDamageAmount, pnKnockBack));
    }

    //Method to set Attack Effect durations on damage
    private void SetSuitEffects(float[] pafEffects)
    {
        for (int i = 0; i < pafEffects.Length; i++)
        {
            //If added effect time applied and current effect is not active
            if (pafEffects[i] != 0 && mafEffectAppliedTime[i] <= 0 && macEffectIcons[i] != null)
            {
                mafEffectAppliedTime[i] = pafEffects[i];
                macEffectIcons[i].SetActive(true);
            }
        }
    }

    private void CheckSuitEffects()
    {
        for (int i = 0; i < mafEffectAppliedTime.Length; i++)
        {
            if(mafEffectAppliedTime[i] > 0)
            {
                mafEffectAppliedTime[i] -= Time.deltaTime;

                if(mafEffectAppliedTime[i] <= 0)
                {
                    macEffectIcons[i].SetActive(false);
                }
            }
        }
    }

    //Causes enemy to perform the attack
    private IEnumerator Attack(AttackAttributes pcAttackToPerform)
    {
        meUpdateEnemyState = EnemyState.eeEnemyAttack;

        mcAnimator.SetInteger("EnemyState", pcAttackToPerform.GetEnemyAttackId());

        mcRigidBody.velocity = new Vector2(0, mcRigidBody.velocity.y);

        //Get enemy attack based on distances to target and enemy position
        //Pass attack card set along with direction to attack generator
        mcAttackGenerator.GenerateAttack(pcAttackToPerform, mcEnemyAttributes.GetEnemyAttackDamage(), GetAttackDirection());

        AnimatorStateInfo stateInfo = mcAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(mcEnemyAttributes.GetEnemyAttackAnimationClipLength(pcAttackToPerform.GetEnemyAttackId()));

        Debug.Log("Attack Animation ID: " + pcAttackToPerform.GetEnemyAttackId() + " length " + mcEnemyAttributes.GetEnemyAttackAnimationClipLength(pcAttackToPerform.GetEnemyAttackId()));

        //set cool down time back to zero
        mfAttackCoolDownElapsedTime = 0;

        meUpdateEnemyState = EnemyState.eeEnemyIdle;

        mbLockMovement = false;

        mcAnimator.SetInteger("EnemyState",(int) meUpdateEnemyState);
    }

    private AttackDirection GetAttackDirection()
    {
        AttackDirection leAttackDirection = (isFacingRight) ? AttackDirection.eeRightward : AttackDirection.eeLeftward;

        if (mcRigidBody.position.y - 2 > mcTarget.position.y)
        {
            leAttackDirection = AttackDirection.eeDownwards;
        }
        else if (mcRigidBody.position.y + 2 < mcTarget.position.y)
        {
            leAttackDirection = AttackDirection.eeUpwards;
        }

        return leAttackDirection;
    }

    /*
     * METHOD: Coroutine to Take Damage function called to update enemy health and apply knockback
     */
    private IEnumerator TakeDamage(int pnDamageAmount, float pnKnockBack)
    {
        //If enemy has armor points then no knock back 
        if(mnArmorPoints > 0)
        {
            mnArmorPoints -= pnDamageAmount;
            mcSpriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            mcSpriteRenderer.color = mcOriginalColor;

        }
        //Else remove health points
        else
        {
            mnHealthPoints -= pnDamageAmount;

            if (meUpdateEnemyState == EnemyState.eeEnemyAttack)
            {
                //StopCoroutine(Attack());

                mcAttackGenerator.CancelAttacks();
            }

            meUpdateEnemyState = EnemyState.eeEnemyKnockback;

            mfAttackCoolDownElapsedTime = 0;

            mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyKnockback);
            mcRigidBody.velocity = Vector3.zero;
            yield return null;

            //Direction is Waypoint minus the current enemy position
            Vector2 lcDirection = (transform.position - mcTarget.position).normalized;

            //TODO: Add KnockBack to attack attributes
            lcDirection.y += 0.5f;

            mcRigidBody.AddForce(lcDirection * 2, ForceMode2D.Impulse);

            mcSpriteRenderer.color = Color.red;

            if (mnHealthPoints <= 0)
            {
                float lfDissolve = 0f;
                while (lfDissolve < 1f)
                {
                    lfDissolve += 0.01f;
                    mcSpriteRenderer.material.SetFloat("_DissolveAmount", lfDissolve);
                    yield return null;
                }

                Destroy(gameObject);
            }

            //TODO: Set to attacks stun time
            yield return new WaitForSeconds(0.2f);
            mcSpriteRenderer.color = mcOriginalColor;

            meUpdateEnemyState = EnemyState.eeEnemyIdle;
            mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyIdle);
        }
    }

    /*
     * METHOD: Continuously called and resets path 
     */
    private void UpdatePath()
    {
        if(mcSeeker.IsDone() && mbAggroed)
        {
            mcSeeker.StartPath(mcRigidBody.position, mcTarget.position + mcTargetPositionOffset, OnPathComplete);
        }
    }

    private void Update()
    {
        mfAttackCoolDownElapsedTime += Time.deltaTime;

        mfJumpCoolDownElapsedTime += Time.deltaTime;

        mfIdleCoolDownElapsedTime += Time.deltaTime;

        //Handle Animation updates
        mbIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, (mcEnemyAttributes.GetBoxColliderSize().y / 2) + Mathf.Abs(mcEnemyAttributes.GetBoxColliderOffset().y) + 0.2f, mcGroundLayer);

        mcAnimator.SetBool("Grounded", mbIsGrounded || mcEnemyAttributes.GetFlyingEnemy());

        //Determine distances to target in cardinal space
        mfDistanceToTarget = Vector2.Distance(mcRigidBody.position, mcTarget.position);
        mfDistanceToTarget_X = Math.Abs(mcRigidBody.position.x - mcTarget.position.x);
        mfDistanceToTarget_Y = Math.Abs(mcRigidBody.position.y - mcTarget.position.y);

        RaycastHit2D lcDistance = Physics2D.Raycast(transform.position, Vector3.down, 15, mcGroundLayer);

        if(lcDistance)
        {
            mfDistanceToGround = lcDistance.distance;
        }

        lcDistance = Physics2D.Raycast(transform.position, Vector3.left, 15, mcWallLayer);

        if (lcDistance)
        {
            mfDistanceToWall = lcDistance.distance;
        }
        else
        {
            mfDistanceToWall = 15;
        }

        lcDistance = Physics2D.Raycast(transform.position, Vector3.right, 15, mcWallLayer);

        if (lcDistance)
        {
            //Take the shorter distance if right raycast finds closer wall
            mfDistanceToWall = (mfDistanceToWall > lcDistance.distance) ?  lcDistance.distance : mfDistanceToWall;
        }

        CheckSuitEffects();

        //Check if target within aggro range
        WithinAggroRange();

        if (mbDebug)
        {
            DisplayDebug();
        }
    }

    private void DisplayDebug()
    {
        //Ground ray cast debug
        //Debug.DrawRay(transform.position, Vector2.down, Color.red, ((mcEnemyAttributes.GetBoxColliderSize().y / 2) + 1f));

        Vector2 lcDirection = (mcTarget.transform.position - transform.position).normalized;

        Debug.DrawRay(transform.position, lcDirection * mcEnemyAttributes.GetAttackDistance(), Color.green);
        Debug.DrawRay(transform.position, mcRigidBody.velocity, Color.red);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Enemy is in idle state if not aggroed
        if (!mbAggroed)
        {
            // Set Idle animation
            meUpdateEnemyState = EnemyState.eeEnemyIdle;
            mcAnimator.SetInteger("EnemyState", (int)meUpdateEnemyState);

            //
            if(mfIdleCoolDownElapsedTime > mcEnemyAttributes.GetIdleMovementRange())
            {
                if(mfDistanceToWall < 1)
                {
                    mcIdleDirection.x *= -1;
                }
                else
                {
                    mcIdleDirection.x *= (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
                }

                if (mfDistanceToGround < 1)
                {
                    mcIdleDirection.y *= -1;
                }
                else
                {
                    mcIdleDirection.y *= (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
                }

                mfIdleCoolDownElapsedTime = 0;

            }

            mcRigidBody.velocity = new Vector2 (mcIdleDirection.x * mcEnemyAttributes.GetIdleMovementSpeed(), 
                (mcEnemyAttributes.GetFlyingEnemy()) ? mcIdleDirection.y * mcEnemyAttributes.GetIdleMovementSpeed() : mcRigidBody.velocity.y);

            //Check if enemy is facing correct way
            Flip();
        }

        //Check if path points exist, if not do not move
        if (mcPath == null || mnCurrentWaypoint >= mcPath.vectorPath.Count)
        {
            return;
        }

        //Else Enemy should perform combat actions
        if (meUpdateEnemyState != EnemyState.eeEnemyKnockback && mbAggroed)
        {

            AttackAttributes lcAttackToPerform = mcEnemyAttributes.GetAttack(mfDistanceToGround, mfDistanceToWall, mfDistanceToTarget_X, mfDistanceToTarget_Y);

            //If attack is in range and cooldown has passed
            if (lcAttackToPerform != null && (mfAttackCoolDownElapsedTime > mcEnemyAttributes.GetAttackCooldown()) && mfJumpCoolDownElapsedTime > 2 
                && meUpdateEnemyState != EnemyState.eeEnemyAttack)
            {
                StartCoroutine(Attack(lcAttackToPerform));
            }
            //Wait Period
            else if(mfAttackCoolDownElapsedTime < mcEnemyAttributes.GetAttackCooldown())
            {
                //flip if velocity changes direction
                if ((isFacingRight && mcTarget.position.x < transform.position.x) || (!isFacingRight && mcTarget.position.x > transform.position.x))
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }
                mcRigidBody.velocity = new Vector2(0, mcRigidBody.velocity.y);
                mcAnimator.SetInteger("EnemyState", (int) EnemyState.eeEnemyWait);
            }
            //Else move enemy towards target
            else if (!mbLockMovement)
            {
                //Direction is Waypoint minus the current enemy position
                Vector2 lcDirection = ((Vector2)mcPath.vectorPath[mnCurrentWaypoint] - mcRigidBody.position).normalized;
                Vector2 lcForce = lcDirection * mcEnemyAttributes.GetMovementSpeed();

                lcForce.x *= mfHorizontalMovementMultiplier;

                //if Enemy is flying ground check need not occur
                if (!mcEnemyAttributes.GetFlyingEnemy())
                {
                    float lfJumpPower = mcRigidBody.velocity.y;
                    if (mbIsGrounded)
                    {
                        if (lcDirection.y > 0.7 && mfJumpCoolDownElapsedTime > 2)
                        {
                            lfJumpPower = mcEnemyAttributes.GetJumpPower();
                            mfJumpCoolDownElapsedTime = 0;
                        }

                        mcRigidBody.velocity = new Vector2(lcForce.x, lfJumpPower);
                    }
                    else
                    {
                        mcRigidBody.velocity = new Vector2(lcForce.x, lfJumpPower);
                    }

                }
                //Flying enemy movement
                else
                {
                    if (mcEnemyAttributes.GetForceMovement())
                    {
                        mcRigidBody.AddForce(lcForce);
                    }
                    else
                    {
                        mcRigidBody.velocity = lcForce;
                        //Debug.Log("lcDirection : " + lcDirection + " CurrentWaypoint: " + mnCurrentWaypoint);
                    }

                }

                if (meUpdateEnemyState != EnemyState.eeEnemyAttack)
                {
                    meUpdateEnemyState = EnemyState.eeEnemyMove;
                    mcAnimator.SetInteger("EnemyState", (int)meUpdateEnemyState);
                    //Check if enemy is facing correct way
                    Flip();
                }
            }
            else
            {
                mcRigidBody.velocity = Vector2.zero;
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
        //flip if velocity changes direction
        if(isFacingRight && mcRigidBody.velocity.x < 0 || !isFacingRight && mcRigidBody.velocity.x > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            mbFacingAwayFromTarget = (isFacingRight && mcRigidBody.position.x > mcTarget.position.x) ||
               (!isFacingRight && mcRigidBody.position.x < mcTarget.position.x);
        }
    }

    /*
     * METHOD: checks if enemy is within range to go aggro
     */
    private void WithinAggroRange()
    {
        //reduce aggro distance if enemy is facing away from target
        //float lfAggroDistance = mcEnemyAttributes.GetFollowDistance() / (mbFacingAwayFromTarget ? 3 : 1);

        float lfAggroDistance = mcEnemyAttributes.GetFollowDistance();
        if (mfDistanceToTarget <= lfAggroDistance)
        {
            mbAggroed = true;
        }
        else if(mbAggroed)
        {
            StartCoroutine(Disengage());
        }
    }

    private IEnumerator Disengage()
    {
        yield return new WaitForSeconds(1);
        mbAggroed = false;
    }

    //Gets angle to closest target
    public float GetAngleToClosestTarget()
    {
        float lfAttackAngle = 0;

        if(mcTarget)
        {
            lfAttackAngle = Mathf.Atan2(mcTarget.position.y - transform.position.y, mcTarget.position.x - transform.position.x) * Mathf.Rad2Deg;
        }
        
        return lfAttackAngle;
    }
}
