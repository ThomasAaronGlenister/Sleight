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
            100, // HealthPoints
            0, //   ArmorPoints
            2, //   MovementSpeed
            4, //   JumpPower
            1, //   Mass
            3, //   GravityScale
            false, //   Flying
            7, //   Follow Distance
            2, //   Attack Distance
            1, //   Attack Cooldown
            0.3f, // Lunge Y addition
            4, //   Lunge Multiplier
            3, //    Waypoint distance
            Deck.EnemyAttacks.eeChaseFreakAttack_1, //   Attack
            5, //    Attack Damage
            0.3f //     Attack Windup
            ));

        macEnemyDataSet[0].SetEnemyAnimations("FreakAttack", "FreakDamage", "FreakWalk", "FreakWindup", "FreakJump");
    }

    public EnemyAttributes GetBaseEnemyAttributes(int pnEnemyId)
    {
        return macEnemyDataSet[pnEnemyId];
    }
}
