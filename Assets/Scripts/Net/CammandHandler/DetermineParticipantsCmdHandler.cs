using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public sealed class DetermineParticipantsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<DetermineParticipantsCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        List<int> candidateSourceIds = ParticipantResolver.DetermineCandidates(payload.effect.source, session.gameState, session.ctx);
        List<int> candidateTargetIds = ParticipantResolver.DetermineCandidates(payload.effect.target, session.gameState, session.ctx);

        // return event
        CommandResult results = new CommandResult();

        results.events.Enqueue(MakeEvent(
            "GetGameState",
            new GetGameStateEvent    // need change
            {
                gameState = session.gameState,
            }
        ));

        results.events.Enqueue(MakeEvent(
            "GetCtx",
            new GetCtxEvent    // need change
            {
                ctx = session.ctx,
            }
        ));

        bool sourceNeedChoose = payload.effect.source.participantSelectionMode == ParticipantSelectionMode.Choose;
        bool targetNeedChoose = payload.effect.target.participantSelectionMode == ParticipantSelectionMode.Choose;

        results.events.Enqueue(MakeEvent(
            "DetermineParticipants",
            new DetermineParticipantsEvent    // need change
            {
                playerId = payload.playerId,
                sourceNeedChoose = sourceNeedChoose,
                targetNeedChoose = targetNeedChoose,
                candidateSourceIds = candidateSourceIds,
                candidateTargetIds = candidateTargetIds,
                sourceSelectCount = payload.effect.source.maxSelectCount.Evaluate(session.gameState, session.ctx, -1),
                targetSelectCount = payload.effect.target.maxSelectCount.Evaluate(session.gameState, session.ctx, -1),
            }
        ));
        return results;
    }
}
