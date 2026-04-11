using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json;
using DG.Tweening;
using Game.Domain;

public class DetermineParticipants : MonoBehaviour
{
    [Header("Volume Control")]
    [SerializeField] private Volume volume;
    [SerializeField] private UIClickSuggest zoneClickSuggest;

    private ColorAdjustments colorAdjust;

    private void Awake()
    {
        ProcessDispatcher.Register("DetermineParticipantsTest", DetermineParticipantsTest);
    }

    private void Start()
    {
        volume.profile.TryGet(out colorAdjust);
    }


    void Update()
    {
        if (ClientGameState.gateway == null) return;
    }


    // Debug Test
    public void DetermineParticipantsTest(object[] parameters)
    {
        bool success = (bool)parameters[0];
        int skillCardInstanceId = (int)parameters[1];
        bool sourceNeedChoose = (bool)parameters[2];
        bool targetNeedChoose = (bool)parameters[3];
        bool isSourceParticipantZone = (bool)parameters[4];
        bool isTargetParticipantZone = (bool)parameters[5];
        List<int> candidateSourceIds = (List<int>)parameters[6];
        List<int> candidateTargetIds = (List<int>)parameters[7];
        int sourceSelectCount = (int)parameters[8];
        int targetSelectCount = (int)parameters[9];

        if (!success)
        {
            ClientEffectContext.Instance.selectedSourceIds = new List<int>();
            ClientEffectContext.Instance.selectedTargetIds = new List<int>();
            ClientEffectContext.ChooseDone = true;
            return;
        }

        bool sourceClick = true;
        bool targetClick = true;
        if (!sourceNeedChoose || candidateSourceIds.Count <= 1)
        {
            Debug.Log("[Client] No SOURCE to choose, executing effect directly");
            ClientEffectContext.Instance.selectedSourceIds = candidateSourceIds;
            sourceClick = false;
        }
        if (!targetNeedChoose || candidateTargetIds.Count <= 1)
        {
            Debug.Log("[Client] No TARGET to choose, executing effect directly");
            ClientEffectContext.Instance.selectedTargetIds = candidateTargetIds;
            targetClick = false;
        }
        StartCoroutine(ClickTest(
            skillCardInstanceId,
            isSourceParticipantZone, candidateSourceIds, sourceSelectCount, sourceClick,
            isTargetParticipantZone, candidateTargetIds, targetSelectCount, targetClick
        ));
    }

    private IEnumerator ClickTest(int skillCardInstanceId,
        bool isSourceParticipantZone, List<int> candidateSouceIds, int SourceSelectCount, bool sourceClick,
        bool isTargetParticipantZone, List<int> candidateTargetIds, int targetSelectCount, bool targetClick)
    {
        List<int> selectedSourceIds = candidateSouceIds;
        List<int> selectedTargetIds = candidateTargetIds;

        int cardLayerMask = LayerMask.GetMask("Card");
        int zoneLayerMask = LayerMask.GetMask("Zone");

        if (sourceClick)
        {
            selectedSourceIds = new List<int>();
            Debug.Log($"[Client] Wating for select source {candidateSouceIds}");
            HighLight(candidateSouceIds);
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
                    if (!candidateSouceIds.Contains(instanceId)) continue;

                    selectedSourceIds.Add(instanceId);
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
                    if (!candidateSouceIds.Contains(instanceId)) continue;

                    selectedSourceIds.Add(instanceId);
                    Debug.Log($"[Client] Select Source instaceId {instanceId}");
                    count0++;
                }
            }
            ClientEffectContext.Instance.selectedSourceIds = selectedSourceIds;
            CancelHighLight(candidateSouceIds);
        }

        if (targetClick)
        {
            selectedTargetIds = new List<int>();
            Debug.Log("[Client] Wating for select target");
            HighLight(candidateTargetIds);
            int count1 = 0;
            while (count1 < targetSelectCount)
            {
                yield return null;
                if (!Input.GetMouseButtonDown(0))
                    continue;

                // 如果候选有zone
                if (isTargetParticipantZone)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, zoneLayerMask)) continue;

                    ZoneInstance zoneInstance = hit.collider.GetComponent<ZoneInstance>();
                    if (!zoneInstance) continue;

                    int instanceId = zoneInstance.isType ? (int)zoneInstance.zoneType : zoneInstance.zoneId;
                    if (!candidateTargetIds.Contains(instanceId)) continue;

                    selectedTargetIds.Add(instanceId);
                    Debug.Log($"[Client] Select Target instaceId {instanceId}");
                    count1++;
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
                    if (!candidateTargetIds.Contains(instanceId)) continue;

                    selectedTargetIds.Add(instanceId);
                    Debug.Log($"[Client] Select Target instaceId {instanceId}");
                    count1++;
                }
            }
            ClientEffectContext.Instance.selectedTargetIds = selectedTargetIds;
            CancelHighLight(candidateTargetIds);
        }

        // ClientEffectContext.ChooseDone = true;
        CommitChosenIdsCommand discardCmd = new CommitChosenIdsCommand
        {
            playerId = ClientGameState.playerSlot,
            instanceId = skillCardInstanceId,
            selectedSourceIds = selectedSourceIds,
            selectedTargetIds = selectedTargetIds,
        };
        ClientGameState.gateway.SendCommandServerRpc("CommitChosenIds", JsonConvert.SerializeObject(discardCmd));
    }

    private void HighLight(List<int> candidateIds)
    {
        DOTween.To(
            () => colorAdjust.saturation.value,
            x => colorAdjust.saturation.value = x,
            -100,
            0.5f
        );

        int layer = LayerMask.NameToLayer("HighlightOnly");
        foreach (int id in candidateIds)
        {
            if (EventProcessFunction.instanceMap.TryGetValue(id, out GameObject instance))
                SetLayerRecursively(instance, layer);
            // 如果是Zone
            else
            {
                zoneClickSuggest.Show(id);
            }
        }
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
            if (EventProcessFunction.instanceMap.TryGetValue(id, out GameObject instance))
                SetLayerRecursively(instance, layer);
            else
            {
                zoneClickSuggest.Hide();
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    // 你在真实项目里会这样更新 lastEventIndex：
    // - 把 TargetEvent(...) 里打印的 ev.Index 存到这里（例如通过事件转发或单例）
    //
    // 这个最小示例为了“只给你完整可编译代码”，就不做跨脚本回调了。
    // 你要的话我可以把 TargetEvent 改为触发 C# event，然后这里订阅并更新 lastEventIndex。
}
