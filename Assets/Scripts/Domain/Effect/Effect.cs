using System;

namespace Game.Domain
{
    public enum EffectType
    {
        DrawPoint,     // 抽点数牌
        DrawSkill,     // 抽技能牌
        Discard,       // 弃牌
        ModifyPoint,   // 改变目标牌点数
        Move,          // 移动目标牌
    }

    /*
     * EffectOp
     * - type：效果类型，例如抽牌、改变点数等
     * - source：来源选择规范，指定这个效果的来源，例如技能牌、点数牌等
     * - target：目标选择规范，指定这个效果作用于哪些牌
     * - value：数值表达式，指定这个效果的数值，例如抽几张牌、点数增加多少等
     */
    public class EffectOp
    {
        public EffectType type;
        public ParticipantSpec source;
        public ParticipantSpec target;
        public ValueExpr value;
    }
}