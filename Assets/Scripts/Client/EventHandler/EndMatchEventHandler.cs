using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMatchEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<EndMatchEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.finalWinnerId, payload.winnerId, payload.currentBet }, 0);

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.finalWinnerId}:{payload.currentBet}");
        // END

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield return  ProcessDispatcher.Process("EndMatchTest", objects);
    }
}