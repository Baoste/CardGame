using Cinemachine;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawCardTest : MonoBehaviour
{
    // 按 playerId 存储在屏幕上显示的牌（用于重排）
    private static readonly Dictionary<int, List<GameObject>> handMap = new();
    private static Dictionary<int, GameObject> instanceMap = new();

    [SerializeField] private HandView handView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private ResolveZoneView ResolveZoneView;

    // Start is called before the first frame update
    void Start()
    {
        ProcessDispatcher.Register("StartGameTest", StartGameTest);
        ProcessDispatcher.Register("DrawCardTest", DrawCard);
        ProcessDispatcher.Register("DrawSkillCardTest", DrawSkillCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
        ProcessDispatcher.Register("MoveCardTest", MoveCard);
        ProcessDispatcher.Register("ClearCardsToResolveTest", ClearResolve);
        handMap[0] = new List<GameObject>();
        handMap[1] = new List<GameObject>();
    }

    public void StartGameTest(object[] parameters)
    {
        CinemachineVirtualCamera vcam = GameObject.Find("VCamera_Playing").GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 20;
    }

    // parameters[0]: int instanceId
    // parameters[1]: int pointChange
    public void ModifyPoint(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        int pointChange = (int)parameters[1];

        int point = int.Parse(instanceMap[instanceId].GetComponentInChildren<TextMeshPro>().text) + pointChange;
        instanceMap[instanceId].GetComponentInChildren<TextMeshPro>().text = point.ToString();
    }


    // parameters[0]: int instanceId
    public void DiscardCard(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        handMap[0].Remove(instanceMap[instanceId]);
        handMap[1].Remove(instanceMap[instanceId]);
        Destroy(instanceMap[instanceId]);
        instanceMap.Remove(instanceId);
    }


    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    // parameters[3]: bool isHoleCard
    public void DrawCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        bool isHoreCard = (bool)parameters[3];

        bool isOpponent = playerId != ClientGameState.playerSlot;

        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
        if (isHoreCard && isOpponent)
            instance.GetComponent<PointCardInstance>().pointText.text = "";

        handMap[playerId].Add(instance);
        instanceMap[instanceId] = instance;
        StartCoroutine(boardView.AddCard(instance, playerId));
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    public void DrawSkillCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];

        bool isOpponent = playerId != ClientGameState.playerSlot;

        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);

        handMap[playerId].Add(instance);
        instanceMap[instanceId] = instance;

        if (isOpponent) return;
        StartCoroutine(handView.AddCard(instance));
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: ParticipantType toZone
    // parameters[3]: int playerId
    public void MoveCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        ParticipantType toZone = (ParticipantType)parameters[2];
        int playerId = (int)parameters[3];

        switch (toZone)
        {
            case ParticipantType.CardsToResolve:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(ResolveZoneView.AddCard(instance, playerId));    
                break;
            }
            case ParticipantType.MyPointCardsOnBoard:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(boardView.AddCard(instance, playerId));
                break;
            }
        }
    }

    public void ClearResolve(object[] parameters)
    {
        StartCoroutine(ResolveZoneView.ClearCards());
    }
}
