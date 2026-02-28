using Game.Domain;
using UnityEngine;

public sealed class ChatCmdHandler : ICommandHandler
{
    public ResolvedEvent Handle(Command cmd)
    {
        var payload = JsonUtility.FromJson<ChatCommand>(cmd.jsonData);  // need change

        // TODO
        // START
        string message = payload.chatContext;
        //END

        // need change
        var ev = new ChatEvent
        {
            playerId = payload.playerId,
            text = message
        };

        return new ResolvedEvent
        {
            type = "Chat",
            jsonData = JsonUtility.ToJson(ev)
        };
    }
}