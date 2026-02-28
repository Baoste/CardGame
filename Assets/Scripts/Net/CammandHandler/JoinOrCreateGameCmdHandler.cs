using Game.Domain;
using UnityEngine;

public sealed class JoinOrCreateGameCmdHandler : ICommandHandler
{
    public ResolvedEvent Handle(Command cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<JoinOrCreateGameCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        var ev = new JoinOrCreateGameEvent    // need change
        {
            playerId = payload.playerId,
            matchIdOrEmpty = payload.matchIdOrEmpty
        };

        return new ResolvedEvent
        {
            type = "JoinOrCreateGame",  // need change
            jsonData = JsonUtility.ToJson(ev)
        };
    }
}
