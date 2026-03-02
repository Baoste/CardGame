using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.Rendering.DebugUI;

public class ClientMatchInput : MonoBehaviour
{
    public MatchGateway gateway;
    public TMP_InputField inputField;
    public TMP_InputField inputField1;

    [Header("Persisted (copy from log for quick test)")]
    public string matchId;
    public string token;

    [Header("Client state")]
    public int lastEventIndex = -1;
    public int playerSlot = -1;

    void OnEnable()
    {
        MatchGateway.OnClientJoined += OnJoined;
        MatchGateway.OnClientEvent += OnEvent;
    }

    void OnDisable()
    {
        MatchGateway.OnClientJoined -= OnJoined;
        MatchGateway.OnClientEvent -= OnEvent;
    }

    void OnJoined(string matchId, int slot, string token, string snapshotJson)
    {
        inputField.text = matchId;
        playerSlot = slot;
        // Debug.Log($"[UI] Joined match {matchId}, slot {slot}");
    }

    void OnEvent(Game.Domain.NetEvent ev)
    {
        if (ev.Index > lastEventIndex)
            lastEventIndex = ev.Index;
    }

    private void Awake()
    {
        ProcessDispatcher.Register("PlaySkillCardTest", WaitForChoose);
    }

    void Update()
    {
        if (gateway == null) return;

        // C：创建新局
        if (Input.GetKeyDown(KeyCode.F1))
        {
            JoinOrCreateGameCommand cmd = new JoinOrCreateGameCommand { playerId = -1, matchIdOrEmpty = "123" };
            gateway.SendCommandServerRpc("JoinOrCreateGame", JsonUtility.ToJson(cmd));
            Debug.Log("[Client] Requested create match");
        }

        // J：加入指定 matchId
        if (Input.GetKeyDown(KeyCode.F2))
        {
            string matchId = inputField.text.Trim();
            JoinOrCreateGameCommand cmd = new JoinOrCreateGameCommand { playerId = -1, matchIdOrEmpty = matchId };
            gateway.SendCommandServerRpc("JoinOrCreateGame", JsonUtility.ToJson(cmd));

            Debug.Log($"[Client] Requested join matchId={matchId}");
        }

        // M：发消息（写入本局 eventlog 并广播给同局两人）
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ChatCommand cmd = new ChatCommand { playerId = playerSlot, chatContext = inputField1.text };
            gateway.SendCommandServerRpc("Chat", JsonUtility.ToJson(cmd));
        }

        //// R：重连（需要你先断线再连上服务器，然后按 R）
        //if (Input.GetKeyDown(KeyCode.F4))
        //{
        //    gateway.ReconnectServerRpc(matchId, token, lastEventIndex);
        //    Debug.Log($"[Client] Requested reconnect match={matchId} lastEventIndex={lastEventIndex}");
        //}

        // 发牌
        if (Input.GetKeyDown(KeyCode.F4))
        {
            StartGameCommand cmd = new StartGameCommand { playerId = playerSlot };
            gateway.SendCommandServerRpc("StartGame", JsonUtility.ToJson(cmd));
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            DrawPointCardCommand cmd = new DrawPointCardCommand { playerId = playerSlot };
            gateway.SendCommandServerRpc("DrawPointCard", JsonUtility.ToJson(cmd));
        }

        //if (Input.GetKeyDown(KeyCode.F5))
        //{
        //    ReadyToPlaySkillCardCommand cmd = new ReadyToPlaySkillCardCommand { playerId = playerSlot, cardId = 12 };
        //    gateway.SendCommandServerRpc("ReadyToPlaySkillCard", JsonUtility.ToJson(cmd));
        //}

        if (Input.GetKeyDown(KeyCode.F6))
        {
            //PlaySkillCardWithTargetCommand cmd = new PlaySkillCardWithTargetCommand { playerId = playerSlot, instanceId = 12, selectedTargetIds = new List<int> { 1 } };
            //gateway.SendCommandServerRpc("PlaySkillCardWithTarget", JsonUtility.ToJson(cmd));

            EffectOp effect0 = new EffectOp
            {
                type = EffectType.DrawCards,
                source = new ParticipantSpec
                {
                    participantType = ParticipantType.None,
                    participantSelectionMode = ParticipantSelectionMode.None,
                    filter = new AllCondition(),
                    maxCandidateCount = new NoneValue(),
                    maxSelectCount = new NoneValue()
                },
                target = new ParticipantSpec
                {
                    participantType = ParticipantType.PointCardsInDeck,
                    participantSelectionMode = ParticipantSelectionMode.None,
                    filter = new AllCondition(),
                    maxCandidateCount = new NoneValue(),
                    maxSelectCount = new NoneValue()
                },
                value = new ConstValue
                {
                    value = 1
                }
            };
            EffectOp effect1 = new EffectOp
            {
                type = EffectType.DrawCards,
                source = new ParticipantSpec
                {
                    participantType = ParticipantType.None,
                    participantSelectionMode = ParticipantSelectionMode.None,
                    filter = new AllCondition(),
                    maxCandidateCount = new NoneValue(),
                    maxSelectCount = new NoneValue()
                },
                target = new ParticipantSpec
                {
                    participantType = ParticipantType.PointCardsInDeck,
                    participantSelectionMode = ParticipantSelectionMode.All,
                    filter = new AllCondition(),
                    maxCandidateCount = new NoneValue(),
                    maxSelectCount = new NoneValue()
                },
                value = new ConstValue
                {
                    value = 1
                }
            };
            SkillCard tmp = new SkillCard
            {
                id = 999,
                name = "抽牌",
                description = "抽一张牌，再抽一张牌",
                point = 1,
                type = CardType.Skill,
                effects = new List<EffectOp> { effect0, effect1 }
            };

            StartCoroutine(ClientEffectExecutor.ExcuteCard(tmp, gateway, playerSlot));
        }
    }


    // Debug Test
    public void WaitForChoose(object[] parameters)
    {
        List<int> candidateSourceIds = (List<int>)parameters[0];
        List<int> candidateTargetIds = (List<int>)parameters[1];
        if (candidateSourceIds.Count == 0 && candidateTargetIds.Count == 0)
        {
            Debug.Log("[Client] No target to choose, executing effect directly");
            ClientEffectContext.Instance.selectedSourceIds = new List<int>();
            ClientEffectContext.Instance.selectedTargetIds = new List<int>();
            // StartCoroutine(DelayedTest(5f));
            StartCoroutine(ClickTest());
        }
        else
        {
            Debug.Log($"[Client] Waiting for player to choose target");
            ClientEffectContext.Instance.selectedSourceIds = candidateSourceIds; // 这里直接把候选目标当作已选目标了，实际你会弹 UI 让玩家选
            ClientEffectContext.Instance.selectedTargetIds = new List<int> { candidateTargetIds[0] }; // 这里直接把候选目标当作已选目标了，实际你会弹 UI 让玩家选
            // StartCoroutine(DelayedTest(10f));
            StartCoroutine(ClickTest());
        }
    }

    private IEnumerator DelayedTest(float time)
    {
        yield return new WaitForSeconds(time);
        ClientEffectContext.ChooseDone = true;
    }

    private IEnumerator ClickTest()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        ClientEffectContext.ChooseDone = true;
    }

    // 你在真实项目里会这样更新 lastEventIndex：
    // - 把 TargetEvent(...) 里打印的 ev.Index 存到这里（例如通过事件转发或单例）
    //
    // 这个最小示例为了“只给你完整可编译代码”，就不做跨脚本回调了。
    // 你要的话我可以把 TargetEvent 改为触发 C# event，然后这里订阅并更新 lastEventIndex。
}