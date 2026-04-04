using Game.Domain;
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

    private void Update()
    {
        // 模拟开始游戏，生成筹码
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());
            SceneViewManager.myChipView.GenerateChips(6);
            SceneViewManager.opponentChipView.GenerateChips(6);
            foreach (var obj in SceneViewManager.myChipView.chipsInTray)
            {
                ChipDraggable drag = obj.transform.GetChild(0).gameObject.AddComponent<ChipDraggable>();
                drag.Init();
            }
        }

        // 抽技能牌
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ClientCommand.DrawSkillCard();
            instance1 = CardViewCreator.Instance.CreateCardInstance(6219298, 9999);
            StartCoroutine(handView.AddCard(instance1));
            instance2 = CardViewCreator.Instance.CreateCardInstance(6219298, 9999);
            StartCoroutine(ophandView.AddCard(instance2));
        }

        // 技能牌落地
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToFallPosition(instance1));
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToFallPosition(instance2));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(instance1));
            StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToExecutePosition(instance2));
        }

        // 测试牌局区和结算区
        if (Input.GetKeyDown(KeyCode.Q))
        {

            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99999);
            StartCoroutine(ResolveZoneView.AddCard(instance, -1, true));
        }

        // 抽点数牌
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (card <= 5)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98);
                StartCoroutine(boardView.AddCard(instance, ClientGameState.playerSlot, false));
                objs.Push(instance);
            }
            else if (card <= 10)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardInstance(card++, 98);
                StartCoroutine(boardView.AddCard(instance, 99, false));
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

        // 移除牌局区所有牌
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(SceneViewManager.boardView.RemoveAllCards());
            objs.Clear();
            card = 1;
        }

        // 发底牌
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, 99);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, true));
            instance = CardViewCreator.Instance.CreateCardInstance(2, 99);
            StartCoroutine(SceneViewManager.boardView.AddCard(instance, 99, true));
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SceneViewManager.myRevealButtonView.ShowButton(true);
        //    SceneViewManager.opponentRevealButtonView.ShowRandom();
        //}

        // 测试回合灯
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneViewManager.myTurnLightView.SetLight(++turn);
        }
    }
}
