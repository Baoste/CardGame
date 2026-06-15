using Game.Domain;
using Game.Server;
using Newtonsoft.Json;

public sealed class JoinOrCreateMatchCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<JoinOrCreateMatchCommand>(cmd.jsonData);

        // TODO: 服务器端需要做什么

        // return
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "JoinOrCreateMatch",
            new JoinOrCreateMatchEvent    // need change
            (
                payload.playerId,
                true,
                session.Slots[0].accountData,
                session.Slots[1].accountData,
                payload.matchIdOrEmpty
            ),
            payload.playerId
        ));
        return results;
    }
}
