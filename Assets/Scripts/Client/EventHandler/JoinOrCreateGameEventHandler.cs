using Game.Domain;
using Newtonsoft.Json;
using UnityEngine;

public sealed class JoinOrCreateGameEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<JoinOrCreateGameEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { });

        // TODO
        // START
        string context = payload.matchIdOrEmpty;
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        // TODO:
    }
}