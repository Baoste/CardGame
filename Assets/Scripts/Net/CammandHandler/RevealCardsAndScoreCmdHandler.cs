using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class RevealCardsAndScoreCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<RevealCardsAndScoreCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int playerPoints = session.gameState.SumPoint(payload.playerId);
        int opponentPoints = session.gameState.SumPoint(1 - payload.playerId);
        int winnerId = playerPoints > opponentPoints ? payload.playerId : 1 - payload.playerId;

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "RevealCardsAndScore",
            new RevealCardsAndScoreEvent    // need change
            (
                payload.playerId,
                true,
                winnerId
            )
        ));
        return results;
    }
}