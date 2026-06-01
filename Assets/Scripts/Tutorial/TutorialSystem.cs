using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    private int instanceID = 10000;

    private void Start()
    {
        StartCoroutine(StartTutotrial());
    }

    private IEnumerator StartTutotrial()
    {
        // Init
        StartGame();
        yield return new WaitForSecondsRealtime(1f);
        InitPointCard();
        InitSkillCard();
    }

    private void StartGame()
    {
        StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());
        SceneViewManager.myChipView.StartGame(false);
        SceneViewManager.opponentChipView.StartGame(true);
        foreach (var obj in SceneViewManager.myChipView.chipsInTray.Values)
        {
            ChipMouseEventHandler drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipMouseEventHandler>();
            drag.Init();
        }

        // 玩家是闲家
        SceneViewManager.roleView.ShowRole(1 - ClientGameState.playerSlot);
        SceneViewManager.turnIndicator.Rotate2Player(false);
        // 第三回合开始
        SceneViewManager.myTurnLightView.SetLight(3);
        SceneViewManager.opponentTurnLightView.SetLight(3);
    }

    private void InitPointCard()
    {
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(1, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.Hole));
        instance = CardViewCreator.Instance.CreateCardInstance(1, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - ClientGameState.playerSlot, CardVisualState.Hole));

        instance = CardViewCreator.Instance.CreateCardInstance(2, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.None));
    }

    private void InitSkillCard()
    { 
        // 加点牌
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(1001, instanceID++);
        StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
        instance = CardViewCreator.Instance.CreateCardInstance(1001, instanceID++);
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 减点牌
        instance = CardViewCreator.Instance.CreateCardInstance(1002, instanceID++);
        StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
        instance = CardViewCreator.Instance.CreateCardInstance(1002, instanceID++);
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 行动补充
        instance = CardViewCreator.Instance.CreateCardInstance(1004, instanceID++);
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 猜单双
        instance = CardViewCreator.Instance.CreateCardInstance(1107, instanceID++);
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
    }
}
