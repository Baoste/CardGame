using System.Collections.Generic;

namespace Game.Domain
{
    public class PlayerState
    {
        public int playerId;
        public int handCount;
        public List<int> SkillCardsInHand = new();
        public List<int> PointCardsOnBoard = new();
    }
}