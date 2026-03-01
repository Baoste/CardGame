using Game.Domain;
using UnityEngine;

public sealed class ChatEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<ChatEvent>(ev.jsonData);
        ProcessQueueManager.Instance.Enqueue(Process);

        string context = payload.text;
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");

        return true;
    }
    public void Process()
    {
        // TODO:
    }
}