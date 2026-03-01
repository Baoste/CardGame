using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public class DeckState
    {
        public Stack<int> instanceIdsInDeck = new Stack<int>();
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