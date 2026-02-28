using Game.Domain;
using UnityEngine;

public sealed class ChatCmdHandler : ICommandHandler
{
    public ResolvedEvent Handle(Command cmd)
    {
        var payload = JsonUtility.FromJson<ChatCommand>(cmd.jsonData);

        string message = payload.chatContext;

        var ev = new ChatEvent
        {
            PlayerId = payload.PlayerId,
            text = message
        };

        return new ResolvedEvent
        {
            type = "Chat",
            jsonData = JsonUtility.ToJson(ev)
        };
    }
}