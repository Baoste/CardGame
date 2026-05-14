using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBetsEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<PlaceBetsEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.instanceIds }, 0);

        string context = payload.success.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("PlaceBetsTest", objects);
    }
}

