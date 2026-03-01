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
        // 验证 payload 里的 playerId 和 cardId 是否有效，是否符合游戏规则（比如玩家是否有这张牌，牌能否在当前阶段打出等等）
        List<int> targetIds = new List<int>();
        if (TargetResolver.ValidateTarget(payload.effect.target, session.gameState, session.ctx))
            targetIds = payload.targetIds;

        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "PlaySkillCardWithTarget",
            new PlaySkillCardWithTargetEvent    // need change
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
