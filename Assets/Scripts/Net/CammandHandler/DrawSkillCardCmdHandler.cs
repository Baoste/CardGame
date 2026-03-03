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

        // TODO: 服务器端需要做什么
        int drawCardInstanceId = session.gameState.skillCardsDeck.Draw();
        session.gameState.AddCard(payload.playerId, drawCardInstanceId, CardType.Skill);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "DrawSkillCard",
            new DrawSkillCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
                instanceId = drawCardInstanceId,
            }
        ));
        return results;
    }
}
