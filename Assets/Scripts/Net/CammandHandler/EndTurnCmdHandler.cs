using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class EndTurnCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<EndTurnCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.ctx.ClearContext();
        session.gameState.CurrentPlayerId = 1 - payload.playerId;

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "EndTurn",
            new EndTurnEvent    // need change
            {
                playerId = payload.playerId,
                opponentId = 1 - payload.playerId
            }
        ));

        return results;
    }
}
