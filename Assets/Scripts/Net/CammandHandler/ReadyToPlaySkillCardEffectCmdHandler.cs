using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadyToPlaySkillCardEffectCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<ReadyToPlaySkillCardEffectCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.ctx.caster = payload.playerId;
        session.ctx.opponent = 1 - payload.playerId;
        ParticipantResolver.ValidateParticipant(payload.effect.source, session.gameState, session.ctx, out var sourceIds);
        ParticipantResolver.ValidateParticipant(payload.effect.target, session.gameState, session.ctx, out var targetIds);

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

        results.events.Enqueue(MakeEvent(
            "ReadyToPlaySkillCardEffect",
            new ReadyToPlaySkillCardEffectEvent    // need change
            {
                playerId = payload.playerId,
                candidateSourceIds = sourceIds,
                candidateTargetIds = targetIds
            }
        ));
        return results;
    }
}
