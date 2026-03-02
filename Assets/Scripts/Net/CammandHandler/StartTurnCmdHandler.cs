using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTurnCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartTurnCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        int opponentId = 1 - payload.playerId;
        session.ctx.caster = payload.playerId;
        session.ctx.opponent = opponentId;

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartTurn",
            new StartTurnEvent    // need change
            {
                playerId = payload.playerId,
                opponentId = opponentId
            }
        ));

        return results;
    }
}
