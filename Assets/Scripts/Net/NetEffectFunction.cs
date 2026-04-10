using Game.Server;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Domain
{
    public static class NetEffectFunction
    {
        public static void SumPoint(MatchSession session, ref CommandResult results)
        {
            int player0Points = session.gameState.SumPoint(0, out int player0OnBoardPoints);
            int player1Points = session.gameState.SumPoint(1, out int player1OnBoardPoints);

            results.events.Enqueue(CommandHandler.MakeEvent(
                "SumPoint",
                new SumPointEvent
                (
                    0,
                    true,
                    player0Points,
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
                    player1Points,
                    player0OnBoardPoints
                ),
                1
            ));
        }

        public static void ExecuteEffectOp(int effectOpId, Card skillCard, MatchSession session, int playerId, int instanceId, ref CommandResult results)
        {
            while (effectOpId != -1)
            {
                EffectOp op = skillCard.effects[effectOpId];

                // if the effect is judge, evaluate the judge result first, and then determine which node to go next based on the judge result
                if (op.type == EffectType.Judge)
                {
                    List<int> judgePool = ParticipantsResolver.DetermineCandidates(op.source, session.gameState, session.ctx, out bool _);
                    bool judgeResult = judgePool.Count > 0;
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
                    SendInvalidEvent(playerId, instanceId, results, InvalidActionType.InvalidTarget);
                    if (effectOpId != 0)
                        EndExecuteSkill(playerId, instanceId, session, results);
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

                if (op.trueNode == -1 && op.falseNode == -1) break;
                else if (op.trueNode != -1 && op.falseNode == -1) effectOpId = op.trueNode;
                else if (op.trueNode == -1 && op.falseNode != -1) effectOpId = op.falseNode;
                else effectOpId = op.trueNode;
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
                    instanceId
                ),
                -1
            ));

            SumPoint(session, ref results);
        }

        public static bool SpendActionPoint(int playerId, int instanceId, MatchSession session, CommandResult results, int apCount)
        {
            if (session.gameState.players[playerId].actionPoint >= apCount)
            {
                session.gameState.players[playerId].actionPoint -= apCount;
            }
            else
            {
                SendInvalidEvent(playerId, instanceId, results, InvalidActionType.NotEnoughAP);
                return false;
            }

            results.events.Enqueue(CommandHandler.MakeEvent(
                "SpendActionPoint",
                new SpendActionPointEvent    // need change
                (
                    playerId,
                    true
                ),
                -1
            ));
            return true;
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