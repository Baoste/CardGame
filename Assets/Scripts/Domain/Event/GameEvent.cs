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
    public class JoinOrCreateMatchEvent : PlayerEvent
    {
        public string matchIdOrEmpty;
        public JoinOrCreateMatchEvent(int playerId, bool success, string matchIdOrEmpty)
            : base(playerId, success)
        {
            this.matchIdOrEmpty = matchIdOrEmpty;
        }
    }

    /// <summary>
    /// 开始比赛事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class StartMatchEvent : PlayerEvent
    {
        public int seed;
        public StartMatchEvent(int playerId, bool success, int seed)
            : base(playerId, success)
        {
            this.seed = seed;
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
    /// 决定庄闲
    /// </summary>
    public class AssignRolesEvent : PlayerEvent
    {
        public int dealerId;
        public int punterId;
        public AssignRolesEvent(int playerId, bool success, int dealerId, int punterId)
            : base(playerId, success)
        {
            this.dealerId = dealerId;
            this.punterId = punterId;
        }
    }

    /// <summary>
    /// 下一注
    /// </summary>
    public class Place1BetEvent : PlayerEvent
    {
        public Place1BetEvent(int playerId, bool success)
            : base(playerId, success)
        {
        }
    }

    /// <summary>
    /// 确定下注
    /// </summary>
    public class ConfirmBetEvent : PlayerEvent
    {
        public int betCount;
        public ConfirmBetEvent(int playerId, bool success, int betCount)
            : base(playerId, success)
        {
            this.betCount = betCount;
        }
    }

    /// <summary>
    /// 开始回合事件
    /// </summary>
    public class StartTurnEvent : PlayerEvent
    {
        public int opponentId;
        public int turn;
        public StartTurnEvent(int playerId, bool success, int opponentId, int turn)
            : base(playerId, success)
        {
            this.opponentId = opponentId;
            this.turn = turn;
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
    /// 播放动画
    /// </summary>
    public class PlayAnimationEvent : PlayerEvent
    {
        public AnimationType animType;
        public int instanceId;
        public PlayAnimationEvent(int playerId, bool success, AnimationType animType, int instanceId)
            : base(playerId, success)
        {
            this.animType = animType;
            this.instanceId = instanceId;
        }
    }

    /// <summary>
    /// 检查行动点事件
    /// </summary>
    public class ValidateActionPointEvent : PlayerEvent
    {
        public ValidateActionPointEvent(int playerId, bool success)
            : base(playerId, success)
        {
        }
    }

    /// <summary>
    /// 添加行动点事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class AddActionPointEvent : PlayerEvent
    {
        public AddActionPointEvent(int playerId, bool success)
            : base(playerId, success)
        {
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
        public CardState cardState;
        public DrawPointCardEvent(int playerId, bool success, int cardId, int instanceId, CardState cardState)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.cardState = cardState;
        }
    }

    /// <summary>
    /// 抽取一张牌到待处理区事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class DrawPointCardToResolveEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public DrawPointCardToResolveEvent(int playerId, bool success, int cardId, int instanceId)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
        }
    }

    public class ClearCardsToResolveEvent : PlayerEvent
    {
        public bool isPeekZone;
        public ClearCardsToResolveEvent(int playerId, bool success, bool isPeekZone)
            : base(playerId, success)
        {
            this.isPeekZone = isPeekZone;
        }
    }

    public class PlayResolveAnimEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public bool isShown;
        public PlayResolveAnimEvent(int playerId, bool success, int cardId, int instanceId, bool isShown)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.isShown = isShown;
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

    public class ChangeCardStateEvent : PlayerEvent
    {
        public int instanceId;
        public CardState cardState;
        public ChangeCardStateEvent(int playerId, bool success, int instanceId, CardState cardState)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
            this.cardState = cardState;
        }
    }

    /// <summary>
    /// 偷看牌事件，成功时 playerId 是玩家ID，cardId 是被偷看的牌的牌ID，instanceId 是被偷看的牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class PeekTopCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public PeekTopCardEvent(int playerId, bool success, int cardId, int instanceId)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
        }
    }

    public class RevealCardsAndScoreEvent : PlayerEvent
    {
        public int winnerId;
        public int currentBet;
        public RevealCardsAndScoreEvent(int playerId, bool success, int winnerId, int currentBet)
            : base(playerId, success)
        {
            this.winnerId = winnerId;
            this.currentBet = currentBet;
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
        public bool judgeResult;
        public bool sourceNeedChoose;
        public bool targetNeedChoose;
        public bool isSourceParticipantZone;
        public bool isTargetParticipantZone;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
        public int sourceSelectCount;
        public int targetSelectCount;
        public DetermineParticipantsEvent(int playerId, bool success, bool judgeResult, bool sourceNeedChoose, bool targetNeedChoose, bool isSourceParticipantZone, bool isTargetParticipantZone, List<int> candidateSourceIds, List<int> candidateTargetIds, int sourceSelectCount, int targetSelectCount)
            : base(playerId, success)
        {
            this.judgeResult = judgeResult;
            this.sourceNeedChoose = sourceNeedChoose;
            this.targetNeedChoose = targetNeedChoose;
            this.isSourceParticipantZone = isSourceParticipantZone;
            this.isTargetParticipantZone = isTargetParticipantZone;
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
        public int turn;
        public bool reveal;  // 结束游戏
        public EndTurnEvent(int playerId, bool success, int opponentId, int turn, bool reveal)
            : base(playerId, success)
        {
            this.opponentId = opponentId;
            this.turn = turn;
            this.reveal = reveal;
        }
    }
}