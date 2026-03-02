using System;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public enum EffectType
    {
        DrawCards,          // 抽牌
        DiscardCards,       // 弃牌
        ModifyCardPoints,   // 改变目标牌点数
        MoveCards,          // 移动目标牌
    }

    /*
     * EffectOp
     * - type：效果类型，例如抽牌、改变点数等
     * - source：来源选择规范，指定这个效果的来源，例如技能牌、点数牌等
     * - target：目标选择规范，指定这个效果作用于哪些牌
     * - value：数值表达式，指定这个效果的数值，例如抽几张牌、点数增加多少等
     */
    [Serializable]
    public class EffectOp
    {
        public EffectType type;
        [SerializeReference] public ParticipantSpec source;
        [SerializeReference] public ParticipantSpec target;
        [SerializeReference] public ValueExpr value;
    }
}