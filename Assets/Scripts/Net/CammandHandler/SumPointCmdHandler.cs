using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class SumPointCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<SumPointCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int playerPoints = session.gameState.SumPoint(payload.playerId, out int playerOnBoardPoints, out bool hasHiddenCard);
        int opponentPoints = session.gameState.SumPoint(1 - payload.playerId, out int opponentOnBoardPoints, out bool _);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "SumPoint",
            new SumPointEvent    // need change
            (
                payload.playerId,
                true,
                playerOnBoardPoints,
                session.gameState.GetHoleCardPoint(payload.playerId),
                hasHiddenCard,
                opponentOnBoardPoints
            ),
            payload.playerId
        ));
        return results;
    }
}
