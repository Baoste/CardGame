using System;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public enum ParticipantType
    {
        None                        = 0,        // 无目标，表示这个效果没有目标，例如抽牌
        MySkillCardsInHand          = 1 << 0,   // 自己的手牌
        OpponentSkillCardsInHand    = 1 << 1,   // 对手的手牌
        MyPointCardsOnBoard         = 1 << 2,   // 自己场上的牌，不包括底牌
        OpponentPointCardsOnBoard   = 1 << 3,   // 对手场上的牌，不包括底牌
        SkillCardsInDeck            = 1 << 4,   // 牌堆的技能牌
        PointCardsInDeck            = 1 << 5,   // 牌堆的点数牌
    }

    [Serializable]
    public enum ParticipantSelectionMode
    {
        None,
        All,
        Choose,
        First,
        Last,
        Random,
    }

    [Serializable]
    public class ParticipantSpec
    {
        public ParticipantType participantType;
        [SerializeReference] public ConditionExpr filter;        // 例如对手牌进行过滤：只选“攻击牌”、只选“点数>=5”

        public ParticipantSelectionMode participantSelectionMode;
        [SerializeReference] public ValueExpr maxCandidateCount;    // 候选最大数量，只有在Random情况下起作用
        [SerializeReference] public ValueExpr maxSelectCount;       // 选择的最大数量
    }
}