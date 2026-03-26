using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class PlayResolveAnimCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<PlayResolveAnimCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "PlayResolveAnim",
            new PlayResolveAnimEvent    // need change
            (
                payload.playerId,
                true,
                payload.cardId,
                payload.instanceId,
                payload.isShown
            )
        ));
        return results;
    }
}
