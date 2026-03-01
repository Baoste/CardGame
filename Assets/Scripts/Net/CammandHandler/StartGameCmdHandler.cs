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
        List<int> pointCardInstanceIds = new List<int>();
        // instance id 绑定到 Card id
        // tmp 假设card id是0-9
        // TODO: 读取数据库里的牌堆配置，再生成牌堆
        List<int> cardTMP = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        for (int i = 0; i < 2 * cardTMP.Count; i++)
        {
            session.instanceToCardId[i] = cardTMP[i/2];
            pointCardInstanceIds.Add(i);
        }
        StaticFunction.Shuffle(pointCardInstanceIds, session.gameState.rng);

        var pointCardinstanceIdsInDeck = session.gameState.pointCardsDeck.instanceIdsInDeck;
        pointCardinstanceIdsInDeck.Clear();
        foreach (var instanceId in pointCardInstanceIds)
            pointCardinstanceIdsInDeck.Push(instanceId);

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartGame",
            new StartGameEvent    // need change
            {
                playerId = payload.playerId,
            }
        ));

        // 抽底牌
        int drawCardInstanceId = pointCardinstanceIdsInDeck.Pop();
        session.gameState.players[payload.playerId].holeCard = drawCardInstanceId;
        results.events.Enqueue(MakeEvent(
            "DrawCard",
            new DrawCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
                isHoleCard = true
            }
        ));

        return results;
    }
}
