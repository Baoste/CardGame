using Game.Domain;
using Game.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<DrawCardCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么
        int drawCardInstanceId = -1;
        if (session.gameState.pointCardsDeck.instanceIdsInDeck.Count > 0)
        {
            drawCardInstanceId = session.gameState.pointCardsDeck.instanceIdsInDeck.Pop();
            session.gameState.players[payload.playerId].PointCardsOnBoard.Add(drawCardInstanceId);
        }

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "DrawCard",
            new DrawCardEvent    // need change
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