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
        eeSap,
        eeFrost,
        eeKnock
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
        eeUpwards = 3
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

    public enum EnemyAttacks
    {
        eeChaseFreakAttack_1 = 0,
        eeChaseFreakAttack_2 = 1,
        eeChaseFreakAttack_3 = 2,
        eeChaseFreakAttack_4 = 3,
        eeChaseFreakAttack_5 = 4,
        eeChaseFreakAttack_6 = 5,
        eeChaseFreakAttack_7 = 6,

    }
}
