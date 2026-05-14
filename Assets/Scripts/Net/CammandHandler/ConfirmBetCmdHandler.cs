using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class ConfirmBetCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ConfirmBetCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // TODO
        // START
        if (payload.isCall)
        {
            results.events.Enqueue(MakeEvent(
                "ConfirmBet",
                new ConfirmBetEvent
                (
                    payload.playerId,
                    true,
                    payload.betCount
                ),
                -1
            ));
        }
        else
        {
            int currentBet = session.gameState.currentBet;
            int playerPoints = session.gameState.SumPoint(payload.playerId, out int _, out bool _);
            int opponentPoints = session.gameState.SumPoint(1 - payload.playerId, out int _, out bool _);

            results.events.Enqueue(MakeEvent(
                "RevealCardsAndScore",
                new RevealCardsAndScoreEvent    // need change
                (
                    payload.playerId,
                    true,
                    1 - payload.playerId,
                    currentBet - 1,
                    playerPoints,
                    opponentPoints
                ),
                -1
            ));
        }    
        //END

        return results;
    }
}
