using Game.Domain;
using GameKit.Dependencies.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ValidateParticipantsEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<ValidateParticipantsEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { });

        // TODO
        // START
        ClientEffectContext.Instance.selectedSourceIds = payload.sourceIds;
        ClientEffectContext.Instance.selectedTargetIds = payload.targetIds;
        ClientEffectContext.IsCommandValid = payload.success;
        ClientEffectContext.IsValidateDone = true;
        string context = $"{payload.success.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process(object[] objects)
    {
        // TODO:
    }
}
