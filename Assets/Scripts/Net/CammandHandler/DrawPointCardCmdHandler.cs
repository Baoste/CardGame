using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawPointCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<DrawPointCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
        CardVisualState cardState = session.gameState.GetCardState(drawCardInstanceId);
        session.gameState.AddCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Point, cardState);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            (
                payload.playerId,
                drawCardInstanceId != -1,
                session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                drawCardInstanceId,
                cardState
            ),
            -1
        ));
        return results;
    }
}