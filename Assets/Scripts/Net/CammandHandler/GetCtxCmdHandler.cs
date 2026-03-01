using Game.Domain;
using Game.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCtxCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<GetCtxCommand>(cmd.jsonData);

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "GetCtx",
            new GetCtxEvent    // need change
            {
                ctx = session.ctx,
            }
        ));
        return results;
    }
}
