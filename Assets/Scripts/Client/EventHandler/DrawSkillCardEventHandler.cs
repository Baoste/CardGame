using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSkillCardEventHandler : IEventProcess, IEventHandler
{
    private DrawSkillCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonConvert.DeserializeObject<DrawSkillCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.cardId, payload.instanceId, payload.playerId }, 0.1f);
        ClientGameState.SkillCardCount--;

        // TODO
        // START
        string context = $"instanceid:{payload.instanceId.ToString()} cardid:{payload.cardId.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("DrawSkillCardTest", objects);
    }
}
