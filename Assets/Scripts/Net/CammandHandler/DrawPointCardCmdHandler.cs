using Game.Domain;
using Game.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawPointCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<DrawPointCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
        session.gameState.players[payload.playerId].PointCardsOnBoard.Add(drawCardInstanceId);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
                isHoleCard = false
            }
        ));
        return results;
    }
}