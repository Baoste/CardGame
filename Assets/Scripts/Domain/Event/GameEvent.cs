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
        public int PlayerId;
        public string matchIdOrEmpty;
    }

    [Serializable]
    public class ChatEvent : INetEventPayload
    {
        public int PlayerId;
        public string text;
    }

    [Serializable]
    public class DrawCardEvent : INetEventPayload
    {
        public int PlayerId;
        public int CardId;
    }

    [Serializable]
    public class PlayCardEvent : INetEventPayload
    {
        public int PlayerId;
        public int CardId;
        public List<int> targetIds;
    }
}