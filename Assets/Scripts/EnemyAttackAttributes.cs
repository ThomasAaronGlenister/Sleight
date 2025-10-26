using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAttributes
{
    //Base attack creation attributes
    List<AttackAttributes> macAttacksDataSet = new();

    // Start is called before the first frame update
    public EnemyAttackAttributes()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {
        //Horizontal Offset, Vertical Offset, Size Multiplier, Is Disjointed, Travel Distance, Travel Time, Attack Animation String

        //Chase Freak Slash Attack 
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, (int)PlayerAttackAnimation.eeBasic, false, "HeartAttack"));

        //Axe Attack
        macAttacksDataSet.Add(new AttackAttributes(0f, 0.65f, 0.2f, 0.1f, 1.5f, false, false, 0, 0f, false, 0, true, "SwordAttackRedux"));

        //Arrow Attack
        macAttacksDataSet.Add(new AttackAttributes(0, -0.07f, 0.5f, 0.5f, 1.5f, true, false, 8, 0.5f, false, 0, false, "SwordAttackRedux"));

        //Heart Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, 0, false, "SwordAttackRedux"));

        //Spade Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, 0, false, "SwordAttackRedux"));

        //Clover Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, 0, false, "SwordAttackRedux"));

    }

    // Returns the attack attributes aligned with the suit provided
    public AttackAttributes GetBaseAttackAttributes(EnemyAttacks peEnemyAttack)
    {
        return macAttacksDataSet[(int)peEnemyAttack];
    }
}
