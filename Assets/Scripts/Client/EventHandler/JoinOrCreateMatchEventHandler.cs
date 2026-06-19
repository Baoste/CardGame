using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public sealed class JoinOrCreateMatchEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<JoinOrCreateMatchEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.account0, payload.account1 }, 0);

        // TODO
        // START
        string context = $"{payload.matchIdOrEmpty} | {payload.account0.AccountId}:{payload.account0.ChipAppearaceData.ChipColorId} | {payload.account1.AccountId}:{payload.account1.ChipAppearaceData.ChipColorId}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public IEnumerator Process(object[] objects)
    {
        yield break;
        yield return ProcessDispatcher.Process("BothJoinMatch", objects);
    }
}