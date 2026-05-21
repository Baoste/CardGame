using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSkillCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<DrawSkillCardCommand>(cmd.jsonData);
        CommandResult results = new CommandResult();

        // TODO: 服务器端需要做什么
        // 如果牌堆空了，返回错误事件
        if (session.gameState.skillCardsDeck.GetCount() <= 0)
        {
            NetEffectFunction.SendInvalidEvent(payload.playerId, -1, results, InvalidActionType.SkillCardCountEmpty);
            return results;
        }

        // 如果玩家手牌满了，返回错误事件
        if (!NetEffectFunction.ValidSkillCardCount(payload.playerId, -1, session, results))
            return results;

        if (!NetEffectFunction.SpendActionPoint(payload.playerId, -1, session, results, 1))
            return results;

        int drawCardInstanceId = session.gameState.skillCardsDeck.Draw();
        session.gameState.AddCard(payload.playerId, session.instanceToCardId[drawCardInstanceId], drawCardInstanceId, CardType.Skill);

        // return
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
        return results;
    }
}
