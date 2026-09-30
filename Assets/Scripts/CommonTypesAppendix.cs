using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deck
{

    /****************** ENUMS ************************/

    //Rank of Cards
    public enum CardSuit
    {
        eeSword,
        eeAxe,
        eeArcher,
        eeHeart,
        eeSpade,
        eeClover,
        eeDiamond,
        eeDrop,
        eeShield,
        eeMind,
        eeDecay,
        eeWind,
        eeTime,
        eeShock,
        eeStar,
        eeCardSuitEnd
    }

    //Attack types of Cards
    public enum CardAttackType
    {
        eePhysicalAttack,
        eeSpecialAttack
    }

    //Secondary effects of card suits
    public enum SuitEffect
    {
        eeCrit,
        eeBreak,
        eePierce,
        eeBurn,
        eeKnock,
        eeSap,
        eeFrost,
        eeSuitEffectEnd
    }

    //Card Rank values
    public enum CardRank
    {
        eeAce,
        eeTwo,
        eeThree,
        eeFour,
        eeFive,
        eeSix,
        eeSeven,
        eeEight,
        eeNine,
        eeTen,
        eeJack,
        eeQueen,
        eeKing,
        eeCardRankEnd
    }

    //Secondary Card Rank effects
    public enum RankEffect
    {
        eeNone,
        eeDoubleSuitEffect,
        eeDoubleSpecial,
        eeDoublePhysical
    }

    //Chamber Size enumerations
    public enum ChamberSize
    {
        eeDefault = 0,
        eeLong = 1,
        eeTall = 2,
        eeLarge = 3
    }

    //Chamber Exit enumerations
    public enum ChamberExits
    {
        eeBottom = 0,
        eeLeft = 1,
        eeMiddleLeft = 2,
        eeTopLeft = 3,
        eeTop = 4,
        eeTopRight = 5,
        eeMiddleRight = 6,
        eeRight = 7,
        eeBottomRight = 8,
        eeBottomLeft = 9,
        eeNone = 10
    }

    public enum AttackDirection
    {
        eeRightward = 0,
        eeLeftward = 1,
        eeDownwards = 2,
        eeUpwards = 3,
        eeBorderWards = 4
    }

    public enum PlayerAttackAnimation
    {
        eeBasic = 0,
        eeHeavy = 1, 
        eeRanged = 2,
        eeSlowRanged = 3,
        eeMagicBlast = 4,
        eeSlowMagicBlast = 5,
        eeArcSwipe = 6,
        eeSpread = 7
    }

    public enum AttackAnimationType
    {
        eeBasicSideAttack = 0,
        eeBasicDownAttack = 1,
        eeBasicUpAttack = 2,
        eeHeavySideAttack = 3,
        eeHeavyDownAttack = 4,
        eeHeavyUpAttack = 5,
        eeRangedSideAttack = 6,
        eeRangedDownAttack = 7,
        eeRangedUpAttack = 8,
        eeSlowRangedSideAttack = 9,
        eeSlowRangedDownAttack = 10,
        eeSlowRangedUpAttack = 11,
        eeMagicBlastSideAttack = 12,
        eeMagicBlastDownAttack = 13,
        eeMagicBlastUpAttack = 14,
        eeSlowMagicBlastSideAttack = 15,
        eeSlowMagicBlastDownAttack = 16,
        eeSlowMagicBlastUpAttack = 17,
        eeArcSwipeSideAttack = 18,
        eeArcSwipeDownAttack = 19,
        eeArcSwipeUpAttack = 20,
        eeSpreadAttack = 21
    }

    public enum EnemyID
    {
        eeNone = 0,
        eeChaseFreak = 1
    }

    public enum EnemyState
    {
        eeEnemyIdle = 0,
        eeEnemyAttack = 1,
        eeEnemyAttack2 = 2,
        eeEnemyAttack3 = 3,
        eeEnemyMove = 4,
        eeEnemyJump = 5,
        eeEnemyWait = 6,
        eeEnemyStagger= 7,
        eeEnemyKnockback = 8
    }

    public enum EnemyAttacks
    {
        eeNone = 0,
        eeChaseFreakAttack_1 = 1,
        eeBatBiteAttack = 2,
        eeChickFireballAttack = 3,
        eeMinotaurSlashAttack = 4,
        eeSpriteFireballAttack = 5,
        eeGhostSlashAttack = 6,
        eeChaseFreakAttack_7 = 7,

    }

    public enum AttackMovementType
    {
        eeNoMovement = 0,
        eeFixedDistance = 1,
        eeForceApplied = 2,
        eeFollowPlayer = 3
    }

    public enum CoinType
    {
        eeSmallCoin = 0,
        eeMediumCoin = 1,
        eeLargeCoin = 2
    }

    public enum TreasureType
    {
        eeCoinTreasure = 0,
        eeHealthTreasure = 1,
        eeCardTreasure = 2,
        eeSkillTreasure = 3
    }

    public enum SkillType
    {
        eeMovementSkill = 0,
        eeHandSkill = 1,
        eeEffectSkill = 2,
        eeRandom = 3
    }

    public enum SkillRarity
    {
        eeCommonSkill = 0,
        eeRareskill = 1,
        eeEpicSkill = 2,
        eeLegendarySkill = 3
    }

    public enum RewardType
    {
        eeDeckCardReward = 0,
        eeSkillCardReward = 1,
        eeMoneyReward = 2,
        eeKeyReward = 3
    }

    public enum HandState
    {
        eeHandEmpty = 0,
        eeHandPhase_1 = 1,
        eeHandPhase_2 = 2,
        eeHandPhase_3 = 3,
        eeHandPhase_4 = 4,
        eeHandPhase_5 = 5
    }
}
