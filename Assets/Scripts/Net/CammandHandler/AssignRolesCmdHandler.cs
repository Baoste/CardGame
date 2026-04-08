using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AssignRolesCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<AssignRolesCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int dealerId = session.gameState.rng.Next(2);
        int punterId = 1 - dealerId;
        session.gameState.dealerId = dealerId;
        session.gameState.punterId = punterId;

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "AssignRoles",
            new AssignRolesEvent    // need change
            (
                payload.playerId,
                true,
                dealerId,
                punterId
            ),
            -1
        ));
        return results;
    }
}