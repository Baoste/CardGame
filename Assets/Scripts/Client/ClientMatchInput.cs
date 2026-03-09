using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClientMatchInput : MonoBehaviour
{
    [Header("Persisted (copy from log for quick test)")]
    public string matchId;
    public string token;

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
        // inputField.text = matchId;
        ClientGameState.playerSlot = slot;
        Debug.Log($"[UI] Joined match {matchId}, slot {slot}");
    }

    void OnEvent(Game.Domain.NetEvent ev)
    {
        if (ev.Index > ClientGameState.lastEventIndex)
            ClientGameState.lastEventIndex = ev.Index;
    }

    private void Awake()
    {
        ProcessDispatcher.Register("PlaySkillCardTest", PlaySkillCardTest);
    }


    void Update()
    {
        if (ClientGameState.gateway == null) return;
    }


    // Debug Test
    public void PlaySkillCardTest(object[] parameters)
    {
        bool success = (bool)parameters[0];
        bool sourceNeedChoose = (bool)parameters[1];
        bool targetNeedChoose = (bool)parameters[2];
        List<int> candidateSourceIds = (List<int>)parameters[3];
        List<int> candidateTargetIds = (List<int>)parameters[4];
        int sourceSelectCount = (int)parameters[5];
        int targetSelectCount = (int)parameters[6];

        if (!success)
        {
            ClientEffectContext.Instance.selectedSourceIds = new List<int>();
            ClientEffectContext.Instance.selectedTargetIds = new List<int>();
            ClientEffectContext.ChooseDone = true;
        }
        else if ((!sourceNeedChoose && !targetNeedChoose) || (candidateSourceIds.Count == 0 && candidateTargetIds.Count == 0))
        {
            Debug.Log("[Client] No target to choose, executing effect directly");
            ClientEffectContext.Instance.selectedSourceIds = candidateSourceIds;
            ClientEffectContext.Instance.selectedTargetIds = candidateTargetIds;
            ClientEffectContext.ChooseDone = true;
        }
        else
        {
            Debug.Log($"[Client] Waiting for player to choose target");
            // StartCoroutine(DelayedTest(10f));
            StartCoroutine(ClickTest(candidateSourceIds, sourceSelectCount, candidateTargetIds, targetSelectCount));
        }
    }

    private IEnumerator DelayedTest(float time)
    {
        yield return new WaitForSeconds(time);
        ClientEffectContext.ChooseDone = true;
    }

    private IEnumerator ClickTest(List<int> candidateSouceIds, int SourceSelectCount, List<int> candidateTargetIds, int targetSelectCount)
    {
        List<int> selectedSourceIds = new List<int>();
        List<int> selectedTargetIds = new List<int>();

        int count0 = 0;
        while (count0 < SourceSelectCount)
        {
            yield return null;
            if (!Input.GetMouseButtonDown(0))
                continue;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                continue;
            CardInstance cardInstance = hit.collider.GetComponentInParent<PointCardInstance>();
            if (!cardInstance)
            {
                cardInstance = hit.collider.GetComponentInParent<SkillCardInstance>();
                if (!cardInstance)
                    continue;
            }

            int instanceId = cardInstance.instanceId;
            if (!candidateSouceIds.Contains(instanceId))
                continue;
            selectedSourceIds.Add(instanceId);
            Debug.Log($"[Client] Select Source instaceId {instanceId}");
            count0++;
        }
        ClientEffectContext.Instance.selectedSourceIds = selectedSourceIds;

        int count1 = 0;
        while (count1 < targetSelectCount)
        {
            yield return null;
            if (!Input.GetMouseButtonDown(0))
                continue;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                continue;
            CardInstance cardInstance = hit.collider.GetComponentInParent<PointCardInstance>();
            if (!cardInstance)
            {
                cardInstance = hit.collider.GetComponentInParent<SkillCardInstance>();
                if (!cardInstance)
                    continue;
            }

            int instanceId = cardInstance.instanceId;
            if (!candidateTargetIds.Contains(instanceId))
                continue;
            selectedTargetIds.Add(instanceId);
            Debug.Log($"[Client] Select Target instaceId {instanceId}");
            count1++;
        }
        ClientEffectContext.Instance.selectedTargetIds = selectedTargetIds;
        ClientEffectContext.ChooseDone = true;
    }

    // 你在真实项目里会这样更新 lastEventIndex：
    // - 把 TargetEvent(...) 里打印的 ev.Index 存到这里（例如通过事件转发或单例）
    //
    // 这个最小示例为了“只给你完整可编译代码”，就不做跨脚本回调了。
    // 你要的话我可以把 TargetEvent 改为触发 C# event，然后这里订阅并更新 lastEventIndex。
}