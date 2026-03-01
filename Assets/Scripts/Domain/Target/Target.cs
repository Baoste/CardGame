using System;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public enum TargetType
    {
        None                        = 0,
        MySkillCardsInHand          = 1 << 0,   // 自己的手牌
        OpponentSkillCardsInHand    = 1 << 1,   // 对手的手牌
        MyPointCardsOnBoard         = 1 << 2,   // 自己场上的牌，不包括底牌
        OpponentPointCardsOnBoard   = 1 << 3,   // 对手场上的牌，不包括底牌
        SkillCardsInDeck            = 1 << 4,   // 牌堆的技能牌
        PointCardsInDeck            = 1 << 5,   // 牌堆的点数牌
    }

    [Serializable]
    public enum TargetSelectionMode
    {
        All,
        Choose,
        First,
        Last,
        Random,
    }

    [Serializable]
    public class TargetSpec
    {
        public TargetType targetType;
        [SerializeReference] public ConditionExpr filter;        // 例如对手牌进行过滤：只选“攻击牌”、只选“点数>=5”

        public TargetSelectionMode targetSelectionMode;
        [SerializeReference] public ValueExpr maxTargetCount;    // 候选最大数量，只有在Random情况下起作用
        [SerializeReference] public ValueExpr maxPick;           // 选择的最大数量
    }
}