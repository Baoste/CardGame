
namespace Game.Domain
{
    /*
     * PlayerState
     * - playerId：玩家ID
     * - pointCardCount：玩家当前的点数牌数量，包含底牌
     * - holeCard：玩家的底牌 InstanceID，如果没有底牌则为-1
     * - SkillCardsInHand：玩家手牌中技能牌的 InstanceID 列表
     * - PointCardsOnBoard：玩家场上点数牌的 InstanceID 列表，不包含底牌
     */
    public class PlayerState
    {
        public int playerId;
        public int actionPoint = 0;
        public int _holeCard = -1;
        public int chipCount = 6;
        public SkillCardsInHand skillCardsInHand = new SkillCardsInHand();
        public PointCardsOnBoard pointCardsOnBoard = new PointCardsOnBoard();

        public void Init()
        {
            actionPoint = 0;
            _holeCard = -1;
            skillCardsInHand._Clear();
            pointCardsOnBoard._Clear();
        }

        public bool Place1Bet()
        {
            if (chipCount <= 0)
                return false;
            chipCount -= 1;
            return true;
        }
    }
}