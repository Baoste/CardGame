using System;
using System.Collections.Generic;

namespace Game.Domain
{
    [Serializable]
    public struct ResolvedEvent
    {
        public string type;   // e.g. "StartGame"
        public string jsonData;
    }

    public interface ICommand
    {
    }

    [Serializable]
    public class JoinOrCreateGameCommand : ICommand
    {
        public int PlayerId;
        public string matchIdOrEmpty;
    }

    [Serializable]
    public class ChatCommand : ICommand
    {
        public int PlayerId;
        public string chatContext;
    }

    [Serializable]
    public class DrawCardCommand : ICommand
    {
        public int PlayerId;
    }

    [Serializable]
    public class PlayCardCommand : ICommand
    {
        public int PlayerId;
        public int CardId;
        public List<int> targetIds;
    }
}