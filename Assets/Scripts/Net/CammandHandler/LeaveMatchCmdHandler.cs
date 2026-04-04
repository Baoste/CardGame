using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveMatchCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        // var payload = JsonConvert.DeserializeObject<LeaveMatchCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        //results.events.Enqueue(MakeEvent(
        //    "DrawPointCard",
        //    new DrawPointCardEvent    // need change
        //    {
        //        playerId = payload.playerId,
        //        cardId = session.instanceToCardId.GetValueOrDefault(drawCardInstanceId, -1),
        //        instanceId = drawCardInstanceId,
        //        isHoleCard = false
        //    }
        //));
        return results;
    }
}