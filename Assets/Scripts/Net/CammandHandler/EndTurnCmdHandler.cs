using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System;

public class EndTurnCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<EndTurnCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.ctx.ClearContext();
        session.gameState.CurrentPlayerId = 1 - payload.playerId;

        bool reveal = false;
        int endTurnCount = 8;
        if (session.gameState.Turn > endTurnCount)
        {
            float p = 1f - 0.5f * MathF.Exp(-(session.gameState.Turn - endTurnCount - 1) * 0.22f);
            p = Math.Min(p, 1f);

            float r = (float)session.gameState.rng.NextDouble();
            reveal = r < p;
        }

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "EndTurn",
            new EndTurnEvent    // need change
            (
                payload.playerId,
                true,
                1 - payload.playerId,
                reveal
            )
        ));

        return results;
    }
}
