using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public class DeckState
    {
        public List<int> _instanceIdsInDeck = new List<int>();
        public IReadOnlyList<int> instanceIdsInDeck => _instanceIdsInDeck;

        public int Draw()
        {
            if (instanceIdsInDeck.Count == 0)
                return -1;

            int lastIndex = instanceIdsInDeck.Count - 1;
            int value = instanceIdsInDeck[lastIndex];
            _instanceIdsInDeck.RemoveAt(lastIndex);
            return value;
        }

        public void Add(int id)
        {
            _instanceIdsInDeck.Add(id);
        }

        public void Add(List<int> src)
        {
            foreach (int id in src)
                _instanceIdsInDeck.Add(id);
        }

        public void Clear()
        {
            _instanceIdsInDeck.Clear();
        }

        public void Shuffle(Random random)
        {
            for (int i = instanceIdsInDeck.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);  // 0 ~ i
                (_instanceIdsInDeck[i], _instanceIdsInDeck[j]) = (_instanceIdsInDeck[j], _instanceIdsInDeck[i]);  // ½»»»
            }
        }
    }

    [Serializable]
    public class SkillCardsDeckState : DeckState
    {
    }

    [Serializable]
    public class PointCardsDeckState : DeckState
    {
    }
}