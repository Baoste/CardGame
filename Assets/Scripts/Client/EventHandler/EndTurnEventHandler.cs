using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<EndTurnEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.turn, payload.reveal }, 0);

        ClientEffectContext.Instance.ClearContext();
        ClientGameState.Instance.CurrentPlayerId = payload.opponentId;

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.reveal.ToString()}");
        // END

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("EndTurnTest", objects);
    }
}