using System.Collections.Generic;

namespace Game.Domain
{
    public class PlayerState
    {
        public int playerId;
        public int handCount;
        public List<int> SkillCardsInHand = new List<int>();
        public List<int> PointCardsOnBoard = new List<int>();
    }
}