using Game.Domain;
using System.Collections.Generic;
using UnityEngine;

public class PlaySkillCardEffectWithTargetEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<PlaySkillCardEffectWithTargetEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process);

        // TODO
        // START
        ClientEffectContext.IsCommandValid = payload.success;
        ClientEffectContext.IsValidateDone = true;
        string context = payload.success.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process()
    {
        // TODO:
    }
}
