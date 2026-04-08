using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidActionEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<InvalidActionEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.invalidType, payload.instanceId }, 0);

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.invalidType}");

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("InvalidActionTest", objects);
    }
}