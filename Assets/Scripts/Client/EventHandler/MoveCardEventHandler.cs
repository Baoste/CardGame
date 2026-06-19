using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCardEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<MoveCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.cardId, payload.selectedId, payload.toZone, payload.cardState }, 0);

        // TODO
        // START
        string context = $"toZone:{payload.toZone.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("MoveCardTest", objects);
    }
}