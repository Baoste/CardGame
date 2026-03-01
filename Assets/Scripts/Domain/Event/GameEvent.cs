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

    [Serializable]
    public class ChatEvent : INetEventPayload
    {
        public int playerId;
        public string text;
    }

    [Serializable]
    public class DrawCardEvent : INetEventPayload
    {
        public int playerId;
        public int cardId;
        public int instanceId;
        public bool isHoleCard;
    }

    [Serializable]
    public class ReadyToPlaySkillCardEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
        public List<int> targetIds;
    }

    [Serializable]
    public class PlaySkillCardWithTargetEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
        public List<int> targetIds;
    }

    [Serializable]
    public class CardChangeEvent : INetEventPayload
    {
        public int playerId;
        public int instanceId;
        public int cardId;
    }
}