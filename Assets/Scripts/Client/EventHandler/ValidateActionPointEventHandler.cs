using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateActionPointEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<ValidateActionPointEvent>(ev.jsonData);
        CommandExecutionState<ValidateActionPointCommand>.IsDone = true;
        CommandExecutionState<ValidateActionPointCommand>.Success = payload.success;

        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { });

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.success.ToString()}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        // ProcessDispatcher.Process("EventProcessFunction", objects);
    }
}