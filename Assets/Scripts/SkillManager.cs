using Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //Active Movement skills 
    List<Skill> macActiveMovementSkills = new();

    //Active Deck Skills
    List<Skill> macActiveHandSkills = new();

    //Active Deck Skills
    List<Skill> macActiveEffectSkills = new();

    //Set of all skills that allow player movement
    Dictionary<string, Skill> macMovementSkillDataSet = new();

    //Set of skills applied based on the cards in hand
     Dictionary<string, Skill> macHandSkillDataSet = new();

    //Set of skills applied based on the effect chances
    Dictionary<string, Skill> macEffectsSkillDataSet = new();

    //Player access
    PlayerMovement mcPlayerMovementAccess;

    void Start()
    {
        //Create list of Movement skills
        InitiallizeMovementSkills();

        //Create list of Deck skills
        InitiallizePlayerHandSkills();

        //Create list of Effect skills
        InitiallizePlayerEffectSkills();

        //Get access to player to allow movement application
        mcPlayerMovementAccess = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void InitiallizeMovementSkills()
    {

        //Double Jump
        string lcSkillName = "DoubleJump";
        Sprite lacMovementSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/MovementSkillCards/" + lcSkillName + "MovementSkillCard");
        Action<PlayerMovement, EnemyAI, AttackAttributes> llSkillAction = (Player, Enemy, Attack) => 
        {
            Player.EnableDoubleJump(true);
        };
        macMovementSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeMovementSkill, lacMovementSkillSprite));

        //Dodge roll
        lcSkillName = "DodgeRoll";
        lacMovementSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/MovementSkillCards/" + lcSkillName + "MovementSkillCard");
        llSkillAction = (Player, Enemy, Attack) =>
        {
            Player.EnableDodgeRoll(true);
        };
        macMovementSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeMovementSkill, lacMovementSkillSprite));

        //Wall Slide
        lcSkillName = "WallSlide";
        lacMovementSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/MovementSkillCards/" + lcSkillName + "MovementSkillCard");
        llSkillAction = (Player, Enemy, Attack) =>
        {
            Player.EnableWallSlide(true);
        };
        macMovementSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeMovementSkill, lacMovementSkillSprite));

        //Dash
        lcSkillName = "Dash";
        lacMovementSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/MovementSkillCards/" + lcSkillName + "MovementSkillCard");
        llSkillAction = (Player, Enemy, Attack) =>
        {
            Player.EnableDash(true);
        };
        macMovementSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeMovementSkill, lacMovementSkillSprite));
    }

    public void InitiallizePlayerHandSkills()
    {
        //Fire Fighter
        string lcSkillName = "Fire Fighter";
        Sprite lacHandSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/HandSkillCards/FireFighterHandSkillCard");
        Action<PlayerMovement, EnemyAI, AttackAttributes> llSkillAction = (pcPlayer, pcEnemy, pcAttack) =>
        {
            //Get set of suits in hand
            List<CardSuit> lacHandSuits = new List<CardSuit>();
            foreach(Card lcCard in pcPlayer.GetPlayerHand())
            {
                lacHandSuits.Add(lcCard.GetCardSuit());
            }

            //If hand contains heart and sword or Axe card 2X Damage multiplier
            if((lacHandSuits.Contains(CardSuit.eeSword) || lacHandSuits.Contains(CardSuit.eeAxe)) && lacHandSuits.Contains(CardSuit.eeHeart))
            {
                pcAttack.SetAttackMuliplier(2);
            }
        };
        macHandSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeHandSkill, lacHandSkillSprite));


        //One Handed
        lcSkillName = "One Handed";
        lacHandSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/HandSkillCards/OneHandedHandSkillCard");
        llSkillAction = (pcPlayer, pcEnemy, pcAttack) =>
        {

            //If hand is empty or has single card
            if (pcPlayer.GetPlayerHand().Count == 1 || pcPlayer.GetPlayerHand().Count == 0)
            {
                pcAttack.SetAttackMuliplier(3);
            }
        };
        macHandSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeHandSkill, lacHandSkillSprite));
    }

    public void InitiallizePlayerEffectSkills()
    {
        //Blade Master
        string lcSkillName = "Blade Master";
        Sprite lacEffectSkillSprite = Resources.Load<Sprite>("CardSprites/SkillCards/EffectSkillCards/BladeMasterEffectSkillCard");
        Action<PlayerMovement, EnemyAI, AttackAttributes> llSkillAction = (pcPlayer, pcEnemy, pcAttack) =>
        {
            pcAttack.AddEffectSkillAdjustments((int)SuitEffect.eeCrit, 0, 2);
        };
        macEffectsSkillDataSet.Add(lcSkillName, new Skill(lcSkillName, llSkillAction, SkillType.eeEffectSkill, lacEffectSkillSprite));
    }

    //Returns a specified or random Movement skill
    public Skill GetMovementSkill(string pnSkillName = "")
    {
        if (pnSkillName != "")
        {
            return macMovementSkillDataSet[pnSkillName];
        }
        else
        {
            return macMovementSkillDataSet.ElementAt(UnityEngine.Random.Range(0, macMovementSkillDataSet.Count)).Value;
        }
    }

    //Returns a specified or random Deck skill
    public Skill GetHandSkill(string pnSkillName = "")
    {
        if (pnSkillName != "")
        {
            return macHandSkillDataSet[pnSkillName];
        }
        else
        {
            return macHandSkillDataSet.ElementAt(UnityEngine.Random.Range(0, macHandSkillDataSet.Count)).Value;
        }
    }

    //Returns a specified or random Effect skill
    public Skill GetEffectSkill(string pnSkillName = "")
    {
        if (pnSkillName != "")
        {
            return macEffectsSkillDataSet[pnSkillName];
        }
        else
        {
            return macEffectsSkillDataSet.ElementAt(UnityEngine.Random.Range(0, macEffectsSkillDataSet.Count)).Value;
        }
    }

    //Gets a random skill from the set of skill types based on the provided skill type
    public Skill GetSkill(int pnSkillType)
    {
        if(pnSkillType == (int)SkillType.eeMovementSkill)
        {
            return GetMovementSkill();
        }
        else if(pnSkillType == (int)SkillType.eeHandSkill)
        {
            return GetHandSkill();
        }
        else if(pnSkillType == (int)SkillType.eeEffectSkill)
        {
            return GetEffectSkill();
        }
        //Recursive get random skill
        else
        {
            return GetSkill(UnityEngine.Random.Range(0, (int)SkillType.eeRandom ));
        }
    }

    public void EnableSkill(Skill pcSkill)
    {
        //If hand skill, add to list to check whenever hand updates
        if(pcSkill.GetSkillType() == SkillType.eeHandSkill)
        {
            Debug.Log("Added Hand Skill " + pcSkill.GetSkillName());
            macActiveHandSkills.Add(pcSkill);
        }
        //If Effect skill, add to list to check 
        else if (pcSkill.GetSkillType() == SkillType.eeEffectSkill)
        {
            Debug.Log("Added Effect Skill " + pcSkill.GetSkillName());
            //Add the skill to the list of active skills
            macActiveEffectSkills.Add(pcSkill);
        }
        //If movement skill apply immediatly 
        else if(pcSkill.GetSkillType() == SkillType.eeMovementSkill)
        {
            Debug.Log("Added Movement Skill " + pcSkill.GetSkillName());
            //Add the skill to the list of active skills
            macActiveMovementSkills.Add(pcSkill);

            pcSkill.SkillCheck(mcPlayerMovementAccess, null, null);
        }
    }

    public void EvaluateHandSkills(PlayerMovement pcPlayer, AttackAttributes pcAttackAttributes)
    {
        foreach(Skill lcHandSkill in macActiveHandSkills)
        {
            lcHandSkill.SkillCheck(pcPlayer, null, pcAttackAttributes);
        }
    }


    public void EvaluateEffectSkills(AttackAttributes pcAttackAttributes)
    {
        foreach (Skill lcEffectSkill in macActiveEffectSkills)
        {
            lcEffectSkill.SkillCheck(null, null, pcAttackAttributes);
        }
    }
}
