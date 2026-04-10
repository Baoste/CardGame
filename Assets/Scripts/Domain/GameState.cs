using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using static System.Collections.Specialized.BitVector32;

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
     * - m_SkillCardsDeck：技能牌堆状态，包含剩余牌的 InstanceID 列表等信息
     * - pointCardsDeck：点数牌堆状态，包含剩余牌的 InstanceID 列表等信息
     */
    public class GameState
    {
        public int Turn = 0;
        public int CurrentPlayerId = -1;
        public int RandomSeed = 12345;

        public int dealerId = -1;
        public int punterId = -1;

        public Random rng;
        public bool isStart = false;

        public PlayerState[] players;
        public SkillCardsDeck skillCardsDeck = new SkillCardsDeck();
        public PointCardsDeck pointCardsDeck = new PointCardsDeck();
        public DiscardPile discardPile = new DiscardPile();
        public CardsToResolve cardsToResolve = new CardsToResolve();

        // 赌注相关
        public int currentBet = 0;

        // CardInstanceID -> CardZone 的映射，方便快速查询某张牌当前在哪个牌堆/玩家的哪个区域
        private Dictionary<int, CardZone> cardLocationMap = new Dictionary<int, CardZone>();
        // CardInstanceID -> PointCard的点数 的映射，方便快速查询某张点数牌实例的点数
        public Dictionary<int, int> instancePointMap = new Dictionary<int, int>();
        // CardInstanceID -> CardVisualState 的映射，方便查询某张牌的状态（是否被封印、是否被保护等）
        public Dictionary<int, CardVisualState> instanceStateMap = new Dictionary<int, CardVisualState>();

        public GameState()
        {
            rng = new Random(RandomSeed);
            players = new PlayerState[2];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new PlayerState();
            }
        }

        public void Init(int seed)
        {
            RandomSeed = seed;
            rng = new Random(seed);
        }

        public void Start()
        {
            isStart = true;
            Turn = 0;
            CurrentPlayerId = -1;

            currentBet = 0;

            skillCardsDeck._Clear();
            pointCardsDeck._Clear();
            discardPile._Clear();
            cardsToResolve._Clear();
            cardLocationMap.Clear();
            instancePointMap.Clear();
            instanceStateMap.Clear();
            foreach (PlayerState player in players)
            {
                player.Init();
            }
        }

        public void Dispose()
        {
            isStart = false;
            Turn = 0;
            CurrentPlayerId = -1;
            currentBet = 0;

            skillCardsDeck._Clear();
            pointCardsDeck._Clear();
            discardPile._Clear();
            cardsToResolve._Clear();
            cardLocationMap.Clear();
            instancePointMap.Clear();
            instanceStateMap.Clear();
            foreach (PlayerState player in players)
            {
                player.Init();
            }
        }

        public int SumPoint(int playerId, out int onBoardPointSum)
        {
            int sum = 0;
            IReadOnlyList<int> pointCardIds = players[playerId].pointCardsOnBoard.instanceIds;
            foreach (int instanceId in pointCardIds)
            {
                if (instancePointMap.ContainsKey(instanceId))
                    sum += instancePointMap[instanceId];
            }
            onBoardPointSum = sum;

            sum += instancePointMap[players[playerId]._holeCard];
            return sum;
        }

        public int SumPointOnCardsToResolve()
        {
            int sum = 0;
            IReadOnlyList<int> pointCardIds = cardsToResolve.instanceIds;
            foreach (int instanceId in pointCardIds)
            {
                if (instancePointMap.ContainsKey(instanceId))
                    sum += instancePointMap[instanceId];
            }
            return sum;
        }

        public CardVisualState GetCardState(int instanceId)
        {
            if (instanceStateMap.ContainsKey(instanceId))
                return instanceStateMap[instanceId];
            return CardVisualState.None;
        }

        public bool SetCardState(int instanceId, CardVisualState state)
        {
            if (instanceStateMap.ContainsKey(instanceId))
            {
                instanceStateMap[instanceId] = state;
                return true;
            }
            return false;
        }

        public void AddCardsToDeck(List<int> instanceIds, Dictionary<int, int> instanceToCardId, CardType type)
        {
            foreach (int instanceId in instanceIds)
            {
                if (instanceId == -1) continue;
                CardZone board = pointCardsDeck;
                if (type == CardType.Skill)
                    board = skillCardsDeck;

                cardLocationMap[instanceId] = board;
                instancePointMap[instanceId] = CardDatabase.Get(instanceToCardId[instanceId]).point;
                instanceStateMap[instanceId] = CardVisualState.None;
                board._Add(instanceId);
            }
        }

        public void AddCard(int playerId, int cardId, int instanceId, CardType type, CardVisualState state = CardVisualState.None)
        {
            if (instanceId == -1)   return;

            CardZone board = players[playerId].pointCardsOnBoard;
            if (type == CardType.Skill)
                board = players[playerId].skillCardsInHand;
            
            cardLocationMap[instanceId] = board;
            instancePointMap[instanceId] = CardDatabase.Get(cardId).point;
            instanceStateMap[instanceId] = state;
            board._Add(instanceId);
        }

        public void AddHoleCard(int playerId, int cardId, int instanceId)
        {
            if (instanceId == -1) return;

            players[playerId]._holeCard = instanceId;
            instancePointMap[instanceId] = CardDatabase.Get(cardId).point;
            instanceStateMap[instanceId] = CardVisualState.Hole;
        }

        public void AddToResolve(int cardId, int instanceId)
        {
            if (instanceId == -1) return;

            cardLocationMap[instanceId] = cardsToResolve;
            instancePointMap[instanceId] = CardDatabase.Get(cardId).point;
            instanceStateMap[instanceId] = CardVisualState.None;
            cardsToResolve._Add(instanceId);
        }

        public void ClearResolve()
        {
            IReadOnlyList<int> ids = cardsToResolve.instanceIds;
            foreach(int instanceId in ids)
            {
                discardPile._Add(instanceId);
                cardLocationMap[instanceId] = discardPile;
                // instancePointMap.Remove(instanceId);
            }
            cardsToResolve._Clear();
        }

        public bool RemoveCard(int instanceId)
        {
            if (!cardLocationMap.ContainsKey(instanceId))   return false;

            CardZone board = cardLocationMap[instanceId];
            board._Remove(instanceId);
            discardPile._Add(instanceId);
            cardLocationMap[instanceId] = discardPile;
            // instancePointMap.Remove(instanceId);
            return true;
        }

        public bool MoveCard(int instanceId, CardZone targetZone)
        {
            if (targetZone == null) return false;
            if (!cardLocationMap.ContainsKey(instanceId)) return false;

            CardZone board = cardLocationMap[instanceId];
            board._Remove(instanceId);
            targetZone._Add(instanceId);
            cardLocationMap[instanceId] = targetZone;
            return true;
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