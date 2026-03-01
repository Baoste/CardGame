using Game.Domain;
using TMPro;
using UnityEngine;

public sealed class DrawCardEventHandler : MonoBehaviour, IEventProcess, IEventHandler
{
    private DrawCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonUtility.FromJson<DrawCardEvent>(ev.jsonData); // need change
        ProcessQueueManager.Instance.Enqueue(Process);

        // TODO
        // START
        string context = $"instanceid:{payload.instanceId.ToString()} cardid:{payload.cardId.ToString()}";
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process()
    {
        // TODO: change string and parameters
        ProcessDispatcher.Process("DrawCardTest", new object[] { payload.cardId });
    }

    //[ContextMenu("Debug Draw Function")]
    //private void DebugHandle()
    //{
    //    ProcessQueueManager.Instance.Enqueue(() =>
    //    {
    //        ProcessDispatcher.Process("DrawCardTest", new object[] { 0 });
    //    });
    //}
}