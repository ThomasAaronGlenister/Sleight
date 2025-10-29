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

    //Cardinal Direction the Attack is applied to 
    private AttackDirection peAttackDirection;

    //Reference to Player initiating the attack
    private PlayerMovement mcPlayerAttacker;

    //Reference to Enemy initiating the attack
    private EnemyAI mcEnemyAttacker;

    //Player attack animation ID
    AttackAnimationType meAttackAnimationType = AttackAnimationType.eeBasicSideAttack;

    //Animation control memebers
    public Animator mcAttackAnimator;
    AnimatorStateInfo mcAnimationStateInfo;

    //Set of Attack Information that drives the behavior of this attack
    private AttackAttributes mcAttackAttributes;

    //Flag to indicate that the flipped version of the animation should play
    private bool mbAnimationFlip = false;

    //Direction of the Attack
    private AttackDirection meAttackDirection = AttackDirection.eeRightward;

    //Time Tracker for attack movement
    private float AttackMovementElapsedTime = 0f;

    //Attack positions
    private Vector3 mcAttackOriginPosition;
    private Vector3 mcAttackEndPosition;

    //Attack visual Effects
    private GameObject mcSmokeFX;

    //Start Flag for attack
    private bool mbBeginAttack = false;

    private bool mbHitBoxActive = false;

    //Attack visual Effects
    private GameObject mcChildHitbox;

    //Pop Up Damage Template prefab assigned in the editor
    public GameObject mcPopUpDamageTemplate;

    private bool mbEnemyAttacker = false;
    private bool mbPlayerAttacker = false;

    EnemyAI mcEnemyCollided;
    PlayerMovement mcPlayerCollided; 

    void Start()
    {
        //Set Attack Origin 
        mcAttackOriginPosition = transform.position;

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
        }

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
        if (mbEnemyAttacker)
        {
            mcPlayerCollided = lcCollision.GetComponent<PlayerMovement>();
        }
        else if (mbPlayerAttacker) 
        {
            mcEnemyCollided = lcCollision.GetComponent<EnemyAI>();
        }

        if (mbBeginAttack && (mcEnemyCollided || mcPlayerCollided))
        {
            //Trigger hit effect
            HitFX.transform.position = lcCollision.transform.position;
            HitFX.transform.SetParent(null);
            HitFX.Play();

            //Trigger Damage pop up
            GameObject lcPopUp = Instantiate(mcPopUpDamageTemplate, lcCollision.transform.position, Quaternion.identity);
            lcPopUp.GetComponent<TMP_Text>().text = mcAttackAttributes.GetAttackDamage().ToString();

            if(transform.position.x > lcCollision.transform.position.x)
            {
                lcPopUp.GetComponent<PopUpDamage>().mbHitFromRight = true;
            }

            if(mbPlayerAttacker)
            {
                //TODO: Add KnockBack
                mcEnemyCollided.Damage(mcAttackAttributes.GetAttackDamage(), 2f, meAttackDirection);
            }
            else if(mbEnemyAttacker)
            {
                mcPlayerCollided.Damage(mcAttackAttributes.GetAttackDamage(), 2f, meAttackDirection);
            }

            if (mcPlayerAttacker != null && meAttackDirection == AttackDirection.eeDownwards)
            {
                mcPlayerAttacker.ApplyAttackMovement();
            }

            if(mcAttackAttributes.GetTravelDistance() != 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //Debug.Log("Enemy not found " +  lcCollision.gameObject.name);
        }
    }

    public void BeginAttack()
    {
        if (mbAnimationFlip || !mcAttackAttributes.GetHasAnimationFlip())
        {
            mcAttackAnimator.Play(mcAttackAttributes.GetAttackAnimationString());
        }
        else
        {
            mcAttackAnimator.Play(mcAttackAttributes.GetAttackAnimationString() + "Reverse");
        }

        if(mcPlayerAttacker != null)
        {
            mcPlayerAttacker.SetAttackAnimationValue(meAttackAnimationType);
        }

        mbBeginAttack = true;

        smokeFX.Play();
    }

    public void SetAttributes(AttackAttributes pcAttackAttributes)
    {
        //Assign Attack Attributes
        mcAttackAttributes = pcAttackAttributes;

        //Set Direction
        meAttackDirection = mcAttackAttributes.GetDirection();

            //Find attack end position
            switch (meAttackDirection)
            {
                case AttackDirection.eeLeftward:
                    if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                    {
                        meAttackAnimationType = AttackAnimationType.eeHeavySideAttack;
                    }
                    else if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeRanged)
                    {
                        meAttackAnimationType = AttackAnimationType.eeRangedSideAttack;
                    }
                    mcAttackEndPosition = new Vector3(transform.position.x - mcAttackAttributes.GetTravelDistance(), transform.position.y, 0);
                    break;
                case AttackDirection.eeRightward:
                    if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                    {
                        meAttackAnimationType = AttackAnimationType.eeHeavySideAttack;
                    }
                    else if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeRanged)
                    {
                        meAttackAnimationType = AttackAnimationType.eeRangedSideAttack;
                    }
                    mcAttackEndPosition = new Vector3(transform.position.x + mcAttackAttributes.GetTravelDistance(), transform.position.y, 0);
                    break;
                case AttackDirection.eeUpwards:
                    if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                    {
                        meAttackAnimationType = AttackAnimationType.eeHeavyUpAttack;
                    }
                    else
                    {
                        meAttackAnimationType = AttackAnimationType.eeBasicUpAttack;
                    }

                    mcAttackEndPosition = new Vector3(transform.position.x, transform.position.y + mcAttackAttributes.GetTravelDistance(), 0);
                    break;
                case AttackDirection.eeDownwards:
                    if (mcAttackAttributes.GetPlayerAttackAnimation() == (int)PlayerAttackAnimation.eeHeavy)
                    {
                        meAttackAnimationType = AttackAnimationType.eeHeavyDownAttack;
                    }
                    else
                    {
                        meAttackAnimationType = AttackAnimationType.eeBasicDownAttack;
                    }

                    mcAttackEndPosition = new Vector3(transform.position.x, transform.position.y - mcAttackAttributes.GetTravelDistance(), 0);
                    break;

            }
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

    void Update()
    {

        //If Attack has been started
        if(mbBeginAttack)
        {
            if (mcChildHitbox && !mbHitBoxActive)
            {
                mcChildHitbox.GetComponent<CapsuleCollider2D>().direction = mcAttackAttributes.GetAttackCapsuleColliderDirection();
                mcChildHitbox.SetActive(true);
                mbHitBoxActive = true;
            }

            //If Attack ends at end of animation
            if (mcAttackAttributes.GetSingleAnimationLifetime())
            {
                mcAnimationStateInfo = mcAttackAnimator.GetCurrentAnimatorStateInfo(0);

                if (mcAnimationStateInfo.normalizedTime >= 1.0f)
                {
                    Destroy(gameObject);
                }
            }

            if (mcAttackAttributes.GetTravelDistance() > 0.1f)
            {
                if (AttackMovementElapsedTime < mcAttackAttributes.GetTravelTime())
                {
                    AttackMovementElapsedTime += Time.deltaTime;

                    //Move fake card to top of hand over time
                    transform.position = Vector3.Lerp(mcAttackOriginPosition, mcAttackEndPosition,
                        AttackMovementElapsedTime / mcAttackAttributes.GetTravelTime());
                }
                else
                {
                    Destroy(gameObject);
                }
            }

            if(mcAttackAttributes.GetRevolveAround())
            {

                if (AttackMovementElapsedTime < mcAttackAttributes.GetTravelTime())
                {
                    AttackMovementElapsedTime += Time.deltaTime;

                    transform.RotateAround(transform.parent.transform.position, Vector3.forward,
                        2 * ((meAttackDirection == AttackDirection.eeLeftward) ? 1 : -1));
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
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
