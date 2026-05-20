using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class StartMatchCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartMatchCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // TODO
        // START
        session.ReadyCount += 1;

        if (session.ReadyCount == 2)
        {
            session.gameState.Init(payload.seed);
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
        }

        return results;
    }
}