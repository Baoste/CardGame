using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PeekTopCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<PeekTopCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么        
        CommandResult results = new CommandResult();

        for (int i = 0; i < payload.count; i++)
        {
            int drawCardInstanceId = session.gameState.pointCardsDeck.Peek(i);
            int drawCardId = session.instanceToCardId[drawCardInstanceId];

            results.events.Enqueue(MakeEvent(
                "PeekTopCard",
                new PeekTopCardEvent    // need change
                (
                    payload.playerId,
                    true,
                    drawCardId,
                    drawCardInstanceId
                ),
                -1
            ));
        }

        // return
        return results;
    }
}