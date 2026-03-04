using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class CardZone
    {
        public List<int> _instanceIds = new List<int>();
        public IReadOnlyList<int> instanceIds => _instanceIds;

        // unsafe: 请从 GameState 的 AddCardToBoard 方法添加牌到牌堆中，以保证 cardLocationMap 的正确性
        public void _Add(int id)
        {
            if (id == -1) return;
            _instanceIds.Add(id);
        }

        public void _Add(List<int> src)
        {
            foreach (int id in src)
                _instanceIds.Add(id);
        }

        // unsafe: 请从 GameState 的 RemoveCardFromBoard 方法删牌，以保证 cardLocationMap 的正确性
        public void _Remove(int id)
        {
            _instanceIds.Remove(id);
        }

        public void _Clear()
        {
            _instanceIds.Clear();
        }

        public void Shuffle(Random random)
        {
            for (int i = instanceIds.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);  // 0 ~ i
                (_instanceIds[i], _instanceIds[j]) = (_instanceIds[j], _instanceIds[i]);  // 交换
            }
        }

        public int GetCount()
        {
            return _instanceIds.Count;
        }
    }

    public class SkillCardsDeck : CardZone
    {
        public int Draw()
        {
            if (instanceIds.Count == 0)
                return -1;

            int lastIndex = instanceIds.Count - 1;
            int value = instanceIds[lastIndex];
            _instanceIds.RemoveAt(lastIndex);
            return value;
        }
    }

    public class PointCardsDeck : CardZone
    {
        public int Draw()
        {
            if (instanceIds.Count == 0)
                return -1;

            int lastIndex = instanceIds.Count - 1;
            int value = instanceIds[lastIndex];
            _instanceIds.RemoveAt(lastIndex);
            return value;
        }
    }

    public class SkillCardsInHand : CardZone
    {
    }

    public class PointCardsOnBoard : CardZone
    {
    }

    public class DiscardPile : CardZone
    {
    }

    // 正在结算的牌堆，包含正在结算的技能牌和点数牌
    public class CardsToResolve : CardZone
    {
    }
}