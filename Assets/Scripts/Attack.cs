using Deck;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.PlacematContainer;

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

    //Player attack animation ID
    AttackAnimationType meAttackAnimationType = AttackAnimationType.eeBasicSideAttack;

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
    private float AttackDelayTime = 0f;

    //Attack positions
    private Vector3 mcAttackOriginPosition;
    private Vector3 mcAttackEndPosition;

    Vector2 mcForceDirection;

    //Attack visual Effects
    private GameObject mcSmokeFX;

    //Attack Hitbox
    private GameObject mcChildHitbox;

    //Tangible boxes used to allow player interaction with attack
    private GameObject mcGroundBox;
    private GameObject mcWallBox;

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

    void Start()
    {
        //Set Attack Origin 
        mcAttackOriginPosition = transform.position;

        //Get Rigid Body
        mcRigidBody = GetComponent<Rigidbody2D>();

        foreach (Transform lcChild in transform)
        {
            if(lcChild.name == "SmokeFX")
            {
                mcSmokeFX = lcChild.gameObject;
            }
            else if(lcChild.name == "HitBox")
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
            }
        }

        mcSpriteRenderer = GetComponent<SpriteRenderer>();
        mcSpriteRenderer.enabled = false;

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
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        Vector3 lcCollisionPosition = mcChildHitbox.transform.position;

        lcCollisionPosition = lcCollision.ClosestPoint(lcCollisionPosition);

        if (mbEnemyAttacker)
        {
            mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();

            if(!mcPlayerCollided)
            {
                Debug.Log("Couldnt find player collider");
            }
        }
        else if (mbPlayerAttacker) 
        {
            mcEnemyCollided = lcCollision.GetComponent<EnemyAI>();
        }

        bool lbStruckFighter = ((mbPlayerAttacker && mcEnemyCollided) || (mbEnemyAttacker && mcPlayerCollided));
        bool lbStruckBorder = (lcCollision.name == "Walls" || lcCollision.name == "Ground");

        if (mbAttackOut)
        {

            if (lbStruckFighter)
            {
                //TODO: Add KnockBack
                if (mbPlayerAttacker)
                {
                    mcEnemyCollided.Damage(mcAttackAttributes.GetAttackDamage(), 2f, meAttackDirection);

                    //TODO: Adjust attack movement 
                    if (meAttackDirection == AttackDirection.eeDownwards)
                    {
                        mcPlayerAttacker.ApplyAttackMovement();
                    }
                }
                //Enemy Attack, Damage the player
                else
                {
                    mcPlayerCollided.Damage(mcAttackAttributes.GetAttackDamage(), 2f, meAttackDirection);
                }

                //Trigger Damage pop up
                GameObject lcPopUp = Instantiate(mcPopUpDamageTemplate, lcCollisionPosition, Quaternion.identity);
                lcPopUp.GetComponent<TMP_Text>().text = mcAttackAttributes.GetAttackDamage().ToString();

                if (transform.position.x > lcCollision.transform.position.x)
                {
                    lcPopUp.GetComponent<PopUpDamage>().mbHitFromRight = true;
                }
            }

            if(lbStruckFighter || lbStruckBorder)
            {
                //Trigger hit effect
                mcStruckFx.transform.position = lcCollisionPosition;
                mcStruckFx.transform.SetParent(null);
                mcStruckFx.Play();
            }

            if(mcAttackAttributes.GetDisjointed() && (lbStruckBorder || lbStruckFighter))
            {
                //if sub Attack is generated at end of parent attacks life time
                if (mcAttackAttributes.GetCreateSubAttackOnEnd() && mnNumSubAttacks != 0)
                {
                    if(mcAttackAttributes.GetSubAttackDirection(meAttackDirection) == AttackDirection.eeBorderWards)
                    {
                        if (lbStruckBorder)
                        {
                            AttackDirection lcStruckBorderAttackDirection = AttackDirection.eeRightward;

                            if (lcCollision.name == "Walls")
                            {
                                //Collided with right wall
                                if (lcCollisionPosition.x > transform.position.x)
                                {
                                    //90 Degree rotation
                                    lcStruckBorderAttackDirection = AttackDirection.eeLeftward;

                                    Debug.Log("Collided with Right wall");
                                }
                                //Collided with left wall
                                else if (lcCollisionPosition.x < transform.position.x)
                                {
                                    //270 Degree rotation
                                    lcStruckBorderAttackDirection = AttackDirection.eeRightward;

                                    Debug.Log("Collided with Left wall");
                                }
                            }
                            else if (lcCollision.name == "Ground")
                            {
                                //Collided with ceiling
                                if (lcCollisionPosition.y > transform.position.y)
                                {
                                    //180 Degree rotation
                                    lcStruckBorderAttackDirection = AttackDirection.eeDownwards;

                                    Debug.Log("Collided with Ceiling");
                                }
                                //Collided with floor
                                else if (lcCollisionPosition.y < transform.position.y)
                                {
                                    //0 degree rotation
                                    lcStruckBorderAttackDirection = AttackDirection.eeUpwards;

                                    Debug.Log("Collided with Ground");
                                }
                            }

                            //Sub Attack direction derived from parent attack direction
                            GenerateSubAttack(lcStruckBorderAttackDirection, lcCollisionPosition);
                        }
                    }
                    else
                    {
                        //Sub Attack direction derived from parent attack direction
                        GenerateSubAttack(mcAttackAttributes.GetSubAttackDirection(meAttackDirection), lcCollisionPosition);
                    }
                    mnNumSubAttacks--;
                }

                EndAttack();
            }
        }
    }

    public void BeginAttack()
    {
        mcAttackAnimator.speed = 0;

        //Play Animation
        if (mbAnimationFlip || !mcAttackAttributes.GetHasAnimationFlip())
        {
            Debug.Log("Play Attack: " + mcAttackAttributes.GetAttackAnimationString());

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

        //Set Direction
        meAttackDirection = mcAttackAttributes.GetDirection();

        float lfAttackAngleDeg = 0;

        //Find attack end position
        switch (meAttackDirection)
        {
            case AttackDirection.eeLeftward:
                lfAttackAngleDeg = 180 - pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, -pfAdjustedAngle);

                if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                {
                    meAttackAnimationType = AttackAnimationType.eeHeavySideAttack;
                }
                else if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeRanged)
                {
                    meAttackAnimationType = AttackAnimationType.eeRangedSideAttack;
                }
                break;
            case AttackDirection.eeRightward:

                lfAttackAngleDeg += pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, pfAdjustedAngle);

                if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                {
                    meAttackAnimationType = AttackAnimationType.eeHeavySideAttack;
                }
                else if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeRanged)
                {
                    meAttackAnimationType = AttackAnimationType.eeRangedSideAttack;
                }
                break;
            case AttackDirection.eeUpwards:
                lfAttackAngleDeg = 90 + pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, 90 + ((mcAttackAttributes.GetGroundOrigination()) ? 90 : 0) + pfAdjustedAngle);
                if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                {
                    meAttackAnimationType = AttackAnimationType.eeHeavyUpAttack;
                }
                else
                {
                    meAttackAnimationType = AttackAnimationType.eeBasicUpAttack;
                }
                break;
            case AttackDirection.eeDownwards:
                lfAttackAngleDeg = 270 - pfAdjustedAngle;
                transform.rotation = Quaternion.Euler(0, 0, -90 + ((mcAttackAttributes.GetGroundOrigination()) ? 90 : 0) - pfAdjustedAngle);
                if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                {
                    meAttackAnimationType = AttackAnimationType.eeHeavyDownAttack;
                }
                else
                {
                    meAttackAnimationType = AttackAnimationType.eeBasicDownAttack;
                }
                break;

        }

        //Attack movement is a force applied momentarily 
        mcForceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * lfAttackAngleDeg), Mathf.Sin(Mathf.Deg2Rad * lfAttackAngleDeg));

        //Attack movement is set to end when reaching a specific point
        mcAttackEndPosition = new Vector2(transform.position.x + (mcAttackAttributes.GetTravelDistance() * Mathf.Cos(Mathf.Deg2Rad * lfAttackAngleDeg)), 
            transform.position.y + (mcAttackAttributes.GetTravelDistance() * Mathf.Sin(Mathf.Deg2Rad * lfAttackAngleDeg)));
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
        if(mbBeginAttack && AttackDelayTime >= mcAttackAttributes.GetAttackDelay())
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

                if (mcPlayerAttacker != null)
                {
                    mcPlayerAttacker.SetAttackAnimationValue(meAttackAnimationType);
                }

                mcSpriteRenderer.enabled = true;
                mcAttackAnimator.speed = 1;
                smokeFX.Play();

                mcChildHitbox.GetComponent<CapsuleCollider2D>().direction = mcAttackAttributes.GetAttackCapsuleColliderDirection();
                mcChildHitbox.SetActive(true);
                mbHitBoxActive = true;


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
                }
            }


            //If Attack ends at end of animation
            if (mcAttackAttributes.GetSingleAnimationLifetime())
            {
                mcAnimationStateInfo = mcAttackAnimator.GetCurrentAnimatorStateInfo(0);

                if (mcAnimationStateInfo.normalizedTime >= 1.0f)
                {
                    EndAttack();
                }
            }
            //If lifetime of attack is not tied to animation check lifetime value
            else if (mcAttackAttributes.GetLifeTime() != 0)
            {
                AttackElapsedTime += Time.deltaTime;

                if (AttackElapsedTime >= mcAttackAttributes.GetLifeTime())
                {
                    EndAttack();
                }
            }

            //if attack has a set travel distance
            if (mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeFixedDistance)
            {
                    //Move fake card to top of hand over time
                    transform.position = Vector3.Lerp(mcAttackOriginPosition, mcAttackEndPosition,
                        AttackElapsedTime / mcAttackAttributes.GetLifeTime());
            }
            else if(mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeForceApplied 
                && mcRigidBody.bodyType != RigidbodyType2D.Dynamic)
            {   
                //TODO: Add mass and gravity to attack attributes
                mcRigidBody.bodyType = RigidbodyType2D.Dynamic;
                mcRigidBody.mass = 20;
                mcRigidBody.gravityScale = 3;
                mcRigidBody.AddForce(mcForceDirection * mcAttackAttributes.GetMovementForce() * 35, ForceMode2D.Impulse);
            }
            else if(mcAttackAttributes.GetAttackMovementType() == AttackMovementType.eeNoMovement &&
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
            if(mcAttackAttributes.GetRevolveAround())
            {
                //TODO: Revolution Rate
                transform.RotateAround(transform.parent.transform.position, Vector3.forward,
                    2 * ((meAttackDirection == AttackDirection.eeLeftward) ? 1 : -1));
            }
        }
        else if(mbBeginAttack)
        {
            AttackDelayTime += Time.deltaTime;
        }
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
            lcAttackComponent.SetAttacker(mcPlayerAttacker);
            lcAttackComponent.SetAttributes(mcSubAttackAttributes, 0);

            //Lock attack to player if it is not disjointed
            if (!mcSubAttackAttributes.GetDisjointed())
            {
                lcAttack.transform.SetParent(this.transform);
            }
        }
        else
        {
            Debug.Log("Attack component is NULL");
        }

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
