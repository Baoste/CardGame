using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyPointEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<ModifyPointEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.instanceId, payload.pointChange }, 0);

        // TODO
        // START
        string context = payload.pointChange.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("ModifyPointTest", objects);
    }
}