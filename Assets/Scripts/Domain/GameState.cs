using System;

namespace Game.Domain
{
    /*
     * GameState
     * 需要包含
     * - Turn：当前是第几轮
     * - CurentPlayerID：当前是谁的回合
     * - RandomSeed：游戏开始时的随机数种子，所有随机事件都基于这个种子生成，保证每个玩家看到的随机事件一致
     * - rng：随机数生成器，基于 RandomSeed 初始化
     * - players：玩家状态列表，包含每个玩家的手牌、场上牌等信息
     * - skillCardsDeck：技能牌堆状态，包含剩余牌的 InstanceID 列表等信息
     * - pointCardsDeck：点数牌堆状态，包含剩余牌的 InstanceID 列表等信息
     */
    public class GameState
    {
        public int Turn;
        public int CurrentPlayerId;
        public int RandomSeed = 12345;

        public Random rng;

        public PlayerState[] players;
        public SkillCardsDeckState skillCardsDeck = new SkillCardsDeckState();
        public PointCardsDeckState pointCardsDeck = new PointCardsDeckState();
        //public BoardState Board = new BoardState();

        public GameState()
        {
            rng = new Random(RandomSeed);
            players = new PlayerState[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerState();
            }
        }
    }

    public static class ClientGameState
    {
        public static bool GetDone = false;     // 是否已经拿到 ClientGameState 的数据了
        public static GameState Instance;
    }
}