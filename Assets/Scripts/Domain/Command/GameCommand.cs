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

    public class LeaveGameCommand : ICommand
    {
        public int playerId;
    }

    public class StartGameCommand : ICommand
    {
        public int playerId;
        public int seed;
    }

    public class AssignRolesCommand : ICommand
    {
        public int playerId;
    }

    public class Place1BetCommand : ICommand
    {
        public int playerId;
    }

    public class ConfirmBetCommand : ICommand
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

    public class EndTurnCommand : ICommand
    {
        public int playerId;
    }

    // 获取牌局快照
    public class GetGameStateCommand : ICommand
    {
        public int playerId;
    }

    // 获取牌局上下文（例如当前技能卡的目标选择规范等）
    public class GetCtxCommand : ICommand
    {
        public int playerId;
    }

    public class ChatCommand : ICommand
    {
        public int playerId;
        public string chatContext;
    }

    public class PlayAnimationCommand : ICommand
    {
        public int playerId;
        public AnimationType animType;
        public int instanceId;
    }

    public class ValidateActionPointCommand : ICommand
    {
        public int playerId;
    }

    public class SpendActionPointCommand : ICommand
    {
        public int playerId;
    }

    public class DrawSkillCardCommand : ICommand
    {
        public int playerId;
    }

    public class DrawPointCardCommand : ICommand
    {
        public int playerId;
    }

    public class ClearCardsToResolveCommand : ICommand
    {
        public int playerId;
    }

    public class PlayResolveAnimCommand : ICommand
    {
        public int playerId;
        public int cardId;
        public int instanceId;
        public bool isShown;
    }

    public class DiscardCardCommand : ICommand
    {
        public int playerId;
        public int instanceId;
    }

    public class ModifyPointCommand : ICommand
    {
        public int playerId;
        public int instanceId;
        public int pointChange;
    }

    public class MoveCardCommand : ICommand
    {
        public int playerId;
        public int instanceId;
        public ParticipantType toZone;
    }

    public class RevealCardsAndScoreCommand : ICommand
    {
        public int playerId;
    }

    // 根据效果返回可选的操作源、目标列表
    public class DetermineParticipantsCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
    }

    // 验证玩家选择的操作源、目标是否合法
    public class ValidateParticipantsCommand : ICommand
    {
        public int playerId;
        public EffectOp effect;
        public List<int> selectedSourceIds;
        public List<int> selectedTargetIds;
    }

}