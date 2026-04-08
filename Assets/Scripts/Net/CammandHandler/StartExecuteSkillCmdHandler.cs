using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class StartExecuteSkillCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartExecuteSkillCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // START
        int skillCardId = session.instanceToCardId[payload.instanceId];
        Card skillCard = CardDatabase.Get(skillCardId);
        if (skillCard == null)
        {
            // TODO: error handling
        }
        else
        {
            if (!NetEffectFunction.SpendActionPoint(payload.playerId, payload.instanceId, session, results, 1))
                return results;

            // execute skill card effects
            int effectOpId = 0;
            while (effectOpId != -1)
            {
                EffectOp op = skillCard.effects[effectOpId];

                // if the effect is judge, evaluate the judge result first, and then determine which node to go next based on the judge result
                if (op.type == EffectType.Judge)
                {
                    List<int> judgePool = ParticipantsResolver.DetermineCandidates(op.source, session.gameState, session.ctx, out bool _);
                    bool judgeResult = judgePool.Count > 0;
                    if (judgeResult)    effectOpId = op.trueNode;
                    else                effectOpId = op.falseNode;
                    continue;
                }

                bool success = ParticipantsResolver.ReturnCandidates(
                    op.source, op.target, session.gameState, session.ctx,
                    out List<int> candidateSourceIds, out List<int> candidateTargetIds,
                    out bool isSourceParticipantZone, out bool isTargetParticipantZone
                );
                if (!success)
                {
                    NetEffectFunction.SendInvalidEvent(payload.playerId, payload.instanceId, results, InvalidActionType.InvalidTarget);
                    if (effectOpId != 0 )
                        NetEffectFunction.EndExecuteSkill(payload.playerId, payload.instanceId, session, results);
                    return results;
                }


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
                    results.events.Enqueue(MakeEvent(
                        "WaitForPlayer2Choose",
                        new WaitForPlayer2ChooseEvent
                        (
                            payload.playerId,
                            success,
                            payload.instanceId,
                            sourceNeedChoose,
                            targetNeedChoose,
                            isSourceParticipantZone,
                            isTargetParticipantZone,
                            candidateSourceIds,
                            candidateTargetIds,
                            op.source.maxSelectCount.Evaluate(session.gameState, session.ctx),
                            op.target.maxSelectCount.Evaluate(session.gameState, session.ctx)
                        ),
                        payload.playerId
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

                    NetEffectExecutor.ExecuteOp(payload.playerId, op, session, results, selectedSourceIds, selectedTargetIds);
                }

                if (op.trueNode == -1 && op.falseNode == -1) break;
                else if (op.trueNode != -1 && op.falseNode == -1) effectOpId = op.trueNode;
                else if (op.trueNode == -1 && op.falseNode != -1) effectOpId = op.falseNode;
                else effectOpId = op.trueNode;
            }
        }

        if (session.ctx.opStack.Count == 0)
        {
            NetEffectFunction.EndExecuteSkill(payload.playerId, payload.instanceId, session, results);
        }
        //END

        return results;
    }
}
