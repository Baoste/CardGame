using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DrawCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<DrawCardCommand>(cmd.jsonData);

        // TODO 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        results.events.Add(MakeEvent(
            "DrawCard",
            new DrawCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = UnityEngine.Random.Range(0, 25)
            }
        ));
        return results;
    }
}