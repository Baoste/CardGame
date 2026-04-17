using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmBetEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<ConfirmBetEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId }, 0.5f);

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=Confirm_Bet");

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("ConfirmBetTest", objects);
    }
}
