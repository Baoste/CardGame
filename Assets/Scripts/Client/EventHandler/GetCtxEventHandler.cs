using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCtxEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<GetCtxEvent>(ev.jsonData);

        ClientEffectContext.Instance = payload.ctx;
        ClientEffectContext.GetServerCtxDone = true;

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload=ctx");

        return true;
    }
    public void Process()
    {
        // Nothing to do
    }
}
