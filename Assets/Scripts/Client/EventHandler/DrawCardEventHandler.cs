using Game.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class DrawCardEventHandler : MonoBehaviour, IEventProcess, IEventHandler
{
    [SerializeField]
    private GameObject cardPrefab;

    private DrawCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonUtility.FromJson<DrawCardEvent>(ev.jsonData); // need change

        // TODO
        // START
        // TODO: Client draw function
        ProcessQueueManager.Instance.Enqueue(this);

        string context = payload.cardId.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} slot={payload.playerId} payload={context}");
        // END

        return true;
    }

    public void Process()
    {
        Card card = CardDatabase.Get(payload.cardId);

        Vector3 position = new Vector3(0, 0, 0);
        Quaternion rotation = Quaternion.identity;

        GameObject newCard = Instantiate(cardPrefab, position, rotation);
    }
}