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
            0.2f //     Attack Windup
            ));

        macEnemyDataSet[0].SetEnemyAnimations("Animations/EnemyAnimations/FreakAnimations");

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

        macEnemyDataSet[1].SetEnemyAnimations("Animations/EnemyAnimations/BatAnimations");

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

        macEnemyDataSet[2].SetEnemyAnimations("Animations/EnemyAnimations/ChickAnimations");

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

        macEnemyDataSet[3].SetEnemyAnimations("Animations/EnemyAnimations/MinotaurAnimations");
    }

    public EnemyAttributes GetBaseEnemyAttributes(int pnEnemyId)
    {
        return macEnemyDataSet[pnEnemyId];
    }
}
