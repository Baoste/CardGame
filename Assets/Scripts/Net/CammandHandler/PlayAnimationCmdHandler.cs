using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public class PlayAnimationCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<PlayAnimationCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "PlayAnimation",
            new PlayAnimationEvent    // need change
            (
                payload.playerId,
                true,
                payload.animType,
                payload.instanceId
            )
        ));
        return results;
    }
}