using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class ModifyPointCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<ModifyPointCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        session.gameState.instancePointMap[payload.instanceId] += payload.pointChange;

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ModifyPoint",
            new ModifyPointEvent    // need change
            {
                playerId = payload.playerId,
                instanceId = payload.instanceId,
                pointChange = payload.pointChange
            }
        ));
        return results;
    }
}