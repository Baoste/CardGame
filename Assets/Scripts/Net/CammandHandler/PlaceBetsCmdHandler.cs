using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System;

public class PlaceBetsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<PlaceBetsCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // TODO
        // START
        int apCount = ++session.gameState.placeBetTimes;
        if (!NetEffectFunction.SpendActionPoint(payload.playerId, -1, session, results, apCount))
        {
            results.events.Enqueue(MakeEvent(
                "PlaceBets",
                new PlaceBetsEvent
                (
                    payload.playerId,
                    true,
                    new int[] { }
                ),
                -1
            ));
            return results;
        }

        int count = Math.Min(
            payload.instanceIds.Length, 
            Math.Min(session.gameState.players[1 - payload.playerId].chipCount, session.gameState.players[payload.playerId].chipCount)
        );
        if (session.gameState.players[1 - payload.playerId].PlaceBets(count) &&
            session.gameState.players[payload.playerId].PlaceBets(count))
        {
            session.gameState.currentBet += count;
        }
        // END

        // need change
        Array.Sort(payload.instanceIds);
        results.events.Enqueue(MakeEvent(
            "PlaceBets",
            new PlaceBetsEvent
            (
                payload.playerId,
                true,
                payload.instanceIds[^count..]
            ),
            -1
        ));
        return results;
    }
}
