using System.Collections.Generic;

namespace Game.Domain
{
    public class DeckState
    {
        public Stack<int> cardIdsInDeck = new Stack<int>();
    }

    public class SkillDeckState : DeckState
    {
    }

    public class PointCardsDeck : DeckState
    {
    }
}