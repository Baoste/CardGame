using System;

namespace Game.Domain
{
    public enum ValueSource
    {
        CardPointInPool,                // 候选区卡的点数
        CasterSkillCardsCount,          // 施法者技能牌数
        CasterPointCardsCount,          // 施法者点数牌数
        SourceSpecSelectedPointsSum,    // 源效果指定的牌的点数总和
        TargetSpecSelectedPointsSum,    // 目标效果指定的牌的点数总和
        ResolvedCardsPointsSum,         // 处理区的牌的点数总和
    }

    public enum BinaryOp
    {
        Add,
        Sub,
        Mul,
        Div
    }
}