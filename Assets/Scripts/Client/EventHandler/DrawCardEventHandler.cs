using Game.Domain;
using UnityEngine;

public sealed class DrawCardEventHandler : EventHandler, IEventHandler
{
    public bool Handle(NetEvent ev)
    {
        var payload = JsonUtility.FromJson<DrawCardEvent>(ev.jsonData); // need change

        // TODO
        // START
        // TODO: Client draw function
        string context = payload.cardId.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }
}