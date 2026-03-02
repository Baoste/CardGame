using System;
using System.Collections.Generic;

namespace Game.Domain
{
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

    public class JoinOrCreateGameCommand : ICommand
    {
        public int playerId;
        public string matchIdOrEmpty;
    }

    public class StartGameCommand : ICommand
    {
        public int playerId;
    }

    /// <summary>
    /// 开始回合命令
    /// </summary>
    public class StartTurnCommand : ICommand
    {
        public int playerId;
    }

    // 获取牌局快照
    public class GetGameStateCommand : ICommand
    {
    }

    // 获取牌局上下文（例如当前技能卡的目标选择规范等）
    public class GetCtxCommand : ICommand
    {
    }

    public class ChatCommand : ICommand
    {
        public int playerId;
        public string chatContext;
    }

    public class DrawPointCardCommand : ICommand
    {
        public int playerId;
    }

    public class ReadyToPlaySkillCardEffectCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
    }

    public class PlaySkillCardEffectWithTargetCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
        public List<int> selectedSourceIds;
        public List<int> selectedTargetIds;
    }
}