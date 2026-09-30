using Deck;
using System;
using UnityEngine;

public class Skill
{
	//Name of the skill
	private string mcSkillName;

	//If this skill is active
	private bool mbActive = false;

	//How Rare/powerful this skill is
	private SkillRarity meRarity;

	//Delegate Action that this skill provides
	private readonly Action<PlayerMovement, EnemyAI, AttackAttributes> mlSkillAction;

    //The Sprite of the skill Card
    private Sprite mcSkillSprite;

	private SkillType meSkillType; 

    //Sets name and functionality of the Skill
	/**
	 * NAME,
	 * ACTION,
	 * TYPE,
	 * SPRITE,
	 * DESCRIPTION,
	 * RARITY
	 */
    public Skill(string lcSkillName,
        Action<PlayerMovement, EnemyAI, AttackAttributes> plSkillAction, 
		SkillType peSkillType = SkillType.eeMovementSkill,
		Sprite pcSkillSprite = null, 
		string pcSkillDescription = "", 
		SkillRarity peRarity = SkillRarity.eeCommonSkill)
	{
		mcSkillName = lcSkillName;
        mlSkillAction = plSkillAction;
        meSkillType = peSkillType;
        mcSkillSprite = pcSkillSprite;
        meRarity = peRarity;
    }

	public void SkillCheck(PlayerMovement lcPlayer, EnemyAI lcEnemy, AttackAttributes lcAttack)
	{
		//Engage Skill
		mlSkillAction(lcPlayer, lcEnemy, lcAttack);
    }

    public void SetSkillActive(bool lbActive)
    {
        mbActive = lbActive;
    }

	//Gets the string name of the skill
	public string GetSkillName()
	{
		return mcSkillName;
	}

	//Returns the sprite of the skill card applied
	public Sprite GetSkillCardSprite()
	{
		return mcSkillSprite;
	}

	//Returns the type of this skill
	public SkillType GetSkillType()
	{
		return meSkillType;
	}
}
