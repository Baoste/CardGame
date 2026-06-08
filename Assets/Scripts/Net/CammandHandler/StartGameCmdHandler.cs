using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class StartGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartGameCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // TODO: 服务器端需要做什么
        if (session.gameState.isStart || session.gameState.readyToStartCount++ < 1)
            return results;   // 已经开始了，直接返回

        session.gameState.Start();

        // 初始化牌堆
        List<int> pointCardInstanceIds = new List<int>();
        List<int> skillCardInstanceIds = new List<int>();
        // instance id 绑定到 Card id
        foreach (int key in CardDatabase.GetKeys())
        {
            for (int i = 0; i < CardDatabase.Get(key).count; i++)
            {
                int instanceId = session.gameState.rng.Next(100000, int.MaxValue);
                session.instanceToCardId[instanceId] = key;
                switch (CardDatabase.Get(key).type)
                {
                    case CardType.Point:
                        pointCardInstanceIds.Add(instanceId);
                        break;
                    case CardType.Skill:
                        skillCardInstanceIds.Add(instanceId);
                        break;
                }
            }
        }

        StaticFunction.Shuffle(pointCardInstanceIds, session.gameState.rng);
        StaticFunction.Shuffle(skillCardInstanceIds, session.gameState.rng);

        var pointCardsDeck = session.gameState.pointCardsDeck;
        session.gameState.AddCardsToDeck(pointCardInstanceIds, session.instanceToCardId, CardType.Point);

        var skillCardsDeck = session.gameState.skillCardsDeck;
        session.gameState.AddCardsToDeck(skillCardInstanceIds, session.instanceToCardId, CardType.Skill);

        int skillCardCount = session.gameState.skillCardsDeck.GetCount();

        // return results
        results.events.Enqueue(MakeEvent(
            "StartGame",
            new StartGameEvent    // need change
            (
                payload.playerId,
                true,
                skillCardCount,
                session.gameState.GameRound
            ),
            -1
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
                CardVisualState.Hole,
                EffectAnimation.DrawPoint_Normal
            ),
            -1
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
                CardVisualState.Hole,
                EffectAnimation.DrawPoint_Normal
            ),
            -1
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
                CardVisualState.None,
                EffectAnimation.DrawPoint_Normal
            ),
            -1
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
                CardVisualState.None,
                EffectAnimation.DrawPoint_Normal
            ),
            -1
        ));

        // 抽技能牌
        for ( int i = 0; i < GameConfigLoader.Config.StartGame.startSkillCardCount; i++ )
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
                    drawCardInstanceId,
                    EffectAnimation.DrawSkill_Normal
                ),
            -1
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
                    drawCardInstanceId,
                    EffectAnimation.DrawSkill_Normal
                ),
            -1
            ));
        }

        // 分配庄闲
        int dealerId = session.gameState.dealerId == -1
            ? session.gameState.rng.Next(2)
            : 1 - session.gameState.dealerId;

        int punterId = 1 - dealerId;
        session.gameState.dealerId = dealerId;
        session.gameState.punterId = punterId;

        // Place bets
        int placeCount = Math.Max(session.gameState.GameRound - 1, 1);
        placeCount = Math.Min(
            placeCount, 
            Math.Min(session.gameState.players[0].chipCount, session.gameState.players[1].chipCount)
        );
        if (session.gameState.players[1 - payload.playerId].PlaceBets(placeCount) &&
            session.gameState.players[payload.playerId].PlaceBets(placeCount))
        {
            session.gameState.currentBet = placeCount;
        }

        // return
        results.events.Enqueue(MakeEvent(
            "AssignRoles",
            new AssignRolesEvent    // need change
            (
                payload.playerId,
                true,
                dealerId,
                punterId,
                placeCount
            ),
            -1
        ));

        NetEffectFunction.SumPoint(session, payload.playerId, ref results);

        return results;
    }
}
