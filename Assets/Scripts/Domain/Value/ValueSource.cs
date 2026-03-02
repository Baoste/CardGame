using System;

namespace Game.Domain
{
    public enum ValueSource
    {
        CasterSkillCardsCount,          // 施法者技能牌数
        CasterPointCardsCount,          // 施法者点数牌数
        TargetPoints,                   // 目标牌点数
        //PointsOfCardsDrawnThisTurn,     // 抽到的牌的点数
    }

    public enum BinaryOp
    {
        Add,
        Sub,
        Mul,
        Div
    }
}