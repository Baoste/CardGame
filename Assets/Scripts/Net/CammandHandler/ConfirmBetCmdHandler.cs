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
            int winnerId = 1 - payload.playerId;
            int currentBet = session.gameState.currentBet - payload.betCount;
            int playerPoints = session.gameState.SumPoint(payload.playerId, out int _, out bool _);
            int opponentPoints = session.gameState.SumPoint(1 - payload.playerId, out int _, out bool _);
            session.gameState.players[winnerId].chipCount += session.gameState.currentBet + currentBet;
            session.gameState.players[1 - winnerId].chipCount += session.gameState.currentBet - currentBet;

            session.gameState.Dispose();

            results.events.Enqueue(MakeEvent(
                "RevealCardsAndScore",
                new RevealCardsAndScoreEvent    // need change
                (
                    payload.playerId,
                    true,
                    winnerId,
                    currentBet,
                    playerPoints,
                    opponentPoints
                ),
                -1
            ));

            NetEffectFunction.EndMatch(session, payload.playerId, winnerId, currentBet, ref results);
        }    
        //END

        return results;
    }
}
