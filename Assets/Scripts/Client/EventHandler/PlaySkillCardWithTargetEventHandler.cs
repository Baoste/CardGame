using Game.Domain;
using System.Collections.Generic;
using UnityEngine;

public class PlaySkillCardWithTargetEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<PlaySkillCardWithTargetEvent>(ev.jsonData); // need change

        // TODO
        // START
        List<int> targetIds = payload.targetIds;
        string context = "";
        if (targetIds.Count > 0)
        {
            context = $"targets:{string.Join(",", targetIds)}";
        }
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process()
    {

    }
}
