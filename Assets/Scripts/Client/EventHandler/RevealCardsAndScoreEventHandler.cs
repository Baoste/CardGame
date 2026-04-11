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
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.winnerId, payload.currentBet, payload.playerPoints, payload.opponentPoints }, 0.5f);

        // TODO
        // START
        ClientGameState.Instance.Dispose();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=winner:{payload.winnerId.ToString()}");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("RevealTest", objects);
    }
}