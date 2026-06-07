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
        CommandResult results = new CommandResult();

        // TODO: 服务器端需要做什么
        int winnerId;

        int playerPoints = session.gameState.SumPoint(payload.playerId, out int _, out bool _);
        int opponentPoints = session.gameState.SumPoint(1 - payload.playerId, out int _, out bool _);
        bool playerBust = playerPoints > 21;
        bool opponentBust = opponentPoints > 21;

        if (playerPoints == opponentPoints)
        {
            winnerId = session.gameState.dealerId;
        }
        else if (playerBust && opponentBust)
        {
            // 都爆，点数小的赢
            winnerId = playerPoints < opponentPoints ? payload.playerId : 1 - payload.playerId;
        }
        else if (playerBust)
        {
            // 你爆了，对面赢
            winnerId = 1 - payload.playerId;
        }
        else if (opponentBust)
        {
            // 对面爆了，你赢
            winnerId = payload.playerId;
        }
        else
        {
            // 都没爆，比大小
            winnerId = playerPoints > opponentPoints ? payload.playerId : 1 - payload.playerId;
        }

        int currentBet = session.gameState.currentBet;
        session.gameState.Dispose();
        session.gameState.players[winnerId].chipCount += 2 * currentBet;
        // session.gameState.players[1 - winnerId].chipCount -= currentBet; // 输家扣除筹码在 PlaceBets 阶段已经处理了

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

        NetEffectFunction.EndMatch(session, payload.playerId, winnerId, ref results);

        return results;
    }
}