using FishNet.Demo.AdditiveScenes;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private HandView ophandView;
    [SerializeField] private BoardView boardView;
    [SerializeField] private ResolveZoneView ResolveZoneView;
    private Stack<GameObject> objs = new Stack<GameObject>();
    private int turn = 0;
    private int card = 1;
    private GameObject instance1;
    private GameObject instance2;

    [SerializeField] private EventProcessFunction eventProcessFunction;

    private void Update()
    {
        // 模拟开始游戏，生成筹码
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());
            SceneViewManager.myChipView.StartGame(false);
            SceneViewManager.opponentChipView.StartGame(true);
            foreach (var obj in SceneViewManager.myChipView.chipsInTray.Values)
            {
                ChipMouseEventHandler drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipMouseEventHandler>();
                drag.Init();
            }
        }

        // 抽技能牌
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ClientCommand.DrawSkillCard();
            instance1 = CardViewCreator.Instance.CreateCardInstance(1001, 9999);
            StartCoroutine(handView.AddCard(instance1));
            instance2 = CardViewCreator.Instance.CreateCardInstance(1301, 9998);
            StartCoroutine(ophandView.AddCard(instance2));
        }

        // 技能牌落地
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(instance1, false));
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToFallPosition(instance2, true));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(instance1));
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToExecutePosition(instance2));
        }

        // 测试ResolveZoneView
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (card <= 10)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardResolved(card++, 989);
                StartCoroutine(ResolveZoneView.AddCard(instance, -1, true));
            }
        }

        // 测试PeekZoneView
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardResolved(2, 99);
            StartCoroutine(SceneViewManager.peekZoneView.AddCard(instance, -1, false));
        }

        // 抽点数牌
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (card <= 1)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98);
                StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.Hidden));
                objs.Push(instance);
            }
            else if (card <= 5)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98);
                StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.None));
                objs.Push(instance);
            }
            else if (card <= 10)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98);
                StartCoroutine(boardView.AddCard(instance, 99, CardVisualState.None));
                objs.Push(instance);
            }
        }

        // 移除牌局区最近一张牌
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objs.Count > 0)
            {
                StartCoroutine(SceneViewManager.boardView.RemoveCard(objs.Pop()));
            }
        }

        // 移除一方所有牌
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneViewManager.boardView.GenerateLazer(objs.Pop().transform.position);
        }

        // 发底牌
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.Hole));
            instance = CardViewCreator.Instance.CreateCardInstance(3, 99);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, 99, CardVisualState.Hole));
        }

        // 测试移动
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(SceneViewManager.boardView.MoveCard(objs.Pop(), ClientGameState.playerSlot));
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            eventProcessFunction.RevealTest(new object[] { 1 - ClientGameState.playerSlot, 1, 2, 3});
            SceneViewManager.myRevealButtonView.ShowRandom();
            StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(true));
        }

        // 测试回合灯
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneViewManager.myTurnLightView.SetLight(++turn);
        }

        // 测试庄闲
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneViewManager.roleView.ShowRole(1-ClientGameState.playerSlot);
            SceneViewManager.roleView.ShowRole(ClientGameState.playerSlot);
        }
    }
}
