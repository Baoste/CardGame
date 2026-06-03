using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeResultEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<JudgeResultEvent>(ev.jsonData);
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.playerId, payload.judgeResult, payload.effectAnimation }, 0.5f);

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={payload.judgeResult}");

        return true;
    }
    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("JudgeResultTest", objects);
    }
}