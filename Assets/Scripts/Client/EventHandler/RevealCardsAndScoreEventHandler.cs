using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealCardsAndScoreEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<RevealCardsAndScoreEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { });

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=winner:{payload.winnerId.ToString()}");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        // TODO:
    }
}