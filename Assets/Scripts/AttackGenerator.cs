using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackGenerator : MonoBehaviour
{
    //Gameobject performing the attacks
    public GameObject mcAttacker;

    public PlayerMovement mcPlayerMovement;

    //Horizontal Origin Point for the attacks
    private Transform mcAttackOrigin;

    //Prefab Attack Object to be created on each attack
    public GameObject mcAttackPrefab;

    //Base attack creation attributes for player
    PlayerAttackAttributes mcPlayerAttackAttributes;

    //Base attack creation attributes for Enemys
    EnemyAttackAttributes mcEnemyAttackAttributes;

    //List of Attacks currently out
    public List<Attack> macActiveAttacks = new List<Attack>();

    public Vector2[] macDirectionVectors = new Vector2[4];

    /****************************************
     * TEST CODE: TODO REMOVE
     */

    public bool Test = false;

    //Forces attack to particular suit
    public CardSuit TestSuit = CardSuit.eeCardSuitEnd;
    public CardSuit TestSuit2 = CardSuit.eeCardSuitEnd;
    public CardSuit TestSuit3 = CardSuit.eeCardSuitEnd;

    private List<Card> macTestCards = new List<Card>();

    // Attack Test Parameters
    public float mfHorizontalOffsetTest = 0;
    public float mfVerticalOffsetTest = 0;
    public float mfSideAttackOffsetTest = 0;
    public float mfUpDownAttackOffsetTest = 0;
    public float mfAttackBaseSizeMultiplierTest = 1;
    public bool mbDisjointedTest = false;
    public bool mbRevolveAroundTest = false;
    public Vector3 mcRevolutionPointTest;
    public float mfTravelDistanceTest = 0f;
    public float mfTravelTimeTest = 0f;
    public string mcAttackAnimationStringTest;
    public bool mbSingleAnimationAttackTest = false;

    public int mnNumInstancesTest = 0;
    public float mfAdjustedAngleTest = 0;

    /****************************************/

    // Start is called before the first frame update
    void Start()
    {
        mcPlayerAttackAttributes = new PlayerAttackAttributes();
        mcAttackOrigin = mcAttacker.transform;

        if(mcAttacker.tag == "Player")
        {
            mcPlayerMovement = mcAttacker.GetComponent<PlayerMovement>();
        }
        else if(mcAttacker.tag == "Enemy")
        {
            mcEnemyAttackAttributes = new EnemyAttackAttributes();
        }

        macDirectionVectors[(int)AttackDirection.eeRightward] = Vector2.right;
        macDirectionVectors[(int)AttackDirection.eeLeftward] = Vector2.left;
        macDirectionVectors[(int)AttackDirection.eeUpwards] = Vector2.up;
        macDirectionVectors[(int)AttackDirection.eeDownwards] = Vector2.down;
    }

    public int CalculateAttackDamage(List<Card> pacAttackCards)
    {
        int lnAttackDamage = 0;

        foreach(Card lcCard in pacAttackCards)
        {
            lnAttackDamage += lcCard.GetCardDamage();
        }

        return lnAttackDamage;
    }

    public bool GenerateAttack(List<Card> pacAttackCards, AttackDirection peAttackDirection, bool AnimationFlip, bool pbIsFacingRight)
    {
        bool lbGenerationSuccessful = true;

        Vector3 lcAttackOrigin = mcAttackOrigin.position;

        AttackAttributes lcAttackAttributes = mcPlayerAttackAttributes.GetBaseAttackAttributes(pacAttackCards);

        /********************************************************/

        //TODO: REMOVE TEST
        if(Test)
        {
            macTestCards.Clear();

            if(TestSuit != CardSuit.eeCardSuitEnd)
            {
                macTestCards.Add(new Card("", TestSuit, CardAttackType.eePhysicalAttack, SuitEffect.eeCrit, CardRank.eeJack, RankEffect.eeNone, null));
            }

            if (TestSuit2 != CardSuit.eeCardSuitEnd)
            {
                macTestCards.Add(new Card("", TestSuit2, CardAttackType.eePhysicalAttack, SuitEffect.eeCrit, CardRank.eeJack, RankEffect.eeNone, null));
            }

            if (TestSuit3 != CardSuit.eeCardSuitEnd)
            {
                macTestCards.Add(new Card("", TestSuit3, CardAttackType.eePhysicalAttack, SuitEffect.eeCrit, CardRank.eeJack, RankEffect.eeNone, null));
            }

            lcAttackAttributes = mcPlayerAttackAttributes.GetBaseAttackAttributes(macTestCards);

            lcAttackAttributes = AttackAttributeTest(lcAttackAttributes);
        }
        /********************************************************/

        //Attack is produced from the ground or wall
        if (lcAttackAttributes.GetGroundOrigination())
        {
            Vector2 lcRayDirection = (peAttackDirection == AttackDirection.eeUpwards) ? Vector2.up : Vector2.down;

            RaycastHit2D lcOriginhit = Physics2D.Raycast(new Vector2(lcAttackOrigin.x, lcAttackOrigin.y), lcRayDirection, 50f, 15);

            if(lcOriginhit)
            {
                lcAttackOrigin = lcOriginhit.point;
            }
        }

        float lfStartAngle = (lcAttackAttributes.GetNumInstances() != 1) ? (-1 * (lcAttackAttributes.GetAdjustedAngle() / 2)) : lcAttackAttributes.GetAdjustedAngle();

        Vector3 lcStartOrigin = lcAttackOrigin;

        for (int lnDuplicates = 0; lnDuplicates < lcAttackAttributes.GetNumInstances(); lnDuplicates++)
        {
            lcAttackOrigin = lcStartOrigin;

            GameObject lcAttack = Instantiate(mcAttackPrefab, lcAttackOrigin, mcAttackOrigin.rotation);

            Attack lcAttackComponent = lcAttack.GetComponent<Attack>();

            if (lcAttackComponent != null)
            {
                //Set Damage and direction to Attack Attributes
                lcAttackAttributes.SetAttackDamage(CalculateAttackDamage(pacAttackCards));
                lcAttackAttributes.SetDirection(peAttackDirection);

                //Assign Attack components
                lcAttackComponent.SetAttacker(mcPlayerMovement);
                lcAttackComponent.SetAttributes(lcAttackAttributes, lfStartAngle);
                lcAttackComponent.SetFlip(AnimationFlip);

                if (lcAttackAttributes.GetHasSubAttack())
                {
                    lcAttackComponent.SetSubAttackAttributes(mcPlayerAttackAttributes.GetSubAttackAttributes(pacAttackCards));
                }

                //Lock attack to player if it is not disjointed
                if (!lcAttackAttributes.GetDisjointed())
                {
                    lcAttack.transform.SetParent(this.transform);
                }
            }
            else
            {
                Debug.Log("Attack component is NULL");
            }

            lcAttack.transform.localScale *= lcAttackAttributes.GetAttackBaseSizeMultiplier();

            //Offsets derived from attack direction
            if (peAttackDirection == AttackDirection.eeLeftward)
            {
                lcAttackOrigin.x -= lcAttackAttributes.GetSideAttackOffset();
                lcAttackOrigin.y += lcAttackAttributes.GetVerticalOffset();

                Vector3 ls = lcAttack.transform.localScale;
                ls.x *= -1f;
                lcAttack.transform.localScale = ls;
            }
            else if (peAttackDirection == AttackDirection.eeRightward)
            {
                lcAttackOrigin.x += lcAttackAttributes.GetSideAttackOffset();
                lcAttackOrigin.y += lcAttackAttributes.GetVerticalOffset();
            }
            else if (peAttackDirection == AttackDirection.eeUpwards)
            {
                lcAttackOrigin.y += lcAttackAttributes.GetUpDownAttackOffset();
                lcAttackOrigin.x -= lcAttackAttributes.GetHorizontalOffset();

                if (pbIsFacingRight && !(lcAttackAttributes.GetGroundOrigination()))
                {
                    Vector3 ls = lcAttack.transform.localScale;
                    ls.y *= -1f;
                    lcAttack.transform.localScale = ls;
                }
            }
            else if (peAttackDirection == AttackDirection.eeDownwards)
            {
                lcAttackOrigin.y -= lcAttackAttributes.GetUpDownAttackOffset();
                lcAttackOrigin.x += lcAttackAttributes.GetHorizontalOffset();

                if (!pbIsFacingRight && !(lcAttackAttributes.GetGroundOrigination()))
                {
                    Vector3 ls = lcAttack.transform.localScale;
                    ls.y *= -1f;
                    lcAttack.transform.localScale = ls;
                }
            }

            lcAttack.transform.position = lcAttackOrigin;

            //Add Attack to List of attacks
            macActiveAttacks.Add(lcAttackComponent);

            //Finally Start the Attack
            lcAttackComponent.BeginAttack();

            if (lcAttackAttributes.GetNumInstances() != 1)
            {
                lfStartAngle += (lcAttackAttributes.GetAdjustedAngle() / ((lcAttackAttributes.GetNumInstances() - 1)));
            }

        }

        macActiveAttacks.Clear();

        return lbGenerationSuccessful;
    }

    public void GenerateAttack(EnemyAttacks peEnemyAttack, int pnAttackDamage, AttackDirection peAttackDirection)
    {
        Vector3 lcAttackOrigin = mcAttackOrigin.position;
        AttackAttributes lcAttackAttributes = mcEnemyAttackAttributes.GetBaseAttackAttributes(peEnemyAttack);

        if(lcAttackAttributes.GetMoveTowardsTarget())
        {
            peAttackDirection = AttackDirection.eeRightward;
        }

        //Offsets derived from attack direction
        if (peAttackDirection == AttackDirection.eeLeftward)
        {
            lcAttackOrigin.x -= lcAttackAttributes.GetSideAttackOffset();
            lcAttackOrigin.y += lcAttackAttributes.GetVerticalOffset();
        }
        else if (peAttackDirection == AttackDirection.eeRightward)
        {
            lcAttackOrigin.x += lcAttackAttributes.GetSideAttackOffset();
            lcAttackOrigin.y += lcAttackAttributes.GetVerticalOffset();
        }
        else if (peAttackDirection == AttackDirection.eeUpwards)
        {
            lcAttackOrigin.y += lcAttackAttributes.GetUpDownAttackOffset();
            lcAttackOrigin.x -= lcAttackAttributes.GetHorizontalOffset();
        }
        else if (peAttackDirection == AttackDirection.eeDownwards)
        {
            lcAttackOrigin.y -= lcAttackAttributes.GetUpDownAttackOffset();
            lcAttackOrigin.x += lcAttackAttributes.GetHorizontalOffset();
        }

        GameObject lcAttack = Instantiate(mcAttackPrefab, lcAttackOrigin, mcAttackOrigin.rotation);

        Attack lcAttackComponent = lcAttack.GetComponent<Attack>();

        if (lcAttackComponent != null)
        {
            lcAttackAttributes.SetDirection(peAttackDirection);
            lcAttackAttributes.SetAttackDamage(pnAttackDamage);
            lcAttackComponent.SetAttacker(mcAttacker.GetComponent<EnemyAI>());
            lcAttackComponent.SetAttributes(lcAttackAttributes, lcAttackAttributes.GetAdjustedAngle());

            if (!lcAttackAttributes.GetDisjointed())
            {
                lcAttack.transform.SetParent(this.transform);
            }
        }
        else
        {
            Debug.Log("Attack component is NULL");
        }

        lcAttack.transform.localScale *= lcAttackAttributes.GetAttackBaseSizeMultiplier();

        if (peAttackDirection == AttackDirection.eeLeftward)
        {
            Vector3 ls = lcAttack.transform.localScale;
            ls.x *= -1f;
            lcAttack.transform.localScale = ls;
        }
        else if (peAttackDirection == AttackDirection.eeUpwards)
        {
            lcAttack.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (peAttackDirection == AttackDirection.eeDownwards)
        {
            lcAttack.transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        //Finally Start the Attack
        lcAttackComponent.BeginAttack();
    }

    private AttackAttributes AttackAttributeTest(AttackAttributes pcAttackAttributes)
    {

        if(mfHorizontalOffsetTest == 0)
        {
            mfHorizontalOffsetTest = pcAttackAttributes.GetHorizontalOffset();
        }

        if (mfVerticalOffsetTest == 0)
        {
            mfVerticalOffsetTest = pcAttackAttributes.GetVerticalOffset();
        }

        if(mfSideAttackOffsetTest == 0)
        {
            mfSideAttackOffsetTest = pcAttackAttributes.GetSideAttackOffset();
        }

        if (mfUpDownAttackOffsetTest == 0)
        {
            mfUpDownAttackOffsetTest = pcAttackAttributes.GetUpDownAttackOffset();
        }


        if (mfAttackBaseSizeMultiplierTest == 1)
        {
            mfAttackBaseSizeMultiplierTest = pcAttackAttributes.GetAttackBaseSizeMultiplier();
        }

        if (mbDisjointedTest == false)
        {
            mbDisjointedTest = pcAttackAttributes.GetDisjointed();
        }

        if (mbRevolveAroundTest == false)
        {
            mbRevolveAroundTest = pcAttackAttributes.GetRevolveAround();
        }

        if (mfTravelDistanceTest == 0)
        {
            mfTravelDistanceTest = pcAttackAttributes.GetTravelDistance();
        }

        if (mfTravelTimeTest == 0)
        {
            mfTravelTimeTest = pcAttackAttributes.GetLifeTime();
        }

        if (mbSingleAnimationAttackTest == false)
        {
            mbSingleAnimationAttackTest = pcAttackAttributes.GetSingleAnimationLifetime();
        }

        if (mcAttackAnimationStringTest.Length == 0)
        {
            mcAttackAnimationStringTest = pcAttackAttributes.GetAttackAnimationString();
        }

        if(mnNumInstancesTest == 0)
        {
            mnNumInstancesTest = pcAttackAttributes.GetNumInstances();
        }

        if(mfAdjustedAngleTest == 0)
        {
            mfAdjustedAngleTest = pcAttackAttributes.GetAdjustedAngle();
        }


        AttackAttributes lcAttackAtt = new AttackAttributes(
                mfHorizontalOffsetTest,
                mfVerticalOffsetTest,
                mfSideAttackOffsetTest,
                mfUpDownAttackOffsetTest,
                mfAttackBaseSizeMultiplierTest,
                mbDisjointedTest, mbRevolveAroundTest,
                mfTravelDistanceTest,
                mfTravelTimeTest,
                pcAttackAttributes.GetMovementForce(),
                pcAttackAttributes.GetAttackMovementType(),
                pcAttackAttributes.GetHasAnimationFlip(),
                pcAttackAttributes.GetPlayerAttackAnimation(),
                mbSingleAnimationAttackTest,
                mcAttackAnimationStringTest,
                pcAttackAttributes.GetAttackCapsuleColliderDirection(),
                pcAttackAttributes.GetParticleTrailEnabled(),
                pcAttackAttributes.GetParticleTrailColor(),
                pcAttackAttributes.GetRotationRate(),
                pcAttackAttributes.GetGroundOrigination(),
                pcAttackAttributes.GetWallOrigination(),
                pcAttackAttributes.GetHasSubAttack(),
                pcAttackAttributes.GetSubAttackIndex(),
                pcAttackAttributes.GetTangible(),
                mfAdjustedAngleTest,
                mnNumInstancesTest
                );

        return lcAttackAtt;
    }

}
