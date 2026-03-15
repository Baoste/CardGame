using System;

namespace Game.Domain
{
    // TODO: 需要改成ZoneType，表示牌所在的区域，例如手牌、场上、牌堆等
    public enum ParticipantType
    {
        None                        = 0,        // 无，保留
        MySkillCardsInHand          = 1 << 0,   // 自己的手牌
        OpponentSkillCardsInHand    = 1 << 1,   // 对手的手牌
        MyPointCardsOnBoard         = 1 << 2,   // 自己场上的牌，不包括底牌
        OpponentPointCardsOnBoard   = 1 << 3,   // 对手场上的牌，不包括底牌
        SkillCardsInDeck            = 1 << 4,   // 牌堆的技能牌
        PointCardsInDeck            = 1 << 5,   // 牌堆的点数牌
        CardsToResolve              = 1 << 6,   // 要继续解决的牌，例如某些牌需要多次选择目标或多次触发效果
        MyBoardZone                 = 1 << 7,   // 自己的点数牌区域
        OppentBoardZone             = 1 << 8,   // 对方的点数牌区域
    }

    //public enum ParticipantSelectionMode
    //{
    //    None,
    //    All,
    //    Choose,
    //    First,
    //    Last,
    //    Random,
    //}

    public class ParticipantSpec
    {
        public ParticipantType participantType;
        public ConditionExpr filter;        // 例如对手牌进行过滤：只选“攻击牌”、只选“点数>=5”

        public ParticipantSelectionMode participantSelectionMode;
        public ValueExpr maxSelectCount;       // 选择的最大数量
    }
}