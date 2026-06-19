using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumPointEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<SumPointEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.playerPointsOnBoard, payload.playerHoleCardPoint, payload.hasHiddenCard, payload.opponentPointsOnBoard }, 0);

        // TODO
        // START
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.playerPointsOnBoard} | {payload.opponentPointsOnBoard}");
        // END

        return true;
    }

    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("SumPointTest", objects);
    }
}