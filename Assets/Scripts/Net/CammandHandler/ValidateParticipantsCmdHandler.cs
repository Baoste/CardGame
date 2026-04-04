using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ValidateParticipantsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ValidateParticipantsCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        // 验证 payload 里的 playerId 和 cardId 是否有效，是否符合游戏规则（比如玩家是否有这张牌，牌能否在当前阶段打出等等）
        bool isParticipantZone;
        int count;
        bool success0, success1;
        List<int> selectedSourceIds, selectedTargetIds;

        ParticipantSpec source = payload.effect.source;
        List<int> pool = ParticipantResolver.DetermineCandidates(source, session.gameState, session.ctx, out isParticipantZone);
        if (isParticipantZone)
        {
            success0 = true;
            selectedSourceIds = payload.selectedSourceIds;
        }
        else
        { 
            count = source.maxSelectCount.Evaluate(session.gameState, session.ctx);
            selectedSourceIds = source.participantSelectionMode.Execute(session.gameState, pool, count, payload.selectedSourceIds);
            success0 = source.participantSelectionMode.ValidateSelected(pool, selectedSourceIds);
        }

        ParticipantSpec target = payload.effect.target;
        pool = ParticipantResolver.DetermineCandidates(target, session.gameState, session.ctx, out isParticipantZone);
        if (isParticipantZone)
        {
            success1 = true;
            selectedTargetIds = payload.selectedTargetIds;
        }
        else
        {
            count = target.maxSelectCount.Evaluate(session.gameState, session.ctx);
            selectedTargetIds = target.participantSelectionMode.Execute(session.gameState, pool, count, payload.selectedTargetIds);
            success1 = target.participantSelectionMode.ValidateSelected(pool, selectedTargetIds);
        }

        bool success = success0 && success1;
        session.ctx.selectedSourceIds = selectedSourceIds;
        session.ctx.selectedTargetIds = selectedTargetIds;
        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ValidateParticipants",
            new ValidateParticipantsEvent    // need change
            (
                payload.playerId,
                success,
                selectedSourceIds,
                selectedTargetIds
            )
        ));

        return results;
    }
}
