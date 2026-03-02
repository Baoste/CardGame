using System;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        public CardBoard SkillCardsInHand = new CardBoard();
        public CardBoard PointCardsOnBoard = new CardBoard();
    }

    // 卡牌列表，指在玩家手牌或场上存在的卡牌，包含技能牌和点数牌
    [Serializable]
    public class CardBoard
    {
        public List<int> _cardInstanceIds = new List<int>();
        public IReadOnlyList<int> cardInstanceIds => _cardInstanceIds;

        public void Add(int id)
        {
            if (id == -1)   return;
            _cardInstanceIds.Add(id);
        }

        public int GetCount()
        {
            return _cardInstanceIds.Count;
        }
    }
}