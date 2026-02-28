using System;

namespace Game.Domain
{
    [Serializable]
    public enum TargetType
    {
        None                = 0,
        CardsInHand         = 1 << 0,   // 手牌
        CardsOnBoard        = 1 << 1,   // 场上的牌
        SkillCardsInDeck    = 1 << 2,   // 牌堆的技能牌
        PointCardsInDeck    = 1 << 3,   // 牌堆的点数牌
    }

    [Serializable]
    public enum TargetSelectionMode
    {
        All,
        Random,
        Choose
    }

    [Serializable]
    public class TargetSpec
    {
        public TargetType targetType;
        public TargetSelectionMode targetSelectionMode;

        public ConditionExpr filter;    // 例如对手牌进行过滤：只选“攻击牌”、只选“点数>=5”
        public int count;               // 候选最大数量
        public int maxPick;             // 选择时最大数量
    }
}