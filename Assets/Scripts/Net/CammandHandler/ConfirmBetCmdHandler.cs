using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class ConfirmBetCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ConfirmBetCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        int betCount = session.gameState.currentBet;
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ConfirmBet",
            new ConfirmBetEvent
            (
                payload.playerId,
                true,
                betCount
            )
        ));
        return results;
    }
}
