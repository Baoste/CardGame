using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendActionPointEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<SpendActionPointEvent>(ev.jsonData);   
        CommandExecutionState<SpendActionPointCommand>.IsDone = true;
        CommandExecutionState<SpendActionPointCommand>.Success = payload.success;

        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.apCount }, 0);

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.success.ToString()}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("SpendActionPointTest", objects);
    }
}