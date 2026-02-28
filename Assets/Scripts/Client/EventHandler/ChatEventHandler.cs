using Game.Domain;
using UnityEngine;

public sealed class ChatEventHandler : EventHandler, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<ChatEvent>(ev.jsonData);

        string context = payload.text;
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload={context}");

        return true;
    }
}