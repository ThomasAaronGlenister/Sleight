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
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

        //Axe Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

        //Arrow Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

        //Heart Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

        //Spade Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

        //Clover Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2.5f, false, false, 0, 0, false, 0, true, "ChaseFreakAttack_1", CapsuleDirection2D.Vertical));

    }

    // Returns the attack attributes aligned with the suit provided
    public AttackAttributes GetBaseAttackAttributes(EnemyAttacks peEnemyAttack)
    {
        return macAttacksDataSet[(int)peEnemyAttack];
    }
}
