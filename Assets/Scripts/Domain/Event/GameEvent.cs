using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class PlayerEvent
    {
        public int playerId;
        public bool success;
        public PlayerEvent(int playerId, bool success)
        {
            this.playerId = playerId;
            this.success = success;
        }
    }

    /// <summary>
    /// 加入或创建房间事件，成功时 playerId 是玩家ID，matchIdOrEmpty 是房间ID；失败时 playerId 是玩家ID，matchIdOrEmpty 是空字符串
    /// </summary>
    public class JoinOrCreateGameEvent : PlayerEvent
    {
        public string matchIdOrEmpty;
        public JoinOrCreateGameEvent(int playerId, bool success, string matchIdOrEmpty)
            : base(playerId, success)
        {
            this.matchIdOrEmpty = matchIdOrEmpty;
        }
    }

    /// <summary>
    /// 开始游戏事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class StartGameEvent : PlayerEvent
    {
        public StartGameEvent(int playerId, bool success) 
            : base(playerId, success)
        {
        }
    }

    /// <summary>
    /// 开始回合事件
    /// </summary>
    public class StartTurnEvent : PlayerEvent
    {
        public int opponentId;
        public StartTurnEvent(int playerId, bool success, int opponentId)
            : base(playerId, success)
        {
            this.opponentId = opponentId;
        }
    }

    /// <summary>
    /// 回复牌局快照
    /// </summary>
    public class GetGameStateEvent : PlayerEvent
    {
        public GameState gameState;
        public GetGameStateEvent(int playerId, bool success, GameState gameState)
            : base(playerId, success)
        {
            this.gameState = gameState;
        }
    }

    /// <summary>
    /// 回复牌局上下文
    /// </summary>
    public class GetCtxEvent : PlayerEvent
    {
        public EffectContext ctx;
        public GetCtxEvent(int playerId, bool success, EffectContext ctx)
            : base(playerId, success)
        {
            this.ctx = ctx;
        }
    }

    /// <summary>
    /// 聊天事件，成功时 playerId 是玩家ID，text 是聊天内容；失败时 playerId 是玩家ID，text 是空字符串
    /// </summary>
    public class ChatEvent : PlayerEvent
    {
        public string text;
        public ChatEvent(int playerId, bool success, string text)
            : base(playerId, success)
        {
            this.text = text;
        }
    }

    /// <summary>
    /// 消耗行动点事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class SpendActionPointEvent : PlayerEvent
    {
        public SpendActionPointEvent(int playerId, bool success)
            : base(playerId, success)
        {
        }
    }

    /// <summary>
    /// 抽牌事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class DrawSkillCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public DrawSkillCardEvent(int playerId, bool success, int cardId, int instanceId)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
        }
    }

    /// <summary>
    /// 抽牌事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID，isHoleCard 表示是否是底牌；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1，isHoleCard 是 false
    /// </summary>
    public class DrawPointCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public bool isHoleCard;
        public DrawPointCardEvent(int playerId, bool success, int cardId, int instanceId, bool isHoleCard)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.isHoleCard = isHoleCard;
        }
    }

    /// <summary>
    /// 弃牌事件，成功时 playerId 是玩家ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，instanceId 是 -1
    /// </summary>
    public class DiscardCardEvent : PlayerEvent
    {
        public int instanceId;
        public DiscardCardEvent(int playerId, bool success, int instanceId)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
        }
    }

    /// <summary>
    /// 修改点数事件，成功时 playerId 是玩家ID，instanceId 是牌的实例ID，pointChange 是点数变化值（正数表示增加，负数表示减少）；失败时 playerId 是玩家ID，instanceId 是 -1，pointChange 是 0
    /// </summary>
    public class ModifyPointEvent : PlayerEvent
    {
        public int instanceId;
        public int pointChange;
        public ModifyPointEvent(int playerId, bool success, int instanceId, int pointChange)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
            this.pointChange = pointChange;
        }
    }

    /// <summary>
    /// 移动牌事件，成功时 playerId 是玩家ID，selectedId 是被移动的牌的实例ID，toZone 是目标区域；失败时 playerId 是玩家ID，selectedId 是 -1，toZone 是默认值
    /// </summary>
    public class MoveCardEvent : PlayerEvent
    {
        public int cardId;
        public int selectedId;
        public ParticipantType toZone;
        public MoveCardEvent(int playerId, bool success, int cardId, int selectedId, ParticipantType toZone)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.selectedId = selectedId;
            this.toZone = toZone;
        }
    }

    /// <summary>
    /// 确定参与者事件，成功时 playerId 是玩家ID，
    /// sourceNeedChoose 和 targetNeedChoose 分别表示是否需要选择来源和目标，
    /// candidateSourceIds 和 candidateTargetIds 分别是可选的来源和目标的实例ID列表，
    /// sourceSelectCount 和 targetSelectCount 分别是需要选择的来源和目标的数量；
    /// </summary>
    public class DetermineParticipantsEvent : PlayerEvent
    {
        public bool sourceNeedChoose;
        public bool targetNeedChoose;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
        public int sourceSelectCount;
        public int targetSelectCount;
        public DetermineParticipantsEvent(int playerId, bool success, bool sourceNeedChoose, bool targetNeedChoose, List<int> candidateSourceIds, List<int> candidateTargetIds, int sourceSelectCount, int targetSelectCount)
            : base(playerId, success)
        {
            this.sourceNeedChoose = sourceNeedChoose;
            this.targetNeedChoose = targetNeedChoose;
            this.candidateSourceIds = candidateSourceIds;
            this.candidateTargetIds = candidateTargetIds;
            this.sourceSelectCount = sourceSelectCount;
            this.targetSelectCount = targetSelectCount;
        }
    }

    /// <summary>
    /// 验证参与者事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class ValidateParticipantsEvent : PlayerEvent
    {
        public List<int> sourceIds;
        public List<int> targetIds;
        public ValidateParticipantsEvent(int playerId, bool success, List<int> sourceIds, List<int> targetIds)
            : base(playerId, success)
        {
            this.sourceIds = sourceIds;
            this.targetIds = targetIds;
        }
    }

    /// <summary>
    /// 结束回合事件，成功时 playerId 是玩家ID，opponentId 是对手玩家ID；失败时 playerId 是玩家ID，opponentId 是 -1
    /// </summary>
    public class EndTurnEvent : PlayerEvent
    {
        public int opponentId;
        public EndTurnEvent(int playerId, bool success, int opponentId)
            : base(playerId, success)
        {
            this.opponentId = opponentId;
        }
    }
}