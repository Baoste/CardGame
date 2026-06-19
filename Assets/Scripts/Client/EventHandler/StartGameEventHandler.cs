using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<StartGameEvent>(ev.jsonData); // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { }, 0);
        ClientGameState.Instance.Start();
        ClientGameState.SkillCardCount = payload.skillCardCount;

        // TODO
        // START
        // TODO: Client start game function
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId}:{payload.skillCardCount} payload={payload.gameRound}");
        // END

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield return ProcessDispatcher.Process("StartGameTest", objects);
    }
}
