using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public class PlaySkillCardWithTargetCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<PlaySkillCardWithTargetCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        List<int> targetIds = new List<int> { 1 };  // TODO: 需要根据技能卡的效果来确定目标

        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "PlaySkillCardWithTarget",
            new PlaySkillCardWithTargetEvent    // need change
            {
                playerId = payload.playerId,
                cardId = payload.cardId,
                targetIds = targetIds
            }
        ));
        return results;
    }
}
