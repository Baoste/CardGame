using Game.Domain;
using Game.Server;
using UnityEngine;

public sealed class ChatCmdHandler : CommandHandler, ICommandHandler
{
    public CommandResult Handle(MatchSession session, NetCommand cmd)
    {
        var payload = JsonUtility.FromJson<ChatCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        string message = payload.chatContext;
        //END

        // need change
        CommandResult results = new CommandResult();
        results.events.Add(MakeEvent(
            "Chat",
            new ChatEvent
            {
                playerId = payload.playerId,
                text = message
            }
        ));
        return results;
    }
}