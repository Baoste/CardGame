using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCardStateCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<ChangeCardStateCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        bool success = session.gameState.SetCardState(
            payload.instanceId,
            payload.cardState
        );

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ChangeCardState",
            new ChangeCardStateEvent    // need change
            (
                payload.playerId,
                success,
                payload.instanceId,
                payload.cardState
            )
        ));
        return results;
    }
}