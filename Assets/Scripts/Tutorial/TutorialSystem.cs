using DG.Tweening;
using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TutorialSystem : MonoBehaviour
{
    private int instanceID = 10000;
    private Dictionary<int, GameObject> instanceMap;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] clips;

    [Header("Scene Object")]
    public ClickToDrawSkillCard clickToDrawSkillCard;
    public ClickToDrawPointCard clickToDrawPointCard;
    public RevealButton revealButton;
    [SerializeField] private Volume volume;
    [SerializeField] private UIClickSuggest zoneClickSuggest;
    [SerializeField] private GameObject switchObject;
    private ColorAdjustments colorAdjust;

    private void Start()
    {
        instanceMap = new Dictionary<int, GameObject>();
        ClientGameState.IsTutorial = true;
        StartCoroutine(StartTutotrial());
        volume.profile.TryGet(out colorAdjust);
    }

    private IEnumerator StartTutotrial()
    {
        // Init
        GameObject instance = null;
        yield return new WaitForSecondsRealtime(1f);

        yield return PlayTutorialTTS(0);
        StartGame();
        InitPointCard();
        InitSkillCard();


        // 第一回合
        StartTurn(2);
        yield return PlayTutorialTTS(1);
        yield return PlayTutorialTTS(2);
        yield return PlayTutorialTTS(3);
        yield return PlayTutorialTTS(4);
        yield return PlayTutorialTTS(5);
        yield return PlayTutorialTTS(6);

        yield return DrawPointCard();

        yield return PlayTutorialTTS(7);
        instanceMap[1001].GetComponentInChildren<Outline>().Enable = 1f;
        yield return PlaySkillCard(1001);
        yield return new WaitForSeconds(1f);
        yield return ClickTest(true, new List<int> { 1 << 7 }, 1);
        instance = CardViewCreator.Instance.CreateCardInstance(4, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.None));
        SceneViewManager.mySumPointView.ChangeSum(11, true);

        yield return PlayTutorialTTS(8);
        yield return PlayTutorialTTS(9);
        yield return DrawSkillCard(1004);

        yield return PlayTutorialTTS(10);
        yield return EndTurn();

        // AI 就抽牌
        SceneViewManager.opponentTurnLightView.SetLight(4);
        yield return new WaitForSeconds(1f);
        instance = CardViewCreator.Instance.CreateCardInstance(3, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - ClientGameState.playerSlot, CardVisualState.None));
        yield return new WaitForSeconds(2f);
        SceneViewManager.opponentSumPointView.ChangeSum(9, false);

        // 第二回合
        StartTurn(2);
        yield return PlayTutorialTTS(11);

        SceneViewManager.myTurnLightView.SetLight(5);
        yield return ShowButton();
        yield return PlayTutorialTTS(12);

        yield return DrawSkillCard(1002);
        SceneViewManager.myRevealButtonView.HideButton();

        yield return PlayTutorialTTS(13);

        yield return PlaySkillCard(1004);
        yield return new WaitForSeconds(1f);
        SceneViewManager.myActionPointView.AddPoint(2);

        yield return PlayTutorialTTS(14);

        yield return PlaySkillCard(1002);
        yield return new WaitForSeconds(1f);
        yield return ClickTest(true, new List<int> { 1 << 8 }, 1);
        StartCoroutine(SceneViewManager.boardView.RemoveCard(instanceMap[100]));
        SceneViewManager.opponentSumPointView.ChangeSum(3, false);

        yield return PlayTutorialTTS(15);
        yield return AddChip();
        SceneViewManager.myActionPointView.SpendPoint(1);

        yield return PlayTutorialTTS(16);
        yield return EndTurn();

        // AI 就抽牌
        SceneViewManager.opponentTurnLightView.SetLight(5);
        yield return new WaitForSeconds(1f);
        instance = CardViewCreator.Instance.CreateCardInstance(1, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - ClientGameState.playerSlot, CardVisualState.None));
        yield return new WaitForSeconds(2f);
        SceneViewManager.opponentSumPointView.ChangeSum(4, false);
        yield return new WaitForSeconds(1f);
        yield return PlayTutorialTTS(17);

        yield return EndGame();
        yield return PlayTutorialTTS(18);

        ClientGameState.IsTutorial = false;
    }

    private IEnumerator PlayTutorialTTS(int index)
    {
        audioSource.clip = clips[index];
        audioSource.Play();

        // 防止打开教程的那一次点击立刻把语音跳过
        yield return null;

        while (audioSource.isPlaying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                audioSource.Stop();
                break;
            }

            yield return null;
        }
    }

    private IEnumerator ChangeText(string text)
    {
        // TutorialText.text = text;
        while (true)
        {
            yield return null;
            if (!Input.GetMouseButtonDown(0))
                continue;
            else
            {
                break;
            }
        }
    }

    private IEnumerator EndGame()
    {
        SceneViewManager.myRevealButtonView.ShowRandom();
        yield return SceneViewManager.myRevealButtonView.RandomAnimation(true);
        yield return SceneViewManager.boardView.HoleCardFlip();
        SceneViewManager.opponentSumPointView.ChangeSum(9, true);
        yield return new WaitForSecondsRealtime(0.5f);

        yield return SceneViewManager.boardView.RemoveHoleCard(1 - ClientGameState.playerSlot);
        // yield return SceneViewManager.roleView.ShowWin(ClientGameState.playerSlot);
        // yield return SceneViewManager.viewAnimController.PlayGameEndAnim();

        // yield return SceneViewManager.viewAnimController.PlayMatchEndAnim(true);
    }

    private IEnumerator ShowButton()
    {
        SceneViewManager.myRevealButtonView.ShowButton(true);
        yield return new WaitForSeconds(1f);

        //revealButton.gameObject.layer = LayerMask.NameToLayer("Tutorial");
        //yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        //ClientGameState.TutorialStepDone = false;
        //revealButton.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private IEnumerator EndTurn()
    {
        SceneViewManager.endTurnView.gameObject.layer = LayerMask.NameToLayer("Tutorial");
        yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        ClientGameState.TutorialStepDone = false;
        SceneViewManager.endTurnView.gameObject.layer = LayerMask.NameToLayer("Default");

        SceneViewManager.turnIndicator.Rotate2Player(true);
    }

    private IEnumerator ClickTest(bool isSourceParticipantZone, List<int> candidateSourceIds, int SourceSelectCount)
    {
        int cardLayerMask = LayerMask.GetMask("Card");
        int zoneLayerMask = LayerMask.GetMask("Zone");

        Debug.Log($"[Client] Wating for select source {candidateSourceIds}");
        HighLight(candidateSourceIds);
        int count0 = 0;
        while (count0 < SourceSelectCount)
        {
            yield return null;
            if (!Input.GetMouseButtonDown(0))
                continue;

            // 如果候选有zone
            if (isSourceParticipantZone)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, zoneLayerMask)) continue;

                ZoneInstance zoneInstance = hit.collider.GetComponent<ZoneInstance>();
                if (!zoneInstance) continue;

                int instanceId = zoneInstance.isType ? (int)zoneInstance.zoneType : zoneInstance.zoneId;
                if (!candidateSourceIds.Contains(instanceId)) continue;

                Debug.Log($"[Client] Select Source instaceId {instanceId}");
                count0++;
            }
            // 如果候选没有zone
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, cardLayerMask)) continue;

                CardInstance cardInstance = hit.collider.GetComponent<PointCardInstance>();
                if (!cardInstance)
                {
                    cardInstance = hit.collider.GetComponent<SkillCardInstance>();
                    if (!cardInstance)
                        continue;
                }

                int instanceId = cardInstance.instanceId;
                if (!candidateSourceIds.Contains(instanceId)) continue;

                Debug.Log($"[Client] Select Source instaceId {instanceId}");
                count0++;
            }
        }
        CancelHighLight(candidateSourceIds);
        
    }

    private void HighLight(List<int> candidateIds)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            -100,
            0.5f
        );

        int layer = LayerMask.NameToLayer("Tutorial");
        foreach (int id in candidateIds)
        {
            if (instanceMap.TryGetValue(id, out GameObject instance))
                SetLayerRecursively(instance, layer);
            // 如果是Zone
            else
            {
                zoneClickSuggest.Show(id);
            }
        }
    }

    private void HighLight(GameObject instance)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            -100,
            0.5f
        );

        int layer = LayerMask.NameToLayer("Tutorial");
        SetLayerRecursively(instance, layer);
    }

    private void CancelHighLight(List<int> candidateIds)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            0,
            0.5f
        );

        int layer = LayerMask.NameToLayer("Default");
        foreach (int id in candidateIds)
        {
            if (instanceMap.TryGetValue(id, out GameObject instance))
                SetLayerRecursively(instance, layer);
            else
            {
                zoneClickSuggest.Hide();
            }
        }
    }

    private void CancelHighLight(GameObject instance)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            0,
            0.5f
        );

        int layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(instance, layer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private IEnumerator PlaySkillCard(int instanceId)
    {
        GameObject obj = instanceMap[instanceId];
        obj.layer = LayerMask.NameToLayer("Tutorial");
        yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        ClientGameState.TutorialStepDone = false;

        SceneViewManager.myActionPointView.SpendPoint(1);

        SkillCardController skillCard = obj.GetComponent<SkillCardController>();
        skillCard.stateMachine.ChangeState(skillCard.executeState);

        obj.layer = LayerMask.NameToLayer("Default");
    }

    private IEnumerator DrawPointCard()
    {
        clickToDrawPointCard.gameObject.layer = LayerMask.NameToLayer("Tutorial");
        yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        ClientGameState.TutorialStepDone = false;

        GameObject instance = CardViewCreator.Instance.CreateCardInstance(3, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.None));
        SceneViewManager.mySumPointView.ChangeSum(2 + 2 + 3, true);
        clickToDrawPointCard.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private IEnumerator DrawSkillCard(int id)
    {
        clickToDrawSkillCard.gameObject.layer = LayerMask.NameToLayer("Tutorial");
        yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        ClientGameState.TutorialStepDone = false;

        ClientGameState.SkillCardCount--;
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(id, id);
        instanceMap[id] = instance;
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
        AudioManager.Instance.Play("DrawSkillCard");
        clickToDrawSkillCard.gameObject.layer = LayerMask.NameToLayer("Default");
        SceneViewManager.myActionPointView.SpendPoint(1);
    }

    private IEnumerator AddChip()
    {
        yield return new WaitUntil(() => ClientGameState.TutorialStepDone);
        ClientGameState.TutorialStepDone = false;
    }

    private void StartTurn(int ap)
    {
        SceneViewManager.turnIndicator.Rotate2Player(false);
        SceneViewManager.endTurnView.btnLight.intensity = 1;
        SceneViewManager.endTurnView.hasClicked = false;
        SceneViewManager.myActionPointView.AddPoint(ap);
    }

    private void StartGame()
    {
        GameManager.ChangeInteractMask("Tutorial");

        StartCoroutine(SceneViewManager.viewAnimController.PlayStartGameAnim());
        SceneViewManager.myChipView.StartGame(false, 4);
        SceneViewManager.opponentChipView.StartGame(true, 3);
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
        SceneViewManager.myTurnLightView.SetLight(4);
        SceneViewManager.opponentTurnLightView.SetLight(3);
    }

    private void InitPointCard()
    {
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(2, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.Hole));
        instance = CardViewCreator.Instance.CreateCardInstance(5, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - ClientGameState.playerSlot, CardVisualState.Hole));

        instance = CardViewCreator.Instance.CreateCardInstance(6, 100);
        instanceMap[100] = instance;
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, 1 - ClientGameState.playerSlot, CardVisualState.None));

        instance = CardViewCreator.Instance.CreateCardInstance(2, instanceID++);
        StartCoroutine(SceneViewManager.boardView.AddCard(instance, ClientGameState.playerSlot, CardVisualState.None));

        SceneViewManager.mySumPointView.ChangeSum(2 + 2, true);
        SceneViewManager.opponentSumPointView.ChangeSum(6, false);
    }

    private void InitSkillCard()
    { 
        // 加点牌
        GameObject instance = CardViewCreator.Instance.CreateCardInstance(1007, instanceID++);
        StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
        instance = CardViewCreator.Instance.CreateCardInstance(1007, 1001);
        instanceMap[1001] = instance;
        StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 减点牌
        instance = CardViewCreator.Instance.CreateCardInstance(1002, instanceID++);
        StartCoroutine(SceneViewManager.opponentHandView.AddCard(instance));
        //instance = CardViewCreator.Instance.CreateCardInstance(1002, 1002);
        //instanceMap[1002] = instance;
        //StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 行动补充
        //instance = CardViewCreator.Instance.CreateCardInstance(1004, 1000);
        //instanceMap[1000] = instance;
        //StartCoroutine(SceneViewManager.myHandView.AddCard(instance));

        // 猜单双
        //instance = CardViewCreator.Instance.CreateCardInstance(1107, instanceID++);
        //StartCoroutine(SceneViewManager.myHandView.AddCard(instance));
    }
}
