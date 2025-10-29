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

    /****************************************
     * TEST CODE: TODO REMOVE
     */

    public bool Test = false;

    //Forces attack to particular suit
    public CardSuit TestSuit = CardSuit.eeAxe;

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

    public void GenerateAttack(List<Card> pacAttackCards, AttackDirection peAttackDirection, bool AnimationFlip, bool pbIsFacingRight)
    {
        Vector3 lcAttackOrigin = mcAttackOrigin.position;
        CardSuit leSuit = pacAttackCards[0].GetCardSuit();

        if(Test)
        {
            leSuit = TestSuit;
        }

        AttackAttributes lcAttackAttributes = mcPlayerAttackAttributes.GetBaseAttackAttributes(leSuit);

        if(Test)
        {
            lcAttackAttributes = AttackAttributeTest(lcAttackAttributes);
        }

        GameObject lcAttack = Instantiate(mcAttackPrefab, lcAttackOrigin, mcAttackOrigin.rotation);

        Attack lcAttackComponent = lcAttack.GetComponent<Attack>();

        if(lcAttackComponent != null)
        {
            //Set Damage and direction to Attack Attributes
            lcAttackAttributes.SetAttackDamage(CalculateAttackDamage(pacAttackCards));
            lcAttackAttributes.SetDirection(peAttackDirection);

            //Assign Attack components
            lcAttackComponent.SetAttacker(mcPlayerMovement);
            lcAttackComponent.SetAttributes(lcAttackAttributes);
            lcAttackComponent.SetFlip(AnimationFlip);

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

            if (pbIsFacingRight)
            {
                Vector3 ls = lcAttack.transform.localScale;
                ls.y *= -1f;
                lcAttack.transform.localScale = ls;
            }

            lcAttack.transform.rotation = Quaternion.Euler(0,0,90);
        }
        else if(peAttackDirection == AttackDirection.eeDownwards)
        {
            lcAttackOrigin.y -= lcAttackAttributes.GetUpDownAttackOffset();
            lcAttackOrigin.x += lcAttackAttributes.GetHorizontalOffset();

            if (!pbIsFacingRight)
            {
                Vector3 ls = lcAttack.transform.localScale;
                ls.y *= -1f;
                lcAttack.transform.localScale = ls;
            }
            lcAttack.transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        lcAttack.transform.position = lcAttackOrigin;

        //Add Attack to List of attacks
        macActiveAttacks.Add(lcAttackComponent);

        //Finally Start the Attack
        lcAttackComponent.BeginAttack();
    }

    public void GenerateAttack(EnemyAttacks peEnemyAttack, int pnAttackDamage, AttackDirection peAttackDirection)
    {
        Vector3 lcAttackOrigin = mcAttackOrigin.position;
        AttackAttributes lcAttackAttributes = mcEnemyAttackAttributes.GetBaseAttackAttributes(peEnemyAttack);

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
            lcAttackComponent.SetAttributes(lcAttackAttributes);
            lcAttackComponent.SetAttacker(mcAttacker.GetComponent<EnemyAI>());

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
            mfTravelTimeTest = pcAttackAttributes.GetTravelTime();
        }

        if (mbSingleAnimationAttackTest == false)
        {
            mbSingleAnimationAttackTest = pcAttackAttributes.GetSingleAnimationLifetime();
        }

        if (mcAttackAnimationStringTest.Length == 0)
        {
            mcAttackAnimationStringTest = pcAttackAttributes.GetAttackAnimationString();
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
                pcAttackAttributes.GetHasAnimationFlip(),
                pcAttackAttributes.GetPlayerAttackAnimation(),
                mbSingleAnimationAttackTest,
                mcAttackAnimationStringTest,
                pcAttackAttributes.GetAttackCapsuleColliderDirection());

        return lcAttackAtt;
    }

}
