using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekTopCardEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<PeekTopCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.cardId, payload.instanceId }, 0.5f);

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.instanceId.ToString()}");
        // END

        return true;
    }

    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("PeekTopCardEventTest", objects);
    }
}