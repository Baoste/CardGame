using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class AssignRolesCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<AssignRolesCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int dealerId = session.gameState.dealerId == -1
            ? session.gameState.rng.Next(2)
            : 1 - session.gameState.dealerId;

        int punterId = 1 - dealerId;
        session.gameState.dealerId = dealerId;
        session.gameState.punterId = punterId;

        // Place bets
        int placeCount = Math.Max(session.gameState.GameRound - 1, 1);
        placeCount = Math.Min(
            placeCount,
            Math.Min(session.gameState.players[0].chipCount, session.gameState.players[1].chipCount)
        );
        if (session.gameState.players[1 - payload.playerId].PlaceBets(placeCount) &&
            session.gameState.players[payload.playerId].PlaceBets(placeCount))
        {
            session.gameState.currentBet = placeCount;
        }

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "AssignRoles",
            new AssignRolesEvent    // need change
            (
                payload.playerId,
                true,
                dealerId,
                punterId,
                placeCount
            ),
            -1
        ));
        return results;
    }
}