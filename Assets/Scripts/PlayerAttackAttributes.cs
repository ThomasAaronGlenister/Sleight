using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAttributes
{

    //Base attack creation attributes
    AttackAttributes[,,] macAttacksDataSet = new AttackAttributes[7, 7, 7];

    //Sub attacks attribute sets
    AttackAttributes[,,] macSubAttacksDataSet = new AttackAttributes[7, 7, 7];

    // Start is called before the first frame update
    public PlayerAttackAttributes()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {

        //Sword Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, 0, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0.4f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            2f, // Size Multiplier
            false, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0, // Travel Time
            0, //Force
            AttackMovementType.eeFollowPlayer,
            true, // Has Animation Flup
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "SwordAttackRedux", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            false, // Partical Trail Enabled
            Color.white // Partical Trail Color
            ));

        //Axe Attack
        macAttacksDataSet[(int)CardSuit.eeAxe, 0, 0] = (new AttackAttributes(
            0f,
            0.65f,
            0.2f,
            0.1f,
            1.5f,
            false,
            false,
            0,
            0f,
            0,
            AttackMovementType.eeFollowPlayer,
            false,
            (int)PlayerAttackAnimation.eeHeavy,
            true,
            "AxeAttack",
            CapsuleDirection2D.Horizontal,
            false,
            Color.white));

        //Arrow Attack
        macAttacksDataSet[(int)CardSuit.eeArcher, 0, 0] = (new AttackAttributes(
            0,
            -0.07f,
            0.5f,
            0.5f,
            1.5f,
            true,
            false,
            8,
            0.5f,
            0,
            AttackMovementType.eeFixedDistance,
            false,
            (int)PlayerAttackAnimation.eeRanged,
            false,
            "ArrowAttack",
            CapsuleDirection2D.Horizontal,
            false,
            Color.white));

        //Heart attack
        macAttacksDataSet[(int)CardSuit.eeHeart, 0, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            5, // Travel Distance
            0.5f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "HeartAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.red // Partical Trail Color
            ));

        //Spade ground Attack
        macAttacksDataSet[(int)CardSuit.eeSpade, 0, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0.75f, // Vertical Offset
            1f, // Side Attack Offset
            -0.75f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0f, // Travel Time
            0, // Force
            AttackMovementType.eeNoMovement,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "SpadeGroundAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            false, // Partical Trail Enabled
            Color.white, // Partical Trail Color
            0, //rotation rate
            true, //Ground origination
            false,
            true,
            7,
            false,
            0,
            1,
            1,
            1,
            0,
            false,
            AttackDirection.eeUpwards,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            AttackDirection.eeUpwards
            ));

        //Clover Seed Attack
        macAttacksDataSet[(int)CardSuit.eeClover, 0, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0f, // Side Attack Offset
            0f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0f, // Travel Time
            5, //Force
            AttackMovementType.eeForceApplied,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "CloverSeedAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.green, // Partical Trail Color
            50, //Rotation rate
            false, // Ground origination
            false, // wall origination
            true, // has sub attack
            0, // sub attack index
            false, // is tangible
            0,
            1,
            1,
            1,
            0,
            true,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards
            ));

        //Diamond Attack
        macAttacksDataSet[(int)CardSuit.eeDiamond, 0, 0] = (new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, 0, AttackMovementType.eeFixedDistance, false,
            (int)PlayerAttackAnimation.eeBasic, false, "DiamondAttack", CapsuleDirection2D.Horizontal, true, Color.blue, 0, false, false, false, 0, false, 20f, 3));

        //Spade rock Attack
        macSubAttacksDataSet[(int)CardSuit.eeSpade, 0, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0f, // Vertical Offset
            0f, // Side Attack Offset
            0f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0f, // Travel Time
            7, // Force
            AttackMovementType.eeForceApplied,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "SpadeSubAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.white, // Partical Trail Color
            30, //Rotation rate
            false, // Ground origination
            false, // wall origination
            false, // has sub attack
            0, // sub attack index
            true, // is tangible
            0,
            1,
            1,
            1,
            0,
            false
            ));


        //Double Heart attack
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeHeart, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            5, // Travel Distance
            0.5f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "DoubleHeartAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.red // Partical Trail Color
            ));

        //Double Arrow attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, 0] = (new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            8, // Travel Distance
            0.5f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeRanged, // Player attack animation to play
            false, // Single animation lifetime
            "ArrowAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.white, // Partical Trail Color
            0,
            false,
            false,
            false,
            0,
            false,
            10,
            2
            ));


        //Clover Plant Attack
        macSubAttacksDataSet[(int)CardSuit.eeClover, 0, 0] = (new AttackAttributes(
            0f, // horizontal Offset
            0f, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            10f, // Life Time
            0, // Force
            AttackMovementType.eeNoMovement,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "CloverGrowAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.white, // Partical Trail Color
            0, //Rotation rate
            false, // Ground origination
            false, // wall origination
            false, // has sub attack
            0, // sub attack index
            false, // is tangible
            0,
            1,
            1,
            1,
            0,
            false
            ));
    }

    // Returns the attack attributes aligned with the suits provided
    public AttackAttributes GetBaseAttackAttributes(List<Card> pacAttackCards)
    {
        int[] lanCardIndex = new int[5] {0,0,0,0,0};

        //Determine index from card suits
        for(int lnCardId = 0; lnCardId < pacAttackCards.Count; lnCardId++)
        {
            lanCardIndex[lnCardId] = (int)pacAttackCards[lnCardId].GetCardSuit();
        }

        if (macAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]] != null)
        {
            return macAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]];
        }

        return macAttacksDataSet[0,0,0];
    }

    // Returns the sub attack attributes aligned with the suits provided
    public AttackAttributes GetSubAttackAttributes(List<Card> pacAttackCards)
    {
        int[] lanCardIndex = new int[5] { 0, 0, 0, 0, 0 };

        //Determine index from card suits
        for (int lnCardId = 0; lnCardId < pacAttackCards.Count; lnCardId++)
        {
            lanCardIndex[lnCardId] = (int)pacAttackCards[lnCardId].GetCardSuit();
        }

        if (macSubAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]] != null)
        {
            return macSubAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]];
        }

        return macSubAttacksDataSet[0, 0, 0];
    }
}
