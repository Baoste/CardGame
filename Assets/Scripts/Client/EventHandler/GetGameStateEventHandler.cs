using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameStateEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<GetGameStateEvent>(ev.jsonData);

        ClientGameState.Instance = payload.gameState;
        ClientGameState.GetServerGameStateDone = true;

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload=gamestate");

        return true;
    }
    public void Process(object[] objects)
    {
        // Nothing to do
    }
}