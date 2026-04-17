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
        ProcessDispatcher.Register("SumPointTest", SumPointTest);
        ProcessDispatcher.Register("EmojiTest", EmojiTest);
    }

    public void EmojiTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int emojiId = (int)parameters[1];

        Debug.Log($"EmojiTest {emojiId}");
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

        SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().isEnabled = true;
        SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().startText.SetActive(true);

        SceneViewManager.myChipView.StartGame(false);
        SceneViewManager.opponentChipView.StartGame(true);
    }

    public void StartGameTest(object[] parameters)
    {
        StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());

        instanceMap.Clear();
        foreach (int k in SceneViewManager.myChipView.chipsInTray.Keys)
        {
            instanceMap[k] = SceneViewManager.myChipView.chipsInTray[k];
        }
        foreach (int k in SceneViewManager.opponentChipView.chipsInTray.Keys)
        {
            instanceMap[k] = SceneViewManager.opponentChipView.chipsInTray[k];
        }

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

        foreach (var obj in SceneViewManager.myChipView.chipsInTray.Values)
        {
            if (obj.transform.childCount > 0)
            {
                ChipMouseEventHandler drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipMouseEventHandler>();
                drag.Init();
            }
        }

        StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(false));
        StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true));

        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(ClientGameState.Instance.punterId);
    }

    // parameters[0]: int playerId
    // parameters[1]: int apCount
    // parameters[2]: bool reset
    public void AddActionPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int apCount = (int)parameters[1];
        bool reset = (bool)parameters[2];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            if (reset) SceneViewManager.opponentActionPointView.ResetPoint();
            SceneViewManager.opponentActionPointView.AddPoint(apCount);
        }
        else
        {
            if (reset) SceneViewManager.myActionPointView.ResetPoint();
            SceneViewManager.myActionPointView.AddPoint(apCount);
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int apCount
    public void SpendActionPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int apCount = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
            SceneViewManager.opponentActionPointView.SpendPoint(apCount);
        else
            SceneViewManager.myActionPointView.SpendPoint(apCount);
    }


    // parameters[0]: int playerId
    // parameters[1]: int instanceId
    public void Place1BetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int instanceId = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(false));
            StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true));
        }
        else
        {
            GameObject obj = SceneViewManager.myChipView.chipsInTray[instanceId];
            ChipMouseEventHandler script = obj.GetComponentInChildren<ChipMouseEventHandler>();
            if (script != null)
            {
                script.enabled = false;
            }
            StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true));
            SceneViewManager.myChipView.Place1Bet(instanceId);
        }
    }

    // parameters[0]: int playerId
    public void ConfirmBetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];

        //foreach (var obj in SceneViewManager.myChipView.chipsInTray.Keys)
        //{
        //    ChipMouseEventHandler script = obj.GetComponentInChildren<ChipMouseEventHandler>();
        //    if (script != null)
        //    {
        //        Destroy(script);
        //    }
        //}

        StartCoroutine(SceneViewManager.viewAnimController.CloseChipCover());
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(ClientGameState.Instance.punterId);
        //ClientCommand.RevealCardsAndScore();

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
                SkillCardController skillCard = obj.GetComponent<SkillCardController>();
                skillCard.stateMachine.ChangeState(skillCard.readyFallState);
            }
            break;

            case AnimationType.ReturnToHand:
            {
                SkillCardController skillCard = obj.GetComponent<SkillCardController>();
                if (skillCard != null)
                {
                    if (isOpponent)
                        SceneViewManager.opponentHandView.ReturnCard(obj);
                    else
                        SceneViewManager.myHandView.ReturnCard(obj);
                    skillCard.stateMachine.ChangeState(skillCard.inHandState);
                }

                ChipController chipController = obj.GetComponentInChildren<ChipController>();
                if (chipController != null)
                {
                    if (!isOpponent)
                    {
                        SceneViewManager.myChipView.ReturnCard(instanceId, obj);
                        obj.GetComponentInChildren<ChipMouseEventHandler>().enabled = true;
                        chipController.stateMachine.ChangeState(chipController.inTrayState);
                    }
                }
            }
            break;

            case AnimationType.MoveToExecutePosition:
            {
                SkillCardController skillCard = obj.GetComponent<SkillCardController>();
                skillCard.stateMachine.ChangeState(skillCard.executeState);
            }
            break;
        }
    }

    // parameters[0]: int instanceId
    // parameters[1]: int targeValue
    public void ModifyPoint(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        int targeValue = (int)parameters[1];

        instanceMap[instanceId].GetComponentInChildren<PointCardViewController>().ChangeCardTexture_None(targeValue);
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

    // parameters[0]: int playerId
    // parameters[1]: int turn
    // parameters[2]: bool reveal
    public void EndTurnTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int turn = (int)parameters[1];
        bool reveal = (bool)parameters[2];

        // 亮灯
        if (ClientGameState.Instance.dealerId == playerId)
        {
            int myTurn = (turn + 1) / 2;
            int opTurn = turn / 2 + 1;
            SceneViewManager.myTurnLightView.SetLight(myTurn);
            SceneViewManager.opponentTurnLightView.SetLight(opTurn);

            StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(reveal));
            StartCoroutine(SceneViewManager.opponentRevealButtonView.RandomAnimation(reveal));
        }
        else
        {
            int myTurn = turn / 2 + 1;
            int opTurn = (turn + 1) / 2;
            SceneViewManager.myTurnLightView.SetLight(myTurn);
            SceneViewManager.opponentTurnLightView.SetLight(opTurn);
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int winnerId
    // parameters[2]: int currentBet
    // parameters[3]: int playerPointsOnBoard
    // parameters[4]: int opponentPoints
    public void RevealTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int winnerId = (int)parameters[1];
        int currentBet = (int)parameters[2];
        int playerPoints = (int)parameters[3];
        int opponentPoints = (int)parameters[4];

        StartCoroutine(_Reveal(playerId, winnerId, currentBet, playerPoints, opponentPoints));
    }

    private IEnumerator _Reveal(int playerId, int winnerId, int currentBet, int playerPoints, int opponentPoints)
    {
        bool isOpponent = winnerId != ClientGameState.playerSlot;

        SceneViewManager.endTurnView.btnLight.intensity = 0;

        yield return SceneViewManager.boardView.HoleCardFlip();
        if (playerId == ClientGameState.playerSlot)
        {
            SceneViewManager.mySumPointView.ChangeSum(playerPoints, true);
            SceneViewManager.opponentSumPointView.ChangeSum(opponentPoints, true);
        }
        else
        {
            SceneViewManager.mySumPointView.ChangeSum(opponentPoints, true);
            SceneViewManager.opponentSumPointView.ChangeSum(playerPoints, true);
        }
        yield return new WaitForSecondsRealtime(1f);
        yield return SceneViewManager.boardView.RemoveOneSideCards(1 - winnerId);
        yield return SceneViewManager.roleView.ShowWin(winnerId);

        // chip
        if (isOpponent)
        {
            // 多余的筹码退回筹码盘
            int returnCount = SceneViewManager.myChipView.chipsPlaced.Count - currentBet;
            SceneViewManager.myChipView.GenerateChips(returnCount, false);
            // 对方获得筹码
            SceneViewManager.opponentChipView.GenerateChips(currentBet, true);
        }
        else
        {
            // 筹码退回筹码盘
            SceneViewManager.myChipView.GenerateChips(SceneViewManager.myChipView.chipsPlaced.Count, false);
            // 获得筹码
            SceneViewManager.myChipView.GenerateChips(currentBet, false);
        }
        // 销毁筹码
        SceneViewManager.myChipView.DestroyChipsPlaced();
        SceneViewManager.opponentChipView.DestroyChipsPlaced();

        yield return SceneViewManager.viewAnimController.PlayGameEndAnim(1f);

    }

    // parameters[0]: int playerId
    // parameters[1]: int playerPointsOnBoard
    // parameters[2]: int playerHoleCardPoint
    // parameters[3]: bool hasHiddenCard
    // parameters[4]: int opponentPointsOnBoard
    public void SumPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int playerPointsOnBoard = (int)parameters[1];
        int playerHoleCardPoint = (int)parameters[2];
        bool hasHiddenCard = (bool)parameters[3];
        int opponentPointsOnBoard = (int)parameters[4];

        SceneViewManager.mySumPointView.ChangeSum(playerPointsOnBoard + playerHoleCardPoint, !hasHiddenCard);
        SceneViewManager.opponentSumPointView.ChangeSum(opponentPointsOnBoard, false);
    }
}
