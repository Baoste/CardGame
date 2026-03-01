using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadyToPlaySkillCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<ReadyToPlaySkillCardCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        List<int> targetIds = new List<int>();

        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ReadyToPlaySkillCard",
            new ReadyToPlaySkillCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = payload.cardId,
                instanceId = payload.instanceId,
                targetIds = targetIds
            }
        ));
        return results;
    }
}
