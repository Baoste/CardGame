using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public sealed class DetermineParticipantsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<DetermineParticipantsCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        ParticipantSpec source = payload.effect.source;
        List<int> candidateSourceIds = ParticipantResolver.DetermineCandidates(source, session.gameState, session.ctx);
        bool success0 = source.participantSelectionMode.ValidatePool(candidateSourceIds, source.maxSelectCount.Evaluate(session.gameState, session.ctx));

        ParticipantSpec target = payload.effect.target;
        List<int> candidateTargetIds = ParticipantResolver.DetermineCandidates(target, session.gameState, session.ctx);
        bool success1 = target.participantSelectionMode.ValidatePool(candidateTargetIds, target.maxSelectCount.Evaluate(session.gameState, session.ctx));

        bool success = success0 && success1;

        session.ctx.candidateSourceIds = candidateSourceIds;
        session.ctx.candidateTargetIds = candidateTargetIds;

        // return event
        CommandResult results = new CommandResult();

        results.events.Enqueue(MakeEvent(
            "GetGameState",
            new GetGameStateEvent    // need change
            (
                payload.playerId,
                true,
                session.gameState
            )
        ));

        results.events.Enqueue(MakeEvent(
            "GetCtx",
            new GetCtxEvent    // need change
            (
                payload.playerId,
                true,
                session.ctx
            )
        ));

        bool sourceNeedChoose = payload.effect.source.participantSelectionMode is SelectionModeChoose;
        bool targetNeedChoose = payload.effect.target.participantSelectionMode is SelectionModeChoose;

        results.events.Enqueue(MakeEvent(
            "DetermineParticipants",
            new DetermineParticipantsEvent    // need change
            (
                payload.playerId,
                success,
                sourceNeedChoose,
                targetNeedChoose,
                candidateSourceIds,
                candidateTargetIds,
                payload.effect.source.maxSelectCount.Evaluate(session.gameState, session.ctx),
                payload.effect.target.maxSelectCount.Evaluate(session.gameState, session.ctx)
            )
        ));
        return results;
    }
}
