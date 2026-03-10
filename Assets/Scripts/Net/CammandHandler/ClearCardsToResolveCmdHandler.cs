using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ClearCardsToResolveCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ClearCardsToResolveCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        session.gameState.ClearResolve();
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "ClearCardsToResolve",
            new ClearCardsToResolveEvent
            (
                payload.playerId,
                true
            )
        ));
        return results;
    }
}