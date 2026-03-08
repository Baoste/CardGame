using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class GetGameStateCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<GetGameStateCommand>(cmd.jsonData);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "GetGameState",
            new GetGameStateEvent    // need change
            (
                payload.playerId,
                true,
                session.gameState
            )
        ));
        return results;
    }
}
