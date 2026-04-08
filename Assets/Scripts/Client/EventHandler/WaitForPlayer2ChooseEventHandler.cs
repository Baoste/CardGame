using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayer2ChooseEventHandler : IEventProcess, IEventHandler
{

    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<WaitForPlayer2ChooseEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] {
            payload.success,
            payload.skillCardInstanceId,
            payload.sourceNeedChoose,
            payload.targetNeedChoose,
            payload.isSourceParticipantZone,
            payload.isTargetParticipantZone,
            payload.candidateSourceIds,
            payload.candidateTargetIds,
            payload.sourceSelectCount,
            payload.targetSelectCount,
        }, 0.5f);

        // TODO
        // START
        List<int> candidateSourceIds = payload.candidateSourceIds;
        List<int> candidateTargetIds = payload.candidateTargetIds;

        string context = $"{payload.success} - S:";
        if (candidateSourceIds.Count > 0)
        {
            context += $"{string.Join(",", candidateSourceIds)}";
        }
        context += " ; T:";
        if (candidateTargetIds.Count > 0)
        {
            context += $"{string.Join(",", candidateTargetIds)}";
        }
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("DetermineParticipantsTest", objects);
    }
}
