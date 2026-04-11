using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class StartTurnCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartTurnCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.gameState.Turn++;
        session.gameState.CurrentPlayerId = payload.playerId;

        int opponentId = 1 - payload.playerId;
        session.ctx.caster = payload.playerId;
        session.ctx.opponent = opponentId;
        session.ctx.opStack.Clear();

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartTurn",
            new StartTurnEvent    // need change
            (
                payload.playerId,
                true,
                opponentId,
                session.gameState.Turn
            ),
            -1
        ));

        session.gameState.players[payload.playerId].actionPoint = 1;
        results.events.Enqueue(MakeEvent(
            "AddActionPoint",
            new AddActionPointEvent    // need change
            (
                payload.playerId,
                true,
                1,
                true
            ),
            -1
        ));

        results.events.Enqueue(MakeEvent(
            "GetGameState",
            new GetGameStateEvent    // need change
            (
                payload.playerId,
                true,
                session.gameState
            ),
            -1
        ));

        return results;
    }
}
