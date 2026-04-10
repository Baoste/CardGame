using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class ValidateActionPointCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<ValidateActionPointCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        bool success = session.gameState.players[payload.playerId].actionPoint >= payload.apCount;

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ValidateActionPoint",
            new ValidateActionPointEvent    // need change
            (
                payload.playerId,
                success
            ),
            -1
        ));
        return results;
    }
}
