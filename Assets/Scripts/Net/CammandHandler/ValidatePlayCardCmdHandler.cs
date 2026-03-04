using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidatePlayCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ValidatePlayCardCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        // 验证 payload 里的 playerId 和 cardId 是否有效，是否符合游戏规则（比如玩家是否有这张牌，牌能否在当前阶段打出等等）
        ParticipantSpec source = payload.effect.source;
        List<int> pool = ParticipantResolver.DetermineCandidates(source, session.gameState, session.ctx);
        bool success0 = source.participantSelectionMode.ValidatePool(pool, payload.effect.source.maxSelectCount.Evaluate(session.gameState, session.ctx));

        ParticipantSpec target = payload.effect.target;
        pool = ParticipantResolver.DetermineCandidates(target, session.gameState, session.ctx);
        bool success1 = target.participantSelectionMode.ValidatePool(pool, payload.effect.target.maxSelectCount.Evaluate(session.gameState, session.ctx));

        bool success = success0 && success1;
        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ValidatePlayCard",
            new ValidatePlayCardEvent    // need change
            {
                playerId = payload.playerId,
                success = success,
            }
        ));

        return results;
    }
}
