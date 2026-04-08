using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class AddActionPointCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<AddActionPointCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        if (session.gameState.players[payload.playerId].actionPoint < 3)
            session.gameState.players[payload.playerId].actionPoint += 1;

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "AddActionPoint",
            new AddActionPointEvent    // need change
            (
                payload.playerId,
                true
            ),
            -1
        ));

        return results;
    }
}
