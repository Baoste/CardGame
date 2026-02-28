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
        int drawCardId = UnityEngine.Random.Range(0, 25);
        session.gameState.players[payload.playerId].PointCardsOnBoard.Add(drawCardId);

        // return
        CommandResult results = new CommandResult();
        results.events.Add(MakeEvent(
            "DrawCard",
            new DrawCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = drawCardId
            }
        ));
        return results;
    }
}