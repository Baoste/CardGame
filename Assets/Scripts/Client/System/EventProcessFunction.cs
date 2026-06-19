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
        ProcessDispatcher.Register("EndMatchTest", EndMatchTest);
        ProcessDispatcher.Register("SumPointTest", SumPointTest);
        ProcessDispatcher.Register("EmojiTest", EmojiTest);
    }

    public IEnumerator EmojiTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int emojiId = (int)parameters[1];

        Debug.Log($"EmojiTest {emojiId}");
        yield break;
        //bool isOpponent = playerId != ClientGameState.playerSlot;
        //if (isOpponent)
        //    SceneViewManager.opponentEmojiView.ShowEmoji(emojiId);
        //else
        //    SceneViewManager.myEmojiView.ShowEmoji(emojiId);
    }

    public IEnumerator StartMatchTest(object[] parameters)
    {
        CinemachineVirtualCamera vcam = GameObject.Find("VCamera_Playing").GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 20;

        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().isEnabled = true;
        // SceneViewManager.boardView.transform.GetComponentInChildren<ClickToStartGame>().startText.SetActive(true);
        yield return new WaitForSeconds(1f);
        ClientCommand.StartGame();

        SceneViewManager.myChipView.StartGame(false);
        SceneViewManager.opponentChipView.StartGame(true);
    }

    public IEnumerator StartGameTest(object[] parameters)
    {
        SceneViewManager.myHandView.skillCardDeck.ResetCardStack();

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
        yield break;
    }

    // parameters[0]: int playerId
    // parameters[1]: int turn
    public IEnumerator StartTurnTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int turn = (int)parameters[1];

        // rotate turn indicator
        SceneViewManager.turnIndicator.Rotate2Player(ClientGameState.playerSlot != playerId);
        yield return new WaitForSeconds(SceneViewManager.turnIndicator.rotateTime);

        if (ClientGameState.playerSlot == playerId)
        {
            SceneViewManager.endTurnView.btnLight.intensity = 1;
            SceneViewManager.endTurnView.hasClicked = false;
        }

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
        AudioManager.Instance.Play("TurnLightOn");
        
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
        yield break;
    }

    // parameters[0]: int playerId
    // parameters[1]: InvalidActionType invalidType
    // parameters[2]: int instanceId
    public IEnumerator InvalidActionTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        InvalidActionType invalidType = (InvalidActionType)parameters[1];
        int instanceId = (int)parameters[2];

        AudioManager.Instance.Play("InvalidAction");

        // TODO: 显示错误提示
        Debug.Log($"[Client] Player {playerId} performed invalid action: {invalidType}");

        switch (invalidType)
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
        yield break;
    }

    // parameters[0]: int dealerId
    // parameters[1]: int punterId
    // parameters[2]: int placeBetCount
    public IEnumerator AssignRolesTest(object[] parameters)
    {
        int dealerId = (int)parameters[0];
        int punterId = (int)parameters[1];
        int placeBetCount = (int)parameters[2];

        SceneViewManager.roleView.ShowRole(dealerId);

        foreach (var obj in SceneViewManager.myChipView.chipsInTray.Values)
        {
            if (obj.transform.childCount > 0)
            {
                ChipMouseEventHandler drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipMouseEventHandler>();
                drag.Init();
            }
        }

        StartCoroutine(AudioManager.Instance.Play("Chip_Up", 1f));
        for (int i = 0; i < placeBetCount; i++)
        {
            StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(false, 1f));
            yield return SceneViewManager.opponentChipView.Place1BetAuto(true, 1f);
        }

        if (punterId == ClientGameState.playerSlot)
            ClientCommand.StartTurn(punterId);
    }

    // parameters[0]: int playerId
    // parameters[1]: int apCount
    // parameters[2]: bool reset
    public IEnumerator AddActionPointTest(object[] parameters)
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
        yield break;
    }

    // parameters[0]: int playerId
    // parameters[1]: int apCount
    public IEnumerator SpendActionPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int apCount = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
            SceneViewManager.opponentActionPointView.SpendPoint(apCount);
        else
            SceneViewManager.myActionPointView.SpendPoint(apCount);
        yield break;
    }

    // parameters[0]: int playerId
    // parameters[1]: bool judgeResult
    // parameters[2]: EffectAnimation effectAnimation
    public IEnumerator JudgeResultTest(object[] parameters)
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
        yield break;
    }


    // parameters[0]: int playerId
    // parameters[1]: int instanceId
    public IEnumerator Place1BetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int instanceId = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        if (isOpponent)
        {
            AudioManager.Instance.Play("Chip_Up");
            yield return SceneViewManager.opponentChipView.Place1BetAuto(true, 0);
            yield return SceneViewManager.callOrFoldMachine.Show(1);
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
            yield return SceneViewManager.callOrFoldMachineBack.Show(1);
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int[] instanceIds
    public IEnumerator PlaceBetsTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int[] instanceId = (int[])parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;


        if (isOpponent)
        {
            if (instanceId.Length < 1)  yield break;
            AudioManager.Instance.Play("Chip_Up");
            for (int i = 0; i < instanceId.Length; i++)
            {
                StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 0));
            }
            yield return SceneViewManager.callOrFoldMachine.Show(instanceId.Length);
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
    public IEnumerator ConfirmBetTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int betCount = (int)parameters[1];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        AudioManager.Instance.Play("Chip_Up");
        if (isOpponent)
        {
            for (int i = 0; i < betCount; i++)
                StartCoroutine(SceneViewManager.opponentChipView.Place1BetAuto(true, 0));
            yield return SceneViewManager.callOrFoldMachineBack.Hide();
        }
        else
        {
            for (int i = 0; i < betCount; i++)
                StartCoroutine(SceneViewManager.myChipView.Place1BetAuto(true, 0));
            yield return SceneViewManager.callOrFoldMachine.Hide();
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
    public IEnumerator PlayAnimation(object[] parameters)
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
                ClientEffectContext.isExecutingSkillCard = false;

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
        yield break;
    }

    // parameters[0]: int instanceId
    // parameters[1]: int targeValue
    public IEnumerator ModifyPoint(object[] parameters)
    {
        int instanceId = (int)parameters[0];
        int targeValue = (int)parameters[1];

        GameObject obj = instanceMap[instanceId];
        PointCardInstance pointIns = obj.GetComponent<PointCardInstance>();
        obj.GetComponent<PointCardViewController>().ChangeCardTexture(pointIns.cardVisualState, targeValue);
        yield break;
    }


    // parameters[0]: int playerId
    // parameters[1]: List<int> instanceIds
    // parameters[2]: EffectAnimation effectAnimation
    public IEnumerator DiscardCard(object[] parameters)
    {
        int playerId = (int)parameters[0];
        List<int> instanceIds = (List<int>)parameters[1];
        EffectAnimation effectAnimation = (EffectAnimation)parameters[2];

        if (instanceIds.Count < 1)
        {
            yield break;
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
            yield break;
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
    public IEnumerator DrawPointCard(object[] parameters)
    {
        int cardId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        int playerId = (int)parameters[2];
        CardVisualState cardState = (CardVisualState)parameters[3];

        bool isOpponent = playerId != ClientGameState.playerSlot;
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
        instanceMap[instanceId] = instance;

        AudioManager.Instance.Play("DrawPointCard");

        yield return SceneViewManager.boardView.AddCard(instance, playerId, cardState);

        // 操作了就隐藏爆牌按钮
        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            SceneViewManager.myRevealButtonView.HideButton();
        else
            SceneViewManager.opponentRevealButtonView.HideButton();
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    public IEnumerator DrawSkillCard(object[] parameters)
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

        yield break;
    }

    // parameters[0]: int cardId
    // parameters[1]: int instanceId
    // parameters[2]: int playerId
    // parameters[3]: bool isShown
    // parameters[4]: CardVisualState cardState
    public IEnumerator ToResolveTest(object[] parameters)
    {
        int cardId      = (int)parameters[0];
        int instanceId  = (int)parameters[1];
        int playerId    = (int)parameters[2];
        bool isShown    = (bool)parameters[3];
        CardVisualState cardState = (CardVisualState)parameters[4];

        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId);
        yield return SceneViewManager.resolveZoneView.AddCard(instance, playerId, isShown, cardState, 1.5f);
    }

    // parameters[0]: int playerId
    // parameters[1]: int cardId
    // parameters[2]: int instanceId
    // parameters[3]: ParticipantType toZone
    // parameters[4]: CardVisualState cardState
    public IEnumerator MoveCard(object[] parameters)
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
                    yield return SceneViewManager.boardView.MoveCard(obj, 1 - playerId);
                }
                else
                {
                    GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                    yield return SceneViewManager.boardView.AddCard(instance, playerId, cardState);
                    instanceMap[instanceId] = instance;
                }
                break;
            }
            case ParticipantType.OppentBoardZone:
            {
                if (instanceMap.TryGetValue(instanceId, out var obj))
                {
                    yield return SceneViewManager.boardView.MoveCard(obj, playerId);
                }
                else
                {
                    GameObject instance = CardViewCreator.Instance.CreateCardInstance(cardId, instanceId);
                    yield return SceneViewManager.boardView.AddCard(instance, 1 - playerId, cardState);
                    instanceMap[instanceId] = instance;
                }
                break;
            }
            case ParticipantType.MySkillCardsInHand:
            {
                if (isOpponent)
                {
                    yield return (SceneViewManager.myHandView.RemoveCard(instanceMap[instanceId]));
                    yield return (SceneViewManager.opponentHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                else
                {
                    yield return (SceneViewManager.opponentHandView.RemoveCard(instanceMap[instanceId]));
                    yield return (SceneViewManager.myHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                break;
            }
            case ParticipantType.OpponentSkillCardsInHand:
            {
                if (isOpponent)
                {
                    yield return (SceneViewManager.opponentHandView.RemoveCard(instanceMap[instanceId]));
                    yield return (SceneViewManager.myHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                else
                {
                    yield return (SceneViewManager.myHandView.RemoveCard(instanceMap[instanceId]));
                    yield return (SceneViewManager.opponentHandView.AddCardFromOthers(instanceMap[instanceId]));
                }
                break;
            }
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int instanceId
    // parameters[2]: CardVisualState cardVisualState
    // parameters[3]: EffectAnimation effectAnimation
    public IEnumerator ChangeCardStateTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int instanceId = (int)parameters[1];
        CardVisualState cardState = (CardVisualState)parameters[2];
        EffectAnimation effectAnimation = (EffectAnimation)parameters[3];

        switch (effectAnimation)
        {
            case EffectAnimation.ChangeCardState_Normal:
            {
                break;
            }
            case EffectAnimation.ChangeCardState_Hidden:
            {
                AudioManager.Instance.Play("SkillCard_Effect_Tar");

                SceneViewManager.viewAnimController.TablePlaneMatManager.SetFirstMaterial("Tar");
                SceneViewManager.viewAnimController.TablePlaneMatManager.PlayPlaneAnim(1.2f, 0.5f, 1.2f);
                break;
            }
        }

        if (instanceMap.TryGetValue(instanceId, out GameObject obj))
        {
            PointCardInstance pointIns = obj.GetComponent<PointCardInstance>();
            bool isOpponent = playerId != ClientGameState.playerSlot;
            if (pointIns != null)
            {
                pointIns.ChangeCardState(cardState, isOpponent);
            }
        }
        yield break;
    }

    // parameters[0]: int playerId
    // parameters[1]: int cardId
    // parameters[2]: int instanceId
    public IEnumerator PeekTopCardEventTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int cardId = (int)parameters[1];
        int instanceId = (int)parameters[2];
        GameObject instance = CardViewCreator.Instance.CreateCardResolved(cardId, instanceId);
        yield return SceneViewManager.peekZoneView.AddCard(instance, playerId, false, CardVisualState.None, 0.7f);
    }

    // parameters[0]: bool isPeekZone
    public IEnumerator ClearResolve(object[] parameters)
    {
        bool isPeekZone = (bool)parameters[0];

        if (isPeekZone)
            yield return (SceneViewManager.peekZoneView.ClearCards());
        else
            yield return (SceneViewManager.resolveZoneView.ClearCards());
    }

    // parameters[0]: int playerId
    // parameters[1]: int turn
    // parameters[2]: bool reveal
    public IEnumerator EndTurnTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int turn = (int)parameters[1];
        bool reveal = (bool)parameters[2];

        if (ClientGameState.Instance.punterId == ClientGameState.playerSlot)
            SceneViewManager.myRevealButtonView.HideButton();
        else
            SceneViewManager.opponentRevealButtonView.HideButton();

        if (ClientGameState.Instance.dealerId == playerId)
        {
            yield return SceneViewManager.myRevealButtonView.RandomAnimation(reveal);
            yield return SceneViewManager.opponentRevealButtonView.RandomAnimation(reveal);
        }

        yield return new WaitForSeconds(0.5f);
        if (ClientGameState.playerSlot == playerId)
            ClientCommand.StartTurn(1 - ClientGameState.playerSlot);
    }

    // parameters[0]: int playerId
    // parameters[1]: int winnerId
    // parameters[2]: int currentBet
    // parameters[3]: int playerPointsOnBoard
    // parameters[4]: int opponentPoints
    public IEnumerator RevealTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int winnerId = (int)parameters[1];
        int currentBet = (int)parameters[2];
        int playerPoints = (int)parameters[3];
        int opponentPoints = (int)parameters[4];

        StartCoroutine(SceneViewManager.callOrFoldMachine.Hide());
        StartCoroutine(SceneViewManager.callOrFoldMachineBack.Hide());

        // StartCoroutine(_Reveal(playerId, winnerId, currentBet, playerPoints, opponentPoints));
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
        SceneViewManager.myExecuteCardView.DestroyCard(null);
        SceneViewManager.opponentExecuteCardView.DestroyCard(null);
        yield return new WaitForSecondsRealtime(0.5f);
        yield return SceneViewManager.boardView.RemoveHoleCard(1 - winnerId);
        yield return SceneViewManager.boardView.RemoveOneSideCards(1 - winnerId);
        yield return SceneViewManager.roleView.ShowWin(winnerId);
    }

    // parameters[0]: int finalWinnerId
    // parameters[1]: int winnerId
    // parameters[2]: int currentBet
    public IEnumerator EndMatchTest(object[] parameters)
    {
        int finalWinnerId = (int)parameters[0];
        int winnerId = (int)parameters[1];
        int currentBet = (int)parameters[2];

        // DelayToStartGame
        if (finalWinnerId == -1)
        {
            bool isOpponentWin = winnerId != ClientGameState.playerSlot;

            yield return new WaitForSeconds(2f);

            StartCoroutine(SceneViewManager.myChipView.RotateContainer());
            yield return SceneViewManager.opponentChipView.RotateContainer();
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
            StartCoroutine(SceneViewManager.myChipView.RotateContainer());
            yield return SceneViewManager.opponentChipView.RotateContainer();

            yield return SceneViewManager.viewAnimController.PlayGameEndAnim();

            yield return new WaitForSeconds(2f);
            ClientCommand.StartGame();
        }
        // DelayToEndGame
        else
        {
            Transform root = CardViewCreator.Instance.transform;
            foreach (Transform child in root)
            {
                yield return null;
                IDiscardPresentation discardPresentation = child.gameObject.GetComponent<IDiscardPresentation>();
                discardPresentation?.DiscardPlay();
            }

            bool isWinner = finalWinnerId == ClientGameState.playerSlot;
            yield return new WaitForSeconds(2f);

            SceneViewManager.myChipView.MoveChipsPlaced();
            SceneViewManager.opponentChipView.MoveChipsPlaced();
            // TODO: 显示胜负动画
            StartCoroutine(SceneViewManager.viewAnimController.PlayMatchEndAnim(isWinner));
        }
    }

    // parameters[0]: int playerId
    // parameters[1]: int playerPointsOnBoard
    // parameters[2]: int playerHoleCardPoint
    // parameters[3]: bool hasHiddenCard
    // parameters[4]: int opponentPointsOnBoard
    public IEnumerator SumPointTest(object[] parameters)
    {
        int playerId = (int)parameters[0];
        int playerPointsOnBoard = (int)parameters[1];
        int playerHoleCardPoint = (int)parameters[2];
        bool hasHiddenCard = (bool)parameters[3];
        int opponentPointsOnBoard = (int)parameters[4];

        SceneViewManager.mySumPointView.ChangeSum(playerPointsOnBoard + playerHoleCardPoint, !hasHiddenCard);
        SceneViewManager.opponentSumPointView.ChangeSum(opponentPointsOnBoard, false);
        yield break;
    }
}
