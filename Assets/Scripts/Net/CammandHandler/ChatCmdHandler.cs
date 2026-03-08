using Game.Domain;
using Game.Server;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ChatCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonConvert.DeserializeObject<ChatCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        string message = payload.chatContext;
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Enqueue(MakeEvent(
            "Chat",
            new ChatEvent
            (
                payload.playerId,
                true,
                message
            )
        ));
        return results;
    }
}