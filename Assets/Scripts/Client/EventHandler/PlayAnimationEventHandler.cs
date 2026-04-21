using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<PlayAnimationEvent>(ev.jsonData);

        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.animType, payload.instanceId }, 1f);

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.animType.ToString()}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("PlayAnimation", objects);
    }
}