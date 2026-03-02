using System.Collections.Generic;

namespace Game.Domain
{
    public enum CardDatabaseType
    {
        PointCard,
        SkillCard
    }

    public static class CardDatabase
    {
        private static Dictionary<int, Card> _cards = new Dictionary<int, Card>();

        // TODO: load from json file
        public static void Init(string fileName, CardDatabaseType type)
        {
            switch (type)
            {
                case CardDatabaseType.PointCard:
                { 
                    List<PointCard> defs = CardJsonUtility.LoadCardsFromJson<PointCard>(fileName);
                    foreach (var def in defs)
                    {
                        _cards[def.id] = def;
                    }
                    break;
                }
                case CardDatabaseType.SkillCard:
                {
                    List<SkillCard> defs = CardJsonUtility.LoadCardsFromJson<SkillCard>(fileName);
                    foreach (var def in defs)
                    {
                        _cards[def.id] = def;
                    }
                    break;
                }
            }
        }

        public static Card Get(int cardId)
        {
            _cards.TryGetValue(cardId, out var card);
            return card;
        }

        public static List<int> GetKeys()
        {
            return new List<int>(_cards.Keys);
        }
    }
}