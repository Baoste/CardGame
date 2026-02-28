using Game.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class DrawCardEventHandler : IEventProcess, IEventHandler
{
    private DrawCardTest drawCardTest;
    DrawCardEvent payload;

    public bool Handle(NetEvent ev)
    {
        payload = JsonUtility.FromJson<DrawCardEvent>(ev.jsonData); // need change

        // TODO
        // START
        // TODO: Client draw function
        //DrawCardTest


        string context = payload.cardId.ToString();
        Debug.Log($"[Client] Event#{ev.Index} type={ev.type} payload={context}");
        // END

        ProcessQueueManager.Instance.Enqueue(this);

        return true;
    }

    public void Process()
    {
        Card card = FindCard(payload.cardId);

        Vector3 position = new Vector3(0, 0, 0);
        Quaternion rotation = Quaternion.identity;

        drawCardTest.InitiateCard(position, rotation, card.id, card.name, card.description, card.point);
    }

    public Card FindCard(int cardId)
    {
        Card card = new Card
        {
            id = cardId,
            name = $"Card {cardId}",
            description = $"Description for card {cardId}",
            point = cardId * 10 // Example point calculation
        };
        return card;
    }
}