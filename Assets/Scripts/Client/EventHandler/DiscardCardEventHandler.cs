using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscardCardEventHandler : IEventProcess, IEventHandler
{
    private DiscardCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonConvert.DeserializeObject<DiscardCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        float delay = payload.instanceIds.Count > 1 ? 1f : 0.3f;
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.instanceIds, payload.effectAnimation }, delay);

        // TODO
        // START
        string context = string.Join("\n", payload.instanceIds.Select(id => $"instanceid:{id}"));
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("DiscardCardTest", objects);
    }
}