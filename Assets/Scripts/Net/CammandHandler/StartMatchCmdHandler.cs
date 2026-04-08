using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class StartMatchCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartMatchCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        session.gameState.Init(payload.seed);
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartMatch",
            new StartMatchEvent
            (
                payload.playerId,
                true,
                payload.seed
            ),
            -1
        ));
        return results;
    }
}