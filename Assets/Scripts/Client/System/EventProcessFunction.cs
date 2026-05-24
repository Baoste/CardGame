using Cinemachine;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        ProcessDispatcher.Register("JudgeResultTest", JudgeResultTest);
        ProcessDispatcher.Register("Place1BetTest", Place1BetTest);
        ProcessDispatcher.Register("PlaceBetsTest", PlaceBetsTest);
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

        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().isEnabled = true;
        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().startText.SetActive(true);
        StartCoroutine(_SendStartGameCmd());

        SceneViewManager.myChipView.StartGame(false);
        SceneViewManager.opponentChipView.StartGame(true);
    }

    private IEnumerator _SendStartGameCmd()
    {
        yield return new WaitForSeconds(1f);
        ClientCommand.StartGame();
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

        // rotate turn indicator
        SceneViewManager.turnIndicator.Rotate2Player(ClientGameState.playerSlot != playerId);

        if (ClientGameState.playerSlot == playerId)
            SceneViewManager.endTurnView.btnLight.intensity = 1;

        if (turn == 1)
        {
            if (ClientGameState.Instance.dealerId == ClientGameState.playerSlot)
                SceneViewManager.opponentTurnLightView.SetLight(1);
            else
                SceneViewManager.myTurnLightView.SetLight(1);
        }

        int endTurnCount = 9;
        if (playerId != ClientGameState.Instance.dealerId && turn >= endTurnCount)
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

        // 亮灯
        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (ClientGameState.Instance.dealerId == playerId)
        {
            int dealerTurn = (turn + 1) / 2;
            if (isOpponent)
                SceneViewManager.opponentTurnLightView.SetLight(dealerTurn);
            else
                SceneViewManager.myTurnLightView.SetLight(dealerTurn);
        }
        else
        {
            int playerTurn = turn / 2 + 1;
            if (isOpponent)
                SceneViewManager.opponentTurnLightView.SetLight(playerTurn);
            else
                SceneViewManager.myTurnLightView.SetLight(playerTurn);
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
                if (instanceId == -1) break;
                ClientEffectContext.isExecutingSkillCard = false;
                PlayAnimationCommand cmd = new PlayAnimationCommand { playerId = playerId, animType = AnimationType.ReturnToHand, instanceId = instanceId };
                ClientGameState.gateway.SendCommandServerRpc("PlayAnimation", JsonConvert.SerializeObject(cmd));
                break;
            case InvalidActionType.NoCardToDraw:
                // 显示无牌可抽提示
                break;
            case InvalidActionType.SkillCardCountFull:
                // 显示手牌满了
                break;
            case InvalidActionType.SkillCardCountEmpty:
                // 显示没有技能牌了
                break;
            case InvalidActionType.PointCardDrawLimit:
                // 显示点数牌抽牌限制
                break;
            default:
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

        StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(false, 1f));
        StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 1f));

        if (punterId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(punterId);
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
    // parameters[1]: bool judgeResult
    // parameters[2]: EffectAnimation effectAnimation
    public void JudgeResultTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        bool judgeResult = (bool)parameters[1];
        EffectAnimation effectAnimation = (EffectAnimation)parameters[2];

        switch (effectAnimation)
        {
            case EffectAnimation.Judge_Normal:
            {
                break;
            }
            case EffectAnimation.Judge_OddEven:
            {
                if (judgeResult)
                {
                    AudioManager.Instance.Play("GuessOddEvenWin");
                }
                else
                {
                    AudioManager.Instance.Play("GuessOddEvenLose");
                }
                break;
            }
        }
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
            StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 0));
            StartCoroutine(SceneViewManager.callOrFoldMachine.Show(1));
        }
        else
        {
            GameObject obj = SceneViewManager.myChipView.chipsInTray[instanceId];
            ChipMouseEventHandler script = obj.GetComponentInChildren<ChipMouseEventHandler>();
            if (script != null)
            {
                script.enabled = false;
            }
            SceneViewManager.myChipView.Place1Bet(instanceId);
            StartCoroutine(SceneViewManager.callOrFoldMachineBack.Show(1));
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int[] instanceIds
    public void PlaceBetsTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int[] instanceId = (int[])parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;


        if (isOpponent)
        {
            if (instanceId.Length < 1)  return;
            for (int i = 0; i < instanceId.Length; i++)
            {
                StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 0));
            }
            StartCoroutine(SceneViewManager.callOrFoldMachine.Show(instanceId.Length));
        }
        else
        {
            if (instanceId.Length > 0)
                StartCoroutine(SceneViewManager.callOrFoldMachineBack.Show(instanceId.Length));

            List<ChipController> copiedList = SceneViewManager.myChipView.chipRaycastSelect.SelectedChips.ToList();
            // 放置有的
            foreach (int id in instanceId)
            {
                GameObject obj = SceneViewManager.myChipView.chipsInTray[id];

                ChipController chipController = obj.GetComponentInChildren<ChipController>();
                chipController.stateMachine.ChangeState(chipController.placedState);

                copiedList.Remove(chipController);

                ChipMouseEventHandler script = obj.GetComponentInChildren<ChipMouseEventHandler>();
                if (script != null)
                {
                    script.enabled = false;
                }
                SceneViewManager.myChipView.Place1Bet(id);
            }

            // 返回没有的
            foreach (var chipController in copiedList)
            {
                chipController.stateMachine.ChangeState(chipController.inTrayState);
            }

            SceneViewManager.myChipView.chipRaycastSelect.ClearList();
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int betCount
    public void ConfirmBetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int betCount = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            for (int i = 0; i < betCount; i++)
                StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 0));
            StartCoroutine(SceneViewManager.callOrFoldMachineBack.Hide());
        }
        else
        {
            for (int i = 0; i < betCount; i++)
                StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(true, 0));
            StartCoroutine(SceneViewManager.callOrFoldMachine.Hide());
        }

        //foreach (var obj in SceneViewManager.myChipView.chipsInTray.Keys)
        //{
        //    ChipMouseEventHandler script = obj.GetComponentInChildren<ChipMouseEventHandler>();
        //    if (script != null)
        //    {
        //        Destroy(script);
        //    }
        //}

        //StartCoroutine(SceneViewManager.viewAnimController.CloseChipCover());
        //if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
        //    ClientCommand.StartTurn(ClientGameState.Instance.punterId);
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

        GameObject obj = instanceMap[instanceId];
        PointCardInstance pointIns = obj.GetComponent<PointCardInstance>();
        obj.GetComponent<PointCardViewController>().ChangeCardTexture(pointIns.cardVisualState, targeValue);
    }


    // parameters[0]: int playerId
    // parameters[1]: List<int> instanceIds
    // parameters[2]: EffectAnimation effectAnimation
    public void DiscardCard(object[] parameters)
    {
        int playerId = (int)parameters[0];
        List<int> instanceIds = (List<int>)parameters[1];
        EffectAnimation effectAnimation = (EffectAnimation)parameters[2];

        if (instanceIds.Count < 1)
        {
            return;
        }

        if (instanceIds.Count == 1)
        {
            foreach (int instanceId in instanceIds)
            {
                GameObject obj = instanceMap[instanceId];
                instanceMap.Remove(instanceId);
                IDiscardPresentation discardPresentation = obj.GetComponent<IDiscardPresentation>();
                discardPresentation?.DiscardPlay();
            }
            return;
        }

        switch (effectAnimation)
        {
            case EffectAnimation.Discard_Normal:
            {
                foreach (int instanceId in instanceIds)
                {
                    GameObject obj = instanceMap[instanceId];
                    instanceMap.Remove(instanceId);
                    IDiscardPresentation discardPresentation = obj.GetComponent<IDiscardPresentation>();
                    discardPresentation?.DiscardPlay();
                }
                break;
            }
            case EffectAnimation.Discard_Lazer:
            {
                List<GameObject> objs = new List<GameObject>();
                foreach (int instanceId in instanceIds)
                {
                    objs.Add(instanceMap[instanceId]);
                    instanceMap.Remove(instanceId);
                }
                SceneViewManager.boardView.GenerateLazer(objs[0].transform.position, objs);
                break;
            }
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

        // 操作了就隐藏爆牌按钮
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            SceneViewManager.myRevealButtonView.HideButton();
        else
            SceneViewManager.opponentRevealButtonView.HideButton();
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

        AudioManager.Instance.Play("DrawSkillCard");

        // 操作了就隐藏爆牌按钮
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            SceneViewManager.myRevealButtonView.HideButton();
        else
            SceneViewManager.opponentRevealButtonView.HideButton();
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    // parameters[3]: bool isShown
    // parameters[4]: CardVisualState cardState
    public void ToResolveTest(object[] parameters)
    {
        int cardId      = (int)parameters[0];
        int instanceId  = (int)parameters[1];
        int playerId    = (int)parameters[2];
        bool isShown    = (bool)parameters[3];
        CardVisualState cardState = (CardVisualState)parameters[4];

        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId);
        StartCoroutine(SceneViewManager.resolveZoneView.AddCard(instance, playerId, isShown, cardState));
    }

    // parameters[0]: int playerId
    // parameters[1]: int cardId
    // parameters[2]: int instanceId
    // parameters[3]: ParticipantType toZone
    // parameters[4]: CardVisualState cardState
    public void MoveCard(object[] parameters)
    {
        int playerId                = (int)parameters[0];
        int cardId                  = (int)parameters[1];
        int instanceId              = (int)parameters[2];
        ParticipantType toZone      = (ParticipantType)parameters[3];
        CardVisualState cardState   = (CardVisualState)parameters[4];

        bool isOpponent = playerId != ClientGameState.playerSlot;

        switch (toZone)
        {
            case ParticipantType.MyBoardZone:
            {
                if (instanceMap.TryGetValue(instanceId, out var obj))
                {
                    StartCoroutine(SceneViewManager.boardView.MoveCard(obj, 1 - playerId));
                }
                else
                {
                    GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                    StartCoroutine(SceneViewManager.boardView.AddCard(instance, playerId, cardState));
                    instanceMap[instanceId] = instance;
                }
                break;
            }
            case ParticipantType.OppentBoardZone:
            {
                if (instanceMap.TryGetValue(instanceId, out var obj))
                {
                    StartCoroutine(SceneViewManager.boardView.MoveCard(obj, playerId));
                }
                else
                {
                    GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                    StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - playerId, cardState));
                    instanceMap[instanceId] = instance;
                }
                break;
            }
            case ParticipantType.MySkillCardsInHand:
            {
                if (isOpponent)
                {
                    StartCoroutine(SceneViewManager.myHandView.RemoveCard(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.opponentHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                else
                {
                    StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.myHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                break;
            }
            case ParticipantType.OpponentSkillCardsInHand:
            {
                if (isOpponent)
                { 
                    StartCoroutine(SceneViewManager.opponentHandView.RemoveCard(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.myHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                else
                {
                    StartCoroutine(SceneViewManager.myHandView.RemoveCard(instanceMap[instanceId]));
                    StartCoroutine(SceneViewManager.opponentHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
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
        StartCoroutine(SceneViewManager.peekZoneView.AddCard(instance, playerId, false, CardVisualState.None));
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

        bool isOpponent = playerId != ClientGameState.playerSlot;
        // 亮灯
        if (ClientGameState.Instance.dealerId == playerId)
        {
            //int dealerTurn = (turn + 1) / 2;
            //if (isOpponent)
            //    SceneViewManager.opponentTurnLightView.SetLight(dealerTurn + 1);
            //else
            //    SceneViewManager.myTurnLightView.SetLight(dealerTurn + 1);

            StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(reveal));
            StartCoroutine(SceneViewManager.opponentRevealButtonView.RandomAnimation(reveal));
        }
        //else
        //{
        //    int playerTurn = turn / 2 + 1;
        //    if (isOpponent)
        //        SceneViewManager.opponentTurnLightView.SetLight(playerTurn + 1);
        //    else
        //        SceneViewManager.myTurnLightView.SetLight(playerTurn + 1);
        //}

        // 隐藏爆牌按钮
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            SceneViewManager.myRevealButtonView.HideButton();
        else
            SceneViewManager.opponentRevealButtonView.HideButton();
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

        StartCoroutine(SceneViewManager.callOrFoldMachine.Hide());
        StartCoroutine(SceneViewManager.callOrFoldMachineBack.Hide());

        StartCoroutine(_Reveal(playerId, winnerId, currentBet, playerPoints, opponentPoints));
    }

    private IEnumerator _Reveal(int playerId, int winnerId, int currentBet, int playerPoints, int opponentPoints)
    {
        bool isOpponentWin = winnerId != ClientGameState.playerSlot;

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
        yield return SceneViewManager.boardView.RemoveHoleCard(1 - winnerId);
        yield return SceneViewManager.boardView.RemoveOneSideCards(1 - winnerId);
        yield return SceneViewManager.roleView.ShowWin(winnerId);

        // chip
        if (isOpponentWin)
        {
            // 筹码退回筹码盘
            SceneViewManager.opponentChipView.GenerateChips(SceneViewManager.opponentChipView.chipsPlaced.Count, true);
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
