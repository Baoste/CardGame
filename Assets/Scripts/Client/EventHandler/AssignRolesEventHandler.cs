using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignRolesEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<AssignRolesEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.dealerId, payload.punterId }, 0.5f);

        ClientGameState.Instance.dealerId = payload.dealerId;
        ClientGameState.Instance.punterId = payload.punterId;

        string context = payload.dealerId.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("AssignRolesTest", objects);
    }
}