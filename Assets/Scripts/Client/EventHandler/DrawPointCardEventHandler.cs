using Game.Domain;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public sealed class DrawPointCardEventHandler : IEventProcess, IEventHandler
{
    private DrawPointCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonConvert.DeserializeObject<DrawPointCardEvent>(ev.jsonData);   // need change
        // need change, 需要把参数在这里传进去
        ProcessQueueManager.Instance.Enqueue(Process, new object[] { payload.cardId, payload.instanceId, payload.playerId, payload.isHoleCard }, 1.5f);

        // TODO
        // START
        string context = $"instanceid:{payload.instanceId.ToString()} cardid:{payload.cardId.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process(object[] objects)
    {
        ProcessDispatcher.Process("DrawPointCard", objects);
    }
}