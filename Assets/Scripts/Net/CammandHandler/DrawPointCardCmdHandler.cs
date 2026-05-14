using FishNet.Demo.AdditiveScenes;
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
        CommandResult results = new CommandResult();

        // TODO: 服务器端需要做什么
        //if (!NetEffectFunction.SpendActionPoint(payload.playerId, -1, session, results, 1))
        //    return results;

        // 只能抽一次
        if (session.ctx.drawPointCardCount >= 1)
        { 
            NetEffectFunction.SendInvalidEvent(payload.playerId, -1, results, InvalidActionType.PointCardDrawLimit);
            return results;
        }

        int drawCardInstanceId = session.gameState.pointCardsDeck.Draw();
        CardVisualState cardState = session.gameState.GetCardState(drawCardInstanceId);
        session.gameState.AddCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Point, cardState);

        session.ctx.drawPointCardCount++;

        // return
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
        NetEffectFunction.SumPoint(session, ref results);
        return results;
    }
}