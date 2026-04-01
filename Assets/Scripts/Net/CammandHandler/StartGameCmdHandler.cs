using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class StartGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartGameCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        session.gameState.Init(payload.seed);

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
        session.gameState.AddCardsToDeck(pointCardInstanceIds, CardType.Point);

        var skillCardsDeck = session.gameState.skillCardsDeck;
        session.gameState.AddCardsToDeck(skillCardInstanceIds, CardType.Skill);

        // return results
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "StartGame",
            new StartGameEvent    // need change
            (
                payload.playerId,
                true,
                payload.seed
            )
        ));

        // 抽底牌
        int drawCardInstanceId = -1;
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.AddHoleCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId);
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            (
                payload.playerId,
                drawCardInstanceId != -1,
                session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                drawCardInstanceId,
                true
            )
        ));
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.AddHoleCard(1 - payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId);
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            (
                1 - payload.playerId,
                drawCardInstanceId != -1,
                session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                drawCardInstanceId,
                true
            )
        ));

        // 抽一张明牌
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.AddCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Point);
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            (
                payload.playerId,
                drawCardInstanceId != -1,
                session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                drawCardInstanceId,
                false
            )
        ));
        drawCardInstanceId = pointCardsDeck.Draw();
        session.gameState.AddCard(1 - payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Point);
        results.events.Enqueue(MakeEvent(
            "DrawPointCard",
            new DrawPointCardEvent    // need change
            (
                1 - payload.playerId,
                drawCardInstanceId != -1,
                session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                drawCardInstanceId,
                false
            )
        ));

        // 抽技能牌
        for ( int i = 0; i < 4; i++ )
        {
            drawCardInstanceId = skillCardsDeck.Draw();
            session.gameState.AddCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Skill);
            results.events.Enqueue(MakeEvent(
                "DrawSkillCard",
                new DrawSkillCardEvent    // need change
                (
                    payload.playerId,
                    drawCardInstanceId != -1,
                    session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                    drawCardInstanceId
                )
            ));
            drawCardInstanceId = skillCardsDeck.Draw();
            session.gameState.AddCard(1 - payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Skill);
            results.events.Enqueue(MakeEvent(
                "DrawSkillCard",
                new DrawSkillCardEvent    // need change
                (
                    1 - payload.playerId,
                    drawCardInstanceId != -1,
                    session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                    drawCardInstanceId
                )
            ));
        }

        return results;
    }
}
