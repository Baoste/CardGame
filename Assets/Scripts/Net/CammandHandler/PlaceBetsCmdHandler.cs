using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class PlaceBetsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<PlaceBetsCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // TODO
        // START
        int apCount = session.gameState.currentBet / 2 + 1;
        if (!NetEffectFunction.SpendActionPoint(payload.playerId, -1, session, results, apCount))
            return results;

        int count = payload.instanceIds.Length;
        if (session.gameState.players[1 - payload.playerId].PlaceBets(count) &&
            session.gameState.players[payload.playerId].PlaceBets(count))
        {
            session.gameState.currentBet += count;
        }
        // END

        // need change
        results.events.Enqueue(MakeEvent(
            "PlaceBets",
            new PlaceBetsEvent
            (
                payload.playerId,
                true,
                payload.instanceIds
            ),
            -1
        ));
        return results;
    }
}
