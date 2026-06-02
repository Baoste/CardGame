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
    private int apCount = 0;
    private GameObject instance1;
    private GameObject instance2;

    private Dictionary<char, int> keyParamMap = new Dictionary<char, int>();

    [SerializeField] private EventProcessFunction eventProcessFunction;

    private void Start()
    {
        //AudioManager.Instance.Play("BGM");
        //AudioManager.Instance.Play("Electric_Buzz");

        for (char c = 'A'; c <= 'Z'; c++)
        {
            keyParamMap[c] = 0;
        }
    }

    private IEnumerator EndGame()
    {
        // yield return StartCoroutine(SceneViewManager.boardView.RemoveHoleCard(999));
        yield return StartCoroutine(SceneViewManager.boardView.RemoveHoleCard(ClientGameState.playerSlot));
        StartCoroutine(SceneViewManager.viewAnimController.PlayGameEndAnim());
    }

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

        // 模拟结束
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StartCoroutine(EndGame());
        }

        // 抽技能牌
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneViewManager.viewAnimController.displaySkillCard.Display("test", "test", 1);
            //ClientCommand.DrawSkillCard();
            instance1 = CardViewCreator.Instance.CreateCardInstance(1001, 9999);
            StartCoroutine(handView.AddCard(instance1));
            instance2 = CardViewCreator.Instance.CreateCardInstance(1001, 9998);
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
            //StartCoroutine(SceneViewManager.myExecuteCardView.MoveToExecutePosition(instance1));
            //StartCoroutine(SceneViewManager.opponentExecuteCardView.MoveToExecutePosition(instance2));

            PointCardInstance pointIns = objs.Peek().GetComponent<PointCardInstance>();
            objs.Peek().GetComponent<PointCardViewController>().ChangeCardTexture(pointIns.cardVisualState, 9);
        }

        // 测试ResolveZoneView
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (card <= 10)
            {
                GameObject instance = CardViewCreator.Instance.CreateCardResolved(card++, 989);
                StartCoroutine(ResolveZoneView.AddCard(instance, -1, true, CardVisualState.None));
            }
        }

        // 测试PeekZoneView
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject instance = CardViewCreator.Instance.CreateCardResolved(2, 99);
            StartCoroutine(SceneViewManager.peekZoneView.AddCard(instance, -1, false, CardVisualState.None));
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
            SceneViewManager.boardView.GenerateLazer(objs.Pop().transform.position, new List<GameObject>());
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

        // 老虎机
        if (Input.GetKeyDown(KeyCode.J))
        {
            // eventProcessFunction.RevealTest(new object[] { 1 - ClientGameState.playerSlot, 1, 2, 3});
            SceneViewManager.myRevealButtonView.ShowRandom();
            StartCoroutine(SceneViewManager.myRevealButtonView.RandomAnimation(false));
        }

        // 测试回合灯
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneViewManager.myTurnLightView.SetLight(++turn);

            if (apCount++ < 3)
            {
                SceneViewManager.myActionPointView.AddPoint(1);
                SceneViewManager.opponentActionPointView.AddPoint(1);
            }
            else
            {
                SceneViewManager.myActionPointView.SpendPoint(1);
                SceneViewManager.opponentActionPointView.SpendPoint(1);
            }
        }

        // 测试庄闲
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneViewManager.roleView.ShowRole(1-ClientGameState.playerSlot);
            SceneViewManager.roleView.ShowRole(ClientGameState.playerSlot);
        }

        // 测试跟注装置
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (keyParamMap['K']++ % 2 == 0)
            {
                StartCoroutine(SceneViewManager.callOrFoldMachine.Show(2));
                StartCoroutine(SceneViewManager.callOrFoldMachineBack.Show(2));
            }
            else
            {
                StartCoroutine(SceneViewManager.callOrFoldMachine.Hide());
                StartCoroutine(SceneViewManager.callOrFoldMachineBack.Hide());
            }
        }

        // 测试牌桌特效
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneViewManager.viewAnimController.TablePlaneMatManager.SetFirstMaterial("Tar");
            SceneViewManager.viewAnimController.TablePlaneMatManager.PlayPlaneAnim(1.2f, 0.5f, 1.2f);
        }
    }
}
