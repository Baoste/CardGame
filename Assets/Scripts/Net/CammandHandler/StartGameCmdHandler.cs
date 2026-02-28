using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public class StartGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<StartGameCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        // 初始化牌堆
        List<int> pointCardIds = new List<int>();
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 10; j++)
                pointCardIds.Add(j);
        StaticFunction.Shuffle(pointCardIds, session.gameState.rng);

        var pointCardIdsInDeck = session.gameState.pointCardsDeck.cardIdsInDeck;
        pointCardIdsInDeck.Clear();
        foreach (var cardId in pointCardIds)
            pointCardIdsInDeck.Push(cardId);

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartGame",
            new StartGameEvent    // need change
            {
                playerId = payload.playerId,
            }
        ));

        // 抽牌
        int drawCardId = pointCardIdsInDeck.Pop();
        session.gameState.players[payload.playerId].PointCardsOnBoard.Add(drawCardId);
        results.events.Enqueue(MakeEvent(
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
