using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SpendActionPointCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<SpendActionPointCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        bool success = false;
        if (session.gameState.players[payload.playerId].actionPoint >= payload.apCount)
        {
            session.gameState.players[payload.playerId].actionPoint -= payload.apCount;
            success = true;
        }
        

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "SpendActionPoint",
            new SpendActionPointEvent    // need change
            (
                payload.playerId,
                success,
                payload.apCount
            ),
            -1
        ));
        return results;
    }
}
