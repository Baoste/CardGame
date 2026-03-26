using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayResolveAnimEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        // need change
        var payload = JsonConvert.DeserializeObject<PlayResolveAnimEvent>(ev.jsonData);

        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.isShown, payload.cardId, payload.instanceId });

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.isShown}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("PlayResolveAnimTest", objects);
    }
}