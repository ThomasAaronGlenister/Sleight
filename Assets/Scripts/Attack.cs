using Deck;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.PlacematContainer;
using Color = UnityEngine.Color;

public class Attack : MonoBehaviour
{
    //Particle Smoke
    public ParticleSystem smokeFX;

    //Particle Pop on hit
    public ParticleSystem HitFX;

    private ParticleSystem mcStruckFx;

    //Reference to Player initiating the attack
    private PlayerMovement mcPlayerAttacker;

    //Reference to Enemy initiating the attack
    private EnemyAI mcEnemyAttacker;

    //Enemy Hurt Box
    Rigidbody2D mcRigidBody;

    //Animation control memebers
    public Animator mcAttackAnimator;
    AnimatorStateInfo mcAnimationStateInfo;

    //Set of Attack Information that drives the behavior of this attack
    private AttackAttributes mcAttackAttributes;

    //Flag to indicate that the flipped version of the animation should play
    private bool mbAnimationFlip = false;

    //Direction of the Attack
    private AttackDirection meAttackDirection = AttackDirection.eeRightward;

    //Time Tracker for attack 
    private float AttackElapsedTime = 0f;

    //Attack positions
    private Vector3 mcAttackOriginPosition;
    private Vector3 mcAttackEndPosition;

    Vector2 mcForceDirection;

    //Attack visual Effects
    private GameObject mcSmokeFX;

    //Attack Hitbox
    private GameObject mcChildHitbox;

    //Collider used for attack on attack interaction
    private GameObject mcHurtBox;

    //Tangible boxes used to allow player interaction with attack
    private GameObject mcGroundBox;
    private GameObject mcWallBox;

    //Ground/Wall Layer masks
    public LayerMask mcGroundLayer;
    public LayerMask mcWallLayer;

    //Start Flag for attack
    private bool mbBeginAttack = false;

    private bool mbAttackOut = false;

    //Flag indicating this attacks hitbox is active
    private bool mbHitBoxActive = false;

    //Pop Up Damage Template prefab assigned in the editor
    public GameObject mcPopUpDamageTemplate;

    private bool mbEnemyAttacker = false;
    private bool mbPlayerAttacker = false;

    EnemyAI mcEnemyCollided;
    PlayerMovement mcPlayerCollided;

    //Prefab Attack Object used for sub attacks
    public GameObject mcAttackPrefab;

    //Attack Attributes of the sub attack created
    private AttackAttributes mcSubAttackAttributes;

    //Number of Sub Attacks this attack generates
    private int mnNumSubAttacks = 1;

    private SpriteRenderer mcSpriteRenderer;

    //Array of time that a specific effect is applied for.
    //Functions as both a flag indicating an effect is applied and the time it is applied for
    float[] mafEffectAppliedTime = new float[(int)SuitEffect.eeSuitEffectEnd]
        {0, 0, 0, 0, 0, 0, 0};

    //Pause Manager 
    private PauseManager mcPauseManager;

    private CameraController mcCameraController;

    void Start()
    {
        //Set Attack Origin 
        mcAttackOriginPosition = transform.position;

        //Get Rigid Body
        mcRigidBody = GetComponent<Rigidbody2D>();

        foreach (Transform lcChild in transform)
        {
            if (lcChild.name == "SmokeFX")
            {
                mcSmokeFX = lcChild.gameObject;
            }
            else if (lcChild.name == "HitBox")
            {
                mcChildHitbox = lcChild.gameObject;
            }
            else if (lcChild.name == "GroundCollider")
            {
                mcGroundBox = lcChild.gameObject;
            }
            else if (lcChild.name == "WallCollider")
            {
                mcWallBox = lcChild.gameObject;
            }
            else if (lcChild.name == "HitBurst")
            {
                mcStruckFx = Instantiate(lcChild.gameObject.GetComponent<ParticleSystem>());
                mcStruckFx.transform.SetParent(this.transform);
            }
            else if (lcChild.name == "HurtBox")
            {
                mcHurtBox = lcChild.gameObject;
            }
        }

        Physics2D.IgnoreCollision(mcChildHitbox.GetComponent<CapsuleCollider2D>(), mcHurtBox.GetComponent<CircleCollider2D>(), true);

        mcSpriteRenderer = GetComponent<SpriteRenderer>();
        mcSpriteRenderer.enabled = false;

        mcPauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();
        mcCameraController = GameObject.Find("Virtual Camera").GetComponent<CameraController>();

    }

    //Sets the parent game object if the attack is not disjointed 
    public void SetAttacker(PlayerMovement pcAttacker)
    {
        mcPlayerAttacker = pcAttacker;
        mbPlayerAttacker = true;
    }

    //Sets the parent game object if the attack is not disjointed 
    public void SetAttacker(EnemyAI pcAttacker)
    {
        mcEnemyAttacker = pcAttacker;
        mbEnemyAttacker = true;
    }

    //Catches collider overlaps for the attack
    void OnTriggerEnter2D(Collider2D lcCollision)
    {
        Vector3 lcHitboxPositon = mcChildHitbox.transform.position;

        Vector3 lcCollisionPosition = mcChildHitbox.transform.position;

        lcCollisionPosition = lcCollision.ClosestPoint(lcCollisionPosition);

        if (mbEnemyAttacker)
        {
            mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();

            if (!mcPlayerCollided)
            {
                Debug.Log("Couldnt find player collider");
            }
        }
        else if (mbPlayerAttacker)
        {
            mcEnemyCollided = lcCollision.GetComponent<EnemyAI>();
        }

        bool lbStruckFighter = ((mbPlayerAttacker && mcEnemyCollided) || (mbEnemyAttacker && mcPlayerCollided));
        bool lbStruckBorder = (lcCollision.name == "Walls" || lcCollision.name == "Ground" || lcCollision.name == "ExitWalls" 
            || lcCollision.name == "ExitGround" || lcCollision.name == "DestructibleObject" || lcCollision.name == "PassThroughPlatforms");
        bool lbStruckAttack = (lcCollision.name == "HurtBox");

        if (mbAttackOut)
        {
            if(lbStruckAttack)
            {
                lcCollision.transform.parent.GetComponent<Attack>().AddForce(mcForceDirection, 200);
                Debug.Log(this.name + " collided with attack " + lcCollision.transform.parent.name);
            }
            if (lbStruckFighter && !mcAttackAttributes.GetHitBoxIsTrigger())
            {
                int lnAttackDamage = CalculateAttackDamage(mcAttackAttributes.GetAttackDamage());

                //TODO: Add KnockBack
                if (mbPlayerAttacker)
                {
                    mcEnemyCollided.Damage(lnAttackDamage, 2f, meAttackDirection, mafEffectAppliedTime);

                    //TODO: Set freeze frames to scale with damage
                    //Freeze Frames
                    if (lnAttackDamage > 50)
                    {
                        mcPauseManager.SetFreeze(0.2f);
                        mcCameraController.SetZoom(0.2f);
                    }

                    //TODO: Adjust attack movement 
                    if (meAttackDirection == AttackDirection.eeDownwards)
                    {
                        mcPlayerAttacker.ApplyAttackMovement();
                    }
                }
                //Enemy Attack, Damage the player
                else
                {
                    mcPlayerCollided.Damage(lnAttackDamage, 2f, meAttackDirection);
                }

                //Trigger Damage pop up
                GameObject lcPopUp = Instantiate(mcPopUpDamageTemplate, lcCollisionPosition, Quaternion.identity);
                lcPopUp.GetComponent<TMP_Text>().text = lnAttackDamage.ToString();

                if (transform.position.x > lcCollision.transform.position.x)
                {
                    lcPopUp.GetComponent<PopUpDamage>().mbHitFromRight = true;
                }
            }

            if (mcAttackAttributes.GetDisjointed() && lbStruckBorder)
            {
                //if sub Attack is generated at end of parent attacks life time
                if (mcAttackAttributes.GetCreateSubAttackOnEnd())
                {
                    AttackDirection lcSubAttackDirection = meAttackDirection;

                    if (mcAttackAttributes.GetSubAttackDirection(meAttackDirection) == AttackDirection.eeBorderWards)
                    {
                        if (lbStruckBorder)
                        {

                            lcCollision.transform.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;

                            //Check positions in each cardinal direction from hit
                            bool lbLeftOpen = Physics2D.OverlapCapsule(new Vector2(lcCollisionPosition.x - 0.2f, lcCollisionPosition.y), new Vector2(0f, 0.3f),
                                    CapsuleDirection2D.Vertical, 0, mcGroundLayer | mcWallLayer) == null;

                            bool lbRightOpen = Physics2D.OverlapCapsule(new Vector2(lcCollisionPosition.x + 0.2f, lcCollisionPosition.y), new Vector2(0f, 0.3f),
                                    CapsuleDirection2D.Vertical, 0, mcGroundLayer | mcWallLayer) == null;

                            bool lbUpOpen = Physics2D.OverlapCapsule(new Vector2(lcCollisionPosition.x, lcCollisionPosition.y + 0.2f), new Vector2(0.3f, 0f),
                                CapsuleDirection2D.Vertical, 0, mcGroundLayer | mcWallLayer) == null;

                            bool lbDownOpen = Physics2D.OverlapCapsule(new Vector2(lcCollisionPosition.x, lcCollisionPosition.y - 0.2f), new Vector2(0.3f, 0f),
                                CapsuleDirection2D.Vertical, 0, mcGroundLayer | mcWallLayer) == null;

                            //Determine attack orientation
                            if (lbLeftOpen)
                            {
                                lcSubAttackDirection = AttackDirection.eeLeftward;
                            }
                            else if (lbRightOpen)
                            {
                                lcSubAttackDirection = AttackDirection.eeRightward;
                            }
                            else if (lbUpOpen)
                            {
                                lcSubAttackDirection = AttackDirection.eeUpwards;
                            }
                            else if (lbDownOpen)
                            {
                                lcSubAttackDirection = AttackDirection.eeDownwards;
                            }

                            Debug.Log("Borderwards Direction: " + lcSubAttackDirection);

                            lcCollision.transform.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Outlines;

                            //Sub Attack direction derived from parent attack direction
                            GenerateSubAttack(lcSubAttackDirection, lcCollisionPosition);
                        }
                    }
                    else
                    {
                        //Sub Attack direction derived from parent attack direction
                        GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), lcCollisionPosition);
                    }
                }
            }

            if (lbStruckFighter || lbStruckBorder || lbStruckAttack)
            {
                if(!mcAttackAttributes.GetHitBoxIsTrigger())
                {
                    //Trigger hit effect
                    mcStruckFx.transform.position = lcCollisionPosition;
                    mcStruckFx.transform.SetParent(null);
                    mcStruckFx.Play();
                }

                //If piercing is not in effect destroy attack
                if (mafEffectAppliedTime[(int)SuitEffect.eePierce] == 0 && !mcAttackAttributes.GetSingleAnimationLifetime())
                {
                    if (mcAttackAttributes.GetCreateSubAttackOnEnd() && lbStruckFighter 
                        && mcAttackAttributes.GetSubAttackDirection(meAttackDirection) != AttackDirection.eeBorderWards)
                    {
                        //Sub Attack direction derived from parent attack direction
                        GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), transform.position);
                    }

                    EndAttack();
                }
            }
        }
    }

    private int CalculateAttackDamage(int pnBaseDamage)
    {
        int lnDamage = pnBaseDamage;

        if (mafEffectAppliedTime[(int)SuitEffect.eeBurn] > 0)
        {
            lnDamage += 20;
        }

        if (mafEffectAppliedTime[(int)SuitEffect.eeCrit] > 0)
        {
            lnDamage *= 3;
        }

        return lnDamage;
    }

    public void BeginAttack()
    {
        StartCoroutine(InitiateAttack());
    }

    private IEnumerator InitiateAttack()
    {

        yield return new WaitForSeconds(mcAttackAttributes.GetAttackDelay());

        mcAttackAnimator.speed = 0;

        //Play Animation
        if (mbAnimationFlip || !mcAttackAttributes.GetHasAnimationFlip())
        {
            mcAttackAnimator.Play(mcAttackAttributes.GetAttackAnimationString());
        }
        else
        {
            mcAttackAnimator.Play(mcAttackAttributes.GetAttackAnimationString() + "Reverse");
        }

        mbBeginAttack = true;
    }

    //Sets the sub attack attributes
    public void SetSubAttackAttributes(AttackAttributes pcAttackAttributes)
    {
        mcSubAttackAttributes = pcAttackAttributes;
    }

    //Sets the Attack attributes 
    public void SetAttributes(AttackAttributes pcAttackAttributes, float pfAdjustedAngle)
    {
        //Assign Attack Attributes
        mcAttackAttributes = pcAttackAttributes;

        this.name = mcAttackAttributes.GetAttackName();

        //Check for secondary effects
        for (int lnEffects = 0; lnEffects < mcAttackAttributes.GetEffectChances().Length; lnEffects++)
        {
            if (mcAttackAttributes.GetEffectChances()[lnEffects] != 0)
            {
                // Use effect chance to check if applied debuff set to true
                mafEffectAppliedTime[lnEffects] = mcAttackAttributes.GetEffectDuration((SuitEffect)lnEffects);
            }
        }

        //Set Direction
        meAttackDirection = mcAttackAttributes.GetDirection();

        float lfAttackAngleDeg = 0;

        //Auto corrects attack angle towards the target if flag set
        if (mcAttackAttributes.GetMoveTowardsTarget())
        {
            meAttackDirection = AttackDirection.eeRightward;

            pfAdjustedAngle = GetAngleToClosestTarget();
        }

        //Orient attack direction 
        switch (meAttackDirection)
        {
            case AttackDirection.eeLeftward:
                lfAttackAngleDeg = 180 - pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, -pfAdjustedAngle);
                break;
            case AttackDirection.eeRightward:
                lfAttackAngleDeg += pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, pfAdjustedAngle);
                break;
            case AttackDirection.eeUpwards:
                lfAttackAngleDeg = 90 + pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, 90 + ((mcAttackAttributes.GetGroundOrigination()) ? 90 : 0) + pfAdjustedAngle);
                break;
            case AttackDirection.eeDownwards:
                lfAttackAngleDeg = 270 - pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, -90 + ((mcAttackAttributes.GetGroundOrigination()) ? 90 : 0) - pfAdjustedAngle);
                break;

        }

        //Attack movement is a force applied momentarily 
        mcForceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * lfAttackAngleDeg), Mathf.Sin(Mathf.Deg2Rad * lfAttackAngleDeg));

        //Attack movement is set to end when reaching a specific point
        mcAttackEndPosition = new Vector2(transform.position.x + (mcAttackAttributes.GetTravelDistance() * Mathf.Cos(Mathf.Deg2Rad * lfAttackAngleDeg)),
            transform.position.y + (mcAttackAttributes.GetTravelDistance() * Mathf.Sin(Mathf.Deg2Rad * lfAttackAngleDeg)));
    }

    //Method used to check which target the attacker should point this attack to.
    private float GetAngleToClosestTarget()
    {
        float lfReturnAngle = 0;

        //Debug.Log("GetAngleToClosestTarget ");

        if (mbEnemyAttacker)
        {
            lfReturnAngle = mcEnemyAttacker.GetAngleToClosestTarget();
            //Debug.Log("Attack Angle = " + lfReturnAngle);
        }

        //TODO: Add function for player to auto target

        return lfReturnAngle;
    }

    /**
     * METHOD:: Sets the animation flip
     */
    public void SetFlip(bool pbAnimationFlip)
    {
        mbAnimationFlip = pbAnimationFlip;
    }

    //Method to rotate attack around a point
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot)
    {
        return Quaternion.Euler(Vector3.forward) * (point - pivot) + pivot;
    }

    //Called once per frame
    void Update()
    {

        //If Attack has been started
        if (mbBeginAttack)
        {

            //Create any sub attacks if settings permit
            if (mcAttackAttributes.GetHasSubAttack() && mnNumSubAttacks != 0
                && !mcAttackAttributes.GetCreateSubAttackOnEnd())
            {
                //Sub Attack direction derived from parent attack direction
                GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), transform.position);
                mnNumSubAttacks--;
            }

            //Settings to set only once when Begin attack is set 
            if (mcChildHitbox && !mbHitBoxActive)
            {
                mbAttackOut = true;

                mcSpriteRenderer.enabled = true;
                mcAttackAnimator.speed = 1;
                smokeFX.Play();

                mcChildHitbox.GetComponent<CapsuleCollider2D>().direction = mcAttackAttributes.GetAttackCapsuleColliderDirection();
                
                mbHitBoxActive = mcAttackAttributes.GetHasHitbox();

                mcChildHitbox.SetActive(mbHitBoxActive);

                //Set Color of burst when attack strikes object
                var HitBurstPartMain = mcStruckFx.main;
                HitBurstPartMain.startColor = mcAttackAttributes.GetParticleTrailColor();

                if (mcAttackAttributes.GetParticleTrailEnabled())
                {
                    var PartMain = mcSmokeFX.GetComponent<ParticleSystem>().main;

                    PartMain.startColor = mcAttackAttributes.GetParticleTrailColor();

                    mcSmokeFX.SetActive(true);

                    mcSmokeFX.GetComponent<ParticleSystem>().Play();
                }

                if (mcAttackAttributes.GetTangible())
                {
                    mcGroundBox.SetActive(true);
                    mcWallBox.SetActive(true);
                    mcHurtBox.SetActive(true);
                }
            }


            //If Attack ends at end of animation
            if (mcAttackAttributes.GetSingleAnimationLifetime())
            {
                mcAnimationStateInfo = mcAttackAnimator.GetCurrentAnimatorStateInfo(0);

                if (mcAnimationStateInfo.normalizedTime >= 1.0f)
                {
                    if(mcAttackAttributes.GetCreateSubAttackOnEnd() && mcAttackAttributes.GetSubAttackDirection(meAttackDirection) != AttackDirection.eeBorderWards)
                    {
                        //Sub Attack direction derived from parent attack direction
                        GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), transform.position);
                    }

                    EndAttack();
                }
            }
            //If lifetime of attack is not tied to animation check lifetime value
            else if (mcAttackAttributes.GetLifeTime() != 0)
            {
                AttackElapsedTime += Time.deltaTime;

                if (AttackElapsedTime >= mcAttackAttributes.GetLifeTime())
                {
                    if (mcAttackAttributes.GetCreateSubAttackOnEnd() && mcAttackAttributes.GetSubAttackDirection(meAttackDirection) != AttackDirection.eeBorderWards)
                    {
                        //Sub Attack direction derived from parent attack direction
                        GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), transform.position);
                    }

                    EndAttack();
                }
            }

            //if attack has a set travel distance
            if (mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeFixedDistance && mcAttackAttributes.GetLifeTime() != 0)
            {
                //Move fake card to top of hand over time
                transform.position = Vector3.Lerp(mcAttackOriginPosition, mcAttackEndPosition,
                    AttackElapsedTime / mcAttackAttributes.GetLifeTime());
            }
            else if (mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeForceApplied
                && mcRigidBody.bodyType != RigidbodyType2D.Dynamic)
            {
                //TODO: Add mass and gravity to attack attributes
                mcRigidBody.bodyType = RigidbodyType2D.Dynamic;
                mcRigidBody.mass = 20;
                mcRigidBody.gravityScale = 3;
                mcRigidBody.AddForce(mcForceDirection * mcAttackAttributes.GetMovementForce() * 35, ForceMode2D.Impulse);
            }
            else if (mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeNoMovement &&
                mcRigidBody.bodyType != RigidbodyType2D.Static)
            {
                //TODO Add sway
                mcRigidBody.bodyType = RigidbodyType2D.Static;
                mcRigidBody.Sleep();
            }

            //If Rotation rate not zero spin attack by rotation rate
            if (mcAttackAttributes.GetRotationRate() != 0)
            {
                transform.Rotate(Vector3.forward * (mcAttackAttributes.GetRotationRate() * Time.deltaTime));
            }

            //if Revolution flag set, then revolve around point
            if (mcAttackAttributes.GetRevolveAround())
            {
                //TODO: Revolution Rate
                transform.RotateAround(transform.parent.transform.position, Vector3.forward,
                    2 * ((meAttackDirection == AttackDirection.eeLeftward) ? 1 : -1));
            }
        }
    }

    public void AddForce(Vector2 pcForceDirection, float pfKnockback)
    {
        Debug.Log("Force Direction: " + pcForceDirection);
        mcRigidBody.velocity = Vector3.zero;
        mcRigidBody.AddForce(pcForceDirection * pfKnockback, ForceMode2D.Impulse);
    } 

    public bool IsTangible()
    {
        return mcAttackAttributes.GetTangible();
    }

    public void GenerateSubAttack(AttackDirection peAttackDirection, Vector3 pcAttackOrigin)
    {
        Vector3 lcAttackOrigin = pcAttackOrigin;

        //Attack is produced from the ground or wall
        if (mcSubAttackAttributes.GetGroundOrigination())
        {
            Vector2 lcRayDirection = (peAttackDirection == AttackDirection.eeUpwards) ? Vector2.up : Vector2.down;

            RaycastHit2D lcOriginhit = Physics2D.Raycast(new Vector2(lcAttackOrigin.x, lcAttackOrigin.y), lcRayDirection, 50f, 15);

            if (lcOriginhit)
            {
                lcAttackOrigin = lcOriginhit.point;
            }
        }

        GameObject lcAttack = Instantiate(mcAttackPrefab, lcAttackOrigin, Quaternion.identity);

        lcAttack.transform.rotation = Quaternion.identity;

        if (transform.localScale.x < 0)
        {
            Vector3 ls = lcAttack.transform.localScale;
            ls.x *= -1f;
            lcAttack.transform.localScale = ls;
        }

        if(transform.localScale.y < 0)
        {
            Vector3 ls = lcAttack.transform.localScale;
            ls.y *= -1f;
            lcAttack.transform.localScale = ls;
        }


        Attack lcAttackComponent = lcAttack.GetComponent<Attack>();

        if (lcAttackComponent != null)
        {
            //Set Damage and direction to Attack Attributes
            mcSubAttackAttributes.SetAttackDamage(mcAttackAttributes.GetAttackDamage());
            mcSubAttackAttributes.SetDirection(peAttackDirection);

            //Assign Attack components
            if (mbEnemyAttacker)
            {
                lcAttackComponent.SetAttacker(mcEnemyAttacker);
            }
            else if (mbPlayerAttacker)
            {
                lcAttackComponent.SetAttacker(mcPlayerAttacker);
            }

            lcAttackComponent.SetAttributes(mcSubAttackAttributes, 0);

            //Lock attack to player if it is not disjointed
            if (!mcSubAttackAttributes.GetDisjointed())
            {
                lcAttack.transform.SetParent(this.transform.parent);
            }

            //Sub Attack may have sub attack of its own
            if (mcSubAttackAttributes.GetHasSubAttack())
            {
                lcAttackComponent.SetSubAttackAttributes(mcSubAttackAttributes.GetSubAttack());
            }
        }
        else
        {
            Debug.Log("Attack component is NULL");
        }

        //reduce inherited Attack size from parent
        lcAttack.transform.localScale /= mcAttackAttributes.GetAttackBaseSizeMultiplier();

        //Multiply attack size by attribute multiplier
        lcAttack.transform.localScale *= mcSubAttackAttributes.GetAttackBaseSizeMultiplier();

        //Offsets derived from attack direction
        if (peAttackDirection == AttackDirection.eeLeftward)
        {
            lcAttackOrigin.x -= mcSubAttackAttributes.GetSideAttackOffset();
            lcAttackOrigin.y += mcSubAttackAttributes.GetVerticalOffset();

            Vector3 ls = lcAttack.transform.localScale;
            ls.x *= -1f;
            lcAttack.transform.localScale = ls;

        }
        else if (peAttackDirection == AttackDirection.eeRightward)
        {
            lcAttackOrigin.x += mcSubAttackAttributes.GetSideAttackOffset();
            lcAttackOrigin.y += mcSubAttackAttributes.GetVerticalOffset();
        }
        else if (peAttackDirection == AttackDirection.eeUpwards)
        {
            lcAttackOrigin.y += mcSubAttackAttributes.GetUpDownAttackOffset();
            lcAttackOrigin.x -= mcSubAttackAttributes.GetHorizontalOffset();
        }
        else if (peAttackDirection == AttackDirection.eeDownwards)
        {
            lcAttackOrigin.y -= mcSubAttackAttributes.GetUpDownAttackOffset();
            lcAttackOrigin.x += mcSubAttackAttributes.GetHorizontalOffset();
        }

        lcAttack.transform.position = lcAttackOrigin;

        //Finally Start the Attack
        lcAttackComponent.BeginAttack();
    }

    /**
     * Method called when Attack has reached end point in its life time.
     * will complete call to destroy itself after 
     */
    private void EndAttack()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        CapsuleCollider2D cc = mcChildHitbox.GetComponent<CapsuleCollider2D>();
        DrawWireCapsule(cc.transform.position, cc.transform.rotation, (cc.size.x / 2), cc.size.y, Color.yellow);
    }

    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

        }
    }
}
