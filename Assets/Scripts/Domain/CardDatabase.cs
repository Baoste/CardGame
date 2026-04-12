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
        private static List<PointCard> _pointCards = new List<PointCard>();
        private static List<SkillCard> _skillCards = new List<SkillCard>();

        // TODO: load from json file
        public static void Init(string fileName, CardDatabaseType type)
        {
            switch (type)
            {
                case CardDatabaseType.PointCard:
                { 
                    List<PointCard> defs = CardJsonUtility.LoadCardsFromFile<PointCard>(fileName);
                    foreach (var def in defs)
                    {
                        _cards[def.id] = def;
                        _pointCards.Add(def);
                    }
                    break;
                }
                case CardDatabaseType.SkillCard:
                {
                    List<SkillCard> defs = CardJsonUtility.LoadCardsFromFile<SkillCard>(fileName);
                    foreach (var def in defs)
                    {
                        _cards[def.id] = def;
                        _skillCards.Add(def);
                    }
                    break;
                }
            }
        }

        public static void InitFromString(string pointCardDeckJson, string skillCardDeckJson)
        {
            _cards.Clear();
            _pointCards.Clear();
            _skillCards.Clear();

            List<PointCard> pdefs = CardJsonUtility.LoadCardsFromJson<PointCard>(pointCardDeckJson);
            foreach (var def in pdefs)
            {
                _cards[def.id] = def;
                _pointCards.Add(def);
            }
            List<SkillCard> sdefs = CardJsonUtility.LoadCardsFromJson<SkillCard>(skillCardDeckJson);
            foreach (var def in sdefs)
            {
                _cards[def.id] = def;
                _skillCards.Add(def);
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

        public static void GetValues(out List<PointCard> pointCards, out List<SkillCard> skillCards)
        {
            pointCards = _pointCards;
            skillCards = _skillCards;
        }
    }
}