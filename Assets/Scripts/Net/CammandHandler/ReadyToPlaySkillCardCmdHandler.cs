using Game.Domain;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadyToPlaySkillCardCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<ReadyToPlaySkillCardCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        List<int> targetIds = new List<int> { 1, 2, 3 };  // TODO: 需要根据技能卡的效果来确定目标

        // return event
        CommandResult results = new CommandResult();
        results.events.Add(MakeEvent(
            "ReadyToPlaySkillCard",
            new ReadyToPlaySkillCardEvent    // need change
            {
                playerId = payload.playerId,
                cardId = payload.cardId,
                targetIds = targetIds
            }
        ));
        return results;
    }
}
