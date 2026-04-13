using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardCardEventHandler : IEventProcess, IEventHandler
{
    private DiscardCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonConvert.DeserializeObject<DiscardCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.instanceId }, 0);

        // TODO
        // START
        string context = $"instanceid:{payload.instanceId.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("DiscardCardTest", objects);
    }
}