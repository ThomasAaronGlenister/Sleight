using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributesLoader
{
    //Base attack creation attributes
    List<EnemyAttributes> macEnemyDataSet = new();

    //Constructor
    public EnemyAttributesLoader()
    {
        GenerateBaseEnemyAttributeSet();
    }

    //TODO Load from file
    private void GenerateBaseEnemyAttributeSet()
    {
        //Chase Freak
        macEnemyDataSet.Add(new EnemyAttributes(
            "Freak",
            5, // HealthPoints
            0, //   ArmorPoints
            3, //   MovementSpeed
            6, //   JumpPower
            1, //   Mass
            3, //   GravityScale
            false, //   Flying
            7, //   Follow Distance
            2, //   Attack Distance
            1, //   Attack Cooldown
            0.4f, // Lunge Y addition
            5, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeChaseFreakAttack_1, //   Attack
            5, //    Attack Damage
            0.2f, //     Attack Windup
            1,  //Size Multiplier
            0.3f, // HitBox X
            0.6f, // HitBox Y
            0.02f, //HitBox Offset X
            -0.03f //Hitbox Offset Y
            ));

        //Bat
        macEnemyDataSet.Add(new EnemyAttributes(
            "Bat",
            50, // HealthPoints
            0, //   ArmorPoints
            3, //   MovementSpeed
            6, //   JumpPower
            1, //   Mass
            0, //   GravityScale
            true, //   Flying
            7, //   Follow Distance
            3, //   Attack Distance
            1, //   Attack Cooldown
            0f, // Lunge Y addition
            4, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeBatBiteAttack, //   Attack index into attributes
            5, //    Attack Damage
            0.5f //     Attack Windup
            ));

        //Chick
        macEnemyDataSet.Add(new EnemyAttributes(
            "Chick",
            50, // HealthPoints
            0, //   ArmorPoints
            3, //   MovementSpeed
            5, //   JumpPower
            1, //   Mass
            3, //   GravityScale
            false, //   Flying
            5, //   Follow Distance
            3, //   Attack Distance
            1, //   Attack Cooldown
            0f, // Lunge Y addition
            10, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeChickFireballAttack, //   Attack
            5, //    Attack Damage
            0.5f, //     Attack Windup
            0.8f, // Size Multiplier
            0.5f, // HitBox X
            0.8f, // HitBox Y
            -0.08f, //HitBox Offset X
            -0.08f //Hitbox Offset Y
            ));

        //Minotaur
        macEnemyDataSet.Add(new EnemyAttributes(
            "Minotaur",
            100, // HealthPoints
            0, //   ArmorPoints
            2, //   MovementSpeed
            0, //   JumpPower
            1, //   Mass
            3, //   GravityScale
            false, //   Flying
            7, //   Follow Distance
            3, //   Attack Distance
            1, //   Attack Cooldown
            0f, // Lunge Y addition
            0, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeMinotaurSlashAttack, //   Attack
            10, //    Attack Damage
            0f, //     Attack Windup
            3f, // Size multiplier
            0.32f, // HitBox X
            0.42f, // HitBox Y
            -0.06f, //HitBox Offset X
            -0.04f //Hitbox Offset Y
            ));

        //Sprite
        macEnemyDataSet.Add(new EnemyAttributes(
            "Sprite",
            50, // HealthPoints
            0, //   ArmorPoints
            2, //   MovementSpeed
            6, //   JumpPower
            1, //   Mass
            0, //   GravityScale
            true, //   Flying
            7, //   Follow Distance
            4, //   Attack Distance
            1, //   Attack Cooldown
            0f, // Lunge Y addition
            4, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeSpriteFireballAttack, //   Attack index into attributes
            7, //    Attack Damage
            0.5f, //     Attack Windup
            1, // Size Multiplier
            0.32f, //Box Collider Width
            0.6f, //Box Collider Height
            0.08f, //Box Collider Offset X
            -0.14f, //Box Collider Offset Y
            0, //Target Position X offset
            1f, // Target Position Y Offset
            true // Movement is Forces
            ));

        //Ghost
        macEnemyDataSet.Add(new EnemyAttributes(
            "Ghost",
            70, // HealthPoints
            0, //   ArmorPoints
            2, //   MovementSpeed
            6, //   JumpPower
            1, //   Mass
            0, //   GravityScale
            true, //   Flying
            7, //   Follow Distance
            3, //   Attack Distance
            1.5f, //   Attack Cooldown
            0f, // Lunge Y addition
            3, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeGhostSlashAttack, //   Attack index into attributes
            6, //    Attack Damage
            0f, //     Attack Windup
            1.7f, //Size Multiplier
            0.32f, //Box Collider Width
            0.6f, //Box Collider Height
            0.08f, //Box Collider Offset X
            -0.14f, //Box Collider Offset Y
            0, //Target Position X offset
            1f, // Target Position Y Offset
            false // Movement is Forces
            ));
    }

    public EnemyAttributes GetBaseEnemyAttributes(int pnEnemyId)
    {
        return macEnemyDataSet[pnEnemyId];
    }
}
