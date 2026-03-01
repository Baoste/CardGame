using Game.Domain;
using Game.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameStateCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<GetGameStateCommand>(cmd.jsonData);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "GetGameState",
            new GetGameStateEvent    // need change
            {
                gameState = session.gameState,
            }
        ));
        return results;
    }
}
