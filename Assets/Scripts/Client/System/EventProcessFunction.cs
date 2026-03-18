using Cinemachine;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventProcessFunction : MonoBehaviour
{
    private static Dictionary<int, GameObject> instanceMap = new();

    void Start()
    {
        ProcessDispatcher.Register("StartGameTest", StartGameTest);
        ProcessDispatcher.Register("AssignRolesTest", AssignRolesTest);
        ProcessDispatcher.Register("PlayAnimation", PlayAnimation);
        ProcessDispatcher.Register("DrawCardTest", DrawCard);
        ProcessDispatcher.Register("DrawSkillCardTest", DrawSkillCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
        ProcessDispatcher.Register("MoveCardTest", MoveCard);
        ProcessDispatcher.Register("ClearCardsToResolveTest", ClearResolve);
    }

    public void StartGameTest(object[] parameters)
    {
        CinemachineVirtualCamera vcam = GameObject.Find("VCamera_Playing").GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 20;
    }

    public void AssignRolesTest(object[] parameters)
    {
        int dealerId = (int)parameters[0];
        int punterId = (int)parameters[1];

        SceneViewManager.roleView.ShowRole(dealerId);
        ClientCommand.StartTurn(dealerId);
    }

    // parameters[0]: int playerId
    // parameters[1]: AnimationType animType
    // parameters[2]: int instanceId
    public void PlayAnimation(object[] parameters)
    {
        int playerId = (int)parameters[0];
        AnimationType animType = (AnimationType)parameters[1];
        int instanceId = (int)parameters[2];

        GameObject obj = instanceMap[instanceId];
        bool isOpponent = playerId != ClientGameState.playerSlot;

        switch (animType)
        {
            case AnimationType.MoveToFallPosition:
                if (isOpponent)
                    StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToFallPosition(obj));
                else
                    StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(obj));
                break;
            case AnimationType.ReturnToHand:
                if (isOpponent)
                    StartCoroutine(obj.GetComponent<SkillCardDraggable>().ReturnToHand());
                else
                    StartCoroutine(obj.GetComponent<SkillCardDraggable>().ReturnToHand());
                break;
            case AnimationType.MoveToExecutePosition:
                if (isOpponent)
                    StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToExecutePosition(obj));
                else
                    StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(obj));
                break;
        }
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


    // parameters[0]: int playerId
    // parameters[1]: int instanceId
    public void DiscardCard(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int instanceId = (int)parameters[1];

        GameObject obj = instanceMap[instanceId];
        instanceMap.Remove(instanceId);

        if (obj.GetComponent<SkillCardInstance>() != null)
        {
            bool isOpponent = playerId != ClientGameState.playerSlot;
            if (isOpponent)
                StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(obj));
            else
                StartCoroutine(SceneViewManager.myHandView.RemoveCard(obj));
        }
        else
        {
            StartCoroutine(SceneViewManager.boardView.RemoveCard(obj));
        }
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

        instanceMap[instanceId] = instance;
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId));
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

        instanceMap[instanceId] = instance;

        if (isOpponent)
        {
            StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
        }
        else
        {
            StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
        }
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
                StartCoroutine(SceneViewManager.resolveZoneView.AddCard(instance, playerId));    
                break;
            }
            case ParticipantType.MyBoardZone:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId));
                instanceMap[instanceId] = instance;
                break;
            }
            case ParticipantType.OppentBoardZone:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - playerId));
                instanceMap[instanceId] = instance;
                break;
            }
        }
    }

    public void ClearResolve(object[] parameters)
    {
        StartCoroutine(SceneViewManager.resolveZoneView.ClearCards());
    }
}
