using Game.Domain;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ChatEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<ChatEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { }, 0);

        string context = payload.text;
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");

        return true;
    }
    public void Process(object[] objects)
    {
        // TODO:
    }
}