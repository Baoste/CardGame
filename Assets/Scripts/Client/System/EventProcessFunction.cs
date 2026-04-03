using Cinemachine;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventProcessFunction : MonoBehaviour
{
    public static Dictionary<int, GameObject> instanceMap = new();

    void Start()
    {
        ProcessDispatcher.Register("StartGameTest", StartGameTest);
        ProcessDispatcher.Register("StartTurnTest", StartTurnTest);
        ProcessDispatcher.Register("AssignRolesTest", AssignRolesTest);
        ProcessDispatcher.Register("Place1BetTest", Place1BetTest);
        ProcessDispatcher.Register("PlayAnimation", PlayAnimation);
        ProcessDispatcher.Register("DrawPointCard", DrawPointCard);
        ProcessDispatcher.Register("DrawSkillCardTest", DrawSkillCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
        ProcessDispatcher.Register("MoveCardTest", MoveCard);
        ProcessDispatcher.Register("ToResolveTest", ToResolveTest);
        ProcessDispatcher.Register("PlayResolveAnimTest", PlayResolveAnimTest);
        ProcessDispatcher.Register("EndTurnTest", EndTurnTest);
        ProcessDispatcher.Register("ClearCardsToResolveTest", ClearResolve);
        ProcessDispatcher.Register("RevealTest", RevealTest);
    }

    public void StartGameTest(object[] parameters)
    {
        CinemachineVirtualCamera vcam = GameObject.Find("VCamera_Playing").GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 20;
        SceneViewManager.viewAnimController.PlayStartGameAnim();
        SceneViewManager.myChipView.GenerateChips();
    }

    // parameters[0]: int turn
    public void StartTurnTest(object[] parameters)
    {
        int turn = (int)parameters[0];
        if (turn == 1)
        {
            if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
                SceneViewManager.opponentTurnLightView.SetLight(1);
            else
                SceneViewManager.myTurnLightView.SetLight(1);
        }

        int endTurnCount = 8;
        if (turn == endTurnCount)
        {
            if (ClientGameState.Instance.punkerId == ClientGameState.playerSlot)
                SceneViewManager.myRevealButtonView.ShowButton(true);
            else
                SceneViewManager.opponentRevealButtonView.ShowButton(false);
        }
        if (turn == endTurnCount + 1)
        {
            if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
                SceneViewManager.myRevealButtonView.ShowRandom();
            else
                SceneViewManager.opponentRevealButtonView.ShowRandom();
        }
    }

    // parameters[0]: int dealerId
    // parameters[1]: int punterId
    public void AssignRolesTest(object[] parameters)
    {
        int dealerId = (int)parameters[0];
        int punterId = (int)parameters[1];

        SceneViewManager.roleView.ShowRole(dealerId);
        if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(punterId);
    }

    // parameters[0]: int playerId
    public void Place1BetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            // TODO: 加上对手
        }
        else
        {
            foreach (var obj in SceneViewManager.myChipView.chipsPlaced)
            {
                ChipDraggable script = obj.GetComponent<ChipDraggable>();
                if (script != null)
                {
                    Destroy(script);
                }
            }
        }
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
    public void DrawPointCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        bool isHoleCard = (bool)parameters[3];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
        instanceMap[instanceId] = instance;

        StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId, isHoleCard));
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
    // parameters[2]: int playerId
    public void ToResolveTest(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];

        PlayResolveAnimCommand cmd = new PlayResolveAnimCommand { playerId = playerId, cardId = cardId, instanceId = instanceId, isShown = true };
        ClientGameState.gateway.SendCommandServerRpc("PlayResolveAnim", JsonConvert.SerializeObject(cmd));
    }

    // parameters[0]: int playerId
    // parameters[1]: bool isShown
    // parameters[2]: int cardId
    // parameters[3]: int instanceId
    public void PlayResolveAnimTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        bool isShown = (bool)parameters[1];
        int cardId = (int)parameters[2];
        int instanceId = (int)parameters[3];

        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId, transform.position, Quaternion.identity);
        StartCoroutine(SceneViewManager.resolveZoneView.AddCard(instance, playerId, isShown));
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
            case ParticipantType.MyBoardZone:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId, false));
                instanceMap[instanceId] = instance;
                break;
            }
            case ParticipantType.OppentBoardZone:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId, transform.position, Quaternion.identity);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - playerId, false));
                instanceMap[instanceId] = instance;
                break;
            }
        }
    }

    public void ClearResolve(object[] parameters)
    {
        StartCoroutine(SceneViewManager.resolveZoneView.ClearCards());
    }

    // parameters[0]: int turn
    // parameters[1]: bool reveal
    public void EndTurnTest(object[] parameters)
    {
        int turn = (int)parameters[0];
        bool reveal = (bool)parameters[1];

        // 亮灯
        if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
        {
            int myTurn = (turn + 1) / 2;
            int opTurn = turn / 2 + 1;
            SceneViewManager.myTurnLightView.SetLight(myTurn);
            SceneViewManager.opponentTurnLightView.SetLight(opTurn);
        }
        else
        {
            int myTurn = turn / 2 + 1;
            int opTurn = (turn + 1) / 2;
            SceneViewManager.myTurnLightView.SetLight(myTurn);
            SceneViewManager.opponentTurnLightView.SetLight(opTurn);
        }

        StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(reveal));
    }

    // parameters[0]: int winnerId
    public void RevealTest(object[] parameters)
    {
        int winnerId = (int)parameters[0];

        SceneViewManager.roleView.ShowWin(winnerId);
    }
}
