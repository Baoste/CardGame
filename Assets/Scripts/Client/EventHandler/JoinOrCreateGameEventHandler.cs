using Game.Domain;
using UnityEngine;

public sealed class JoinOrCreateGameEventHandler : IEventProcess, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<JoinOrCreateGameEvent>(ev.jsonData); // need change

        // TODO
        // START
        string context = payload.matchIdOrEmpty;
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
    public void Process()
    {

    }
}