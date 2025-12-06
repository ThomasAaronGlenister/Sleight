using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAttributes
{
    //Base attack creation attributes
    List<AttackAttributes> macAttacksDataSet = new();

    //Constructor
    public EnemyAttackAttributes()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {
        //Horizontal Offset, Vertical Offset, Size Multiplier, Is Disjointed, Travel Distance, Travel Time, Attack Animation String

        //Chase Freak Slash Attack 
        macAttacksDataSet.Add(new AttackAttributes(
            0, // Horizontal Offset
            0, // Vertical Offset
            0.4f, // Side attack offset
            0.5f, // Up/Down attack offset
            2.5f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ChaseFreakAttack_1", //Attack string
            CapsuleDirection2D.Vertical, // Capsule direction
            false, // Partical Trail
            Color.white // Particle Trail color
            ));

        //Bat Bite attack
        macAttacksDataSet.Add(new AttackAttributes(
            0, // Horizontal Offset
            0, // Vertical Offset
            0.4f, // Side attack offset
            0.5f, // Up/Down attack offset
            2.5f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "BatBiteAttack", //Attack string
            CapsuleDirection2D.Vertical, // Capsule direction
            false, // Partical Trail
            Color.white // Particle Trail color
            ));

        //Chick Fireball Attack 
        macAttacksDataSet.Add(new AttackAttributes(
            0, // Horizontal Offset
            0, // Vertical Offset
            0f, // Side attack offset
            0f, // Up/Down attack offset
            1f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0.5f, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            false, //Single animation lifetime
            "EmptyAttack", //Attack string
            CapsuleDirection2D.Vertical, // Capsule direction
            true, // Partical Trail
            Color.red // Particle Trail color
            ));

        //Minotaur Slash Attack 
        macAttacksDataSet.Add(new AttackAttributes(
            0, // Horizontal Offset
            0, // Vertical Offset
            0.9f, // Side attack offset
            0.5f, // Up/Down attack offset
            3.5f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ChaseFreakAttack_1", //Attack string
            CapsuleDirection2D.Vertical, // Capsule direction
            false, // Partical Trail
            Color.white, // Particle Trail color
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
            0.4f
            ));

        //Sprite Fireball attack
        macAttacksDataSet.Add(new AttackAttributes(
            0, // horizontal Offset
            0, // Vertical Offset
            0f, // Side Attack Offset
            0f, // Up/Down Attack Offset
            1f, // Size Multiplier
            true, // Disjointed
            false, // revolve around
            7, // Travel Distance
            1f, // Travel Time
            0, //Force
            AttackMovementType.eeFixedDistance,
            false, // Has Animation Flip
            (int)PlayerAttackAnimation.eeBasic, // Player attack animation to play
            false, // Single animation lifetime
            "PurpleFireballAttack", // Attack Animation
            CapsuleDirection2D.Horizontal, // Capsule Direction
            true, // Partical Trail Enabled
            Color.magenta, // Partical Trail Color
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
            0,
            false,
            AttackDirection.eeRightward,
            AttackDirection.eeRightward,
            AttackDirection.eeRightward,
            AttackDirection.eeRightward,
            true
            ));

        //Ghost Slash Attack 
        macAttacksDataSet.Add(new AttackAttributes(
            0, // Horizontal Offset
            0, // Vertical Offset
            0.7f, // Side attack offset
            0.5f, // Up/Down attack offset
            3f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ChaseFreakAttack_1", //Attack string
            CapsuleDirection2D.Vertical, // Capsule direction
            false, // Partical Trail
            Color.white, // Particle Trail color
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
            0.3f
            ));

    }

    // Returns the attack attributes aligned with the suit provided
    public AttackAttributes GetBaseAttackAttributes(EnemyAttacks peEnemyAttack)
    {
        return macAttacksDataSet[(int)peEnemyAttack - 1];
    }
}
