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
        public int playerId;
        public string matchIdOrEmpty;
    }

    [Serializable]
    public class ChatCommand : ICommand
    {
        public int playerId;
        public string chatContext;
    }

    [Serializable]
    public class DrawCardCommand : ICommand
    {
        public int playerId;
    }

    [Serializable]
    public class ReadyToPlaySkillCardCommand : ICommand
    {
        public int playerId;
        public int cardId;
    }
}