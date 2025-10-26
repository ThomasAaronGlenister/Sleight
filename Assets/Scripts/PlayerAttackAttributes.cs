using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAttributes
{

    //Base attack creation attributes
    List<AttackAttributes> macAttacksDataSet = new();

    // Start is called before the first frame update
    public PlayerAttackAttributes()
    {
        GenerateBaseAttackAttributeSet();
    }

    private void GenerateBaseAttackAttributeSet()
    {
        //Horizontal Offset, Vertical Offset, Size Multiplier, Is Disjointed, Travel Distance, Travel Time, Attack Animation String

        //Sword Attack 
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.4f, 0.5f, 2f, false, false, 0, 0, true, (int)PlayerAttackAnimation.eeBasic, true, "SwordAttackRedux"));

        //Axe Attack
        macAttacksDataSet.Add(new AttackAttributes(0f, 0.65f, 0.2f, 0.1f, 1.5f, false, false, 0, 0f, false, (int)PlayerAttackAnimation.eeHeavy, true, "AxeAttack"));

        //Arrow Attack
        macAttacksDataSet.Add(new AttackAttributes(0, -0.07f, 0.5f, 0.5f, 1.5f, true, false, 8, 0.5f, false, (int)PlayerAttackAnimation.eeRanged, false, "ArrowAttack"));

        //Heart Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, (int)PlayerAttackAnimation.eeBasic, false, "HeartAttack"));

        //Spade Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, (int)PlayerAttackAnimation.eeBasic, false, "HeartAttack"));

        //Clover Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, (int)PlayerAttackAnimation.eeBasic, false, "HeartAttack"));

        //Diamond Attack
        macAttacksDataSet.Add(new AttackAttributes(0, 0, 0.5f, 0.5f, 1f, true, false, 5, 0.5f, false, (int)PlayerAttackAnimation.eeBasic, false, "HeartAttack"));
    }

    // Returns the attack attributes aligned with the suit provided
    public AttackAttributes GetBaseAttackAttributes(CardSuit peSuit)
    {
        return macAttacksDataSet[(int)peSuit];
    }
}
