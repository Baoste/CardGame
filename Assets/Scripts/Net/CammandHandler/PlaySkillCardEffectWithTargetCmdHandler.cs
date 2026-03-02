using Game.Domain;
using Game.Server;
using System.Collections.Generic;
using UnityEngine;

public class PlaySkillCardEffectWithTargetCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<PlaySkillCardEffectWithTargetCommand>(cmd.jsonData);  // need change

        // TODO: 服务器端需要做什么
        // 验证 payload 里的 playerId 和 cardId 是否有效，是否符合游戏规则（比如玩家是否有这张牌，牌能否在当前阶段打出等等）
        session.ctx.caster = payload.playerId;
        session.ctx.opponent = 1 - payload.playerId;

        session.ctx.tmpSelectedIds = new List<int>(payload.selectedSourceIds);
        bool success0 = ParticipantResolver.ValidateParticipant(payload.effect.source, session.gameState, session.ctx, out var sourceIds);
        session.ctx.tmpSelectedIds = new List<int>(payload.selectedTargetIds);
        bool success1 = ParticipantResolver.ValidateParticipant(payload.effect.target, session.gameState, session.ctx, out var targetIds);

        // return event
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "PlaySkillCardEffectWithTarget",
            new PlaySkillCardEffectWithTargetEvent    // need change
            {
                playerId = payload.playerId,
                success = success0 && success1
            }
        ));
        return results;
    }
}
