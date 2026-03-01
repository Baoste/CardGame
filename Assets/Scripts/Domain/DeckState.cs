using System.Collections.Generic;

namespace Game.Domain
{
    public class DeckState
    {
        public Stack<int> instanceIdsInDeck = new Stack<int>();
    }

    public class SkillCardsDeckState : DeckState
    {
    }

    public class PointCardsDeckState : DeckState
    {
    }
}