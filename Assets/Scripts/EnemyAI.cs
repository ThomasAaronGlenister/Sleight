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

    //offset to the target this enemy should be moving towards
    public Vector3 mcTargetPositionOffset = Vector2.zero;

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

    //Enemy Box Collider
    BoxCollider2D mcEnemyBoxCollider;

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

    // Animation overrides
    private AnimatorOverrideController mcAnimationOverrideController;

    //Set of durations suit effects are applied for
    float[] mafEffectAppliedTime = new float[(int)SuitEffect.eeSuitEffectEnd]
    {0, 0, 0, 0, 0, 0, 0};

    List<GameObject> macEffectIcons = new List<GameObject>();

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
        if(GameObject.Find("EffectArc"))
        {
            foreach (Transform child in GameObject.Find("EffectArc").transform)
            {
                macEffectIcons.Add(child.gameObject);
            }
        }

        InvokeRepeating("UpdatePath", 0f, 0.5f);

        Time.fixedDeltaTime = 1.0f / 60f;

    }

    //Assigns a set of Attributes
    public void SetEnemyAttributes(EnemyAttributes pcEnemyAttributes)
    {
        mcEnemyAttributes = pcEnemyAttributes;

        transform.localScale *= mcEnemyAttributes.GetSizeMultiplier();

        mcTargetPositionOffset.x = pcEnemyAttributes.GetMovementTargetOffsetX();
        mcTargetPositionOffset.y = pcEnemyAttributes.GetMovementTargetOffsetY();

        //Adjust box collider size based on sprite size
        GetComponent<BoxCollider2D>().size = pcEnemyAttributes.GetBoxColliderSize();
        GetComponent<BoxCollider2D>().offset = pcEnemyAttributes.GetBoxColliderOffset();

        mnHealthPoints = mcEnemyAttributes.GetHealthPoints();
        mnArmorPoints = mcEnemyAttributes.GetArmorPoints();

        mcRigidBody = GetComponent<Rigidbody2D>();
        mcRigidBody.gravityScale = mcEnemyAttributes.GetGravityScale();
        mcRigidBody.mass = mcEnemyAttributes.GetEnemyMass();

        mcAnimationOverrideController = new AnimatorOverrideController(mcAnimator.runtimeAnimatorController);

        mcAnimator.runtimeAnimatorController = mcAnimationOverrideController;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(mcAnimationOverrideController.overridesCount);
        mcAnimationOverrideController.GetOverrides(overrides);

        for (int i = 0; i < overrides.Count; i++)
        {
            switch (overrides[i].Key.name)
            {
                case "FreakAttack":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyAttackAnimationClip());
                    break;
                case "FreakMove":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyMoveAnimationClip());
                    break;
                case "FreakDamage":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyDamageAnimationClip());
                    break;
                case "FreakWindup":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyWindupAnimationClip());
                    break;
                case "FreakJump":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyJumpAnimationClip());
                    break;
                case "FreakIdle":
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, mcEnemyAttributes.GetEnemyIdleAnimationClip());
                    break;
            }

        }

        mcAnimationOverrideController.ApplyOverrides(overrides);

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
    public void Damage(int pnDamageAmount, float pnKnockBack, AttackDirection peAttackDirection, float[] pafEffects)
    {
        mnHealthPoints -= pnDamageAmount;

        if (mnHealthPoints <= 0)
        {
            StartCoroutine(Defeated());
        }
        else
        {
            SetSuitEffects(pafEffects);

            StartCoroutine(TakeDamage());
        }
    }

    //Method to set Attack Effect durations on damage
    private void SetSuitEffects(float[] pafEffects)
    {
        for (int i = 0; i < pafEffects.Length; i++)
        {
            //If added effect time applied and current effect is not active
            if (pafEffects[i] != 0 && mafEffectAppliedTime[i] <= 0)
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

    private IEnumerator Attack()
    {
        meUpdateEnemyState = EnemyState.eeEnemyKnockback;

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyWindup);

        if(mcEnemyAttributes.GetEnemyAttackWindupTime() != 0)
        {
            mcRigidBody.velocity = Vector3.zero;
        }

        yield return new WaitForSeconds(mcEnemyAttributes.GetEnemyAttackWindupTime());

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyAttack);

        yield return null;

        //Direction is Waypoint plus the current enemy position
        Vector2 lcDirection = (mcTarget.position - transform.position).normalized;
        lcDirection.y += mcEnemyAttributes.GetAttackLungeVertical();

        mcRigidBody.AddForce(lcDirection * mcEnemyAttributes.GetAttackLungeMultiplier(), ForceMode2D.Impulse);

        //Pass attack card set along with direction to attack generator
        mcAttackGenerator.GenerateAttack(mcEnemyAttributes.GetEnemyAttack(), mcEnemyAttributes.GetEnemyAttackDamage(),
            GetAttackDirection());

        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyIdle);

        yield return new WaitForSeconds(1);

        meUpdateEnemyState = EnemyState.eeEnemyIdle;
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
    private IEnumerator TakeDamage()
    {
        meUpdateEnemyState = EnemyState.eeEnemyKnockback;

        StopCoroutine(Attack());
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

        //TODO: Set to attacks stun time
        yield return new WaitForSeconds(0.2f);
        mcSpriteRenderer.color = mcOriginalColor;

        meUpdateEnemyState = EnemyState.eeEnemyIdle;
        mcAnimator.SetInteger("EnemyState", (int)EnemyState.eeEnemyIdle);
    }

    /*
     * METHOD: Coroutine called to initiate end of enemy object
     */
    private IEnumerator Defeated()
    {
        float lfDissolve = 0f;
        while (lfDissolve < 1f)
        {
            lfDissolve += 0.005f;
            mcSpriteRenderer.material.SetFloat("_DissolveAmount", lfDissolve);
            yield return null;
        }

        yield return null;

        Destroy(gameObject);
    }

    /*
     * METHOD: Continuously called and resets path 
     */
    private void UpdatePath()
    {
        if(mcSeeker.IsDone())
        {
            mcSeeker.StartPath(mcRigidBody.position, mcTarget.position + mcTargetPositionOffset, OnPathComplete);
        }
    }

    private void Update()
    {
        mfAttackCoolDownElapsedTime += Time.deltaTime;

        //Handle Animation updates
        mbIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, (mcEnemyAttributes.GetBoxColliderSize().y / 2) + 1f, mcGroundLayer);

        Debug.DrawRay(transform.position, Vector2.down, Color.red, ((mcEnemyAttributes.GetBoxColliderSize().y / 2) + 1f));

        mcAnimator.SetBool("Grounded", mbIsGrounded || mcEnemyAttributes.GetFlyingEnemy());

        mfDistanceToTarget = Vector2.Distance(mcRigidBody.position, mcTarget.position);

        CheckSuitEffects();

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
            //TODO Set Idle animation
            meUpdateEnemyState = EnemyState.eeEnemyIdle;
            mcAnimator.SetInteger("EnemyState", (int)meUpdateEnemyState);
        }
        //If target is within range of Follow
        else if (meUpdateEnemyState != EnemyState.eeEnemyKnockback && WithinAggroRange())
        {
            meUpdateEnemyState = EnemyState.eeEnemyMove;
            mcAnimator.SetInteger("EnemyState", (int)meUpdateEnemyState);

            //Direction is Waypoint minus the current enemy position
            Vector2 lcDirection = ((Vector2)mcPath.vectorPath[mnCurrentWaypoint] - mcRigidBody.position).normalized;
            Vector2 lcForce = lcDirection * mcEnemyAttributes.GetMovementSpeed();

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
                if(mcEnemyAttributes.GetForceMovement())
                {
                    mcRigidBody.AddForce(lcForce);
                }
                else
                {
                    mcRigidBody.velocity = new Vector2(((lcForce.x > 0f) ? 1 : -1) * mcEnemyAttributes.GetMovementSpeed(),
                        ((lcForce.y > 0f) ? 1 : -1) * mcEnemyAttributes.GetMovementSpeed());
                }

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
    private bool WithinAggroRange()
    {
        bool lbWithinRange = false;
        if(mcPath != null && mcPath.vectorPath.Count <= mcEnemyAttributes.GetFollowDistance())
        {
            lbWithinRange = true;
        }

        return lbWithinRange;
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
