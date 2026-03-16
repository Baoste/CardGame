using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public sealed class DetermineParticipantsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<DetermineParticipantsCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        ParticipantSpec source = payload.effect.source;
        List<int> candidateSourceIds = ParticipantResolver.DetermineCandidates(source, session.gameState, session.ctx, out bool isSourceParticipantZone);
        bool success0 = source.participantSelectionMode.ValidatePool(candidateSourceIds, source.maxSelectCount.Evaluate(session.gameState, session.ctx));

        ParticipantSpec target = payload.effect.target;
        List<int> candidateTargetIds = ParticipantResolver.DetermineCandidates(target, session.gameState, session.ctx, out bool isTargetParticipantZone);
        bool success1 = target.participantSelectionMode.ValidatePool(candidateTargetIds, target.maxSelectCount.Evaluate(session.gameState, session.ctx));

        bool success = success0 && success1;

        session.ctx.candidateSourceIds = candidateSourceIds;
        session.ctx.candidateTargetIds = candidateTargetIds;

        bool sourceNeedChoose = payload.effect.source.participantSelectionMode is SelectionModeChoose;
        bool targetNeedChoose = payload.effect.target.participantSelectionMode is SelectionModeChoose;

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

        // 如果是抽点数牌到待处理区，需要先把牌抽出来
        if (payload.effect.type == EffectType.DrawPointToResolve)
        {
            int count = payload.effect.source.maxSelectCount.Evaluate(session.gameState, session.ctx);
            for (int i = 0; i < count; i++)
            {
                int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
                int drawCardId = session.instanceToCardId[drawCardInstanceId];
                session.gameState.AddToResolve(drawCardId, drawCardInstanceId);

                results.events.Enqueue(MakeEvent(
                    "DrawPointCardToResolve",
                    new DrawPointCardToResolveEvent    // need change
                    (
                        payload.playerId,
                        success,
                        drawCardId,
                        drawCardInstanceId
                    )
                ));
            }
        }

        // 如果是抽点数牌到待处理区，需要先把牌抽出来
        bool judgeResult = true;
        if (payload.effect.type == EffectType.Judge)
        {
            judgeResult = payload.effect.source.filter.Evaluate(session.gameState, session.ctx);
        }

        results.events.Enqueue(MakeEvent(
            "DetermineParticipants",
            new DetermineParticipantsEvent    // need change
            (
                payload.playerId,
                success,
                judgeResult,
                sourceNeedChoose,
                targetNeedChoose,
                isSourceParticipantZone,
                isTargetParticipantZone,
                candidateSourceIds,
                candidateTargetIds,
                payload.effect.source.maxSelectCount.Evaluate(session.gameState, session.ctx),
                payload.effect.target.maxSelectCount.Evaluate(session.gameState, session.ctx)
            )
        ));
        return results;
    }
}
