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

    public class GetCardDeckEvent : PlayerEvent
    {
        public string pointCardDeckJson;
        public string skillCardDeckJson;
        public GetCardDeckEvent(int playerId, bool success, string pointCardDeckJson, string skillCardDeckJson)
            : base(playerId, success)
        {
            this.pointCardDeckJson = pointCardDeckJson;
            this.skillCardDeckJson = skillCardDeckJson;
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
        public int skillCardCount;
        public int gameRound;
        public StartGameEvent(int playerId, bool success, int skillCardCount, int gameRound) 
            : base(playerId, success)
        {
            this.skillCardCount = skillCardCount;
            this.gameRound = gameRound;
        }
    }

    /// <summary>
    /// 决定庄闲
    /// </summary>
    public class AssignRolesEvent : PlayerEvent
    {
        public int dealerId;
        public int punterId;
        public int placeBetCount;
        public AssignRolesEvent(int playerId, bool success, int dealerId, int punterId, int placeBetCount)
            : base(playerId, success)
        {
            this.dealerId = dealerId;
            this.punterId = punterId;
            this.placeBetCount = placeBetCount;
        }
    }

    /// <summary>
    /// 下一注
    /// </summary>
    public class Place1BetEvent : PlayerEvent
    {
        public int instanceId;
        public Place1BetEvent(int playerId, bool success, int instanceId)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
        }
    }

    public class PlaceBetsEvent : PlayerEvent
    {
        public int[] instanceIds;
        public PlaceBetsEvent(int playerId, bool success, int[] instanceIds)
            : base(playerId, success)
        {
            this.instanceIds = instanceIds;
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
    /// 无效行动事件，成功时 playerId 是玩家ID，invalidType 是无效类型；失败时 playerId 是玩家ID，invalidType 是默认值
    /// </summary>
    public class InvalidActionEvent : PlayerEvent
    {
        public InvalidActionType invalidType;
        public int instanceId;
        public InvalidActionEvent(int playerId, InvalidActionType invalidType, int instanceId)
            : base(playerId, false)
        {
            this.invalidType = invalidType;
            this.instanceId = instanceId;
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
    /// 表情事件，成功时 playerId 是玩家ID，emojiId 是表情ID；失败时 playerId 是玩家ID，emojiId 是 -1
    /// </summary>
    public class EmojiEvent : PlayerEvent
    {
        public int emojiId;
        public EmojiEvent(int playerId, bool success, int emojiId)
            : base(playerId, success)
        {
            this.emojiId = emojiId;
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
    /// 判断结果事件，成功时 playerId 是玩家ID，cardId 是被判断的牌的牌ID，instanceId 是被判断的牌的实例ID，judgeResult 表示判断结果；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1，judgeResult 是 false
    /// </summary>
    public class JudgeResultEvent : PlayerEvent
    {
        public bool judgeResult;
        public EffectAnimation effectAnimation;
        public JudgeResultEvent(int playerId, bool success, bool judgeResult, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.judgeResult = judgeResult;
            this.effectAnimation = effectAnimation;
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
        public int apCount;
        public bool reset;  // 是否重置行动点为 apCount，false 表示在原有基础上增加 apCount
        public AddActionPointEvent(int playerId, bool success, int apCount, bool reset)
            : base(playerId, success)
        {
            this.apCount = apCount;
            this.reset = reset;
        }
    }

    /// <summary>
    /// 消耗行动点事件，成功时 playerId 是玩家ID；失败时 playerId 是玩家ID
    /// </summary>
    public class SpendActionPointEvent : PlayerEvent
    {
        public int apCount;
        public SpendActionPointEvent(int playerId, bool success, int apCount)
            : base(playerId, success)
        {
            this.apCount = apCount;
        }
    }

    /// <summary>
    /// 抽牌事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class DrawSkillCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public EffectAnimation effectAnimation;
        public DrawSkillCardEvent(int playerId, bool success, int cardId, int instanceId, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 抽牌事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID，isHoleCard 表示是否是底牌；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1，isHoleCard 是 false
    /// </summary>
    public class DrawPointCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public CardVisualState cardState;
        public EffectAnimation effectAnimation;
        public DrawPointCardEvent(int playerId, bool success, int cardId, int instanceId, CardVisualState cardState, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.cardState = cardState;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 抽取一张牌到待处理区事件，成功时 playerId 是玩家ID，cardId 是牌ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class DrawPointCardToResolveEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public CardVisualState cardState;
        public EffectAnimation effectAnimation;

        public DrawPointCardToResolveEvent(int playerId, bool success, int cardId, int instanceId, CardVisualState cardState, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.cardState = cardState;
            this.effectAnimation = effectAnimation;
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

    /// <summary>
    /// 弃牌事件，成功时 playerId 是玩家ID，instanceId 是牌的实例ID；失败时 playerId 是玩家ID，instanceId 是 -1
    /// </summary>
    public class DiscardCardEvent : PlayerEvent
    {
        public List<int> instanceIds;
        public EffectAnimation effectAnimation;
        public DiscardCardEvent(int playerId, bool success, List<int> instanceIds, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.instanceIds = instanceIds;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 修改点数事件，成功时 playerId 是玩家ID，instanceId 是牌的实例ID，pointChange 是点数变化值（正数表示增加，负数表示减少）；失败时 playerId 是玩家ID，instanceId 是 -1，pointChange 是 0
    /// </summary>
    public class ModifyPointEvent : PlayerEvent
    {
        public int instanceId;
        public int pointChange;
        public EffectAnimation effectAnimation;
        public ModifyPointEvent(int playerId, bool success, int instanceId, int pointChange, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
            this.pointChange = pointChange;
            this.effectAnimation = effectAnimation;
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
        public CardVisualState cardState;
        public EffectAnimation effectAnimation;
        public MoveCardEvent(int playerId, bool success, int cardId, int selectedId, ParticipantType toZone, CardVisualState cardState, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.selectedId = selectedId;
            this.toZone = toZone;
            this.cardState = cardState;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 改变牌状态事件，成功时 playerId 是玩家ID，instanceId 是被改变状态的牌的实例ID，cardVisualState 是牌的新状态；失败时 playerId 是玩家ID，instanceId 是 -1，cardVisualState 是默认值
    /// </summary>
    public class ChangeCardStateEvent : PlayerEvent
    {
        public int instanceId;
        public CardVisualState cardState;
        public EffectAnimation effectAnimation;
        public ChangeCardStateEvent(int playerId, bool success, int instanceId, CardVisualState cardState, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.instanceId = instanceId;
            this.cardState = cardState;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 偷看牌事件，成功时 playerId 是玩家ID，cardId 是被偷看的牌的牌ID，instanceId 是被偷看的牌的实例ID；失败时 playerId 是玩家ID，cardId 是 -1，instanceId 是 -1
    /// </summary>
    public class PeekTopCardEvent : PlayerEvent
    {
        public int cardId;
        public int instanceId;
        public EffectAnimation effectAnimation;
        public PeekTopCardEvent(int playerId, bool success, int cardId, int instanceId, EffectAnimation effectAnimation)
            : base(playerId, success)
        {
            this.cardId = cardId;
            this.instanceId = instanceId;
            this.effectAnimation = effectAnimation;
        }
    }

    /// <summary>
    /// 结算事件，成功时 playerId 是玩家ID，winnerId 是赢家玩家ID，currentBet 是当前下注；失败时 playerId 是玩家ID，winnerId 是 -1，currentBet 是 0
    /// </summary>
    public class RevealCardsAndScoreEvent : PlayerEvent
    {
        public int winnerId;
        public int currentBet;
        public int playerPoints;
        public int opponentPoints;
        public RevealCardsAndScoreEvent(int playerId, bool success, int winnerId, int currentBet, int playerPoints, int opponentPoints)
            : base(playerId, success)
        {
            this.winnerId = winnerId;
            this.currentBet = currentBet;
            this.playerPoints = playerPoints;
            this.opponentPoints = opponentPoints;
        }
    }

    /// <summary>
    /// 确定参与者事件，成功时 playerId 是玩家ID，
    /// sourceNeedChoose 和 targetNeedChoose 分别表示是否需要选择来源和目标，
    /// candidateSourceIds 和 candidateTargetIds 分别是可选的来源和目标的实例ID列表，
    /// sourceSelectCount 和 targetSelectCount 分别是需要选择的来源和目标的数量；
    /// </summary>
    public class WaitForPlayer2ChooseEvent : PlayerEvent
    {
        public int skillCardInstanceId;
        public bool sourceNeedChoose;
        public bool targetNeedChoose;
        public bool isSourceParticipantZone;
        public bool isTargetParticipantZone;
        public List<int> candidateSourceIds;
        public List<int> candidateTargetIds;
        public int sourceSelectCount;
        public int targetSelectCount;
        public WaitForPlayer2ChooseEvent(int playerId, bool success, int skillCardInstanceId,
            bool sourceNeedChoose, bool targetNeedChoose, 
            bool isSourceParticipantZone, bool isTargetParticipantZone, 
            List<int> candidateSourceIds, List<int> candidateTargetIds, 
            int sourceSelectCount, int targetSelectCount)
            : base(playerId, success)
        {
            this.skillCardInstanceId = skillCardInstanceId;
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

    public class SumPointEvent : PlayerEvent
    {
        public int playerPointsOnBoard;
        public int playerHoleCardPoint;
        public bool hasHiddenCard;

        public int opponentPointsOnBoard;
        public SumPointEvent(int playerId, bool success, int playerPointsOnBoard, int playerHoleCardPoint, bool hasHiddenCard, int opponentPointsOnBoard) 
            : base(playerId, success)
        {
            this.playerPointsOnBoard = playerPointsOnBoard;
            this.playerHoleCardPoint = playerHoleCardPoint;
            this.hasHiddenCard = hasHiddenCard;
            this.opponentPointsOnBoard = opponentPointsOnBoard;
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

    public class EndMatchEvent : PlayerEvent
    {
        public int finalWinnerId;
        public EndMatchEvent(int playerId, bool success, int finalWinnerId)
            : base(playerId, success)
        {
            this.finalWinnerId = finalWinnerId;
        }
    }
}