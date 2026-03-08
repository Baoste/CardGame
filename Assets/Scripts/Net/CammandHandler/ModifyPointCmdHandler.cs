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
        bool success = session.gameState.instancePointMap.TryGetValue(1, out int value);
        if (success)
            session.gameState.instancePointMap[payload.instanceId] = value + payload.pointChange;

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ModifyPoint",
            new ModifyPointEvent    // need change
            (
                payload.playerId,
                success,
                payload.instanceId,
                payload.pointChange
            )
        ));
        return results;
    }
}