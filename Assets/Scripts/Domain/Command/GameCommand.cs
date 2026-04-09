using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public struct ResolvedEvent
    {
        public string type;   // e.g. "StartGame"
        public string jsonData;
        public int sendId;  // -1 for broadcast
    }

    public class CommandResult
    {
        public Queue<ResolvedEvent> events = new();
    }

    public interface ICommand
    {
    }

    public class JoinOrCreateMatchCommand : ICommand
    {
        public int playerId;
        public string matchIdOrEmpty;
    }

    public class LeaveMatchCommand : ICommand
    {
        public int playerId;
    }

    public class StartMatchCommand : ICommand
    {
        public int playerId;
        public int seed;
    }

    public class StartGameCommand : ICommand
    {
        public int playerId;
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

    public class  EmojiCommand : ICommand
    {
        public int playerId;
        public int emojiId;
    }
    public class StartExecuteSkillCommand : ICommand
    {
        public int playerId;
        public int instanceId;
    }

    public class CommitChosenIdsCommand : ICommand
    {
        public int playerId;
        public int instanceId;
        public List<int> selectedSourceIds;
        public List<int> selectedTargetIds;
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

    public class AddActionPointCommand : ICommand
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
        public bool isPeekZone;
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

    public class ChangeCardStateCommand : ICommand
    {
        public int playerId;
        public int instanceId;
        public CardVisualState cardState;
    }

    public class PeekTopCardCommand : ICommand
    {
        public int playerId;
        public int count;
    }

    public class RevealCardsAndScoreCommand : ICommand
    {
        public int playerId;
    }

}