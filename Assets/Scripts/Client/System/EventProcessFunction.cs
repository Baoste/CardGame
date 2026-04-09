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
        ProcessDispatcher.Register("StartMatchTest", StartMatchTest);
        ProcessDispatcher.Register("StartGameTest", StartGameTest);
        ProcessDispatcher.Register("StartTurnTest", StartTurnTest);
        ProcessDispatcher.Register("InvalidActionTest", InvalidActionTest);
        ProcessDispatcher.Register("AssignRolesTest", AssignRolesTest);
        ProcessDispatcher.Register("AddActionPointTest", AddActionPointTest);
        ProcessDispatcher.Register("SpendActionPointTest", SpendActionPointTest);
        ProcessDispatcher.Register("Place1BetTest", Place1BetTest);
        ProcessDispatcher.Register("ConfirmBetTest", ConfirmBetTest);
        ProcessDispatcher.Register("PlayAnimation", PlayAnimation);
        ProcessDispatcher.Register("DrawPointCard", DrawPointCard);
        ProcessDispatcher.Register("DrawSkillCardTest", DrawSkillCard);
        ProcessDispatcher.Register("DiscardCardTest", DiscardCard);
        ProcessDispatcher.Register("ModifyPointTest", ModifyPoint);
        ProcessDispatcher.Register("MoveCardTest", MoveCard);
        ProcessDispatcher.Register("ChangeCardStateTest", ChangeCardStateTest);
        ProcessDispatcher.Register("PeekTopCardEventTest", PeekTopCardEventTest);
        ProcessDispatcher.Register("ToResolveTest", ToResolveTest);
        ProcessDispatcher.Register("EndTurnTest", EndTurnTest);
        ProcessDispatcher.Register("ClearCardsToResolveTest", ClearResolve);
        ProcessDispatcher.Register("RevealTest", RevealTest);
        ProcessDispatcher.Register("EmojiTest", EmojiTest);
    }

    public void EmojiTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int emojiId = (int)parameters[1];
        //bool isOpponent = playerId != ClientGameState.playerSlot;
        //if (isOpponent)
        //    SceneViewManager.opponentEmojiView.ShowEmoji(emojiId);
        //else
        //    SceneViewManager.myEmojiView.ShowEmoji(emojiId);
    }

    public void StartMatchTest(object[] parameters)
    {
        CinemachineVirtualCamera vcam = GameObject.Find("VCamera_Playing").GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 20;
        SceneViewManager.myChipView.GenerateChips(6);
        SceneViewManager.opponentChipView.GenerateChips(6);
    }

    public void StartGameTest(object[] parameters)
    {
        StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());

        instanceMap.Clear();

        Transform root = CardViewCreator.Instance.transform;
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }
        SceneViewManager.ClearViews();
    }

    // parameters[0]: int playerId
    // parameters[1]: int turn
    public void StartTurnTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int turn = (int)parameters[1];

        if (ClientGameState.playerSlot == playerId)
            SceneViewManager.endTurnView.btnLight.intensity = 1;

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
            if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
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

    // parameters[0]: int playerId
    // parameters[1]: InvalidActionType invalidType
    // parameters[2]: int instanceId
    public void InvalidActionTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        InvalidActionType invalidType = (InvalidActionType)parameters[1];
        int instanceId = (int)parameters[2];

        // TODO: 显示错误提示
        Debug.Log($"[Client] Player {playerId} performed invalid action: {invalidType}");

        switch(invalidType)
        {
            case InvalidActionType.InvalidTarget:
            case InvalidActionType.NotEnoughAP:
                ClientEffectContext.isExecutingSkillCard = false;
                PlayAnimationCommand cmd = new PlayAnimationCommand { playerId = playerId, animType = AnimationType.ReturnToHand, instanceId = instanceId };
                ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(cmd));
                break;
            case InvalidActionType.NoCardToDraw:
                // 显示无牌可抽提示
                break;
        }
    }

    // parameters[0]: int dealerId
    // parameters[1]: int punterId
    public void AssignRolesTest(object[] parameters)
    {
        int dealerId = (int)parameters[0];
        int punterId = (int)parameters[1];

        SceneViewManager.roleView.ShowRole(dealerId);

        if (punterId == ClientGameState.playerSlot)
        {
            foreach (var obj in SceneViewManager.myChipView.chipsInTray)
            {
                if (obj.transform.childCount > 0)
                {
                    ChipDraggable drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipDraggable>();
                    drag.Init();
                }
            }
        }
    }

    // parameters[0]: int playerId
    public void AddActionPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];

        //bool isOpponent = playerId != ClientGameState.playerSlot;
        //if (isOpponent)
        //    SceneViewManager.opponentActionPointView.AddPoint();
        //else
        //    SceneViewManager.myActionPointView.AddPoint();
    }

    // parameters[0]: int playerId
    public void SpendActionPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];

        //bool isOpponent = playerId != ClientGameState.playerSlot;
        //if (isOpponent)
        //    SceneViewManager.opponentActionPointView.AddPoint();
        //else
        //    SceneViewManager.myActionPointView.AddPoint();
    }


    // parameters[0]: int playerId
    public void Place1BetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(false));
            StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true));
        }
        else
        {
            foreach (var obj in SceneViewManager.myChipView.chipsPlaced)
            {
                ChipDraggable script = obj.GetComponentInChildren<ChipDraggable>();
                if (script != null)
                {
                    Destroy(script);
                }
            }
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int betCount
    public void ConfirmBetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int betCount = (int)parameters[1];

        foreach (var obj in SceneViewManager.myChipView.chipsInTray)
        {
            ChipDraggable script = obj.GetComponentInChildren<ChipDraggable>();
            if (script != null)
            {
                Destroy(script);
            }
        }

        StartCoroutine(SceneViewManager.viewAnimController.CloseChipCover());
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(ClientGameState.Instance.punterId);
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
                { 
                    SkillCard skillCard = obj.GetComponent<SkillCard>();
                    skillCard.stateMachine.ChangeState(skillCard.readyFallState);
                }
                break;
            case AnimationType.ReturnToHand:
                if (isOpponent)
                {
                    SceneViewManager.opponentHandView.ReturnCard(obj);
                    StartCoroutine(obj.GetComponent<SkillCardInstance>().ReturnToHand());
                }
                else
                {
                    SceneViewManager.myHandView.ReturnCard(obj);
                    SkillCard skillCard = obj.GetComponent<SkillCard>();
                    skillCard.stateMachine.ChangeState(skillCard.inHandState);
                }
                break;
            case AnimationType.MoveToExecutePosition:
                {
                    SkillCard skillCard = obj.GetComponent<SkillCard>();
                    skillCard.stateMachine.ChangeState(skillCard.executeState);
                }
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
    // parameters[3]: CardVisualState cardVisualState
    public void DrawPointCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        CardVisualState cardState = (CardVisualState)parameters[3];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
        instanceMap[instanceId] = instance;

        StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId, cardState));
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

        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);

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
    // parameters[3]: bool isShown
    public void ToResolveTest(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        bool isShown = (bool)parameters[3];

        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId);
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

        bool isOpponent = playerId != ClientGameState.playerSlot;

        switch (toZone)
        {
            case ParticipantType.MyBoardZone:
            {
                if (instanceMap.TryGetValue(instanceId, out var obj))
                {
                    StartCoroutine(SceneViewManager.boardView.RemoveCardInstant(obj));
                }
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId, CardVisualState.None));
                instanceMap[instanceId] = instance;
                break;
            }
            case ParticipantType.OppentBoardZone:
            {
                if (instanceMap.TryGetValue(instanceId, out var obj))
                {
                    StartCoroutine(SceneViewManager.boardView.RemoveCardInstant(obj));
                }
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - playerId, CardVisualState.None));
                instanceMap[instanceId] = instance;
                break;
            }
            case ParticipantType.MySkillCardsInHand:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                if (isOpponent)
                {
                    StartCoroutine(SceneViewManager.myHandView.RemoveCardInstant(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
                }
                else
                {
                    StartCoroutine(SceneViewManager.opponentHandView.RemoveCardInstant(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
                }
                instanceMap[instanceId] = instance;
                break;
            }
            case ParticipantType.OpponentSkillCardsInHand:
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                if (isOpponent)
                { 
                    StartCoroutine(SceneViewManager.opponentHandView.RemoveCardInstant(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
                }
                else
                {
                    StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
                    StartCoroutine(SceneViewManager.myHandView.RemoveCardInstant(instanceMap[instanceId]));
                }
                instanceMap[instanceId] = instance;
                break;
            }
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int instanceId
    // parameters[2]: CardVisualState cardVisualState
    public void ChangeCardStateTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        CardVisualState cardState = (CardVisualState)parameters[2];

        if (instanceMap.TryGetValue(instanceId, out GameObject obj))
        {
            PointCardInstance pointIns = obj.GetComponent<PointCardInstance>();
            bool isOpponent = playerId != ClientGameState.playerSlot;
            if (pointIns != null)
            {
                pointIns.ChangeCardState(cardState, isOpponent);
            }
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int cardId
    // parameters[2]: int instanceId
    public void PeekTopCardEventTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int cardId = (int)parameters[1];
        int instanceId = (int)parameters[2];
        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId);
        StartCoroutine(SceneViewManager.peekZoneView.AddCard(instance, playerId, false));
    }

    // parameters[0]: bool isPeekZone
    public void ClearResolve(object[] parameters)
    {
        bool isPeekZone = (bool)parameters[0];

        if (isPeekZone)
            StartCoroutine(SceneViewManager.peekZoneView.ClearCards());
        else
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

        if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
            StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(reveal));
    }

    // parameters[0]: int winnerId
    // parameters[1]: int currentBet
    public void RevealTest(object[] parameters)
    {
        int winnerId = (int)parameters[0];
        int currentBet = (int)parameters[1];

        SceneViewManager.roleView.ShowWin(winnerId);
        SceneViewManager.endTurnView.btnLight.intensity = 0;

        bool isOpponent = winnerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            // 多余的筹码退回筹码盘
            int returnCount = SceneViewManager.myChipView.chipsPlaced.Count - currentBet;
            SceneViewManager.myChipView.GenerateChips(returnCount);
            // 对方获得筹码
            SceneViewManager.opponentChipView.GenerateChips(currentBet);
        }
        else
        {
            // 筹码退回筹码盘
            SceneViewManager.myChipView.GenerateChips(SceneViewManager.myChipView.chipsPlaced.Count);
            // 获得筹码
            SceneViewManager.myChipView.GenerateChips(currentBet);
        }

        // 销毁筹码
        SceneViewManager.myChipView.DestroyChipsPlaced();
        SceneViewManager.opponentChipView.DestroyChipsPlaced();

        StartCoroutine(SceneViewManager.viewAnimController.PlayGameEndAnim(1f));
    }
}
