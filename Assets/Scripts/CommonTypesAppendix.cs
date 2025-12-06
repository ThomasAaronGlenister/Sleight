using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deck
{
    /****************** STRUCTS ************************/

    //Struct Containing values applied to the attacker when an attack is initialized
    public struct AttackForces
    {
        public AttackForces(float pfPogoForce = 0, float pfStrikeForceX = 0, float pfStrikeForceY = 0)
        {
            PogoForce = pfPogoForce;
            StrikeForceX = pfStrikeForceX;
            StrikeForceY = pfStrikeForceY;
        }

        public float PogoForce { get; }

        //Force applied to the Attacker on the X axis on Hit
        public float StrikeForceX { get; }

        //Force applied to the Attacker on the Y axis on Hit
        public float StrikeForceY { get; }
    }

    /****************** GLOBAL VARIABLES ************************/


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
        eeSpread = 3
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
        eeSpreadAttack = 9
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
        eeEnemyMove = 2,
        eeEnemyJump = 3,
        eeEnemyWindup = 4,
        eeEnemyStagger= 5,
        eeEnemyKnockback = 6
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
}
