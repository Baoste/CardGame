using System;
using System.Collections.Generic;

namespace Game.Domain
{
    /*
     * PlayerState
     * - playerId：玩家ID
     * - pointCardCount：玩家当前的点数牌数量，包含底牌
     * - holeCard：玩家的底牌 InstanceID，如果没有底牌则为-1
     * - SkillCardsInHand：玩家手牌中技能牌的 InstanceID 列表
     * - PointCardsOnBoard：玩家场上点数牌的 InstanceID 列表，不包含底牌
     */
    [Serializable]
    public class PlayerState
    {
        public int playerId;
        public int holeCard = -1;
        public List<int> SkillCardsInHand = new List<int>();
        public List<int> PointCardsOnBoard = new List<int>();
    }
}