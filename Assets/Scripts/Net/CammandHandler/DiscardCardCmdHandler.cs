using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DiscardCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<DiscardCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int cardInstanceId = payload.instanceId;
        bool success = session.gameState.RemoveCard(cardInstanceId);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "DiscardCard",
            new DiscardCardEvent    // need change
            (
                payload.playerId,
                success,
                new List<int> { cardInstanceId }
            ),
            -1
        ));
        return results;
    }
}
