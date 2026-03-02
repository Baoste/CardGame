using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface INetEventPayload
    {
    }

    public class JoinOrCreateGameEvent : INetEventPayload
    {
        public int playerId;
        public string matchIdOrEmpty;
    }

    public class StartGameEvent : INetEventPayload
    {
        public int playerId;
    }

    /// <summary>
    /// 开始回合事件
    /// </summary>
    public class StartTurnEvent : INetEventPayload
    {
        public int playerId;
        public int opponentId;
    }

    // 回复牌局快照
    public class GetGameStateEvent : INetEventPayload
    {
        public GameState gameState;
    }

    // 回复牌局上下文
    public class GetCtxEvent : INetEventPayload
    {
        public EffectContext ctx;
    }

    public class ChatEvent : INetEventPayload
    {
        public int playerId;
        public string text;
    }

    public class DrawPointCardEvent : INetEventPayload
    {
        public int playerId;
        public int cardId;
        public int instanceId;
        public bool isHoleCard;
    }

    public class ReadyToPlaySkillCardEffectEvent : INetEventPayload
    {
        public int playerId;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
    }

    public class PlaySkillCardEffectWithTargetEvent : INetEventPayload
    {
        public int playerId;
        public bool success;
    }

    public class CardChangeEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
    }
}