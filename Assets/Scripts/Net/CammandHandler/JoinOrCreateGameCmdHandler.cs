using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using Game.Server;
using UnityEngine;

public sealed class JoinOrCreateGameCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonUtility.FromJson<JoinOrCreateGameCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "JoinOrCreateGame",
            new JoinOrCreateGameEvent    // need change
            {
                playerId = payload.playerId,
                matchIdOrEmpty = payload.matchIdOrEmpty
            }
        ));
        return results;
    }
}
