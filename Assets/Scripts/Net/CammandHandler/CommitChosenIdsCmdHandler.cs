using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class CommitChosenIdsCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<CommitChosenIdsCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // START
        int skillCardId = session.instanceToCardId[payload.instanceId];
        int effectOpId = -1;

        Card skillCard = CardDatabase.Get(skillCardId);
        if (skillCard == null)
        {
            // TODO: error handling
        }
        else
        {
            // 回来继续执行技能卡的效果
            EffectOpExecutionContext opExecutionContext = session.ctx.opStack.Pop();
            EffectOp op = opExecutionContext.effectOp;

            session.ctx.selectedSourceIds = payload.selectedSourceIds;
            session.ctx.selectedTargetIds = payload.selectedTargetIds;

            bool success0 = op.source.participantSelectionMode.ValidateSelected(opExecutionContext.candidateSourceIds, payload.selectedSourceIds);
            bool success1 = op.target.participantSelectionMode.ValidateSelected(opExecutionContext.candidateTargetIds, payload.selectedTargetIds);
            if (success0 && success1)
            {
                NetEffectExecutor.ExecuteOp(payload.playerId, op, session, results, payload.selectedSourceIds, payload.selectedTargetIds);
            }
            else
            {
                // TODO: error handling
            }

            if (op.trueNode != -1 && op.falseNode == -1)        effectOpId = op.trueNode;
            else if (op.trueNode == -1 && op.falseNode != -1)   effectOpId = op.falseNode;
            else if (op.trueNode != -1 && op.falseNode != -1)   effectOpId = op.trueNode;

            // execute skill card effects
            NetEffectFunction.ExecuteEffectOp(ref effectOpId, skillCard, session, payload.playerId, payload.instanceId, ref results);
        }

        if (session.ctx.opStack.Count == 0 && effectOpId != 0)
        {
            NetEffectFunction.EndExecuteSkill(payload.playerId, payload.instanceId, session, results);
        }
        //END

        // need change
        return results;
    }
}