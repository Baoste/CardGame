using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StartGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartGameCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.gameState.Init();

        // 初始化牌堆
        List<int> pointCardInstanceIds = new List<int>();
        List<int> skillCardInstanceIds = new List<int>();
        // instance id 绑定到 Card id
        int instanceIdCounter = 0;
        foreach (int key in CardDatabase.GetKeys())
        {
            for (int i = 0; i < CardDatabase.Get(key).count; i++)
            {
                session.instanceToCardId[instanceIdCounter] = key;
                switch (CardDatabase.Get(key).type)
                {
                    case CardType.Point:
                        pointCardInstanceIds.Add(instanceIdCounter);
                        break;
                    case CardType.Skill:
                        skillCardInstanceIds.Add(instanceIdCounter);
                        break;
                }
                instanceIdCounter++;
            }

        }

        StaticFunction.Shuffle(pointCardInstanceIds, session.gameState.rng);
        StaticFunction.Shuffle(skillCardInstanceIds, session.gameState.rng);

        var pointCardsDeck = session.gameState.pointCardsDeck;
        pointCardsDeck._Add(pointCardInstanceIds);

        var skillCardsDeck = session.gameState.skillCardsDeck;
        skillCardsDeck._Add(skillCardInstanceIds);

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
        int drawCardInstanceId = -1;
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.players[payload.playerId].holeCard = drawCardInstanceId;
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
                isHoleCard = true
            }
        ));
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.players[1 - payload.playerId].holeCard = drawCardInstanceId;
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            {
                playerId = 1 - payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
                isHoleCard = true
            }
        ));

        // 抽技能牌
        drawCardInstanceId = skillCardsDeck.Draw();
        session.gameState.AddCard(payload.playerId, drawCardInstanceId, CardType.Skill);
        results.events.Enqueue(MakeEvent(
            "DrawSkillCard",
            new DrawSkillCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
            }
        ));
        drawCardInstanceId = skillCardsDeck.Draw();
        session.gameState.AddCard(1- payload.playerId, drawCardInstanceId, CardType.Skill);
        results.events.Enqueue(MakeEvent(
            "DrawSkillCard",
            new DrawSkillCardEvent    // need change
            {
                playerId = 1 - payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
            }
        ));

        return results;
    }
}
