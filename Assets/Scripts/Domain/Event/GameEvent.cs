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

    public class DrawSkillCardEvent : INetEventPayload
    {
        public int playerId;
        public int cardId;
        public int instanceId;
    }

    public class DrawPointCardEvent : INetEventPayload
    {
        public int playerId;
        public int cardId;
        public int instanceId;
        public bool isHoleCard;
    }

    public class DiscardCardEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
    }

    public class ReadyToPlaySkillCardEffectEvent : INetEventPayload
    {
        public int playerId;
        public bool sourceNeedChoose;
        public bool targetNeedChoose;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
        public int sourceSelectCount;
        public int targetSelectCount;
    }

    public class ValidateSkillCardEvent : INetEventPayload
    {
        public int playerId;
        public bool success;
        public List<int> sourceIds;
        public List<int> targetIds;
    }

    public class CardChangeEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
    }
}