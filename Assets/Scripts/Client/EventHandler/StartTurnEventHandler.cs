using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTurnEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<StartTurnEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.turn }, 0);
        
        ClientGameState.Instance.Turn++;
        ClientGameState.Instance.CurrentPlayerId = payload.playerId;
        ClientGameState.Instance.players[payload.playerId].actionPoint = 1;

        ClientEffectContext.Instance.caster = payload.playerId;
        ClientEffectContext.Instance.opponent = payload.opponentId;

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=start_turn");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("StartTurnTest", objects);
    }
}