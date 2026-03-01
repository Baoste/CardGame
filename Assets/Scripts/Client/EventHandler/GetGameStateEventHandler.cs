using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameStateEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<GetGameStateEvent>(ev.jsonData);

        ClientGameState.Instance = payload.gameState;

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload=gamestate");

        return true;
    }
    public void Process()
    {
        // Nothing to do
    }
}