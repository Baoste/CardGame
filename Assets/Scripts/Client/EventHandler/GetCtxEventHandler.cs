using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCtxEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonConvert.DeserializeObject<GetCtxEvent>(ev.jsonData);

        ClientEffectContext.Instance = payload.ctx;
        ClientEffectContext.GetServerCtxDone = true;

        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload=ctx");

        return true;
    }
    public void Process(object[] objects)
    {
        // Nothing to do
    }
}
