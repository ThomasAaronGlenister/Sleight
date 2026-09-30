using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributesSet
{
    //Base attack creation attributes
    List<EnemyAttributes> macEnemyDataSet = new();

    EnemyAttackAttributesSet mcEnemyAttackDataSet;

    //Constructor
    public EnemyAttributesSet(EnemyAttackAttributesSet pcEnemyAttackDataSet)
    {
        mcEnemyAttackDataSet = pcEnemyAttackDataSet;
        GenerateBaseEnemyAttributeSet();
    }

    //TODO Load from file
    public void GenerateBaseEnemyAttributeSet()
    {
        //Knight 0
        macEnemyDataSet.Add(new EnemyAttributes(
            "Knight",
            40, // HealthPoints
            20, //   ArmorPoints
            4.5f, //   MovementSpeed
            13, //   JumpPower
            1, //   Mass
            2, //   GravityScale
            false, //   Flying
            7, //   Follow Distance
            2, //   Attack Distance
            0.5f, //   Attack Cooldown
            0.4f, // Lunge Y addition
            5, //   Lunge Multiplier
            2, //    Waypoint distance
            5, //    Attack Damage
            0f, //     Attack Windup
            1.2f,  //Size Multiplier
            0.36f, // HitBox X
            0.77f, // HitBox Y
            -0.07f, //HitBox Offset X
            -0.11f //Hitbox Offset Y
            ));

        macEnemyDataSet[0].AddEnemyAttack(mcEnemyAttackDataSet.GetBaseAttackAttributes(0));
        macEnemyDataSet[0].AddEnemyAttack(mcEnemyAttackDataSet.GetBaseAttackAttributes(1));

        //Bat 1
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
            5, //    Attack Damage
            0f //     Attack Windup
            ));

        macEnemyDataSet[1].AddEnemyAttack(mcEnemyAttackDataSet.GetBaseAttackAttributes(2));

        //Chick 2
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
            5, //    Attack Damage
            0.5f, //     Attack Windup
            0.8f, // Size Multiplier
            0.5f, // HitBox X
            0.8f, // HitBox Y
            -0.08f, //HitBox Offset X
            -0.08f //Hitbox Offset Y
            ));

        //Minotaur 3
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
            10, //    Attack Damage
            0f, //     Attack Windup
            3f, // Size multiplier
            0.32f, // HitBox X
            0.42f, // HitBox Y
            -0.06f, //HitBox Offset X
            -0.04f //Hitbox Offset Y
            ));

        //Sprite 4
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

        //Ghost 5
        macEnemyDataSet.Add(new EnemyAttributes(
            "Ghost",
            70, // HealthPoints
            0, //   ArmorPoints
            2f, //   MovementSpeed
            6, //   JumpPower
            1, //   Mass
            0, //   GravityScale
            true, //   Flying
            10, //   Follow Distance
            6, //   Attack Distance
            2f, //   Attack Cooldown
            0f, // Lunge Y addition
            3, //   Lunge Multiplier
            0.5f, //    Waypoint distance
            6, //    Attack Damage
            0f, //     Attack Windup
            2f, //Size Multiplier
            0.22f, //Box Collider Width
            0.34f, //Box Collider Height
            -0.04f, //Box Collider Offset X
            0.07f, //Box Collider Offset Y
            0, //Target Position X offset
            0f, // Target Position Y Offset
            false // Movement is Forces
            ));

        macEnemyDataSet[5].AddEnemyAttack(mcEnemyAttackDataSet.GetBaseAttackAttributes(0));

        //Fire Boss 6
        macEnemyDataSet.Add(new EnemyAttributes(
            "FireBoss",
            200, // HealthPoints
            0, //   ArmorPoints
            2.5f, //   MovementSpeed
            0, //   JumpPower
            1, //   Mass
            3, //   GravityScale
            false, //   Flying
            7, //   Follow Distance
            2, //   Attack Distance
            1, //   Attack Cooldown
            0f, // Lunge Y addition
            0.5f, //   Lunge Multiplier
            3, //    Waypoint distance
            20, //    Attack Damage
            0f, //     Attack Windup
            1f, // Size multiplier
            0.666f, // HitBox X
            1.16f, // HitBox Y
            0.4f, //HitBox Offset X
            -0.3f //Hitbox Offset Y
            ));
    }

    public EnemyAttributes GetBaseEnemyAttributes(int pnEnemyId)
    {
        return macEnemyDataSet[pnEnemyId];
    }
}
