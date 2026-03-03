using System;
using System.Collections.Generic;

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
        public SkillCardsDeck skillCardsDeck = new SkillCardsDeck();
        public PointCardsDeck pointCardsDeck = new PointCardsDeck();
        public DiscardPile discardPile = new DiscardPile();

        Dictionary<int, CardZone> cardLocationMap = new Dictionary<int, CardZone>();

        public GameState()
        {
            rng = new Random(RandomSeed);
            players = new PlayerState[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerState();
            }
        }

        public void AddCard(int playerId, int instanceId, CardType type)
        {
            if (instanceId == -1)   return;

            CardZone board = players[playerId].pointCardsOnBoard;
            if (type == CardType.Skill)
                board = players[playerId].skillCardsInHand;
            
            cardLocationMap[instanceId] = board;
            board._Add(instanceId);
        }

        public void RemoveCard(int instanceId)
        {
            if (!cardLocationMap.ContainsKey(instanceId))   return;

            CardZone board = cardLocationMap[instanceId];
            board._Remove(instanceId);
            discardPile._Add(instanceId);
            cardLocationMap[instanceId] = discardPile;
        }

        public void MoveCard(int instanceId, CardZone targetZone)
        {
            if (!cardLocationMap.ContainsKey(instanceId)) return;

            CardZone board = cardLocationMap[instanceId];
            board._Remove(instanceId);
            targetZone._Add(instanceId);
            cardLocationMap[instanceId] = targetZone;
        }
    }

    public static class ClientGameState
    {
        public static int playerSlot = -1;
        public static bool GetServerGameStateDone = false;     // 是否已经拿到 ClientGameState 的数据了
        public static GameState Instance = new GameState();

        public static MatchGateway gateway;
        public static string matchId;
        public static string token;
        public static int lastEventIndex = -1;
    }
}