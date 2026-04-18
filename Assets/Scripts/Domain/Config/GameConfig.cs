
namespace Game.Domain
{
    public class GameConfig
    {
        public EndTurnConfig EndTurn;
        public StartTurnConfig StartTurn;
        public StartGameConfig StartGame;
    }

    public class EndTurnConfig
    {
        public int startTurn;
        public float initialProbability;
        public float growthRate;
    }

    public class StartTurnConfig
    {
        public int startAP;
    }

    public class StartGameConfig
    {
        public int startSkillCardCount;
        public int maxSkillCardCount;
    }
}
