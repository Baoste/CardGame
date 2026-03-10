using Game.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class StartGameEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<StartGameEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { });
        ClientGameState.Instance.Init();

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload=start_game");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("StartGameTest", objects);
    }
}
