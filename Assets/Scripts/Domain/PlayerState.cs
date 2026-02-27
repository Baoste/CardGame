using System.Collections.Generic;

namespace Game.Domain
{
    public class PlayerState
    {
        public int playerId;
        public int handCount;
        public List<int> SkillCardsInHand;
        public List<int> PointCardsOnBoard;
    }
}