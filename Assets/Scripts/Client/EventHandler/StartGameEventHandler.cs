using Game.Domain;
using System.Collections.Generic;
using UnityEngine;

public class StartGameEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<StartGameEvent>(ev.jsonData); // need change

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=start_game");
        // END

        return true;
    }
    public void Process()
    {

    }
}
