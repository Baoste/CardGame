using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class Place1BetCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<Place1BetCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        if (session.gameState.players[1 - payload.playerId].Place1Bet() &&
            session.gameState.players[payload.playerId].Place1Bet())
        {
            session.gameState.currentBet++;
        }
        // END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "Place1Bet",
            new Place1BetEvent
            (
                payload.playerId,
                true
            ),
            -1
        ));
        return results;
    }
}
