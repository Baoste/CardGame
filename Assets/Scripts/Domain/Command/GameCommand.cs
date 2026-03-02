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

    public class CommandResult
    {
        public Queue<ResolvedEvent> events = new();
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
    public class StartGameCommand : ICommand
    {
        public int playerId;
    }

    // 获取牌局快照
    [Serializable]
    public class GetGameStateCommand : ICommand
    {
    }

    // 获取牌局上下文（例如当前技能卡的目标选择规范等）
    [Serializable]
    public class GetCtxCommand : ICommand
    {
    }

    [Serializable]
    public class ChatCommand : ICommand
    {
        public int playerId;
        public string chatContext;
    }

    [Serializable]
    public class DrawPointCardCommand : ICommand
    {
        public int playerId;
    }

    [Serializable]
    public class ReadyToPlaySkillCardEffectCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
    }

    [Serializable]
    public class PlaySkillCardEffectWithTargetCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
        public List<int> selectedSourceIds;
        public List<int> selectedTargetIds;
    }
}