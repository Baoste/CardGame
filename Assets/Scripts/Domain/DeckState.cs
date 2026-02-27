using System.Collections.Generic;

namespace Game.Domain
{
    public class DeckState
    {
        public List<int> cardIdsInDeck;
    }

    public class SkillDeckState : DeckState
    {
    }

    public class PointCardsDeck : DeckState
    {
    }
}