using Deck;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerAttackAttributesSet
{

    //Base attack creation attributes
    AttackAttributes[,,,,] macAttacksDataSet = new AttackAttributes[(int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, 
        (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1];

    //Sub attacks attribute sets
    AttackAttributes[,,] macSubAttacksDataSet = new AttackAttributes[(int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1];

    // Start is called before the first frame update
    public PlayerAttackAttributesSet()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {

        //Sword Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Blade Strike",
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

        //Double Sword Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Double Strike",
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
            Color.white, // Partical Trail Color
            0,
            false,
            false,
            true,
            0,
            false,
            0,
            1,
            1,
            1,
            0f,
            true
            ));

        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd]
            .AddSubAttack(macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd]);

        //Fire Sword Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Fire Strike",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.6f, // Up/Down Attack Offset
            2.3f, // Size Multiplier
            false, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0, // Travel Time
            0, //Force
            AttackMovementType.eeFollowPlayer,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "FireSwordAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.red // Partical Trail Color
            ));

        //Vine Sword Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Vine Lash",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.6f, // Up/Down Attack Offset
            2f, // Size Multiplier
            false, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0, // Travel Time
            0, //Force
            AttackMovementType.eeFollowPlayer,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "VineLashAttack", // Attack Animation
            CapsuleDirection2D.Vertical, // Capsule Direction
            true, // Partical Trail Enabled
            Color.green // Partical Trail Color
            ));

        //Frost Spear Attack 
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Frost Spear",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.6f, // Up/Down Attack Offset
            2.3f, // Size Multiplier
            false, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0, // Travel Time
            0, //Force
            AttackMovementType.eeFollowPlayer,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "FrostSpearAttack", // Attack Animation
            CapsuleDirection2D.Vertical, // Capsule Direction
            true, // Partical Trail Enabled
            Color.blue // Partical Trail Color
            ));

        //Axe Attack
        macAttacksDataSet[(int)CardSuit.eeAxe, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Axe Swing",
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

        //Great Sword Attack
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeAxe, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Great Sword",
            0f,
            0.3f,
            0.2f,
            0.1f,
            1.3f,
            false,
            false,
            0,
            0f,
            0,
            AttackMovementType.eeFollowPlayer,
            false,
            (int)PlayerAttackAnimation.eeArcSwipe,
            true,
            "GreatSwordAttack",
            CapsuleDirection2D.Horizontal,
            false,
            Color.white,
            0,
            false,
            false,
            false,
            0,
            false,
            0,
            1,
            1,
            1,
            0.18f));

        //Fire Axe Attack
        macAttacksDataSet[(int)CardSuit.eeAxe, (int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Flame Axe",
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
            "FireAxeAttack",
            CapsuleDirection2D.Horizontal,
            false,
            Color.red));

        //Frost Axe Attack
        macAttacksDataSet[(int)CardSuit.eeAxe, (int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Frost Mace",
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
            "IceMaceAttack",
            CapsuleDirection2D.Horizontal,
            false,
            new Color(0, 0.9f, 1, 1)));

        //Stone Axe Attack
        macAttacksDataSet[(int)CardSuit.eeAxe, (int)CardSuit.eeSpade, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Hammer Smash",
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
            "StoneHammerAttack",
            CapsuleDirection2D.Horizontal,
            false,
            new Color(0.5f, 0.25f, 0, 1)));

        //Arrow Attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Arrow Shot",
            0,
            -0.07f,
            0.5f,
            0.5f,
            1.5f,
            true,
            false,
            8,
            0.4f,
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
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Fire Spell",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            7, // Travel Distance
            0.6f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "HeartAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(1, 0.5f, 0, 1) // Partical Trail Color
            ));

        //Spade ground Attack
        macAttacksDataSet[(int)CardSuit.eeSpade, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Rock Burst",
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
            new Color(0.5f, 0.25f, 0, 1), // Partical Trail Color
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
            AttackDirection.eeUpwards,
            false,
            true,
            0,
            false
            ));

        //Frost Fire Attack
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Frost Fire",
            0f,
            0f,
            1.7f,
            2f,
            2.5f,
            false,
            false,
            0,
            0f,
            0,
            AttackMovementType.eeFollowPlayer,
            false,
            (int)PlayerAttackAnimation.eeMagicBlast,
            true,
            "FrostFireAttack",
            CapsuleDirection2D.Vertical,
            false,
            new Color(0, 0.9f, 1, 1),
            0,
            false,
            false,
            true, // has sub attack
            2, // sub attack index
            false,
            0,
            1,
            1,
            1,
            0,
            true
            ));

        //Diamond Attack
        macAttacksDataSet[(int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = 
            (new AttackAttributes(
                "Frost Shot",
            0, 
            0, 
            0.5f, 
            0.5f, 
            1f, 
            true, 
            false, 
            4f, 
            0.4f, 
            0, 
            AttackMovementType.eeFixedDistance, 
            false,
            (int)PlayerAttackAnimation.eeBasic, 
            false, "DiamondAttack", 
            CapsuleDirection2D.Horizontal, true, new Color(0, 0.9f, 1, 1), 0, false, false, false, 0, false, 20f, 3));

        //Spade rock Attack
        macAttacksDataSet[(int)CardSuit.eeSpade, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(new AttackAttributes(
            "Rock Burst projectile",
            0, // horizontal Offset
            0f, // Vertical Offset
            0f, // Side Attack Offset
            0f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
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
            new Color(0.5f,0.25f,0,1), // Partical Trail Color
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
            false,
            AttackDirection.eeRightward,
            AttackDirection.eeLeftward,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            false,
            false
            ));

        //Frost Fire sub Attack
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(
            new AttackAttributes(
            "Frost Fire Burst",
            0f,
            0f,
            0,
            0,
            2.5f,
            true,
            false,
            0,
            0f,
            0,
            AttackMovementType.eeNoMovement,
            false,
            (int)PlayerAttackAnimation.eeBasic,
            true,
            "FrostFireSubAttack",
            CapsuleDirection2D.Vertical,
            false,
            new Color(0, 0.9f, 1, 1),
            0,
            false,
            false
            ));


        //Double Heart attack
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Fire Ball",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            7, // Travel Distance
            0.5f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "DoubleHeartAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(1,0.5f,0,1) // Partical Trail Color
            ));

        //Triple Heart attack
        macAttacksDataSet[(int)CardSuit.eeHeart, (int)CardSuit.eeHeart, (int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Fire Blast",
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
            (int)PlayerAttackAnimation.eeMagicBlast, // Player attack animation to play
            false, // Single animation lifetime
            "ThreeFireAttack", // Attack Animation
            CapsuleDirection2D.Vertical, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(1, 0.5f, 0, 1) // Partical Trail Color
            ));

        //Double Arrow attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Double Arrow",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            8, // Travel Distance
            0.4f, // Travel Time
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

        //Double Arrow attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Double Arrow",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            8, // Travel Distance
            0.4f, // Travel Time
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
            3
            ));


        //Clover Seed Attack
        macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Sapling",
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
            new Color(0, 1f, 0, 1), // Partical Trail Color
            0, //Rotation rate
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

        //Clover Plant Attack
        macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(new AttackAttributes(
            "Sapling",
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
            Color.green, // Partical Trail Color
            0, //Rotation rate
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
            AttackDirection.eeRightward,
            AttackDirection.eeLeftward,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            false,
            false,
            0,
            true,
            true
            ));

        //CloverPlantExecute
        macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().AddSubAttack(
            new AttackAttributes(
            "Sapling",
            0f, // horizontal Offset
            0f, // Vertical Offset
            0f, // Side Attack Offset
            0f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            0f, // Life Time
            0, // Force
            AttackMovementType.eeNoMovement,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "CloverEndAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.green, // Partical Trail Color
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
            false,
            AttackDirection.eeRightward,
            AttackDirection.eeLeftward,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            false,
            false));

        //Clover Arrow Attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Seed Arrow Shot",
            0,
            -0.07f,
            0.5f,
            0.5f,
            1.5f,
            true,
            false,
            8,
            0.4f,
            0,
            AttackMovementType.eeFixedDistance,
            false,
            (int)PlayerAttackAnimation.eeRanged,
            false,
            "CloverArrowAttack",
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(0, 1f, 0, 1), // Partical Trail Color
            0, //Rotation rate
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
            AttackDirection.eeBorderWards));

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack());

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().GetSubAttack());

        //Double Seed Arrow attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Double Seed Arrow",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            8, // Travel Distance
            0.4f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeRanged, // Player attack animation to play
            false, // Single animation lifetime
            "ArrowAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(0, 1f, 0, 1), // Partical Trail Color
            0, //Rotation rate
            false, // Ground origination
            false, // wall origination
            true, // has sub attack
            0, // sub attack index
            false, // is tangible
            10,
            2,
            1,
            1,
            0,
            true,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards
            ));

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack());

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().GetSubAttack());

        //Triple Seed Arrow attack
        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Double Seed Arrow",
            0, // horizontal Offset
            0, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            1.5f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            8, // Travel Distance
            0.4f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeRanged, // Player attack animation to play
            false, // Single animation lifetime
            "ArrowAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(0, 1f, 0, 1), // Partical Trail Color
            0, //Rotation rate
            false, // Ground origination
            false, // wall origination
            true, // has sub attack
            0, // sub attack index
            false, // is tangible
            10,
            3,
            1,
            1,
            0,
            true,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards
            ));

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd].AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack());

        macAttacksDataSet[(int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeArcher, (int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd].GetSubAttack().AddSubAttack(
            macAttacksDataSet[(int)CardSuit.eeClover, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].GetSubAttack().GetSubAttack());
    }

    // Returns the attack attributes aligned with the suits provided
    public AttackAttributes GetBaseAttackAttributes(List<Card> pacAttackCards)
    {
        int[] lanCardIndex = new int[5] { (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, 
            (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd };

        //Determine index from card suits
        for(int lnCardId = 0; lnCardId < pacAttackCards.Count; lnCardId++)
        {
            lanCardIndex[lnCardId] = (int)pacAttackCards[lnCardId].GetCardSuit();

            //Sort Card Suit array into ascending order
            Array.Sort(lanCardIndex);
        }

        if (macAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2], lanCardIndex[3], lanCardIndex[4]] != null)
        {
            CalculateEffectPercentages(pacAttackCards, macAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2], lanCardIndex[3], lanCardIndex[4]]);

            return macAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2], lanCardIndex[3], lanCardIndex[4]];
        }

        return macAttacksDataSet[0,7,7,7,7];
    }

    // Returns the sub attack attributes aligned with the suits provided
    public AttackAttributes GetSubAttackAttributes(List<Card> pacAttackCards)
    {
        int[] lanCardIndex = new int[5] { (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd,
            (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd };

        //Determine index from card suits
        for (int lnCardId = 0; lnCardId < pacAttackCards.Count; lnCardId++)
        {
            lanCardIndex[lnCardId] = (int)pacAttackCards[lnCardId].GetCardSuit();
            Array.Sort(lanCardIndex);
        }

        if (macSubAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]] != null)
        {
            CalculateEffectPercentages(pacAttackCards, macSubAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]]);

            return macSubAttacksDataSet[lanCardIndex[0], lanCardIndex[1], lanCardIndex[2]];
        }

        return macSubAttacksDataSet[0, 7, 7];
    }

    //Method to calculate and add percentage chances on debuffs assigned to this attack
    public void CalculateEffectPercentages(List<Card> pacAttackCards, AttackAttributes pcAttackAttributes)
    {
        int[] lanSuitEffectPercentages = new int[(int)SuitEffect.eeSuitEffectEnd] {0,0,0,0,0,0,0};

        for (int i = 0; i < pacAttackCards.Count; i++) 
        {
            //Add Percentages to array for each card in hand
            lanSuitEffectPercentages[(int)pacAttackCards[i].GetCardSuitEffect()] 
                += pacAttackCards[i].GetCardEffectChance();

            pcAttackAttributes.SetEffectChances(lanSuitEffectPercentages);
        }
    }
}
