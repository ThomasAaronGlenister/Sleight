using Deck;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerAttackAttributesSet
{

    //Base attack creation attributes
    AttackAttributes[,,,,] macAttacksDataSet = new AttackAttributes[(int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, 
        (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1];

    AttackAttributes[,,,,] macFileReadAttacksDataSet = new AttackAttributes[(int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1,
    (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1];

    //Sub attacks attribute sets
    AttackAttributes[,,] macSubAttacksDataSet = new AttackAttributes[(int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1, (int)CardSuit.eeCardSuitEnd + 1];

    // Start is called before the first frame update
    public PlayerAttackAttributesSet()
    {
        //GenerateBaseAttackAttributeSet();

        ReadPlayerAttackAttributeFile("Assets/SpreadSheets/PlayerAttackAttributesSheet.csv");

        //PrintAttackAttributes();
    }

    void PrintAttackAttributes()
    {
        foreach(AttackAttributes lcAttackAttributes in macAttacksDataSet)
        {
            if(lcAttackAttributes != null)
            {
                Debug.Log(lcAttackAttributes.GetString());

                if(lcAttackAttributes.GetSubAttack() != null)
                {
                    Debug.Log(lcAttackAttributes.GetSubAttack().GetString());
                }
            }
        }
    }

    public void ReadCSV(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("File " + filePath + " not found!");
            return;
        }

        using (StreamReader sr = new StreamReader(filePath))
        {
            int lnLineIndex = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //Do not read first line, It is used as column Definition
                if(lnLineIndex != 0)
                {
                    string[] values = line.Split(','); // Split by comma
                    try
                    {
                        //Try Parse Hex string to color
                        Color lcParticleColor = Color.white;
                        UnityEngine.ColorUtility.TryParseHtmlString(("#" + values[23]), out lcParticleColor);

                        //Read Row into Attack Attributes Data Set
                        AttackAttributes lcNewAttack = (new AttackAttributes(
                            values[5], // Attack Name
                            float.Parse(values[6]), // horizontal Offset
                            float.Parse(values[7]), // Vertical Offset
                            float.Parse(values[8]), // Side Attack Offset
                            float.Parse(values[9]), // Up/Down Attack Offset
                            float.Parse(values[10]), // Size Multiplier
                            (int.Parse(values[11]) != 0), // Disjointed
                            (int.Parse(values[12]) != 0), // revolve around
                            float.Parse(values[13]), // Travel Distance
                            float.Parse(values[14]), // Travel Time
                            float.Parse(values[15]), //Force
                            (AttackMovementType)int.Parse(values[16]), // Attack Movement Type
                            (int.Parse(values[17]) != 0), // Has Animation Flip
                            int.Parse(values[18]), // Player attack animation to play
                            (int.Parse(values[19]) != 0), // Single animation lifetime
                            values[20], // Attack Animation
                            (CapsuleDirection2D)int.Parse(values[21]), // Capsule Direction
                            (int.Parse(values[22]) != 0), // Partical Trail Enabled
                            lcParticleColor, // Partical Trail Color
                            float.Parse(values[24]), //Rotation rate
                            (int.Parse(values[25]) != 0), // Ground origination
                            (int.Parse(values[26]) != 0), // wall origination
                            (int.Parse(values[27]) != 0), // has sub attack
                            int.Parse(values[28]), // sub attack index
                            (int.Parse(values[29]) != 0), // is tangible
                            float.Parse(values[30]), // Adjusted Angle
                            int.Parse(values[31]), // num attack instances
                            float.Parse(values[32]), // Mass
                            float.Parse(values[33]), // Gravity Scale
                            float.Parse(values[34]), // Attack Delay
                            (int.Parse(values[35]) != 0), //Create sub attack on end
                            (AttackDirection)int.Parse(values[36]), //SubAttackDirection When right
                            (AttackDirection)int.Parse(values[37]), //SubAttackDirection when left
                            (AttackDirection)int.Parse(values[38]), //SubAttackDirection when up
                            (AttackDirection)int.Parse(values[39]), //SubAttackDirection when down
                            (int.Parse(values[40]) != 0), // Move towards target
                            (int.Parse(values[41]) != 0), // animate attacker
                            float.Parse(values[42]), // Knockback
                            (int.Parse(values[43]) != 0), // Has Hitbox
                            (int.Parse(values[44]) != 0), // Hitbox is trigger
                            float.Parse(values[45]) // Force amplifier
                            ));

                        //Check if this attack is new attack or sub attack
                        //Add Base Attack
                        if(lcNewAttack.GetSubAttackIndex() == 0)
                        {
                            //Debug.Log("Add Attack: " + lcNewAttack.GetAttackName());

                            //Process each Value into Attack Attributes data set
                            macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])] = lcNewAttack;
                        }
                        //Add Sub Attack
                        else if(lcNewAttack.GetSubAttackIndex() == 1 
                            && macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])] != null)
                        {
                            //Debug.Log("Add Sub Attack: " + lcNewAttack.GetAttackName());
                            macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])].AddSubAttack(lcNewAttack);
                        }
                        //Add sub attack sub attack
                        else if(lcNewAttack.GetSubAttackIndex() == 2
                            && macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])] != null
                            && macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])].GetSubAttack() != null)
                        {
                            //Debug.Log("Add Sub-Sub Attack: " + lcNewAttack.GetAttackName());
                            macAttacksDataSet[int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])].GetSubAttack().AddSubAttack(lcNewAttack);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Formatting issue reading Player Attack Row: " + lnLineIndex);
                    }
                }

                //Increment to match read rows
                lnLineIndex++;
            }
        }
    }

    private void ReadPlayerAttackAttributeFile(string lcFileName)
    {
        ReadCSV(lcFileName);

        Debug.Log("CSV data Loaded");
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
            0, // Rotation Rate
            false, // Ground Origination
            false, // Wall origination
            true, // has sub attack
            0, // sub attack index
            false, // tangible
            0, // adjusted angle
            1, // num instances
            1, // Mass
            1, // Gravityscale
            0f, // Attack Delay
            true // Create sub attack on end
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

        //Great Flame Sword Attack
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeAxe, (int)CardSuit.eeHeart, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Great Flame Sword",
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
            "GreatFlameSwordAttack",
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
            new Color(0, 0.9f, 1, 1),
            0, // Rotation Rate
            false, // Ground origination
            false, // wall origination
            true, // has sub attack
            0, // sub attack index
            false, // is tangible
            0,
            1,
            1,
            1,
            0, // Attack Delay
            true, //Create sub attack on end
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards,
            AttackDirection.eeBorderWards
            ));

        //Ice Spike Ground attack
        macAttacksDataSet[(int)CardSuit.eeAxe, (int)CardSuit.eeDiamond, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd].AddSubAttack(
            new AttackAttributes("Ice Spikes",
            1f, // horizontal Offset
            0f, // Vertical Offset
            0.5f, // Side Attack Offset
            0.5f, // Up/Down Attack Offset
            2f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            0, // Travel Distance
            10f, // Life Time
            0, // Force
            AttackMovementType.eeNoMovement,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            true, // Single animation lifetime
            "IceSpikeAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            false, // Partical Trail Enabled
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
            0, // Attack Delay
            false, //Create sub attack on end
            AttackDirection.eeRightward,
            AttackDirection.eeLeftward,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            false,
            false,
            0,
            true,
            false)
            );

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

        //Boomerang Attack
        macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeArcher, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd] = (new AttackAttributes(
            "Boomerang",
            0,
            -0.07f,
            0.5f,
            0.5f,
            2f,
            true,
            false,
            8,
            5f,
            1.5f,
            AttackMovementType.eeForceApplied,
            false,
            (int)PlayerAttackAnimation.eeBasic,
            false,
            "BoomerangAttack",
            CapsuleDirection2D.Horizontal,
            true,
            Color.white,
            0,
            false,
            false,
            false,
            0,
            false,
            0,
            1,
            3,
            0,
            0.1f,
            false,
            AttackDirection.eeRightward,
            AttackDirection.eeLeftward,
            AttackDirection.eeUpwards,
            AttackDirection.eeDownwards,
            false,
            true,
            0,
            true,
            false,
            -0.28f
            ));

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
            35, // Force
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
            0, // adjusted angle
            1, // num instances
            3, // Mass
            3, // Gravityscale
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
            3f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            10, // Travel Distance
            0.5f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeMagicBlast, // Player attack animation to play
            false, // Single animation lifetime
            "ThreeFireAttack", // Attack Animation
            CapsuleDirection2D.Vertical, // Capsule Direction
            true, // Partical Trail Enabled
            new Color(1, 0.5f, 0, 1), // Partical Trail Color
            0, // Rotation Rate
            false, // Ground Origination
            false, // Wall origination
            false, // has sub attack
            0, // sub attack index
            false, // tangible
            0, // adjusted angle
            1, // num instances
            1, // Mass
            1, // Gravityscale
            0.15f, // Attack Delay
            false // Create sub attack on end
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
            20, //Force
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
            3,
            3,
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
        //TODO: Remove once Attack data set filled
        else
        {
            CalculateEffectPercentages(pacAttackCards, macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd, 
                (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd]);

            return macAttacksDataSet[(int)CardSuit.eeSword, (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd,
                (int)CardSuit.eeCardSuitEnd, (int)CardSuit.eeCardSuitEnd];
        }

        return macAttacksDataSet[0, 7, 7, 7, 7];
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
