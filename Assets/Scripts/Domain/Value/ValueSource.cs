using System;

namespace Game.Domain
{
    [Serializable]
    public enum ValueSource
    {
        CasterHandCount,                // 技能牌数
        TargetPoints,                   // 目标牌点数
        //PointsOfCardsDrawnThisTurn,     // 抽到的牌的点数
    }

    [Serializable]
    public enum BinaryOp
    {
        Add,
        Sub,
        Mul,
        Div
    }
}