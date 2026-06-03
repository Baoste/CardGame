using Game.Server;
using System.Collections.Generic;

namespace Game.Domain
{
    public static class NetEffectFunction
    {
        public static void SumPoint(MatchSession session, int playerId, ref CommandResult results)
        {
            int player0Points = session.gameState.SumPoint(0, out int player0OnBoardPoints, out bool player0HasHiddenCard);
            int player1Points = session.gameState.SumPoint(1, out int player1OnBoardPoints, out bool player1HasHiddenCard);

            results.events.Enqueue(CommandHandler.MakeEvent(
                "SumPoint",
                new SumPointEvent
                (
                    0,
                    true,
                    player0OnBoardPoints,
                    session.gameState.GetHoleCardPoint(0),
                    player0HasHiddenCard,
                    player1OnBoardPoints
                ),
                0
            ));

            results.events.Enqueue(CommandHandler.MakeEvent(
                "SumPoint",
                new SumPointEvent
                (
                    1,
                    true,
                    player1OnBoardPoints,
                    session.gameState.GetHoleCardPoint(1),
                    player1HasHiddenCard,
                    player0OnBoardPoints
                ),
                1
            ));

            //! if both players have 21 points, the player who triggered the sum point action wins
            int winnerId = -1;
            if (player0Points == 21 && player1Points == 21)
            {
                winnerId = playerId;
            }
            else if (player0Points == 21)
            {
                winnerId = 0;
            }
            else if (player1Points == 21)
            {
                winnerId = 1;
            }
            int playerPoints = playerId == 0 ? player0Points : player1Points;
            int opponentPoints = playerId == 0 ? player1Points : player0Points;

            if (winnerId != -1)
            {
                int currentBet = session.gameState.currentBet;
                session.gameState.Dispose();
                session.gameState.players[winnerId].chipCount += currentBet;
                session.gameState.players[1 - winnerId].chipCount -= currentBet;

                results.events.Enqueue(CommandHandler.MakeEvent(
                    "RevealCardsAndScore",
                    new RevealCardsAndScoreEvent    // need change
                    (
                        playerId,
                        true,
                        winnerId,
                        session.gameState.currentBet,
                        playerPoints,
                        opponentPoints
                    ),
                    -1
                ));
            }

        }

        public static void ExecuteEffectOp(ref int effectOpId, Card skillCard, MatchSession session, int playerId, int instanceId, ref CommandResult results)
        {
            while (effectOpId != -1)
            {
                EffectOp op = skillCard.effects[effectOpId];

                // if the effect is judge, evaluate the judge result first, and then determine which node to go next based on the judge result
                if (op.type == EffectType.Judge)
                {
                    List<int> judgePool = ParticipantsResolver.DetermineCandidates(op.source, session.gameState, session.ctx, out bool isParticipantZone);
                    if (isParticipantZone)
                        judgePool = judgePool.FindAll(c => op.source.filter.Evaluate(session.gameState, session.ctx, c));

                    bool judgeResult = judgePool.Count > 0;

                    // send judge result event to client
                    results.events.Enqueue(CommandHandler.MakeEvent(
                        "JudgeResult",
                        new JudgeResultEvent
                        (
                            playerId,
                            true,
                            judgeResult,
                            op.effectAnimationType
                        ),
                        -1
                    ));

                    if (judgeResult) effectOpId = op.trueNode;
                    else effectOpId = op.falseNode;
                    continue;
                }

                bool success = ParticipantsResolver.ReturnCandidates(
                    op.source, op.target, session.gameState, session.ctx,
                    out List<int> candidateSourceIds, out List<int> candidateTargetIds,
                    out bool isSourceParticipantZone, out bool isTargetParticipantZone
                );
                if (!success)
                {
                    if (effectOpId == 0)
                    {
                        session.gameState.MoveCard(instanceId, session.gameState.players[playerId].skillCardsInHand);
                        SendInvalidEvent(playerId, instanceId, results, InvalidActionType.InvalidTarget);
                    }
                    return;
                }

                // play animation event before executing the effect
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "PlayAnimation",
                    new PlayAnimationEvent
                    (
                        playerId,
                        true,
                        AnimationType.MoveToExecutePosition,
                        instanceId
                    ),
                    -1
                ));

                bool sourceNeedChoose = op.source.participantSelectionMode is SelectionModeChoose;
                bool targetNeedChoose = op.target.participantSelectionMode is SelectionModeChoose;

                // if need choose, send event to client to ask for participant selection,
                // then wait for client's response to get selected participants and continue to execute the effect
                if (sourceNeedChoose || targetNeedChoose)
                {
                    session.ctx.opStack.Push(new EffectOpExecutionContext
                    {
                        effectOp = op,
                        candidateSourceIds = candidateSourceIds,
                        candidateTargetIds = candidateTargetIds
                    });
                    results.events.Enqueue(CommandHandler.MakeEvent(
                        "WaitForPlayer2Choose",
                        new WaitForPlayer2ChooseEvent
                        (
                            playerId,
                            success,
                            instanceId,
                            sourceNeedChoose,
                            targetNeedChoose,
                            isSourceParticipantZone,
                            isTargetParticipantZone,
                            candidateSourceIds,
                            candidateTargetIds,
                            op.source.maxSelectCount.Evaluate(session.gameState, session.ctx),
                            op.target.maxSelectCount.Evaluate(session.gameState, session.ctx)
                        ),
                        playerId
                    ));
                    break;
                }
                // if not need choose, directly execute the effect
                else
                {
                    List<int> selectedSourceIds, selectedTargetIds;
                    int count;

                    count = op.source.maxSelectCount.Evaluate(session.gameState, session.ctx);
                    selectedSourceIds = op.source.participantSelectionMode.Execute(session.gameState, candidateSourceIds, count, new List<int>());
                    count = op.target.maxSelectCount.Evaluate(session.gameState, session.ctx);
                    selectedTargetIds = op.target.participantSelectionMode.Execute(session.gameState, candidateTargetIds, count, new List<int>());

                    NetEffectExecutor.ExecuteOp(playerId, op, session, results, selectedSourceIds, selectedTargetIds);
                }

                if (op.trueNode == -1 && op.falseNode == -1)        effectOpId = -1;
                else if (op.trueNode != -1 && op.falseNode == -1)   effectOpId = op.trueNode;
                else if (op.trueNode == -1 && op.falseNode != -1)   effectOpId = op.falseNode;
                else                                                effectOpId = op.trueNode;
            }
        }

        public static void EndExecuteSkill(int playerId, int instanceId, MatchSession session, CommandResult results)
        {
            session.gameState.ClearResolve();
            results.events.Enqueue(CommandHandler.MakeEvent(
                "ClearCardsToResolve",
                new ClearCardsToResolveEvent
                (
                    playerId,
                    true,
                    false
                ),
                -1
            ));

            bool success = session.gameState.RemoveCard(instanceId);
            results.events.Enqueue(CommandHandler.MakeEvent(
                "DiscardCard",
                new DiscardCardEvent
                (
                    playerId,
                    success,
                    new List<int> { instanceId },
                    EffectAnimation.Discard_Normal
                ),
                -1
            ));

            SumPoint(session, playerId, ref results);
        }

        public static bool SpendActionPoint(int playerId, int instanceId, MatchSession session, CommandResult results, int apCount)
        {
            if (session.gameState.players[playerId].actionPoint >= apCount)
            {
                session.gameState.players[playerId].actionPoint -= apCount;
                results.events.Enqueue(CommandHandler.MakeEvent(
                    "SpendActionPoint",
                    new SpendActionPointEvent    // need change
                    (
                        playerId,
                        true,
                        apCount
                    ),
                    -1
                ));
                return true;
            }
            else
            {
                SendInvalidEvent(playerId, instanceId, results, InvalidActionType.NotEnoughAP);
                return false;
            }
        }

        public static bool ValidSkillCardCount(int playerId, int instanceId, MatchSession session, CommandResult results)
        {
            int maxCardCount = GameConfigLoader.Config.StartGame.maxSkillCardCount;
            if (session.gameState.players[playerId].skillCardsInHand.GetCount() < maxCardCount)
            {
                return true;
            }
            else
            {
                SendInvalidEvent(playerId, instanceId, results, InvalidActionType.SkillCardCountFull);
                return false;
            }
        }

        public static void SendInvalidEvent(int playerId, int instanceId, CommandResult results, InvalidActionType type)
        {
            results.events.Enqueue(CommandHandler.MakeEvent(
                "InvalidAction",
                new InvalidActionEvent    // need change
                (
                    playerId,
                    type,
                    instanceId
                ),
                playerId
            ));
        }
    }
}