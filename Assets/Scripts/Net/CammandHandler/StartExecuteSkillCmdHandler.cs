using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StartExecuteSkillCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<StartExecuteSkillCommand>(cmd.jsonData);  // need change
        CommandResult results = new CommandResult();

        // START
        int skillCardId = session.instanceToCardId[payload.instanceId];
        int effectOpId = 0;
        Card skillCard = CardDatabase.Get(skillCardId);
        if (skillCard == null)
        {
            // TODO: error handling
        }
        else
        {
            if (!NetEffectFunction.SpendActionPoint(payload.playerId, payload.instanceId, session, results, session.gameState.instancePointMap[payload.instanceId]))
                return results;

            // execute skill card effects
            NetEffectFunction.ExecuteEffectOp(ref effectOpId, skillCard, session, payload.playerId, payload.instanceId, ref results);
        }

        if (session.ctx.opStack.Count == 0 && effectOpId != 0)
        {
            NetEffectFunction.EndExecuteSkill(payload.playerId, payload.instanceId, session, results);
        }
        //END

        return results;
    }
}
