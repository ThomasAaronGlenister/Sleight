using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAttributesSet
{
    //Base attack creation attributes
    List<AttackAttributes> macAttacksDataSet = new();

    //Constructor
    public EnemyAttackAttributesSet()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {
        //Horizontal Offset, Vertical Offset, Size Multiplier, Is Disjointed, Travel Distance, Travel Time, Attack Animation String

        /**
         *     public AttackAttributes(
        string pcAttackName,
        float pfHorzOffset,
        float pfVertOffset,
        float pfSideAttackOffset,
        float pfUpDownAttackOffset,
        float pfSizeMult,
        bool pbDisjointed,
        bool pbRevolveAround,
        float pfTravelDistance,
        float pfLifeTime,
        float pfMovementForce,
        AttackMovementType peAttackMovement,
        bool pbHasAnimationFlip,
        int pnAttackerAnimation,
        bool pbSingleAnimationLifetime,
        string pcAttackAnimationString,
        CapsuleDirection2D peAttackCapsuleColliderDirection,
        bool pbParticleTrail,
        Color pcParticleColor,
        float pfRotationRate = 0,
        bool pbGroundOrigination = false,
        bool pbWallOrigination = false,
        bool pbHasSubAttack = false,
        int pnSubAttackIndex = 0,
        bool pbTangible = false,
        float pfAdjustedAngle = 0,
        int pnNumberOfAttackInstances = 1,
        float pfMass = 1,
        float pfGravityScale = 1,
        float pfAttackDelay = 0,
        bool pbCreateSubAttackOnEnd = false,
        AttackDirection pcSubAttackDirectionOnRight = AttackDirection.eeRightward,
        AttackDirection pcSubAttackDirectionOnLeft = AttackDirection.eeLeftward,
        AttackDirection pcSubAttackDirectionOnUp = AttackDirection.eeUpwards,
        AttackDirection pcSubAttackDirectionOnDown = AttackDirection.eeDownwards,
        bool pbMoveTowardsTarget = false,
        bool pbAnimateAttacker = true,
        float pfKnockBack = 0,
        bool pbHasHitbox = true,
        bool pbHitBoxIsTrigger = false
        */

        //Knight Slash Attack 0
        macAttacksDataSet.Add(new AttackAttributes(
            "Knight Slash",
            0, // Horizontal Offset
            0, // Vertical Offset
            0.5f, // Side attack offset
            0.5f, // Up/Down attack offset
            2f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ClawAttack", //Attack string
            CapsuleDirection2D.Horizontal, // Capsule direction
            false, // Partical Trail
            Color.white, // Particle Trail color
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
            0.35f,
            true
            ));

        macAttacksDataSet[0].SetEnemyAttackValues(0, 0, 0, 2, 0, 1, 1, 1, 1f);
        macAttacksDataSet[0].SetEnemyAttackId(1);

        AttackAttributes lcClawSubAttack = new AttackAttributes(
            "Knight Slash",
            0, // Horizontal Offset
            0, // Vertical Offset
            0.3f, // Side attack offset
            0.5f, // Up/Down attack offset
            2f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ClawAttackUpsideDown", //Attack string
            CapsuleDirection2D.Horizontal, // Capsule direction
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
            0.25f
            );

        macAttacksDataSet[0].AddSubAttack(lcClawSubAttack);

        //Knight Jump Attack 1
        macAttacksDataSet.Add(new AttackAttributes(
            "Knight Jump Attack",
            0, // Horizontal Offset
            0, // Vertical Offset
            0, // Side attack offset
            0, // Up/Down attack offset
            1.5f, // Size Multiplier
            false, // Disjointed flag
            false, // Revolve around flag
            0, // Travel Distance
            0, //Life Time
            0, // Force
            AttackMovementType.eeFollowPlayer,
            false, // reverse animation
            0, // Attacker Animation
            true, //Single animation lifetime
            "ClawDoubleDownwardsAttack", //Attack string
            CapsuleDirection2D.Horizontal, // Capsule direction
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
            0.8f,
            false
            ));

        macAttacksDataSet[1].SetEnemyAttackValues(0, 0, 2, 4, 0, 3, 1, 1, 2f);
        macAttacksDataSet[1].SetEnemyAttackId(2);

        //Bat Bite attack 1
        macAttacksDataSet.Add(new AttackAttributes(
            "Bat Bite",
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

        //Chick Fireball Attack 2
        macAttacksDataSet.Add(new AttackAttributes(
            "",
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

        //Minotaur Slash Attack 3
        macAttacksDataSet.Add(new AttackAttributes(
            "Minotaur slash",
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

        //Sprite Fireball attack 4
        macAttacksDataSet.Add(new AttackAttributes(
            "Sprite fireball",
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

        //Ghost Slash Attack 5
        macAttacksDataSet.Add(new AttackAttributes(
            "Ghost Slash",
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

        macAttacksDataSet[6].SetEnemyAttackValues(0, 0, 0, 2, 0, 2, 1, 1);
        macAttacksDataSet[6].SetEnemyAttackId(1);

        //Ghost Laser Attack 6
        macAttacksDataSet.Add(new AttackAttributes(
            "Ghost Laser",
            0f, // Horizontal Offset
            0.3f, // Vertical Offset
            0.3f, // Side attack offset
            0f, // Up/Down attack offset
            1f, // Size Multiplier
            true, // Disjointed flag
            false, // Revolve around flag
            10, // Travel Distance
            0.5f, //Travel Time
            0, // Force
            AttackMovementType.eeFixedDistance,
            false, // reverse animation
            0, // Attacker Animation
            false, //Single animation lifetime
            "GhostLaserAttack", //Attack string
            CapsuleDirection2D.Horizontal, // Capsule direction
            true, // Partical Trail
            Color.green, // Particle Trail color
            0,
            false,
            false,
            false,
            0,
            false, //tangible
            0, //angle
            1, //num instances
            1, //mass
            1, //gravity scale
            0.3f //delay
            ));

        macAttacksDataSet[7].SetEnemyAttackValues(0, 0, 2, 5, 0, 1f, 1, 1);
        macAttacksDataSet[7].SetEnemyAttackId(2);

}

// Returns the attack attributes aligned with the suit provided
public AttackAttributes GetBaseAttackAttributes(EnemyAttacks peEnemyAttack)
{
return macAttacksDataSet[(int)peEnemyAttack - 1];
}

public AttackAttributes GetBaseAttackAttributes(int pnEnemyAttack)
{
return macAttacksDataSet[pnEnemyAttack];
}
}
