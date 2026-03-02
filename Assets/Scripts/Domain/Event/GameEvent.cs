using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface INetEventPayload
    {
    }

    [Serializable]
    public class JoinOrCreateGameEvent : INetEventPayload
    {
        public int playerId;
        public string matchIdOrEmpty;
    }

    [Serializable]
    public class StartGameEvent : INetEventPayload
    {
        public int playerId;
    }

    // 回复牌局快照
    [Serializable]
    public class GetGameStateEvent : INetEventPayload
    {
        public GameState gameState;
    }

    // 回复牌局上下文
    [Serializable]
    public class GetCtxEvent : INetEventPayload
    {
        public EffectContext ctx;
    }

    [Serializable]
    public class ChatEvent : INetEventPayload
    {
        public int playerId;
        public string text;
    }

    [Serializable]
    public class DrawPointCardEvent : INetEventPayload
    {
        public int playerId;
        public int cardId;
        public int instanceId;
        public bool isHoleCard;
    }

    [Serializable]
    public class ReadyToPlaySkillCardEffectEvent : INetEventPayload
    {
        public int playerId;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
    }

    [Serializable]
    public class PlaySkillCardEffectWithTargetEvent : INetEventPayload
    {
        public int playerId;
        public bool success;
    }

    [Serializable]
    public class CardChangeEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
    }
}