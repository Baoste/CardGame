using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawCardCmdHandler : ICommandHandler
{
    public ResolvedEvent Handle(Command cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<DrawCardCommand>(cmd.jsonData);

        // TODO 服务器端需要做什么

        // return
        var ev = new DrawCardCommand    // need change
        {
            PlayerId = payload.PlayerId,
        };

        return new ResolvedEvent
        {
            type = "DrawCard",  // need change
            jsonData = JsonUtility.ToJson(ev)
        };
    }
}