using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawCardCmdHandler : EventHandler, ICommandHandler
{
    public ResolvedEvent Handle(Command cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<DrawCardCommand>(cmd.jsonData);

        // TODO 服务器端需要做什么

        // return
        var ev = new DrawCardEvent    // need change
        {
            playerId = payload.playerId,
            cardId = UnityEngine.Random.Range(0, 25)
        };

        return new ResolvedEvent
        {
            type = "DrawCard",  // need change
            jsonData = JsonUtility.ToJson(ev)
        };
    }
}