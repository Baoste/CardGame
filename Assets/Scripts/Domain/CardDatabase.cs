using System.Collections.Generic;

namespace Game.Domain
{
    public static class CardDatabase
    {
        private static Dictionary<int, Card> _cards;

        // TODO: load from json file
        public static void Init(List<Card> defs)
        {
            _cards = new Dictionary<int, Card>();

            foreach (var def in defs)
            {
                _cards[def.id] = def;
            }
        }

        public static Card Get(int cardId)
        {
            _cards.TryGetValue(cardId, out var card);
            return card;
        }
    }
}